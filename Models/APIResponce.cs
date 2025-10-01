namespace ChurchAPI.Models
{
    public interface IApiResponse
    {
        public object Data { get; set; }
        public string Msg { get; set; }
        public string ResponseCode { get; set; }
        public object AdditionalData { get; set; }
    }
    public class ApiResponse : IApiResponse
    {
        public object Data { get; set; } = new object();
        public string Msg { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public object AdditionalData { get; set; } = string.Empty;
    }
}
