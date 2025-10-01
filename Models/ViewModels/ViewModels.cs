namespace ChurchAPI.Models.ViewModels
{
    public class PaymentInputModel
    {

        public int FamilyId { get; set; }
        public string FamilyRegNo { get; set; } = string.Empty;
        public string FamilyHeadName { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string FeeHeadings { get; set; } = string.Empty;
        public string Feeheadingamt { get; set; } = string.Empty;
        public float ConcessionAmt { get; set; }
        public float Concession { get; set; }
        public float DueFee { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string PaymentGatewayName { get; set; } = string.Empty;
        public long UserId { get; set; }
        public long ChurchId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
