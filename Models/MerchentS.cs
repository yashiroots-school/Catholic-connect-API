

namespace ChurchAPI.Models
{
    public class CreateMerchantModel
    {
        public long Merchant_Id { get; set; }
        public long ChurchId_Id { get; set; }
        public long Bank_Id { get; set; }
        public long Branch_Id { get; set; }
        public long MerchantName_Id { get; set; }
        public string MerchantMID { get; set; } = string.Empty;
        public string MerchantKey { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int CurrentYear { get; set; }
        public string? IP { get; set; }
        public long UserId { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreateBy { get; set; }
        public string? InsertBy { get; set; }
        public string? BatchName { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? MerchantName { get; set; }
    }
    public class MerchantNameModel
    {
        public long MerchantName_Id { get; set; }
        public long ChurchId { get; set; }
        public string MerchantName { get; set; } = string.Empty;
        public long School_Id { get; set; }
        public long Bank_Id { get; set; }
        public long Branch_Id { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int CurrentYear { get; set; }
        public string? IP { get; set; }
        public long UserId { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreateBy { get; set; }
        public string? InsertBy { get; set; }
        public string? BatchName { get; set; }
    }
    //public class CreateMerchantModel
    //{
    //    public long Merchant_Id { get; set; }
    //    public long MerchantName_Id { get; set; }
    //    public string MerchantMID { get; set; } = string.Empty;
    //    public string MerchantKey { get; set; } = string.Empty;
    //    public long Bank_Id { get; set; }
    //    public long Branch_Id { get; set; }
    //    public long School_Id { get; set; }
    //}

    //public class MerchantNameModel
    //{
    //    public long MerchantName_Id { get; set; }
    //    public string MerchantName { get; set; } = string.Empty;
    //}

    public class SchoolSetupModel
    {
        public long ChurchSetUp_Id { get; set; }
        public long Church_Id { get; set; }
        public long Bank_Id { get; set; }
        public long Branch_Id { get; set; }
        public long Merchant_nameId { get; set; }
    }

}
