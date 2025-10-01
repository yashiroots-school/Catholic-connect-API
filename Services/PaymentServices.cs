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

namespace ChurchAPI.Services
{
    public class PaymentServices: IPaymentInterface
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "http://atom.in?";
        private readonly Models.ViewModels.EncrypDecrpt _encrypDecrpt;
        public PaymentServices(IConfiguration configuration, EncrypDecrpt encrypDecrpt)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new Exception("Connection string not found.");
            _encrypDecrpt = encrypDecrpt;
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
                objPayverify = await PaymentApiCallAsync(paymentResultModels);

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
        public async Task<Payverify.Payverify> PaymentApiCallAsync(PaymentResultModels paymentResultModels)
        {
            try
            {
                // Map request data
                Payrequest.Payrequest objre = new Payrequest.Payrequest(_connectionString);
                var mapdata = objre.RequestMap(paymentResultModels, paymentResultModels.ChurchId);
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

    }
}
