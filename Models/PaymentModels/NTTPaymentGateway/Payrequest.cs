//using LumenApi.UseCases.CommonClasses;
//using LumenApi.Web;
//using LumenApi.Web.Models.Params;
using LumenApi.Web.Models.PaymentModels.BusinessModel;
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
  //private readonly Lumen090923Context _lumen;
  //public Payrequest(Lumen090923Context lumen)
  //{
  //  _lumen = lumen ?? throw new ArgumentNullException(nameof(lumen));
  //}

  public HeadDetails? headDetails { get; set; }
    // public MsgBdy msgBdy { get; set; }
    public MerchDetails? merchDetails { get; set; }
    public PayDetails? payDetails { get; set; }
    public CustDetails? custDetails { get; set; }
    public Extras? extras { get; set; }


    public RootObject RequestMap(PaymentResultModels paymentResultModels)
    {

   
    RootObject rt = new RootObject();
      MsgBdy mb = new MsgBdy();
      HeadDetails hd = new HeadDetails();
      MerchDetails md = new MerchDetails();
      PayDetails pd = new PayDetails();
      CustDetails cd = new CustDetails();
      Extras ex = new Extras();
      //Payrequest pr = new Payrequest(_lumen);
      hd.version = "OTSv1.1";
      hd.api = "AUTH";
      hd.platform = "FLASH";
    //md.merchId = "656207"; //A?.MerchantMID.ToString(); //"8952";
    //md.userId = "656207"; //A?.UserId.ToString();//"445842";//user input
    //md.password = "35c9ea61"; //A?.Password.ToString(); //"Test@123";//gateway
     //var A = _lumen.Tbl_CreateMerchantId.ToList().FirstOrDefault();
    md.merchId = A?.MerchantMID.ToString(); //"8952";
    md.userId = A?.UserId.ToString();//"445842";//user input
   md.password = A?.Password;
      md.merchTxnDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //user input
      md.merchTxnId = DateTime.Now.ToString("yyyyddMMhhmmss").ToString();//uesr input


      pd.amount = paymentResultModels.Amount;//user input
                                             // pd.product = "NSE";//need to check
                                             //            "prodDetails": [
                                             //{
                                             //"prodName": "NSE",
                                             //"prodAmount": 11.00
                                             //},
                                             //{
                                             //"prodName": "BSE",
                                             //"prodAmount": 12.00
                                             //}
      //                                       //],
      //if (paymentResultModels.AccountType == "Transport")
      //{
      //  pd.product = "BSE";
      //}
      //else if (paymentResultModels.AccountType == "Nursery")
      //{
      //  pd.product = "BSE";
      //}
       if (paymentResultModels.AccountType == "Primary")
      {
        pd.product = "SCHOOL";
      }

      pd.custAccNo = "213232323";//user input if not default
      pd.txnCurrency = "INR";//user input

      cd.custEmail = paymentResultModels.Email;//user input
      cd.custMobile = paymentResultModels.MobileNO = "8655358006";//user input


      ex.udf1 = paymentResultModels.StudentName;
      ex.udf2 = paymentResultModels.TCBal;
      ex.udf3 = paymentResultModels.ConcessionAmt.ToString();
      ex.udf4 = paymentResultModels.Feeheadingamt;
      ex.udf5 = paymentResultModels.studentid;


      pr.headDetails = hd;
      pr.merchDetails = md;
      pr.payDetails = pd;
      pr.custDetails = cd;
      pr.extras = ex;

      rt.payInstrument = pr;
      //var json = JsonConvert.SerializeObject(rt);




      return rt;
    }



  }
  public class RootObject
  {
    public Payrequest? payInstrument { get; set; }
  }





