
using ChurchAPI.Models.ViewModels;
using ChurchAPI.Services;
using System;
namespace Payrequest;
  public class HeadDetails
  {
    public string? version { get; set; }
    public string? api { get; set; }
    public string? platform { get; set; }

  }
  public class MerchDetails
  {

    public string? merchId { get; set; }
    public string? userId { get; set; }
    public string? password { get; set; }
    public string? merchTxnDate { get; set; }
    public string? merchTxnId { get; set; }


  }
  public class PayDetails
  {
    public string? amount { get; set; }
    public string? product { get; set; }
    public string? custAccNo { get; set; }
    public string? txnCurrency { get; set; }
    public List<prodDetails> prodDetails { get; set; }



  }
  public class CustDetails
  {
    public string? custEmail { get; set; }
    public string? custMobile { get; set; }


  }
  public class Extras
  {
    public string? udf1 { get; set; }
    public string? udf2 { get; set; }
    public string? udf3 { get; set; }
    public string? udf4 { get; set; }
    public string? udf5 { get; set; }
    public string? udf6 { get; set; }
}

  public class MsgBdy
  {
    public HeadDetails? headDetails { get; set; }
    public MerchDetails?     merchDetails { get; set; }
    public PayDetails?   payDetails { get; set; }
    public CustDetails? custDetails { get; set; }
    public Extras? extras { get; set; }




  }

public class Payrequest
{
    private readonly string _connectionString;

    // Pass connection string directly
    public Payrequest(string connectionString)
    {
        _connectionString = connectionString;
    }


    public HeadDetails? headDetails { get; set; }
    public MerchDetails? merchDetails { get; set; }
    public PayDetails? payDetails { get; set; }
    public CustDetails? custDetails { get; set; }
    public Extras? extras { get; set; }


    public RootObject RequestMap(PaymentResultModels paymentResultModels, long churchId)
    {
        // Get merchant details via repository using SP

        var repo = new MerchantRepository(_connectionString);
        var merchant = repo.GetMerchant(churchId);

        if (merchant == null)
            throw new Exception("No merchant found for this church.");

        // Initialize objects
        RootObject rt = new RootObject();
        HeadDetails hd = new HeadDetails();
        MerchDetails md = new MerchDetails();
        PayDetails pd = new PayDetails();
        CustDetails cd = new CustDetails();
        Extras ex = new Extras();

        // Head details
        hd.version = "OTSv1.1";
        hd.api = "AUTH";
        hd.platform = "FLASH";

        // Merchant details
        md.merchId = merchant.MerchantMID;
        md.userId = merchant.UserId.ToString();
        md.password = merchant.Password;
        md.merchTxnDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        md.merchTxnId = DateTime.Now.ToString("yyyyddMMhhmmss");

        // Payment details
        pd.amount = paymentResultModels.Amount;
        pd.txnCurrency = "INR";
        pd.custAccNo = "213232323";

        //if (paymentResultModels.AccountType == "Primary")
        //    pd.product = "Multi";
        if (merchant.MerchantName == "Primary")
           pd.product = "Multi";


        // Define prodDetails list with NSE and BSE
        pd.prodDetails = new List<prodDetails>
    {
        new prodDetails { prodName = "NSE",prodAmount=11 }, //prodAmount = paymentResultModels.NSEAmount },
        new prodDetails { prodName = "BSE",prodAmount=12 } //prodAmount = paymentResultModels.BSEAmount }
    };
        // Customer details
        cd.custEmail = paymentResultModels.Email;
        cd.custMobile = paymentResultModels.MobileNO;

        // Extras
        ex.udf1 = paymentResultModels.FamilyId.ToString();
        ex.udf2 = paymentResultModels.FamilyHeadName;
        ex.udf3 = paymentResultModels.ConcessionAmt.ToString();
        ex.udf4 = paymentResultModels.Feeheadingamt;
        ex.udf5 = paymentResultModels.FamilyRegNo;
        ex.udf6 = paymentResultModels.Title;
        // Assign properties directly to this instance
        this.headDetails = hd;
        this.merchDetails = md;
        this.payDetails = pd;
        this.custDetails = cd;
        this.extras = ex;

        rt.payInstrument = this; // Use the current instance
        return rt;
    }
}

public class RootObject
{
    public Payrequest? payInstrument { get; set; }
}
public class prodDetails
{ 
    public string prodName { get; set; }=string.Empty;
    public decimal prodAmount { get; set; }
}





