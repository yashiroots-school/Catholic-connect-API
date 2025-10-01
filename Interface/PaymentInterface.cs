using ChurchAPI.Models;
using ChurchAPI.Models.ViewModels;
using Payrequest;

namespace ChurchAPI.Interface
{
    public interface IPaymentInterface
    {
        Task<PaymentResultModels> PreapareInput(PaymentInputModel objPaymentInputModel);
        Task<List<BillingYears>> GetBillingYearsAsync();
        Task<IApiResponse> GetBillHeadingsAsync(string churchName, string title);
        Task<decimal> GetBillTotalByBillHeadingAsync(string churchName, string billHeadingIds, string year);
        Task<IApiResponse> GetmarchentDetails(long churchId);
        Task<IApiResponse> SaveOrUpdateSubscriptionBillAndPaymentAsync(BillDetailsViweModel detailsBilling, IEnumerable<prodDetails> prodList);
        Task<IApiResponse> SaveOrUpdateDonationBillAndPaymentAsync(DonationMaster donation, IEnumerable<prodDetails> prodList);
        Task<IApiResponse> SaveOrUpdateCampaignDonationAsync(CampaignDonationModel donation, IEnumerable<prodDetails> prodList);
        Task<IApiResponse> SaveOrUpdateDomeboxDonationAsync(DomeboxDonationModel donation, IEnumerable<prodDetails> prodList);
        Task<IApiResponse> SaveOrUpdateCampaignAsync(CampaigningModel model);
        Task<IApiResponse> GetCampaign(int ChurchId);
        Task<IApiResponse> InsertFailPaymentTransactionAsync(PaymentTransaction objPaymentInputModel);
    }
}
