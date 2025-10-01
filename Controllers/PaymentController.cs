using ChurchAPI.Interface;
using ChurchAPI.Models;
using ChurchAPI.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payrequest;

namespace ChurchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentInterface _paymentInterface;
        IApiResponse res = null!;
        public PaymentController(IPaymentInterface payment)
        {
            _paymentInterface = payment;
        }
        [HttpPost("PrepareInput")]
        public async Task<IApiResponse> PrepareInpur(PaymentInputModel objPaymentInputModel)
        {
            res = new ApiResponse();
            try
            {
                var PaymentResultModelsResponse = await _paymentInterface.PreapareInput(objPaymentInputModel);
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
        [HttpPost("GetBillingYear")]
        public async Task<IApiResponse> GetBillingYear()
        {
            res = new ApiResponse();
            try
            {
                var year = await _paymentInterface.GetBillingYearsAsync();
                res.Data = year;
                res.ResponseCode = "200";
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("GetBillHeading")]
        public async Task<IApiResponse> GetBillHeading(string churchName, string title)
        {
            res = new ApiResponse();
            try
            {
                var BillHeading = await _paymentInterface.GetBillHeadingsAsync(churchName, title);
                res.Data = BillHeading;
                res.ResponseCode = "200";
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("GetBillTotalByBillHeading")]
        public async Task<IApiResponse> GetBillHeadingAmounts(string churchName, string billHeadingIds, string year)
        {
            res = new ApiResponse();
            try
            {
                var BillHeadingTotal = await _paymentInterface.GetBillTotalByBillHeadingAsync(churchName, billHeadingIds, year);
                res.Data = BillHeadingTotal;
                res.ResponseCode = "200";
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("GetMerchentDetails")]
        public async Task<IApiResponse> GetMerchentDetails(long churchId)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.GetmarchentDetails(churchId);
             
                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveOrUpdateSubscriptionBillAndPaymentAsync")]
        public async Task<IApiResponse> SaveOrUpdateSubscriptionBillAndPaymentAsync(SubscriptionPaymentRequest subscriptionPaymentRequest)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.SaveOrUpdateSubscriptionBillAndPaymentAsync(subscriptionPaymentRequest.BillingDetails, subscriptionPaymentRequest.ProdList);

                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveOrUpdateDonationBillAndPaymentAsync")]
        public async Task<IApiResponse> SaveOrUpdateDonationBillAndPaymentAsync(DonationPaymentRequest subscriptionPaymentRequest)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.SaveOrUpdateDonationBillAndPaymentAsync(subscriptionPaymentRequest.BillingDetails, subscriptionPaymentRequest.ProdList);

                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveOrUpdateDomeBoxBillAndPaymentAsync")]
        public async Task<IApiResponse> SaveOrUpdateDomeBoxBillAndPaymentAsync(DomeBoxPaymentRequest subscriptionPaymentRequest)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.SaveOrUpdateDomeboxDonationAsync(subscriptionPaymentRequest.BillingDetails, subscriptionPaymentRequest.ProdList);

                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }

        [HttpPost("SaveOrUpdateCampaignBillAndPaymentAsync")]
        public async Task<IApiResponse> SaveOrUpdateCampaignBillAndPaymentAsync(CampaignPaymentRequest subscriptionPaymentRequest)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.SaveOrUpdateCampaignDonationAsync(subscriptionPaymentRequest.BillingDetails, subscriptionPaymentRequest.ProdList);

                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveOrUpdateCampaign")]
        public async Task<IApiResponse> SaveOrUpdateCampaign(CampaigningModel campaigningModel)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.SaveOrUpdateCampaignAsync(campaigningModel);

                //res = await _familyservices.PreapareInput(objPaymentInputModel);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("GetCampaignDetails")]
        public async Task<IApiResponse> GetCampaignDetails(int churchId)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.GetCampaign(churchId);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveFailureTransaction")]
        public async Task<IApiResponse> SaveFailureTransaction(PaymentTransaction paymentTransaction)
        {
            res = new ApiResponse();
            try
            {
                res = await _paymentInterface.InsertFailPaymentTransactionAsync(paymentTransaction);
            }
            catch (Exception ex)
            {

                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }

    }
    public class SubscriptionPaymentRequest
    {
        public BillDetailsViweModel BillingDetails { get; set; }
        public IEnumerable<prodDetails> ProdList { get; set; }
    }
    public class DonationPaymentRequest
    {
        public DonationMaster BillingDetails { get; set; }
        public IEnumerable<prodDetails> ProdList { get; set; }
    }
    public class DomeBoxPaymentRequest
    {
        public DomeboxDonationModel BillingDetails { get; set; }
        public IEnumerable<prodDetails> ProdList { get; set; }
    }
    public class CampaignPaymentRequest
    {
        public CampaignDonationModel BillingDetails { get; set; }
        public IEnumerable<prodDetails> ProdList { get; set; }
    }
}
