using ChurchAPI.Interface;
using ChurchAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardInterface _dashBoardInterface;
        IApiResponse res = null!;
        public DashBoardController(IDashBoardInterface dashBoard)
        {
            _dashBoardInterface = dashBoard;
        }
        [HttpPost("GetCalendarEvent")]
        public async Task<IApiResponse> GetCalendarEvent(string churchId)
        {
            res = new ApiResponse();
            try
            {
                res = await _dashBoardInterface.GetCalendarEvent(churchId);
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveOrUpdateCalendarEvent")]
        public async Task<IApiResponse> GetCalendarEvent(CalendarEvents calendarEvents)
        {
            res = new ApiResponse();
            try
            {
                res = await _dashBoardInterface.SaveOrUpdateCalendarEventAsync(calendarEvents);
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("GetCalendarEventByDate")]
        public async Task<IApiResponse> GetCalendarEvent(string churchId, string Date)
        {
            res = new ApiResponse();
            try
            {
                res = await _dashBoardInterface.GetCalendarEventByDate(churchId,Date);
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveOrUpdateDailyReadings")]
        public async Task<IApiResponse> SaveDailyReadings(DailyReadings dailyReadings)
        {
            res = new ApiResponse();
            try
            {
                res = await _dashBoardInterface.SaveOrUpdateDailyReadingAsync(dailyReadings);
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("GetDailyReading")]
        public async Task<IApiResponse> GetDailyReading(string churchId, string? ReadingDate=null)
        {
            res = new ApiResponse();
            try
            {
                res = await _dashBoardInterface.GetDailyReading(churchId, ReadingDate);
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("SaveNotice")]
        public async Task<IApiResponse> SaveNotice(NoticeMaster noticeMaster)
        {
            res = new ApiResponse();
            try
            {
                res = await _dashBoardInterface.SaveOrUpdateNotice(noticeMaster);
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.ResponseCode = "500";
            }
            return res;
        }
        [HttpPost("GetNotice")]
        public async Task<IApiResponse> GetNotice(int? churchId = null, int? dioceseId = null)
        {
            res = new ApiResponse();
            try
            {
                res = await _dashBoardInterface.GetNoticesAsync(churchId, dioceseId);
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
