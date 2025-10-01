using ChurchAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ChurchAPI.Interface
{
    public interface IAccountServices
    {
        Task<IApiResponse> GetDioceseDrop();
        Task<IApiResponse> GetMasterDropdown();
        Task<IApiResponse> ValidateUserFromSPAsync(IEmployeeLoginInterface EmpLogingData);
        Task<IApiResponse> InsertUser(UserLogin request);
        Task<IApiResponse> ChurchDetails(int DioceseId);
        Task<IApiResponse> GetRoles();
    }
    public class EmployeeLoginParams : IEmployeeLoginInterface
    {
        public required string UserName { get; set; }


        public required string Password { get; set; }
        public required string FireBaseToken { get; set; }
    }
    internal interface AuthInterface
    {
        string UserName { get; }

        string Email { get; }
        string Password { get; }
        string Description { get; }
    }
    public interface IAuthTokenResponse
    {
        public string Token_type { get; set; }
        public string Access_token { get; set; }
        public long Expires_in { get; set; }
        public long UserId { get; set; }
        public string UserRoleName { get; set; }
        public string UserRoleId { get; set; }
    }


    public interface IEmployeeLoginInterface
    {
        string UserName { get; set; }

        string Password { get; set; }
        string FireBaseToken { get; set; }
    }


    public interface IRestPassword
    {
        string? Email { get; set; }
    }


    public interface ICreatePassword
    {
        //string? Email { get; set; }
        //string? UserId { get; set; }
        //string? Password { get; set; }
        //string? ConfirmPassword { get; set; }


        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public string? UserId { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }



        public string? UserName { get; set; }

        public string? Description { get; set; }
    }
}
