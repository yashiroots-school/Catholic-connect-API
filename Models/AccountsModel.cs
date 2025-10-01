using ChurchAPI.Interface;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace ChurchAPI.Models
{
    public class TokenResponse : IAuthTokenResponse
    {
        public string Token_type { get; set; } = null!;
        public string Access_token { get; set; } = null!;
        public long Expires_in { get; set; }
        public string UserRoleName { get; set; } = null!;
        public long UserId { get; set; }
        public string UserRoleId { get; set; } = null!;

    }


    public class TokenGenrationModel
    {
        public int UserId { get; set; } = 0;

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Id { get; set; }

        public string? DepartmentId { get; set; }
        // public ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();

    }

    public class UserLoginData
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserGuId { get; set; } = null!;
        //public int DepartmentId { get; set; }
        public string RoleName { get; set; } = null!;
        public int UserRoleId { get; set; } = 0;
        //public string ApplicationNo { get; set; } = null!;
        //public int StaffId { get; set; } = 0;
    }
}
