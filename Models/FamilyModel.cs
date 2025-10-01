namespace ChurchAPI.Models
{
    public class ViewFamilyDetailsModel
    {
        public int HeadFamilyId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string HeadName { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public string ZoneCode { get; set; } = string.Empty;
    }
    public class LastYearDues
    {
        public decimal LastYearDue { get; set; }
        public decimal CurrentYearDue { get; set; }
        
    }

}
