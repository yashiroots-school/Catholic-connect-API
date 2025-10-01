namespace ChurchAPI.Models
{
    public class UserLogin
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? FamilyId { get; set; }
        public long DioceseId { get; set; }
        public long ChurchId { get; set; }
        public string FamilyHeadName { get; set; } = string.Empty;
        public string? RoleName { get; set; }
        public long? RoleId { get; set; }
    }
}
