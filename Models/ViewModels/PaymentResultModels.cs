using Payrequest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChurchAPI.Models.ViewModels;
public class PaymentResultModels
{

    public string FamilyRegNo { get; set; } = string.Empty;
    public string FamilyHeadName { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public long ChurchId { get; set; }
    //public string Section { get; set; } = string.Empty;
    // public string Category { get; set; } = string.Empty;
    // public string RoleNumber { get; set; } = string.Empty;
    public string TotalAmount { get; set; } = string.Empty;
    public int FamilyId { get; set; }
    public string FeeHeadings { get; set; } = string.Empty;
    public string Feeheadingamt { get; set; } = string.Empty;
    //public string ApplicationNumber { get; set; } = string.Empty;
    public float Concession { get; set; }
    public float DueAmount { get; set; }
    public float ConcessionAmt { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string OrdedrId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string paymentid { get; set; } = string.Empty;
    //public int classdetails { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string MobileNO { get; set; } = string.Empty;
    public string PaymentGatewayName { get; set; }
    public long UserId { get; set; }
    public string? atomTokenId { get; set; }
    public string? Title { get; set; }
    //merchTxnId and orderID
    public string? TrackID { get; set; }
    public string? ApiUrl { get; set; }
    public string? custEmail { get; set; }
    public string? custMobile { get; set; }
    public string? merchId { get; set; }
    public string? returnurl { get; set; }
    public List<prodDetails> prodDetails { get;set;}
}
public class PaymentTransactionId
{
  public string? Paymentid { get; set; }

  public string? Orderid { get; set; }

  public string? Merchant_Key { get; set; }

  public string? Secret_Key { get; set; }

}
public class BillDetailsViweModel
{
    [Key]
    public int BillDetailId { get; set; } = 0;
    public int BillId { get; set; } = 0;
    public List<int> SelectedBillHeadings { get; set; } = new List<int>();
    public string BillHeading { get; set; }
    public string SubHeading { get; set; }
    public string  FamilyRegistrationNo { get; set; } = "0";
    public int FamilyId { get; set; }
    public string Year { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ToPayAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string PaymentMode { get; set; }
    public DateTime PaidDate { get; set; }
    public string BillGeneratedBy { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string StringIdForOtherTypeBil { get; set; }
    public string RegisterId { get; set; }
    public string ChurchName { get; set; }
    public string AtomTokenId { get; set; }
    public string TransactionId { get; set; }
    public string TrackId { get; set; }
    public string FamilyHeadName { get; set; }
    public string churchId { get; set; }
}
public class DonationMaster
{
    public string ChurchName { get; set; }
    public long DonationId { get; set; }
    public string DonarName { get; set; }
    public string DonarContact { get; set; }
    public string DonarEmail { get; set; }
    public string DonarAddress { get; set; }
    public string DonarCountry { get; set; }
    public string DonarPANCardNo { get; set; }
    public string DonarGSTNo { get; set; }
    public string DonationType { get; set; }
    public string PaymentMode { get; set; }
    public string Description { get; set; }
    public string ChequeNo { get; set; }
    public string BankName { get; set; }
    public string IFSCCode { get; set; }
    public string TransactionNo { get; set; }
    public DateTime? TransactionDate { get; set; }  // Nullable in case it's optional
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public decimal? PaidAmount { get; set; }
    public string AtomTokenId { get; set; }
    public string TransactionId { get; set; }
    public string TrackId { get; set; }
    public string FamilyHeadName { get; set; }
    public string churchId { get; set; }
    public int FamilyId { get; set; }
    public string FamilyRegistrationNo { get; set; } = "0";
}
public class DomeboxDonationModel
{
    public long DonationId { get; set; } = 0; // 0 for insert, >0 for update

    // Donor Info (optional)
    public string DonorName { get; set; }
    public string DonorContact { get; set; }
    public string DonorEmail { get; set; }

    // Donation Details
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public DateTime? DonationDate { get; set; }

    // Domebox Metadata
    public string DomeboxId { get; set; }
    public string Location { get; set; }

    // Payment Info (optional)
    public string PaymentMode { get; set; } // e.g., Cash, Cheque, UPI
    public string ChequeNo { get; set; }
    public string BankName { get; set; }
    public string IFSCCode { get; set; }

    // Other
    public string Remarks { get; set; }
    public bool IsAnonymous { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // System tracking
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string AtomTokenId { get; set; }
    public string TransactionId { get; set; }
    public string TrackId { get; set; }
    public string FamilyHeadName { get; set; }
    public string churchId { get; set; }
    public string FamilyRegistrationNo { get; set; } = "0";
    public int FamilyId { get; set; }
}
public class CampaignDonationModel
{
    public long DonationId { get; set; } = 0; // 0 = insert, >0 = update
    public long CampaignId { get; set; }
    public int ChurchId { get; set; }

    public string DonorName { get; set; }
    public string DonorContact { get; set; }
    public string DonorEmail { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public DateTime? DonationDate { get; set; }

    public string PaymentMode { get; set; }
    public string ChequeNo { get; set; }
    public string BankName { get; set; }
    public string IFSCCode { get; set; }
    public string TransactionNo { get; set; }

    public string Remarks { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string AtomTokenId { get; set; }
    public string TransactionId { get; set; }
    public string TrackId { get; set; }
    public string FamilyHeadName { get; set; }
    //public string churchId { get; set; }
    public int FamilyId { get; set; }
    public string FamilyRegistrationNo { get; set; } = "0";
}

public class CampaigningModel
{
    public long CampaignId { get; set; } = 0; // 0 for insert, >0 for update
    public int ChurchId { get; set; }          // New property for Church ID

    public string CampaignName { get; set; }
    public string Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public decimal TargetAmount { get; set; }
    public decimal RaisedAmount { get; set; }

    public bool IsActive { get; set; } = true;

    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
  
    public string AttachmentName { get; set; } = "0";
    public byte[]? AttachmentData { get; set; }
}
public class ReportFilterModel
{
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? FamilyNo { get; set; }
    
    public string? Year { get; set; }
    public string? PaymentMode { get; set; }
    public string? ChurchName { get; set; }
    public string? DioceseName { get; set; }
}
public class DonationReportFilterModel
{
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? DonarName { get; set; }
    public string? DonarContact { get; set; }
    public string? DonarEmail { get; set; }

    public string? DonarAddress { get; set; }
    public string? PaidAmount { get; set; }
    public string? PaymentMode { get; set; }
    public long? Id { get; set; }
    public string? ChurchName { get; set; }
    public string? DioceseName { get; set; }
}
public class SubscriprtionReportModel
{
    public int BillingId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string MemberId { get; set; }
    public string HeadName { get; set; }
    public int Year { get; set; }
    public string PaymentMode { get; set; }
    public string MainHeading { get; set; }
    public decimal Total { get; set; }
    public decimal BillAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal DueAmount { get; set; }
    public string SubHeadings { get; set; }
    public string BillingYear { get; set; }
    public string ChurchName { get; set; }// assuming this is like "2025"
}
public class DomeBoxReportModel
{
    public long DonationId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DonationDate { get; set; }
    public string? DomeBoxName { get; set; }
    public string? DonarName { get; set; }
    //public string?  DonarAddress { get; set; }
    public string? PaymentMode { get; set; }
    public string? Remarks { get; set; }
    public string? DonarContact { get; set; }
    public string? DonarEmail { get; set; }
    public decimal DonationAmount { get; set; }
    public string ChurchName { get; set; }// assuming this is like "2025"
}
public class CampaignReportModel
{
    public long DonationId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DonationDate { get; set; }
    public string? CampaignName { get; set; }
    public string? DonarName { get; set; }
    //public string?  DonarAddress { get; set; }
    public string? PaymentMode { get; set; }
    public string? Remarks { get; set; }
    public string? DonarContact { get; set; }
    public string? DonarEmail { get; set; }
    public decimal DonationAmount { get; set; }
    public string ChurchName { get; set; }// assuming this is like "2025"
}




