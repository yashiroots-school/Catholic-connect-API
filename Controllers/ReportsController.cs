using ChurchAPI.Interface;
using ChurchAPI.Models.ViewModels;
using ChurchAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsInterface _reportsInterface;
        IApiResponse res = null!;
        public ReportsController(IReportsInterface reports)
        {
            _reportsInterface = reports;
        }
        [HttpPost("SubscriptionsReport")]
        public async Task<IApiResponse> SubscriptionsReport(ReportFilterModel reportFilter)
        {
            res = new ApiResponse();
            try
            {
                var PaymentResultModelsResponse = await _reportsInterface.Subscription(reportFilter);
                res.Data = PaymentResultModelsResponse;
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("DonationReport")]
        public async Task<IApiResponse> DonationReport(DonationReportFilterModel reportFilter)
        {
            res = new ApiResponse();
            try
            {
                var PaymentResultModelsResponse = await _reportsInterface.DonationReport(reportFilter);
                res.Data = PaymentResultModelsResponse;
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("DomeBoxReport")]
        public async Task<IApiResponse> DomeBoxReport(ReportFilterModel reportFilter)
        {
            res = new ApiResponse();
            try
            {
                var PaymentResultModelsResponse = await _reportsInterface.DomeBoxReport(reportFilter);
                res.Data = PaymentResultModelsResponse;
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("CampaignReport")]
        public async Task<IApiResponse> CampaignReport(ReportFilterModel reportFilter)
        {
            res = new ApiResponse();
            try
            {
                var PaymentResultModelsResponse = await _reportsInterface.CampaignReport(reportFilter);
                res.Data = PaymentResultModelsResponse;
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
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
