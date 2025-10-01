using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Payverify;

//namespace LumenApi.UseCases.CommonClasses;
public class Payverify
{


  public string? atomTokenId { get; set; }

  //merchTxnId and orderID
  public string? TrackID { get; set; }
  public string? custEmail { get; set; }
  public string? custMobile { get; set; }
  public string? merchId { get; set; }
  public string? ApiUrl { get;set; }
  public ResponseDetails? responseDetails { get; set; }
  public Payverify()
  {

    //
    // TODO: Add constructor logic here
    //
  }
}

public class ResponseDetails
{
  public string? txnStatusCode { get; set; }
  public string? txnMessage { get; set; }
  public string? txnDescription { get; set; }


}
