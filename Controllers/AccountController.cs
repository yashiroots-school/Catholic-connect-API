using ChurchAPI.Interface;
using ChurchAPI.Models;
using ChurchAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        IApiResponse res = null!;
        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }
        [HttpGet("GetDioceseDropDown")]
        public async Task<IApiResponse> GetDioceseDropDown()
        {
            res = new ApiResponse();
            try
            {
                res = await _accountServices.GetDioceseDrop();
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("ValidateUser")]
        public async Task<IApiResponse> ValidateUserFromSP([FromBody] EmployeeLoginParams EmpLoginData)
        {
            res = new ApiResponse();
            try
            {
                res = await _accountServices.ValidateUserFromSPAsync(EmpLoginData);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("RegisterUser")]
        public async Task<IApiResponse> RegisterUser([FromBody] UserLogin userlogin)
        {
            res = new ApiResponse();
            try
            {
                res = await _accountServices.InsertUser(userlogin);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
    }
}

