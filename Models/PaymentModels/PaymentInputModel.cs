namespace LumenApi.Web.Models.PaymentModels;

public class PaymentInputModel
{

  public int StudentId { get; set; }
  public string Class { get; set; } = string.Empty;
  public string Category { get; set; }=string.Empty;
  public string TCBal { get; set; } = string.Empty;
  public string FeeHeadings { get; set; } = string.Empty;
  public string Feeheadingamt { get; set; } = string.Empty;
  public float ConcessionAmt { get; set; }
  public float Concession { get; set; }
  public string DueFee { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string PaymentGatewayName { get; set; } = string.Empty;

}
