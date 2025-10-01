using ChurchAPI.Models;
using ChurchAPI.Models.ViewModels;

namespace ChurchAPI.Interface
{
    public interface IPaymentInterface
    {
        Task<PaymentResultModels> PreapareInput(PaymentInputModel objPaymentInputModel);
        Task<List<BillingYears>> GetBillingYearsAsync();
        Task<IApiResponse> GetBillHeadingsAsync(string churchName, string title);
        Task<decimal> GetBillTotalByBillHeadingAsync(string churchName, string billHeadingIds, string year);
    }
}
