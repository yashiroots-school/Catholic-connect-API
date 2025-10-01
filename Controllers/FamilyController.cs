using ChurchAPI.Interface;
using ChurchAPI.Models;
using ChurchAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyController : ControllerBase
    {
        private readonly IFamilyServices _familyservices;
        IApiResponse res = null!;
        public FamilyController(IFamilyServices familyservices)
        {
            _familyservices = familyservices;
        }
        [HttpPost("FamilyDetails")]
        public async Task<IApiResponse> GetFamilyAllDetails(string churchIds, string? nameOfTheFamilyHead, string? registrationNumber, string? phoneNo, int pageNumber = 1, int pageSize = 10, string? zoneCode = null, string? zoneName = null)
        {
            res = new ApiResponse();
            try
            {
                res = await _familyservices.GetFamilyAllDetails(churchIds, nameOfTheFamilyHead, registrationNumber, phoneNo, pageNumber, pageSize, zoneCode, zoneName);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("LastYearDues")]
        public async Task<IApiResponse> GetFamilyAllDetails(string churchIds, int HeadFamilyId, string year, string? Heading= "Subscription")
        {
            res = new ApiResponse();
            try
            {
                res = await _familyservices.GetLastYearDues(churchIds, HeadFamilyId, year, Heading);
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
