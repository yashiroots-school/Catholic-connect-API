using ChurchAPI.Interface;
using ChurchAPI.Models;
using ChurchAPI.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChurchAPI.Services
{
    public class DashBoardServices: IDashBoardInterface
    {

        private readonly string _connectionString;
        private readonly DbLogger _logger;
        public DashBoardServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = new DbLogger(_connectionString);
        }
        public async Task<IApiResponse> GetCalendarEvent(string churchName)
        {
            var result = new List<CalendarEvents>();
            var res = new ApiResponse();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetCalendarEvent", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@churchId", churchName);

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new CalendarEvents
                            {
                                ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                EventName = reader["EventName"]?.ToString() ?? string.Empty,
                                EventDate = Convert.ToDateTime(reader["EventDate"]),
                                EventType = reader["EventType"].ToString() ?? string.Empty,
                                ChurchId = reader["ChurchId"] != DBNull.Value ? Convert.ToInt32(reader["ChurchId"]) : 0,
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : 0,
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                            });
                        }
                    }
                }
                res.Data = result;
                res.ResponseCode = "200";
                res.Msg = "Data fetched Successfully!";
            }
            catch (Exception ex)
            {
                // Log exception
                ErrorLogException.LogErrorAsync(ex);
                res.Msg = ex.Message;
                res.ResponseCode = "400";

            }

            return res;
        }
        public async Task<IApiResponse> SaveOrUpdateCalendarEventAsync(CalendarEvents model)
        {
            var res = new ApiResponse();

            // 1. Validate input
            if (model == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Event details cannot be null.";
                return res;
            }

            if (string.IsNullOrWhiteSpace(model.EventName))
            {
                res.ResponseCode = "400";
                res.Msg = "Event name is required.";
                return res;
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("InsertOrUpdateCalendarEvent", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // 2. Add parameters
                command.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = model.ID }); // 0 for insert
                command.Parameters.Add(new SqlParameter("@ChurchId", SqlDbType.Int) { Value = model.ChurchId });
                command.Parameters.Add(new SqlParameter("@EventName", SqlDbType.NVarChar, 255) { Value = model.EventName });
                command.Parameters.Add(new SqlParameter("@EventDate", SqlDbType.Date) { Value = model.EventDate });
                command.Parameters.Add(new SqlParameter("@EventType", SqlDbType.NVarChar) { Value = model.EventType });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = model.CreatedBy });
               // command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = model.IsActive });

                // 3. Execute and get returned ID
                var newId = await command.ExecuteScalarAsync(); // returns NewID or UpdatedID from SP

                res.ResponseCode = "200";
                res.Msg = "Event saved successfully.";
                res.Data = newId; // optional: return inserted/updated ID
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }

            return res;
        }
        public async Task<IApiResponse> GetCalendarEventByDate(string churchName, string Date)
        {
            var result = new List<CalendarEvents>();
            var res = new ApiResponse();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetCalenderEventByDate", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChurchId", churchName);
                    cmd.Parameters.AddWithValue("@EventDate", Date);

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new CalendarEvents
                            {
                                ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                EventName = reader["EventName"]?.ToString() ?? string.Empty,
                                EventDate = Convert.ToDateTime(reader["EventDate"]),
                                EventType = reader["EventType"].ToString()??string.Empty,
                                ChurchId = reader["ChurchId"] != DBNull.Value ? Convert.ToInt32(reader["ChurchId"]) : 0,
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : 0,
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                            });
                        }
                    }
                }
                res.Data = result;
                res.ResponseCode = "200";
                res.Msg = "Data fetched Successfully!";
            }
            catch (Exception ex)
            {
                // Log exception
                ErrorLogException.LogErrorAsync(ex);
                res.Msg = ex.Message;
                res.ResponseCode = "400";

            }

            return res;
        }
        public async Task<IApiResponse> SaveOrUpdateDailyReadingAsync(DailyReadings model)
        {
            var res = new ApiResponse();

            // 1. Validate input
            if (model == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Reading details cannot be null.";
                return res;
            }

            if (string.IsNullOrWhiteSpace(model.Reading))
            {
                res.ResponseCode = "400";
                res.Msg = "Reading is required.";
                return res;
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("InsertOrUpdateDailyReading", connection);
                command.CommandType = CommandType.StoredProcedure;

                // 2. Add all parameters
                command.Parameters.Add(new SqlParameter("@ID", SqlDbType.BigInt) { Value = model.ID });
                command.Parameters.Add(new SqlParameter("@ChurchId", SqlDbType.Int) { Value = model.ChurchId });
                command.Parameters.Add(new SqlParameter("@Reading", SqlDbType.NVarChar) { Value = model.Reading });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = model.CreatedBy });

                await command.ExecuteNonQueryAsync();

                res.ResponseCode = "200";
                res.Msg = "Daily Reading saved successfully.";
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }

            return res;
        }
        public async Task<IApiResponse> GetDailyReading(string churchName,string? ReadingDate=null)
        {
            var result = new List<DailyReadings>();
            var res = new ApiResponse();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("GetDailyReadings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@churchName", churchName);
                    cmd.Parameters.AddWithValue("@ReadingDate", ReadingDate);

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new DailyReadings
                            {
                                ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                Reading = reader["Reading"]?.ToString() ?? string.Empty,
                                ChurchId = reader["ChurchId"] != DBNull.Value ? Convert.ToInt32(reader["ChurchId"]) : 0,
                                CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : 0,
                                CreatedDate = Convert.ToDateTime(reader["Createdate"]),
                            });
                        }
                    }
                }
                res.Data = result;
                res.ResponseCode = "200";
                res.Msg = "Data fetched Successfully!";
            }
            catch (Exception ex)
            {
                // Log exception
                ErrorLogException.LogErrorAsync(ex);
                res.Msg = ex.Message;
                res.ResponseCode = "400";

            }

            return res;
        }
        public async Task<IApiResponse> SaveOrUpdateNotice(NoticeMaster model)
        {
            var res = new ApiResponse();

            // 1. Validate input
            if (model == null)
            {
                res.ResponseCode = "400";
                res.Msg = "Notice details cannot be null.";
                return res;
            }

            if (string.IsNullOrWhiteSpace(model.NoticeName))
            {
                res.ResponseCode = "400";
                res.Msg = "Notice Name is required.";
                return res;
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("InsertOrUpdateNotice", connection);
                command.CommandType = CommandType.StoredProcedure;

                // 2. Add all parameters
                command.Parameters.Add(new SqlParameter("@NoticeId", SqlDbType.BigInt) { Value = model.NoticeId });
                command.Parameters.Add(new SqlParameter("@ChurchId", SqlDbType.Int) { Value = model.ChurchId });
                command.Parameters.Add(new SqlParameter("@NoticeName", SqlDbType.NVarChar) { Value = model.NoticeName });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = model.CreatedBy });
                command.Parameters.Add(new SqlParameter("@DioceseId", SqlDbType.Int) { Value = model.DioceseId });
                command.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.Date) { Value = model.FromDate });
                command.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.Date) { Value = model.ToDate });
                command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = model.IsActive });
                await command.ExecuteNonQueryAsync();

                res.ResponseCode = "200";
                res.Msg = "Notice saved successfully.";
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.ResponseCode = "500";
                res.Msg = "An error occurred: " + ex.Message;
            }

            return res;
        }
        public async Task<IApiResponse> GetNoticesAsync(int? churchId = null, int? dioceseId = null)
        {
            var result = new List<NoticeMaster>();
            var res = new ApiResponse();

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await using var cmd = new SqlCommand("GetNoticesForChurchOrDiocese", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters
                cmd.Parameters.AddWithValue("@ChurchId", churchId.HasValue ? churchId.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DioceseId", dioceseId.HasValue ? dioceseId.Value : (object)DBNull.Value);

                await conn.OpenAsync();

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    result.Add(new NoticeMaster
                    {
                        NoticeId = reader["NoticeId"] != DBNull.Value ? Convert.ToInt64(reader["NoticeId"]) : 0,
                        NoticeName = reader["NoticeName"]?.ToString() ?? string.Empty,
                        DioceseId = reader["DioceseId"] != DBNull.Value ? Convert.ToInt32(reader["DioceseId"]) : (int?)null,
                        ChurchId = reader["ChurchId"] != DBNull.Value ? Convert.ToInt32(reader["ChurchId"]) : (int?)null,
                        CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : 0,
                        CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : DateTime.MinValue,
                        FromDate = reader["FromDate"] != DBNull.Value ? Convert.ToDateTime(reader["FromDate"]) : (DateTime?)null,
                        ToDate = reader["ToDate"] != DBNull.Value ? Convert.ToDateTime(reader["ToDate"]) : (DateTime?)null,
                        IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                    });
                }

                res.Data = result;
                res.ResponseCode = "200";
                res.Msg = "Notices fetched successfully!";
            }
            catch (Exception ex)
            {
                await ErrorLogException.LogErrorAsync(ex);
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }

            return res;
        }

    }
}
