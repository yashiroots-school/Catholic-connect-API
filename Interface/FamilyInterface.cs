using ChurchAPI.Models;

namespace ChurchAPI.Interface
{
    public interface IFamilyServices
    {
        Task<IApiResponse> GetFamilyAllDetails(string churchName, string? nameOfTheFamilyHead, string? registrationNumber, string? phoneNo, int pageNumber = 1, int pageSize = 10, string? zoneCode = null, string? zoneName = null);
        Task<IApiResponse> GetLastYearDues(string churchName, int HeadFamilyId, string year, string Heading);
    }
}
