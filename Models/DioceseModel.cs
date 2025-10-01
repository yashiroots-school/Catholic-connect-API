namespace ChurchAPI.Models
{
    public class DiocesesMasters
    {
        public long DioceseId { get; set; }   // bigint
        public string DioceseName { get; set; } = string.Empty;  // nvarchar
        public string DioceseCode { get; set; } = string.Empty;  // nvarchar
        public string DioceseEstdYear { get; set; } = string.Empty; // nvarchar
        public string DioceseAddress { get; set; } = string.Empty; // nvarchar
        public long DioceseCity { get; set; }   // bigint (FK expected)
        public long DioceseState { get; set; }  // bigint (FK expected)
        public long DioceseCountry { get; set; } // bigint (FK expected)
        public string DioceseContactNo { get; set; } = string.Empty; // nvarchar
        public string DioceseEmail { get; set; } = string.Empty; // nvarchar

        public bool IsActive { get; set; }  // bit
    }
    public class Roles
    { 
        public int RoleID { get; set; }
        public string RolesName { get; set; }

    }
    public class Masters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

    }
}
