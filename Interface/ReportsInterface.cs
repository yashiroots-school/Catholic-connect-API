using ChurchAPI.Models.ViewModels;
using ChurchAPI.Models;

namespace ChurchAPI.Interface
{
    public interface IReportsInterface
    {
        Task<IApiResponse> Subscription(ReportFilterModel request);
        Task<IApiResponse> DonationReport(DonationReportFilterModel request);
        Task<IApiResponse> DomeBoxReport(ReportFilterModel request);
        Task<IApiResponse> CampaignReport(ReportFilterModel request);
    }
}
