namespace ChurchAPI.Models
{
    public class BillingYears
    {
        public int Id { get; set; }
        public int BillingYear { get; set; }
    }
    public class BillHeadings
    {
        public int Id { get; set; }
        public string BillHeading { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
