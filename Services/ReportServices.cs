using ChurchAPI.Interface;
using ChurchAPI.Models;
using ChurchAPI.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace ChurchAPI.Services
{
    public class ReportServices: IReportsInterface
    {
        private readonly string _connectionString;
        private readonly DbLogger _logger;

        public ReportServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = new DbLogger(_connectionString);
        }
        public async Task<IApiResponse> Subscription(ReportFilterModel request)
        {

            var response = new ApiResponse();
            List<SubscriprtionReportModel> subscriprtionReportModels = new List<SubscriprtionReportModel>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetFamilySubscriptionReportsByDiocese", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FromDateStr", request.FromDate);
                    cmd.Parameters.AddWithValue("@ToDateStr", request.ToDate);
                    cmd.Parameters.AddWithValue("@PaymentModeId", (object ?)request.PaymentMode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Year", (object?)request.Year ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FamilyRegId", (object?)request.FamilyNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DioceseId", (object?)request.DioceseName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChurchId", (object?)request.ChurchName ?? DBNull.Value);
                    await conn.OpenAsync();

                    // Execute the stored procedure
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            subscriprtionReportModels.Add(new SubscriprtionReportModel
                            {
                                BillingId = reader["BillingId"] != DBNull.Value ? Convert.ToInt32(reader["BillingId"]) : 0,
                                CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : DateTime.MinValue,
                                MemberId = reader["RegistrationNumber"] != DBNull.Value ? reader["RegistrationNumber"].ToString() : null,
                                Year = reader["Year"] != DBNull.Value ? Convert.ToInt32(reader["Year"]) : 0,
                                PaymentMode = reader["PaymentMode"] != DBNull.Value ? reader["PaymentMode"].ToString() : null,
                                MainHeading = reader["MainHeading"] != DBNull.Value ? reader["MainHeading"].ToString() : null,
                                Total = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0,
                                BillAmount = reader["BillAmount"] != DBNull.Value ? Convert.ToDecimal(reader["BillAmount"]) : 0,
                                PaidAmount = reader["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PaidAmount"]) : 0,
                                Discount = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : 0,
                                DueAmount = reader["DueAmount"] != DBNull.Value ? Convert.ToDecimal(reader["DueAmount"]) : 0,
                                SubHeadings = reader["SubHeadings"] != DBNull.Value ? reader["SubHeadings"].ToString() : null,
                                BillingYear = reader["BillingYear"] != DBNull.Value ? reader["BillingYear"].ToString() : null,
                                HeadName = reader["HeadName"] != DBNull.Value ? reader["HeadName"].ToString() : null,
                                //DioceseId = reader["DioceseId"] != DBNull.Value ? Convert.ToInt64(reader["DioceseId"]) : 0,
                                ChurchName = reader["ChurchName"] != DBNull.Value ? reader["ChurchName"].ToString() : null
                            });
                        }
                    }
                    // ✅ Date filter after fetching data
                    

                    response.Data = subscriprtionReportModels;
                    response.ResponseCode = "200";
                }
            }
            catch (SqlException ex)
            {
                //response.Success = false;
                response.Msg = ex.Message;
                response.ResponseCode = "500";
            }

            return response;
        }
        public async Task<IApiResponse> DonationReport(DonationReportFilterModel request)
        {

            var response = new ApiResponse();
            List<DonationMaster> donations = new List<DonationMaster>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetDonarDetailswithDiocese", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DonarName", (object?)request.DonarName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DonarContact", (object?)request.DonarContact ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DonarEmail", (object?)request.DonarEmail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DonarAddress", (object?)request.DonarAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaidAmount", (object?)request.PaidAmount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentMode", (object?)request.PaymentMode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", (object?)request.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DioceseId", (object?)request.DioceseName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChurchId", (object?)request.ChurchName ?? DBNull.Value);
                    await conn.OpenAsync();

                    // Execute the stored procedure
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            donations.Add(new DonationMaster
                            {
                                DonationId = reader["DonationId"] != DBNull.Value ? Convert.ToInt64(reader["DonationId"]) : 0,
                                DonarName = reader["DonarName"] != DBNull.Value ? reader["DonarName"].ToString() : null,
                                DonarContact = reader["DonarContact"] != DBNull.Value ? reader["DonarContact"].ToString() : null,
                                DonarEmail = reader["DonarEmail"] != DBNull.Value ? reader["DonarEmail"].ToString() : null,
                                DonarAddress = reader["DonarAddress"] != DBNull.Value ? reader["DonarAddress"].ToString() : null,
                                DonarCountry = reader["DonarCountry"] != DBNull.Value ? reader["DonarCountry"].ToString() : null,
                                DonarPANCardNo = reader["DonarPANCardNo"] != DBNull.Value ? reader["DonarPANCardNo"].ToString() : null,
                                DonarGSTNo = reader["DonarGSTNo"] != DBNull.Value ? reader["DonarGSTNo"].ToString() : null,
                                DonationType = reader["DonationType"] != DBNull.Value ? reader["DonationType"].ToString() : null,
                                PaymentMode = reader["PaymentMode"] != DBNull.Value ? reader["PaymentMode"].ToString() : null,
                                PaidAmount = reader["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PaidAmount"]) : 0,
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                                ChequeNo = reader["ChequeNo"] != DBNull.Value ? reader["ChequeNo"].ToString() : null,
                                BankName = reader["BankName"] != DBNull.Value ? reader["BankName"].ToString() : null,
                                IFSCCode = reader["IFSCCode"] != DBNull.Value ? reader["IFSCCode"].ToString() : null,
                                TransactionNo = reader["TransactionNo"] != DBNull.Value ? reader["TransactionNo"].ToString() : null,
                                TransactionDate = reader["TransactionDate"] != DBNull.Value ? Convert.ToDateTime(reader["TransactionDate"]) : (DateTime?)null,
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : (DateTime?)null,
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : null,
                                UpdatedBy = reader["UpdatedBy"] != DBNull.Value ? reader["UpdatedBy"].ToString() : null,
                                UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedDate"]) : (DateTime?)null,
                                //churchId = reader["ChurchId"] != DBNull.Value ? Convert.ToInt32(reader["ChurchId"]) : 0,
                                //DioceseId = reader["DioceseId"] != DBNull.Value ? Convert.ToInt32(reader["DioceseId"]) : 0
                            });
                        }

                    }
                    DateTime fromDate = DateTime.ParseExact(request.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDate = DateTime.ParseExact(request.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    donations = donations.Where(x => x.TransactionDate.HasValue && x.TransactionDate.Value.Date >= fromDate.Date && x.TransactionDate.Value.Date <= toDate.Date).ToList();
                    response.Data = donations;
                    response.ResponseCode = "200";
                }
            }
            catch (SqlException ex)
            {
                //response.Success = false;
                response.Msg = ex.Message;
                response.ResponseCode = "500";
            }

            return response;
        }
        public async Task<IApiResponse> DomeBoxReport(ReportFilterModel request)
        {

            var response = new ApiResponse();
            List<DomeBoxReportModel> domeBoxReports = new List<DomeBoxReportModel>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetDomeboxDonationsDiocese", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FromDateStr", (object?)request.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDateStr", (object?)request.ToDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChurchId", (object?)request.ChurchName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DioceseId", (object?)request.DioceseName ?? DBNull.Value);
                   
                    await conn.OpenAsync();

                    // Execute the stored procedure
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            domeBoxReports.Add(new DomeBoxReportModel
                            {
                                DonationId = reader["DonationId"] != DBNull.Value ? Convert.ToInt64(reader["DonationId"]) : 0,
                                DonarName = reader["DonorName"] != DBNull.Value ? reader["DonorName"].ToString() : null,
                                DonarContact = reader["DonorContact"] != DBNull.Value ? reader["DonorContact"].ToString() : null,
                                DonarEmail = reader["DonorEmail"] != DBNull.Value ? reader["DonorEmail"].ToString() : null,
                                ChurchName = reader["ChurchName"] != DBNull.Value ? reader["ChurchName"].ToString() : null,
                                PaymentMode = reader["PaymentMode"] != DBNull.Value ? reader["PaymentMode"].ToString() : null,
                                DonationAmount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0,
                                Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : null,
                                DomeBoxName = reader["DomeBoxName"] != DBNull.Value ? reader["DomeBoxName"].ToString() : null,
                                
                                CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : (DateTime?)null,
                               
                            });
                        }

                    }
                    response.Data = domeBoxReports;
                    response.ResponseCode = "200";
                }
            }
            catch (SqlException ex)
            {
                //response.Success = false;
                response.Msg = ex.Message;
                response.ResponseCode = "500";
            }

            return response;
        }
        public async Task<IApiResponse> CampaignReport(ReportFilterModel request)
        {

            var response = new ApiResponse();
            List<CampaignReportModel> campaignReports = new List<CampaignReportModel>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("CapaignReports", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FromDateStr", (object?)request.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDateStr", (object?)request.ToDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChurchId", (object?)request.ChurchName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DioceseId", (object?)request.DioceseName ?? DBNull.Value);

                    await conn.OpenAsync();

                    // Execute the stored procedure
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            campaignReports.Add(new CampaignReportModel
                            {
                                DonationId = reader["DonationId"] != DBNull.Value ? Convert.ToInt64(reader["DonationId"]) : 0,
                                DonarName = reader["DonorName"] != DBNull.Value ? reader["DonorName"].ToString() : null,
                                DonarContact = reader["DonorContact"] != DBNull.Value ? reader["DonorContact"].ToString() : null,
                                DonarEmail = reader["DonorEmail"] != DBNull.Value ? reader["DonorEmail"].ToString() : null,
                                ChurchName = reader["ChurchName"] != DBNull.Value ? reader["ChurchName"].ToString() : null,
                                PaymentMode = reader["PaymentMode"] != DBNull.Value ? reader["PaymentMode"].ToString() : null,
                                DonationAmount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0,
                                Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : null,
                                CampaignName = reader["CampaignName"] != DBNull.Value ? reader["CampaignName"].ToString() : null,

                                CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : (DateTime?)null,

                            });
                        }

                    }
                    response.Data = campaignReports;
                    response.ResponseCode = "200";
                }
            }
            catch (SqlException ex)
            {
                //response.Success = false;
                response.Msg = ex.Message;
                response.ResponseCode = "500";
            }

            return response;
        }
    }
}
