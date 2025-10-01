using ChurchAPI.Interface;
using ChurchAPI.Models;
using ChurchAPI.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
