using Payrequest;
using System;
using System.Collections.Generic;
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
