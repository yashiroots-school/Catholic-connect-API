using ChurchAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChurchAPI.Services
{
    public class MerchantRepository
    {
        private readonly string _connectionString;

        public MerchantRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public CreateMerchantModel? GetMerchant(long churchId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetMerchantDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChurchId", churchId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CreateMerchantModel
                            {
                                Merchant_Id = Convert.ToInt64(reader["Merchant_Id"]),
                                MerchantMID = reader["MerchantMID"].ToString(),
                                Password = reader["Password"].ToString(),
                                UserId = Convert.ToInt64(reader["UserId"]),
                                Bank_Id = Convert.ToInt64(reader["Bank_Id"]),
                                Branch_Id = Convert.ToInt64(reader["Branch_Id"]),
                                MerchantName_Id = Convert.ToInt64(reader["MerchantName_Id"]),
                                MerchantName = reader["MerchantName"].ToString(),
                            };
                        }
                    }
                }
            }
            return null;
        }
    }

}
