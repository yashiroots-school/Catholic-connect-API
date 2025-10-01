using ChurchAPI.Interface;
using ChurchAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChurchAPI.Services
{
    public class AccountServices: IAccountServices
    {
        private readonly string _connectionString;
        private readonly DbLogger _logger;

        public AccountServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = new DbLogger(_connectionString);
        }
        public async Task<IApiResponse> GetDioceseDrop()
        {
            var res = new ApiResponse();
            List<DiocesesMasters> students = new List<DiocesesMasters>();
            string procName = "GetDioceseDrop";
            string parameters = null;//$"ClassId={classId}";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                   // command.Parameters.AddWithValue("@ClassId", classId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new DiocesesMasters
                            {
                                DioceseId = reader.GetInt64(reader.GetOrdinal("DioceseId")),
                                DioceseName = reader.GetString(reader.GetOrdinal("DioceseName"))
                            });
                        }
                    }
                }
                res.Data = students;
                res.ResponseCode = "200";
                // Log success response
               // await _logger.LogApiCallAsync(procName, parameters, $"Fetched {students.Count} students", userId);
            }
            catch (Exception ex)
            {
                res.Data=ex.Message;
                // Log failure response
                await _logger.LogApiCallAsync(procName, parameters, $"Error: {ex.Message}","0");
                res.ResponseCode = "500";
                throw;
            }

            return res;
        }
        public async Task<IApiResponse> GetMasterDropdown()
        {
            var res = new ApiResponse();
            List<Masters> students = new List<Masters>();
            string procName = "GetMasters";
            string parameters = null;//$"ClassId={classId}";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    // command.Parameters.AddWithValue("@ClassId", classId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new Masters
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Type = reader.GetString(reader.GetOrdinal("Type"))
                            });
                        }
                    }
                }
                res.Data = students;
                res.ResponseCode = "200";
                // Log success response
                // await _logger.LogApiCallAsync(procName, parameters, $"Fetched {students.Count} students", userId);
            }
            catch (Exception ex)
            {
                res.Data = ex.Message;
                // Log failure response
                await _logger.LogApiCallAsync(procName, parameters, $"Error: {ex.Message}", "0");
                res.ResponseCode = "500";
                throw;
            }

            return res;
        }
        public async Task<IApiResponse> GetRoles()
        {
            var res = new ApiResponse();
            List<Roles> students = new List<Roles>();
            string procName = "GetRoles";
            string parameters = null;//$"ClassId={classId}";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    // command.Parameters.AddWithValue("@ClassId", classId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new Roles
                            {
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                RolesName = reader.GetString(reader.GetOrdinal("RolesName"))
                            });
                        }
                    }
                }
                res.Data = students;
                res.ResponseCode = "200";
                // Log success response
                // await _logger.LogApiCallAsync(procName, parameters, $"Fetched {students.Count} students", userId);
            }
            catch (Exception ex)
            {
                res.Data = ex.Message;
                // Log failure response
                await _logger.LogApiCallAsync(procName, parameters, $"Error: {ex.Message}", "0");
                res.ResponseCode = "500";
                throw;
            }

            return res;
        }
        public async Task<IApiResponse> ChurchDetails(int DioceseId)
        {
            var res = new ApiResponse();
            List<ChurchDetails> students = new List<ChurchDetails>();
            string procName = "GetChurchuDetails";
            string parameters = null;//$"ClassId={classId}";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DioceseId", DioceseId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new ChurchDetails
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                ChurchName = (reader["ChurchName"]?.ToString() ?? string.Empty) + " " + (reader["Address"]?.ToString() ?? string.Empty),
                                Address = reader["Address"]?.ToString(),
                                ContactNo = reader["ContactNo"]?.ToString(),
                                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                                HeadName = reader["HeadName"]?.ToString(),
                                DioceseId = reader["DioceseId"] != DBNull.Value ? Convert.ToInt32(reader["DioceseId"]) : 0
                            });
                        }
                    }
                }
                res.Data = students;
                res.ResponseCode = "200";
                // Log success response
                // await _logger.LogApiCallAsync(procName, parameters, $"Fetched {students.Count} students", userId);
            }
            catch (Exception ex)
            {
                res.Data = ex.Message;
                // Log failure response
                await _logger.LogApiCallAsync(procName, parameters, $"Error: {ex.Message}", "0");
                res.ResponseCode = "500";
                throw;
            }

            return res;
        }
        public async Task<IApiResponse> ValidateUserFromSPAsync(IEmployeeLoginInterface EmpLogingData)
        {
            var res = new ApiResponse();
            List<UserLoginData> login = new List<UserLoginData>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("ValidateFamilyUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", EmpLogingData.UserName);
                    cmd.Parameters.AddWithValue("@Password", EmpLogingData.Password);
                    cmd.Parameters.AddWithValue("@FireBaseToken", EmpLogingData.FireBaseToken);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            login.Add(new UserLoginData
                            {
                                UserId = Convert.ToInt32(reader["ID"]),
                                UserName = reader["UserName"].ToString()!,
                                Email = reader["Email"].ToString()!,
                                RoleName = reader["RolesName"].ToString()!,
                                UserRoleId = reader["RoleId"] != DBNull.Value ? Convert.ToInt32(reader["RoleId"]) : 0,
                                FamilyRegNo= reader["FamilyRegNo"].ToString()!,
                                churchId = reader["ChurchId"] != DBNull.Value ? Convert.ToInt32(reader["ChurchId"]) : 0,
                                DioceseId = reader["DioceseId"] != DBNull.Value ? Convert.ToInt32(reader["DioceseId"]) : 0,
                                FamilyId = reader["FamilyId"] != DBNull.Value ? Convert.ToInt32(reader["FamilyId"]) : 0,
                                FireBaseToken = reader["FireBaseToken"] != DBNull.Value ? Convert.ToString(reader["FireBaseToken"]) : null,
                                FamilyHeadName = reader["FamilyHeadName"] != DBNull.Value ? Convert.ToString(reader["FamilyHeadName"]) : null,
                            });
                        }
                    }
                }
                IAuthTokenResponse token = GenerateJsonWebToken(login.FirstOrDefault());
                if (token != null)
                {
                    // var tokenResponse = GenerateJsonWebToken(login.FirstOrDefault());
                    res.Data = login;
                    res.ResponseCode = "200";
                    res.AdditionalData = token;
                    res.Msg = "Token Generated Successfully!";
                }
                else
                {
                    res.Data = login;
                    res.ResponseCode = "200";
                    res.Msg = "Token Not Generated!";
                    //res.AdditionalData = token;

                }
                return res;
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        public IAuthTokenResponse GenerateJsonWebToken(UserLoginData userData)
        {
            IAuthTokenResponse TokenRes = new TokenResponse();
            try
            {
                var securityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("This_is_Lumen_Seceret_Key_For_Jwt"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var exp = DateTime.UtcNow.AddDays(1);

                // ✅ Build claims dynamically
                var claims = new List<Claim>
        {
            new Claim("Email", userData.Email ?? string.Empty),
            new Claim("UserId", userData.UserId.ToString()),
            new Claim("UserName", userData.UserName ?? string.Empty),
            new Claim("RoleName", userData.RoleName ?? string.Empty),
            new Claim(ClaimTypes.Role, userData.UserRoleId.ToString())
        };

                // Additional claims per role
                //if (userData.RoleName == "Student" && !string.IsNullOrEmpty(userData.ApplicationNo))
                //{
                //    claims.Add(new Claim("ApplicationNo", userData.ApplicationNo));
                //}
                //else if (userData.RoleName != "Administrator" && userData.StaffId > 0)
                //{
                //    claims.Add(new Claim("StaffId", userData.StaffId.ToString()));
                //}

                // ✅ Generate token
                var token = new JwtSecurityToken(
                    issuer: "issuer",
                    audience: "issuer",
                    claims: claims,
                    expires: exp,
                    signingCredentials: credentials
                );

                TokenRes.Access_token = new JwtSecurityTokenHandler().WriteToken(token);
                TokenRes.Expires_in = (int)(exp - DateTime.UtcNow).TotalSeconds; // ✅ correct lifetime
                TokenRes.Token_type = "Bearer";
                TokenRes.UserRoleId = userData.UserRoleId.ToString();
                TokenRes.UserRoleName = userData.RoleName;
                TokenRes.UserId = userData.UserId;
            }
            catch (Exception)
            {
                TokenRes = new TokenResponse();
            }

            return TokenRes;
        }
        public async Task<IApiResponse> InsertUser(UserLogin request)
        {

            var response = new ApiResponse();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("InsertUserLogin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserName", request.UserName);
                    cmd.Parameters.AddWithValue("@Email", request.Email);
                    cmd.Parameters.AddWithValue("@Password", request.Password);
                    cmd.Parameters.AddWithValue("@FamilyId", (object?)request.FamilyId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DioceseId", request.DioceseId);
                    cmd.Parameters.AddWithValue("@ChurchId", request.ChurchId);
                    cmd.Parameters.AddWithValue("@FamilyHeadName", request.FamilyHeadName);
                    cmd.Parameters.AddWithValue("@RoleName", (object?)request.RoleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleId", (object?)request.RoleId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FamilyRegNo", (object?)request.FamilyRegNo ?? DBNull.Value);
                    await conn.OpenAsync();

                    // Execute the stored procedure
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            //response.Success = true;
                            response.Msg = reader["Message"].ToString() ?? "Success";
                            response.ResponseCode = "200";
                        }
                    }
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
