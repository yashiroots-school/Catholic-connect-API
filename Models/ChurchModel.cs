namespace ChurchAPI.Models
{
    public class ChurchDetails
    {
        public int Id { get; set; }
        public string ChurchName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactNo { get; set; }
        public bool IsActive { get; set; }
        public string? HeadName { get; set; }
        public int DioceseId { get; set; }
    }
}
