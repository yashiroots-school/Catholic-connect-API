using ChurchAPI.Interface;
using ChurchAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChurchAPI.Services
{
    public class FamilyServices : IFamilyServices
    {

        private readonly string _connectionString;

        public FamilyServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IApiResponse> GetFamilyAllDetails(string churchName,string? nameOfTheFamilyHead = null,string? registrationNumber = null,string? phoneNo = null,int pageNumber = 1,int pageSize = 10,string? zoneCode = null,string? zoneName = null)
        {
            var res = new ApiResponse();
            var families = new List<ViewFamilyDetailsModel>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetAllFamilyRecords", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ChurchName", SqlDbType.NVarChar) { Value = churchName });
                    cmd.Parameters.Add(new SqlParameter("@NameOfTheFamilyHead", SqlDbType.NVarChar) { Value = (object?)nameOfTheFamilyHead ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@RegistrationNumber", SqlDbType.NVarChar) { Value = (object?)registrationNumber ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@PhoneNo", SqlDbType.NVarChar) { Value = (object?)phoneNo ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ZoneCode", SqlDbType.NVarChar) { Value = (object?)zoneCode ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ZoneName", SqlDbType.NVarChar) { Value = (object?)zoneName ?? DBNull.Value });
                    //cmd.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber });
                    //cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize });

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            families.Add(new ViewFamilyDetailsModel
                            {
                                HeadFamilyId = reader["HeadFamilyId"] != DBNull.Value ? Convert.ToInt32(reader["HeadFamilyId"]) : 0,
                                RegistrationNumber = reader["RegistrationNumber"]?.ToString() ?? string.Empty,
                                HeadName = reader["HeadName"]?.ToString() ?? string.Empty,
                                Ethnicity = reader["Ethnicity"]?.ToString() ?? string.Empty,
                                PhoneNumber = reader["PhoneNumber"]?.ToString() ?? string.Empty,
                                Zone = reader["Zone"]?.ToString() ?? string.Empty,
                                ZoneCode = reader["ZoneCode"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }

                res.Data = families;
                res.ResponseCode = "200";
                res.Msg = "Data fetched successfully!";
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "400";
                res.Msg = ex.Message;
            }

            return res;
        }
        public async Task<IApiResponse> GetLastYearDues(string churchName, int HeadFamilyId, string year, string Heading)
        {
            var res = new ApiResponse();
            var lastYearDues = new List<LastYearDues>();

            try
            {
                // Build SPParams string as comma-separated values
                
                

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetLastYearDuesAmount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@HeadFamilyId", SqlDbType.Int) { Value = HeadFamilyId });
                    cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.NVarChar) { Value = (object?)HeadFamilyId ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Heading", SqlDbType.NVarChar) { Value = (object?)Heading ?? DBNull.Value });
                   

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lastYearDues.Add(new LastYearDues
                            {
                                LastYearDue = reader["LastYearDues"] != DBNull.Value ? Convert.ToDecimal(reader["LastYearDues"]) : 0,
                                CurrentYearDue = reader["CurrentYearDues"] != DBNull.Value ? Convert.ToDecimal(reader["CurrentYearDues"]) : 0

                            });
                        }
                    }
                }

                res.Data = lastYearDues;
                res.ResponseCode = "200";
                res.Msg = "Data fetched successfully!";
            }
            catch (Exception ex)
            {
                ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "400";
                res.Msg = ex.Message;
            }

            return res;
        }

    }

}
