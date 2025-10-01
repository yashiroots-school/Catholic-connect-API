namespace LumenApi.Web.Models.PaymentModels.BusinessModel;

public class TblMerchantclass
{
  public int MerchantName_Id { get; set; }
  public string MerchantName { get; set; } = string.Empty;
  public int School_Id { get; set; }
  public int Bank_Id { get; set; }
  public int Branch_Id { get; set; }
  public string Schoolname { get; set; } = string.Empty;
  public string BankName { get; set; } = string.Empty;
  public string BranchName { get; set; } = string.Empty;
  public int MerchantId { get; set; }
  public string MerchantKey { get; set; } = string.Empty;
  public string MerchantMID { get; set; } = string.Empty;
  public int Schoolsetup_Id { get; set; }
  public string Status { get; set; } = string.Empty;
  public string Feeconfiguration { get; set; } = string.Empty;
}
