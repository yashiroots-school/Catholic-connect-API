using ChurchAPI.Models;

namespace ChurchAPI.Interface
{
    public interface IDashBoardInterface
    {
        public Task<IApiResponse> GetCalendarEvent(string churchName);
        public Task<IApiResponse> SaveOrUpdateCalendarEventAsync(CalendarEvents model);
        public Task<IApiResponse> GetCalendarEventByDate(string churchName, string Date);
        public Task<IApiResponse> SaveOrUpdateDailyReadingAsync(DailyReadings model);
        public Task<IApiResponse> GetDailyReading(string churchName, string? ReadingDate = null);
        public Task<IApiResponse> SaveOrUpdateNotice(NoticeMaster model);
        public Task<IApiResponse> GetNoticesAsync(int? churchId = null, int? dioceseId = null);
    }
}
