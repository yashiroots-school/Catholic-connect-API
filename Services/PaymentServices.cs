using ChurchAPI.Models.ViewModels;
using ChurchAPI.Models;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Payrequest;
using ChurchAPI.Interface;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChurchAPI.Services
{
    public class PaymentServices: IPaymentInterface
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "http://atom.in?";
        private readonly Models.ViewModels.EncrypDecrpt _encrypDecrpt;
        private readonly PayUSettings _payUSettings;
        public PaymentServices(IConfiguration configuration, EncrypDecrpt encrypDecrpt, IOptions<PayUSettings> payUSettings)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new Exception("Connection string not found.");
            _encrypDecrpt = encrypDecrpt;
            _payUSettings = payUSettings.Value;
        }
        public async Task<PaymentResultModels> PreapareInput(PaymentInputModel objPaymentInputModel)
        {
            PaymentResultModels paymentResultModels = new PaymentResultModels();

            // Map basic family/payment info
            paymentResultModels.FamilyId = objPaymentInputModel.FamilyId;
            paymentResultModels.FamilyRegNo = objPaymentInputModel.FamilyRegNo;
            paymentResultModels.FamilyHeadName = objPaymentInputModel.FamilyHeadName;
            paymentResultModels.TotalAmount = objPaymentInputModel.TotalAmount;
            paymentResultModels.FeeHeadings = objPaymentInputModel.FeeHeadings;
            paymentResultModels.Feeheadingamt = objPaymentInputModel.Feeheadingamt;
            paymentResultModels.ConcessionAmt = objPaymentInputModel.ConcessionAmt;
            paymentResultModels.Concession = objPaymentInputModel.Concession;
            paymentResultModels.DueAmount = objPaymentInputModel.DueFee;
            paymentResultModels.Email = objPaymentInputModel.Email;
            paymentResultModels.PaymentGatewayName = objPaymentInputModel.PaymentGatewayName;
            paymentResultModels.UserId = objPaymentInputModel.UserId;
            paymentResultModels.ChurchId=objPaymentInputModel.ChurchId;
            paymentResultModels.Amount = objPaymentInputModel.TotalAmount;
            paymentResultModels.Title = objPaymentInputModel.Title;
            // ChurchId and DioceseId for prodDetails calculation
            long churchId = objPaymentInputModel.ChurchId; // Adjust if actual ChurchId comes from another property
            long dioceseId = 0; // Set properly if available

            // Divide total amount among products
            decimal totalAmount = Convert.ToDecimal(objPaymentInputModel.TotalAmount);
            var prodList = await GetProdDetailsWithAmountAsync(churchId, dioceseId, totalAmount);
            paymentResultModels.prodDetails = prodList;

            // Adjust last product to match totalAmount exactly
            decimal sumAmounts = prodList.Sum(p => p.prodAmount);
            if (sumAmounts != totalAmount)
            {
                prodList.Last().prodAmount += totalAmount - sumAmounts;
            }

            // Payment Gateway Keys
            var keyvalue = await GetKeySecretGateWayAsync(churchId, null, objPaymentInputModel.FeeHeadings);
            string Key = keyvalue.FirstOrDefault().Key;
            string Secret = keyvalue.FirstOrDefault().Value;
            paymentResultModels.AccountType = keyvalue.LastOrDefault().Value;

            // Call payment API if required
            Payverify.Payverify objPayverify = new Payverify.Payverify();
            if (new[] { "Atomic", "PhonePe", "Paytm", "AUTUM", "atom" }
                .Contains(objPaymentInputModel.PaymentGatewayName, StringComparer.OrdinalIgnoreCase))
            {
                objPaymentInputModel.PaymentGatewayName = "Atomic";
                objPayverify = await PaymentApiCallAsync(paymentResultModels, prodList);

                if (!string.IsNullOrEmpty(objPayverify.atomTokenId))
                {
                    paymentResultModels.OrdedrId = objPayverify.TrackID;
                    paymentResultModels.merchId = objPayverify.merchId;
                    paymentResultModels.atomTokenId = objPayverify.atomTokenId;
                    paymentResultModels.custMobile = objPayverify.custMobile;
                    paymentResultModels.custEmail = objPayverify.custEmail;
                    paymentResultModels.ApiUrl = objPayverify.ApiUrl;
                    paymentResultModels.returnurl = _configuration["PayUSettings:atomtechReturnUrl"] ?? string.Empty;
                }
            }

            // Insert transaction using ADO.NET
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("InsertPaymentTransaction", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TxnDate", DateTime.Now.ToString("dd/MM/yyyy"));
                cmd.Parameters.AddWithValue("@Amount", objPaymentInputModel.TotalAmount);
                cmd.Parameters.AddWithValue("@TransactionId", objPayverify.TrackID ?? "");
                cmd.Parameters.AddWithValue("@Pmntmode", "Online");
                cmd.Parameters.AddWithValue("@UserId", objPaymentInputModel.UserId);
                cmd.Parameters.AddWithValue("@TrackId", objPayverify.TrackID ?? "");
                cmd.Parameters.AddWithValue("@PaymentId", objPayverify.atomTokenId ?? "");
                cmd.Parameters.AddWithValue("@FeeIds", objPaymentInputModel.FeeHeadings);
                cmd.Parameters.AddWithValue("@FamilyRegNo", objPaymentInputModel.FamilyRegNo);
                cmd.Parameters.AddWithValue("@FamilyHeadName", objPaymentInputModel.FamilyHeadName);
                cmd.Parameters.AddWithValue("@FeeTitle", objPaymentInputModel.Title);
                cmd.Parameters.AddWithValue("@FamilyId", objPaymentInputModel.FamilyId);
                cmd.Parameters.AddWithValue("@churchId", objPaymentInputModel.ChurchId);

                // Create a JSON string of FeeAmounts for storing
                string feeAmountsJson = JsonConvert.SerializeObject(prodList.Select(p => new { p.prodName, p.prodAmount }));
                cmd.Parameters.AddWithValue("@FeeAmounts", feeAmountsJson);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

            return paymentResultModels;
        }
        public async Task<Dictionary<string, string>> GetKeySecretGateWayAsync(long churchId,int? classDetails,string feeHeadings)
        {
            var keySecretDict = new Dictionary<string, string>();

            var merchants = await GetMerchantsAsync(churchId);
            var merchantNames = await GetMerchantNamesAsync(churchId);
            var activeSchool = await GetActiveSchoolSetupAsync(churchId);

            if (activeSchool == null) return keySecretDict;

            // Filter merchants by active school setup
            var filteredMerchants = merchants
                .Where(m => m.Bank_Id == activeSchool.Bank_Id
                            && m.Branch_Id == activeSchool.Branch_Id
                            && m.ChurchId_Id == activeSchool.Church_Id)
                .ToList();

            string merchantCategory = "Primary";

            if (feeHeadings.Contains("Transport", StringComparison.OrdinalIgnoreCase))
                merchantCategory = "Transport";
            else if (classDetails.HasValue && new[] { 207, 208, 209, 210 }.Contains(classDetails.Value))
                merchantCategory = "Nursery";

            var merchantNameId = merchantNames
                .FirstOrDefault(m => m.MerchantName.Equals(merchantCategory, StringComparison.OrdinalIgnoreCase))
                ?.MerchantName_Id;

            if (merchantNameId != null)
            {
                var merchant = filteredMerchants.FirstOrDefault(m => m.MerchantName_Id == merchantNameId);
                if (merchant != null)
                {
                    keySecretDict.Add(merchant.MerchantMID, merchant.MerchantKey);
                }
            }

            keySecretDict.Add(merchantCategory, merchantCategory);

            return keySecretDict;
        }
        public async Task<List<BillingYears>> GetBillingYearsAsync()
        {
            var result = new List<BillingYears>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetBillingYears", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new BillingYears
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                BillingYear = reader["BillingYear"] != DBNull.Value ? Convert.ToInt32(reader["BillingYear"]) : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching BillingYear data: " + ex.Message, ex);
            }

            return result;
        }
        public async Task<IApiResponse> GetBillHeadingsAsync(string churchName, string title)
        {
            var result = new List<BillHeadings>();
            var res = new ApiResponse();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetBillHeadings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Title", title);

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new BillHeadings
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                BillHeading = reader["BillHeading"]?.ToString() ?? string.Empty,
                                Title = reader["Title"]?.ToString() ?? string.Empty,
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
                res.Data= result;
                res.ResponseCode = "200";
                res.Msg = "Data fetched Successfully!";
            }
            catch (Exception ex)
            {
                // Log exception
                ErrorLogException.LogErrorAsync(ex);
                res.Msg=ex.Message;
                res.ResponseCode = "400";

            }

            return res;
        }
        public async Task<List<CreateMerchantModel>> GetMerchantsAsync(long churchId)
        {
            var list = new List<CreateMerchantModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetMerchantsByChurch", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChurchId", churchId);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new CreateMerchantModel
                        {
                            Merchant_Id = Convert.ToInt64(reader["Merchant_Id"]),
                            MerchantName_Id = Convert.ToInt64(reader["MerchantName_Id"]),
                            MerchantMID = reader["MerchantMID"].ToString() ?? string.Empty,
                            MerchantKey = reader["MerchantKey"].ToString() ?? string.Empty,
                            Bank_Id = Convert.ToInt64(reader["Bank_Id"]),
                            Branch_Id = Convert.ToInt64(reader["Branch_Id"]),
                            ChurchId_Id = Convert.ToInt64(reader["ChurchId_Id"])
                        });
                    }
                }
            }
            return list;
        }
        public async Task<List<MerchantNameModel>> GetMerchantNamesAsync(long churchId)
        {
            var list = new List<MerchantNameModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetMerchantNamesByChurch", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChurchId", churchId);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new MerchantNameModel
                        {
                            MerchantName_Id = Convert.ToInt64(reader["MerchantName_Id"]),
                            MerchantName = reader["MerchantName"].ToString() ?? string.Empty
                        });
                    }
                }
            }
            return list;
        }
        public async Task<SchoolSetupModel?> GetActiveSchoolSetupAsync(long churchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetActiveSchoolSetup", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChurchId", churchId);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new SchoolSetupModel
                        {
                            ChurchSetUp_Id = Convert.ToInt64(reader["ChurchSetup_Id"]),
                            Church_Id = Convert.ToInt64(reader["ChurchId"]),
                            Bank_Id = Convert.ToInt64(reader["Bank_Id"]),
                            Branch_Id = Convert.ToInt64(reader["Branch_Id"]),
                            Merchant_nameId = Convert.ToInt64(reader["Merchant_nameId"])
                        };
                    }
                }
            }
            return null;
        }
        public async Task<Payverify.Payverify> PaymentApiCallAsync(PaymentResultModels paymentResultModels,List<prodDetails>prodDetails)
        {
            try
            {
                // Map request data
                Payrequest.Payrequest objre = new Payrequest.Payrequest(_connectionString);
                var mapdata = objre.RequestMap(paymentResultModels, paymentResultModels.ChurchId,prodDetails);
                var json = JsonConvert.SerializeObject(mapdata);
                string encryptVal = _encrypDecrpt.Encrypt(json);
                var authUrl = _configuration["PayUSettings:atomtechAuthurl"];
                var merchId = _configuration["PayUSettings:atomtechmerchId"];

                if (string.IsNullOrEmpty(authUrl) || string.IsNullOrEmpty(merchId))
                    throw new Exception("Required configuration values (atomtechAuthurl or atomtechmerchId) are missing.");

                string apiUrl = string.Format(authUrl, merchId, "&", encryptVal);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        // Set security protocols
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        var response = await httpClient.PostAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonresponse = await response.Content.ReadAsStringAsync();
                            var result = jsonresponse.Replace("System.Net.HttpWebResponse", "");

                            var uri = new Uri(_configuration["PayUSettings:atomtechResulturl"] + result);
                            var query = HttpUtility.ParseQueryString(uri.Query);
                            string? encData = query.Get("encData");

                            if (!string.IsNullOrEmpty(encData))
                            {
                                string Decryptval = _encrypDecrpt.decrypt(encData);
                                var objPayverify = JsonConvert.DeserializeObject<Payverify.Payverify>(Decryptval);

                                if (objPayverify != null)
                                {
                                    objPayverify.merchId = mapdata?.payInstrument?.merchDetails?.merchId;
                                    objPayverify.TrackID = mapdata?.payInstrument?.merchDetails?.merchTxnId;
                                    objPayverify.custEmail = mapdata?.payInstrument?.custDetails?.custEmail;
                                    objPayverify.custMobile = mapdata?.payInstrument?.custDetails?.custMobile;
                                    objPayverify.ApiUrl = apiUrl;
                                }

                                return objPayverify ?? new Payverify.Payverify(); // Ensure non-null return
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log exception
                        File.AppendAllText(@"C:\ATUM\ErrorLog.txt", ex.ToString() + Environment.NewLine);
                        throw;
                    }

                    return new Payverify.Payverify(); // Return empty Payverify object in case of failure
                }
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow
                File.AppendAllText(@"C:\ATUM\ErrorLog.txt", ex.ToString() + Environment.NewLine);
                throw;
            }
        }
        public async Task<List<prodDetails>> GetProdDetailsWithAmountAsync(long churchId, long dioceseId, decimal totalAmount)
        {
            var prodList = new List<prodDetails>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("GetProdNamesByChurchAndDiocese", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChurchId", churchId);
                cmd.Parameters.AddWithValue("@DioceseId", dioceseId);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        prodList.Add(new prodDetails
                        {
                            prodName = reader["ProdName"].ToString() ?? string.Empty
                        });
                    }
                }
            }

            // Divide totalAmount equally among products
            int count = prodList.Count;
            if (count == 0) return prodList;

            decimal dividedAmount = Math.Round(totalAmount / count, 2); // Round to 2 decimal places

            decimal sumAssigned = 0;

            for (int i = 0; i < count; i++)
            {
                // For the last product, assign remaining amount to ensure total matches
                if (i == count - 1)
                {
                    prodList[i].prodAmount = totalAmount - sumAssigned;
                }
                else
                {
                    prodList[i].prodAmount = dividedAmount;
                    sumAssigned += dividedAmount;
                }
            }

            return prodList;
        }
        public async Task<decimal> GetBillTotalByBillHeadingAsync(string churchName, string billHeadingIds, string year)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("GetBillTotalByBillHeading", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@BillId", billHeadingIds);
                command.Parameters.AddWithValue("@year", year);

                var result = await command.ExecuteScalarAsync();
                return result != null && decimal.TryParse(result.ToString(), out var total) ? total : 0m;
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex); // Make sure your logger is async
                return 0m;
            }
        }
        public async Task<IApiResponse> GetmarchentDetails(long churchId)
        {
            var res = new ApiResponse();
            try
            {
                var repo = new MerchantRepository(_connectionString);
                var merchant = repo.GetMerchant(churchId);
                PayUSettings ps = new PayUSettings();
                ps = _payUSettings;
                ps.Password = merchant?.Password??"";
                res.Data = ps;//_payUSettings;
                res.ResponseCode = "200";
            }
            catch (Exception ex)
            {
                res.Data = ex.Message;
                res.ResponseCode = "500";
                throw;
            }

            return res;
        }
        public async Task<IApiResponse> SaveOrUpdateSubscriptionBillAndPaymentAsync(BillDetailsViweModel detailsBilling,IEnumerable<prodDetails> prodList)
        {
            var res = new ApiResponse();

            // 1. Validate input
            if (detailsBilling == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Billing details cannot be null.";
                return res;
            }
            if (string.IsNullOrWhiteSpace(detailsBilling.ChurchName))
            {
                res.ResponseCode = "400";
                res.Msg = "Church Details are required.";
                return res;
            }

            try
            {
                // 2. Save or update subscription bill
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    await using (var command = new SqlCommand("InsertSubscriptionBill", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters (as you already had)
                        command.Parameters.Add(new SqlParameter("@BillDetailId", SqlDbType.Int) { Value = detailsBilling.BillDetailId });
                        command.Parameters.Add(new SqlParameter("@BillId", SqlDbType.Int) { Value = detailsBilling.BillId });
                        command.Parameters.Add(new SqlParameter("@memberId", SqlDbType.Int) { Value = detailsBilling.FamilyId });
                        command.Parameters.Add(new SqlParameter("@Heading", SqlDbType.NVarChar) { Value = (object)detailsBilling.BillHeading ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@SubHeading", SqlDbType.NVarChar) { Value = (object)string.Join(",", detailsBilling.SelectedBillHeadings) ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@Year", SqlDbType.NVarChar) { Value = (object)detailsBilling.Year ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@BillAmount", SqlDbType.Decimal) { Value = detailsBilling.ToPayAmount });
                        command.Parameters.Add(new SqlParameter("@Total", SqlDbType.Decimal) { Value = detailsBilling.TotalAmount });
                        command.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Decimal) { Value = detailsBilling.Discount });
                        command.Parameters.Add(new SqlParameter("@PaidAmount", SqlDbType.Decimal) { Value = detailsBilling.PaidAmount });
                        command.Parameters.Add(new SqlParameter("@DueAmount", SqlDbType.Decimal) { Value = detailsBilling.DueAmount });
                        command.Parameters.Add(new SqlParameter("@PaymentMode", SqlDbType.NVarChar) { Value = (object)detailsBilling.PaymentMode ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@PaidDate", SqlDbType.Date) { Value = detailsBilling.PaidDate });
                        command.Parameters.Add(new SqlParameter("@BillGeneratedBy", SqlDbType.NVarChar) { Value = (object)detailsBilling.BillGeneratedBy ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@ChurchId", SqlDbType.Int) { Value = (object)detailsBilling.churchId ?? DBNull.Value });
                        command.Parameters.Add(new SqlParameter("@CreatedDate", SqlDbType.Date)
                        {
                            Value = (detailsBilling.CreatedDate == default(DateTime))
                                        ? DateTime.Now
                                        : detailsBilling.CreatedDate
                        });
                        command.Parameters.Add(new SqlParameter("@createdBy", SqlDbType.NVarChar) { Value = (object)detailsBilling.CreatedBy ?? DBNull.Value });

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }

                // 3. Now call InsertPaymentTransactionAsync
                var (paymentSuccess, paymentMsg) = await InsertPaymentTransactionAsync(detailsBilling, prodList);

                if (paymentSuccess)
                {
                    res.ResponseCode = "200";
                    res.Msg = "Bill and payment saved successfully.";
                }
                else
                {
                    res.ResponseCode = "500";
                    // You might still want to say bill succeeded but payment failed
                    res.Msg = "Bill saved, but payment insertion failed: " + paymentMsg;
                }
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }

            return res;
        }
        public async Task<(bool success, string message)> InsertPaymentTransactionAsync(BillDetailsViweModel objPaymentInputModel, IEnumerable<prodDetails> prodList)
        {
            try
            {
                List<int> headings = objPaymentInputModel.SelectedBillHeadings;

                // Convert to comma-separated string
                string headingsCsv = headings != null && headings.Any()
                    ? string.Join(",", headings)
                    : string.Empty;
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("InsertPaymentTransaction", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Setting parameters
                    cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.CreatedDate.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Amount", objPaymentInputModel.TotalAmount);
                    cmd.Parameters.AddWithValue("@TransactionId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@Pmntmode", "Online");
                    cmd.Parameters.AddWithValue("@UserId", objPaymentInputModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@TrackId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@PaymentId", objPaymentInputModel.AtomTokenId ?? "");
                    cmd.Parameters.AddWithValue("@FeeIds", headingsCsv);
                    cmd.Parameters.AddWithValue("@FamilyRegNo", objPaymentInputModel.FamilyRegistrationNo);
                    cmd.Parameters.AddWithValue("@FamilyHeadName", objPaymentInputModel.FamilyHeadName);
                    cmd.Parameters.AddWithValue("@FeeTitle", objPaymentInputModel.BillHeading);
                    cmd.Parameters.AddWithValue("@FamilyId", objPaymentInputModel.FamilyId);
                    cmd.Parameters.AddWithValue("@churchId", objPaymentInputModel.churchId);

                    // Create JSON string of fee amounts
                    string feeAmountsJson = JsonConvert.SerializeObject(
                        prodList.Select(p => new { p.prodName, p.prodAmount })
                    );
                    cmd.Parameters.AddWithValue("@FeeAmounts", feeAmountsJson);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows < 0)
                    {
                        return (true, "Transaction inserted successfully.");
                    }
                    else
                    {
                        return (false, "No rows affected.");
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex); // agar aapka logger async hai
                return (false, $"Exception: {ex.Message}");
            }
        }
        public async Task<IApiResponse> SaveOrUpdateDonationBillAndPaymentAsync(DonationMaster donation, IEnumerable<prodDetails> prodList)
        {
            var res = new ApiResponse();

            // 1. Validate input
            if (donation == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Billing details cannot be null.";
                return res;
            }
            if (string.IsNullOrWhiteSpace(donation.ChurchName))
            {
                res.ResponseCode = "400";
                res.Msg = "Church Details are required.";
                return res;
            }

            try
            {
                // 2. Save or update subscription bill
                await using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    await using (var command = new SqlCommand("usp_InsertOrUpdate_DonationMaster", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt) { Value = Convert.ToInt64(ReturnVal(donation.DonationId)) });
                        command.Parameters.Add(new SqlParameter("@DonarName", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonarName)) });
                        command.Parameters.Add(new SqlParameter("@DonarContact", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonarContact)) });
                        command.Parameters.Add(new SqlParameter("@DonarEmail", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonarEmail)) });
                        command.Parameters.Add(new SqlParameter("@DonarAddress", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonarAddress)) });
                        command.Parameters.Add(new SqlParameter("@DonarCountry", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonarCountry)) });
                        command.Parameters.Add(new SqlParameter("@DonarPANCardNo", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonarPANCardNo)) });
                        command.Parameters.Add(new SqlParameter("@DonarGSTNo", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonarGSTNo)) });
                        command.Parameters.Add(new SqlParameter("@DonationType", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.DonationType)) });
                        command.Parameters.Add(new SqlParameter("@PaymentMode", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.PaymentMode)) });
                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.Description)) });
                        command.Parameters.Add(new SqlParameter("@ChequeNo", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.ChequeNo)) });
                        command.Parameters.Add(new SqlParameter("@BankName", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.BankName)) });
                        command.Parameters.Add(new SqlParameter("@IFSCCode", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.IFSCCode)) });
                        command.Parameters.Add(new SqlParameter("@TransactionNo", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.TransactionNo)) });
                        command.Parameters.Add(new SqlParameter("@TransactionDate", SqlDbType.DateTime) { Value = Convert.ToDateTime(ReturnVal(donation.TransactionDate)) });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = Convert.ToBoolean(ReturnVal(donation.IsActive)) });
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.CreatedBy)) });
                        command.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = Convert.ToString(ReturnVal(donation.UpdatedBy)) });
                        command.Parameters.Add(new SqlParameter("@PaidAmount", SqlDbType.Decimal)
                        {
                            Value = Convert.ToDecimal(ReturnVal(donation.PaidAmount)),
                            Precision = 18,
                            Scale = 2
                        });

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                    }
                }

                // 3. Now call InsertPaymentTransactionAsync
                var (paymentSuccess, paymentMsg) = await InsertDonationPaymentTransactionAsync(donation, prodList);

                if (paymentSuccess)
                {
                    res.ResponseCode = "200";
                    res.Msg = "Bill and payment saved successfully.";
                }
                else
                {
                    res.ResponseCode = "500";
                    // You might still want to say bill succeeded but payment failed
                    res.Msg = "Bill saved, but payment insertion failed: " + paymentMsg;
                }
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }

            return res;
        }
        private object ReturnVal(object value)
        {
            return value ?? DBNull.Value;
        }
        public async Task<(bool success, string message)> InsertDonationPaymentTransactionAsync(DonationMaster objPaymentInputModel, IEnumerable<prodDetails> prodList)
        {
            try
            {
               
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("InsertPaymentTransaction", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Setting parameters
                    cmd.Parameters.AddWithValue("@TxnDate",objPaymentInputModel.TransactionDate.HasValue? objPaymentInputModel.TransactionDate.Value.ToString("dd/MM/yyyy"): DBNull.Value);
                    // cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.TransactionDate.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Amount", objPaymentInputModel.PaidAmount);
                    cmd.Parameters.AddWithValue("@TransactionId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@Pmntmode", "Online");
                    cmd.Parameters.AddWithValue("@UserId", objPaymentInputModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@TrackId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@PaymentId", objPaymentInputModel.AtomTokenId ?? "");
                    cmd.Parameters.AddWithValue("@FeeIds", "");
                    cmd.Parameters.AddWithValue("@FamilyRegNo", objPaymentInputModel.FamilyRegistrationNo);
                    cmd.Parameters.AddWithValue("@FamilyHeadName", objPaymentInputModel.FamilyHeadName);
                    cmd.Parameters.AddWithValue("@FeeTitle", "Donation");
                    cmd.Parameters.AddWithValue("@FamilyId", objPaymentInputModel.FamilyId);
                    cmd.Parameters.AddWithValue("@churchId", objPaymentInputModel.churchId);

                    // Create JSON string of fee amounts
                    string feeAmountsJson = JsonConvert.SerializeObject(
                        prodList.Select(p => new { p.prodName, p.prodAmount })
                    );
                    cmd.Parameters.AddWithValue("@FeeAmounts", feeAmountsJson);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows < 0)
                    {
                        return (true, "Transaction inserted successfully.");
                    }
                    else
                    {
                        return (false, "No rows affected.");
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex); // agar aapka logger async hai
                return (false, $"Exception: {ex.Message}");
            }
        }
        public async Task<IApiResponse> SaveOrUpdateDomeboxDonationAsync(DomeboxDonationModel donation, IEnumerable<prodDetails> prodList)
        {
            var res = new ApiResponse();

            if (donation == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Donation details cannot be null.";
                return res;
            }

            if (donation.Amount <= 0)
            {
                res.ResponseCode = "400";
                res.Msg = "Donation amount must be greater than zero.";
                return res;
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("usp_InsertOrUpdate_DomeboxDonation", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@DonationId", SqlDbType.BigInt) { Value = donation.DonationId });
                command.Parameters.Add(new SqlParameter("@DonorName", SqlDbType.NVarChar) { Value = (object)donation.DonorName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DonorContact", SqlDbType.NVarChar) { Value = (object)donation.DonorContact ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DonorEmail", SqlDbType.NVarChar) { Value = (object)donation.DonorEmail ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Decimal) { Value = donation.Amount, Precision = 18, Scale = 2 });
                command.Parameters.Add(new SqlParameter("@Currency", SqlDbType.NVarChar) { Value = (object)donation.Currency ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DonationDate", SqlDbType.DateTime) { Value = (object)donation.DonationDate ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DomeboxId", SqlDbType.NVarChar) { Value = (object)donation.DomeboxId ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Location", SqlDbType.NVarChar) { Value = (object)donation.Location ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PaymentMode", SqlDbType.NVarChar) { Value = (object)donation.PaymentMode ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@ChequeNo", SqlDbType.NVarChar) { Value = (object)donation.ChequeNo ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@BankName", SqlDbType.NVarChar) { Value = (object)donation.BankName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@IFSCCode", SqlDbType.NVarChar) { Value = (object)donation.IFSCCode ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.NVarChar) { Value = (object)donation.Remarks ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@IsAnonymous", SqlDbType.Bit) { Value = donation.IsAnonymous });
                command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = donation.IsActive });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = (object)donation.CreatedBy ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = (object)donation.UpdatedBy ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@ChurchId", SqlDbType.Int) { Value = (object)donation.churchId ?? DBNull.Value });
                var rowsAffected = await command.ExecuteNonQueryAsync();
                res.ResponseCode = "200";
                res.Msg = "Donation saved successfully.";
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }
            var (paymentSuccess, paymentMsg) = await InsertDomeboxPaymentTransactionAsync(donation, prodList);

            return res;
        }
        public async Task<(bool success, string message)> InsertDomeboxPaymentTransactionAsync(DomeboxDonationModel objPaymentInputModel, IEnumerable<prodDetails> prodList)
        {
            try
            {

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("InsertPaymentTransaction", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Setting parameters
                    cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.DonationDate.HasValue ? objPaymentInputModel.DonationDate.Value.ToString("dd/MM/yyyy") : DBNull.Value);
                    // cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.TransactionDate.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Amount", objPaymentInputModel.Amount);
                    cmd.Parameters.AddWithValue("@TransactionId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@Pmntmode", "Online");
                    cmd.Parameters.AddWithValue("@UserId", objPaymentInputModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@TrackId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@PaymentId", objPaymentInputModel.AtomTokenId ?? "");
                    cmd.Parameters.AddWithValue("@FeeIds", "");
                    cmd.Parameters.AddWithValue("@FamilyRegNo", objPaymentInputModel.FamilyRegistrationNo);
                    cmd.Parameters.AddWithValue("@FamilyHeadName", objPaymentInputModel.FamilyHeadName);
                    cmd.Parameters.AddWithValue("@FeeTitle", "DomeBox");
                    cmd.Parameters.AddWithValue("@FamilyId", objPaymentInputModel.FamilyId);
                    cmd.Parameters.AddWithValue("@churchId", objPaymentInputModel.churchId);

                    // Create JSON string of fee amounts
                    string feeAmountsJson = JsonConvert.SerializeObject(
                        prodList.Select(p => new { p.prodName, p.prodAmount })
                    );
                    cmd.Parameters.AddWithValue("@FeeAmounts", feeAmountsJson);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows < 0)
                    {
                        return (true, "Transaction inserted successfully.");
                    }
                    else
                    {
                        return (false, "No rows affected.");
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex); // agar aapka logger async hai
                return (false, $"Exception: {ex.Message}");
            }
        }
        public async Task<IApiResponse> SaveOrUpdateCampaignAsync(CampaigningModel model)
        {
            var res = new ApiResponse();

            // 1. Validate input
            if (model == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Campaign details cannot be null.";
                return res;
            }

            if (string.IsNullOrWhiteSpace(model.CampaignName))
            {
                res.ResponseCode = "400";
                res.Msg = "Campaign name is required.";
                return res;
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("usp_InsertOrUpdate_Campaigning", connection);
                command.CommandType = CommandType.StoredProcedure;

                // 2. Add all parameters
                command.Parameters.Add(new SqlParameter("@CampaignId", SqlDbType.BigInt) { Value = model.CampaignId });
                command.Parameters.Add(new SqlParameter("@ChurchId", SqlDbType.Int) { Value = model.ChurchId });
                command.Parameters.Add(new SqlParameter("@CampaignName", SqlDbType.NVarChar) { Value = model.CampaignName });
                command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = (object)model.Description ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Date) { Value = model.StartDate });
                command.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Date) { Value = (object)model.EndDate ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@TargetAmount", SqlDbType.Decimal) { Value = model.TargetAmount, Precision = 18, Scale = 2 });
                command.Parameters.Add(new SqlParameter("@RaisedAmount", SqlDbType.Decimal) { Value = model.RaisedAmount, Precision = 18, Scale = 2 });
                command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = model.IsActive });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = model.CreatedBy });
                command.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = (object)model.UpdatedBy ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@AttachmentName", SqlDbType.NVarChar) {Value =(object?)model.AttachmentName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@AttachmentData", SqlDbType.VarBinary) { Value = (object?)model.AttachmentData ?? DBNull.Value });

                await command.ExecuteNonQueryAsync();

                res.ResponseCode = "200";
                res.Msg = "Campaign saved successfully.";
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }

            return res;
        }
        public async Task<IApiResponse> GetCampaign(int ChurchId)
        {
            var res = new ApiResponse();
            try
            {
                var list = new List<CampaigningModel>();
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetActiveCampaigning", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChurchId", ChurchId);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new CampaigningModel
                            {
                                CampaignId = Convert.ToInt64(reader["CampaignId"]),
                                ChurchId = Convert.ToInt32(reader["ChurchId"]),
                                CampaignName = reader["CampaignName"].ToString() ?? string.Empty,
                                Description = reader["Description"].ToString() ?? string.Empty,
                                StartDate = Convert.ToDateTime(reader["StartDate"]),//reader["StartDate"].ToString() ?? string.Empty,
                                EndDate = Convert.ToDateTime(reader["EndDate"]),//reader["EndDate"].ToString() ?? string.Empty,
                                TargetAmount = Convert.ToDecimal(reader["TargetAmount"]),//reader["TargetAmount"].ToString() ?? string.Empty,
                                RaisedAmount = Convert.ToDecimal(reader["RaisedAmount"]),//reader["RaisedAmount"].ToString() ?? string.Empty,
                                CreatedBy = reader["CreatedBy"].ToString() ?? string.Empty,
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]), //reader["CreatedDate"].ToString() ?? string.Empty,
                                UpdatedBy = reader["UpdatedBy"].ToString() ?? string.Empty,
                                UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedDate"]) : (DateTime?)null, //reader["UpdatedDate"].ToString() ?? string.Empty,
                                AttachmentName = reader["AttachmentName"].ToString() ?? string.Empty,
                                AttachmentData = reader["AttachmentData"] != DBNull.Value ? (byte[])reader["AttachmentData"] : null
                            });
                        }
                    }
                    res.Data= list;
                    res.ResponseCode = "200";
                }
            }
            catch (Exception ex) 
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        public async Task<IApiResponse> SaveOrUpdateCampaignDonationAsync(CampaignDonationModel donation, IEnumerable<prodDetails> prodList)
        {
            var res = new ApiResponse();

            if (donation == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Donation details cannot be null.";
                return res;
            }

            if (donation.Amount <= 0)
            {
                res.ResponseCode = "400";
                res.Msg = "Donation amount must be greater than zero.";
                return res;
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("usp_InsertOrUpdate_CampaignDonation", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@DonationId", SqlDbType.BigInt) { Value = donation.DonationId });
                command.Parameters.Add(new SqlParameter("@CampaignId", SqlDbType.BigInt) { Value = donation.CampaignId });
                command.Parameters.Add(new SqlParameter("@ChurchId", SqlDbType.Int) { Value = donation.ChurchId });
                command.Parameters.Add(new SqlParameter("@DonorName", SqlDbType.NVarChar) { Value = (object)donation.DonorName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DonorContact", SqlDbType.NVarChar) { Value = (object)donation.DonorContact ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DonorEmail", SqlDbType.NVarChar) { Value = (object)donation.DonorEmail ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Decimal) { Value = donation.Amount, Precision = 18, Scale = 2 });
                command.Parameters.Add(new SqlParameter("@Currency", SqlDbType.NVarChar) { Value = (object)donation.Currency ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DonationDate", SqlDbType.DateTime) { Value = (object)donation.DonationDate ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@PaymentMode", SqlDbType.NVarChar) { Value = (object)donation.PaymentMode ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@ChequeNo", SqlDbType.NVarChar) { Value = (object)donation.ChequeNo ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@BankName", SqlDbType.NVarChar) { Value = (object)donation.BankName ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@IFSCCode", SqlDbType.NVarChar) { Value = (object)donation.IFSCCode ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@TransactionNo", SqlDbType.NVarChar) { Value = (object)donation.TransactionNo ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.NVarChar) { Value = (object)donation.Remarks ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@IsAnonymous", SqlDbType.Bit) { Value = donation.IsAnonymous });
                command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = donation.IsActive });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = (object)donation.CreatedBy ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = (object)donation.UpdatedBy ?? DBNull.Value });

                var rowsAffected = await command.ExecuteNonQueryAsync();

                res.ResponseCode = "200";
                res.Msg = "Campaign donation saved successfully.";
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }

            // Optional: handle any related payment insert logic if required
            var (paymentSuccess, paymentMsg) = await InsertCampaignPaymentTransactionAsync(donation, prodList);

            return res;
        }
        public async Task<(bool success, string message)> InsertCampaignPaymentTransactionAsync(CampaignDonationModel objPaymentInputModel, IEnumerable<prodDetails> prodList)
        {
            try
            {

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("InsertPaymentTransaction", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Setting parameters
                    cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.DonationDate.HasValue ? objPaymentInputModel.DonationDate.Value.ToString("dd/MM/yyyy") : DBNull.Value);
                    // cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.TransactionDate.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Amount", objPaymentInputModel.Amount);
                    cmd.Parameters.AddWithValue("@TransactionId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@Pmntmode", "Online");
                    cmd.Parameters.AddWithValue("@UserId", objPaymentInputModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@TrackId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@PaymentId", objPaymentInputModel.AtomTokenId ?? "");
                    cmd.Parameters.AddWithValue("@FeeIds", "");
                    cmd.Parameters.AddWithValue("@FamilyRegNo", objPaymentInputModel.FamilyRegistrationNo);
                    cmd.Parameters.AddWithValue("@FamilyHeadName", objPaymentInputModel.FamilyHeadName);
                    cmd.Parameters.AddWithValue("@FeeTitle", "Campaign");
                    cmd.Parameters.AddWithValue("@FamilyId", objPaymentInputModel.FamilyId);
                    cmd.Parameters.AddWithValue("@churchId", objPaymentInputModel.ChurchId);

                    // Create JSON string of fee amounts
                    string feeAmountsJson = JsonConvert.SerializeObject(
                        prodList.Select(p => new { p.prodName, p.prodAmount })
                    );
                    cmd.Parameters.AddWithValue("@FeeAmounts", feeAmountsJson);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows < 0)
                    {
                        return (true, "Transaction inserted successfully.");
                    }
                    else
                    {
                        return (false, "No rows affected.");
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex); // agar aapka logger async hai
                return (false, $"Exception: {ex.Message}");
            }
        }
        public async Task<IApiResponse> InsertFailPaymentTransactionAsync(PaymentTransaction objPaymentInputModel)
        {
            var res = new ApiResponse();
            try
            {

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("InsertPaymentTransaction", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Setting parameters
                    cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.TxnDate);
                    // cmd.Parameters.AddWithValue("@TxnDate", objPaymentInputModel.TransactionDate.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Amount", objPaymentInputModel.Amount);
                    cmd.Parameters.AddWithValue("@TransactionId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@Pmntmode", "Online");
                    cmd.Parameters.AddWithValue("@UserId", objPaymentInputModel.UserId);
                    cmd.Parameters.AddWithValue("@TrackId", objPaymentInputModel.TrackId ?? "");
                    cmd.Parameters.AddWithValue("@PaymentId", objPaymentInputModel.AtomtokenId ?? "");
                    cmd.Parameters.AddWithValue("@FeeIds", "");
                    cmd.Parameters.AddWithValue("@FamilyRegNo", objPaymentInputModel.FamilyRegNo);
                    cmd.Parameters.AddWithValue("@FamilyHeadName", objPaymentInputModel.FamilyHeadName);
                    cmd.Parameters.AddWithValue("@FeeTitle", objPaymentInputModel.FeeTitle);
                    cmd.Parameters.AddWithValue("@FamilyId", objPaymentInputModel.FamilyId);
                    cmd.Parameters.AddWithValue("@churchId", objPaymentInputModel.ChurchId);

                    // Create JSON string of fee amounts
                    string feeAmountsJson = JsonConvert.SerializeObject(
                        objPaymentInputModel.prodDetails.Select(p => new { p.prodName, p.prodAmount })
                    );
                    cmd.Parameters.AddWithValue("@FeeAmounts", feeAmountsJson);

                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows < 0)
                    {
                        res.Msg = "Transaction inserted successfully.";
                        res.ResponseCode = "200";
                       // return (true, "Transaction inserted successfully.");
                    }
                    else
                    {
                        res.Msg = "No rows affected.";
                        res.ResponseCode = "200";
                        //  return (false, "No rows affected.");
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex); // agar aapka logger async hai
                res.Msg=ex.Message;
                res.ResponseCode = "500";
                //return (false, $"Exception: {ex.Message}");
            }
            return res;
        }

    }

    public class PayUSettings
    {
        public string AtomtechAuthurl { get; set; }
        public string AtomtechmerchId { get; set; }
        public string AtomtechEncrptkey { get; set; }
        public string AtomtechDecrptkey { get; set; }
        public string AtomtechResulturl { get; set; }
        public string AtomtechReturnUrl { get; set; }
        public string AtomtechRequestHashKey { get; set; }
        public string AtomtechResponseHashKey { get; set; }
        public string Password { get; set; }
       // public string  { get; set; }

    }
}
