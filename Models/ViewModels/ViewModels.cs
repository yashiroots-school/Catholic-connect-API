using Payrequest;

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

public class PaymentTransaction
    {
        public long PaymentTransactionId { get; set; }          // bigint
        public string? TransactionStatus { get; set; }          // nvarchar
        public string? TransactionError { get; set; }           // nvarchar
        public string? TxnDate { get; set; }                    // nvarchar (consider using DateTime if possible)
        public string? Amount { get; set; }                     // nvarchar (consider decimal if numeric)
        public string? TransactionId { get; set; }              // nvarchar
        public string? TrackId { get; set; }                    // nvarchar
        public string? ReferenceNo { get; set; }                // nvarchar
        public string? Pmntmode { get; set; }                   // nvarchar
        public string? Type { get; set; }                       // nvarchar
        public string? Card { get; set; }                       // nvarchar
        public string? CardType { get; set; }                   // nvarchar
        public string? Member { get; set; }                     // nvarchar
        public string? AtomtokenId { get; set; }                  // nvarchar
        public int FamilyId { get; set; }                       // int
        public long ChurchId { get; set; }                      // bigint
        public long DioceseId { get; set; }                     // bigint
        //public string? FeeIds { get; set; }                     // nvarchar
        public string? UserId { get; set; }                     // nvarchar
        public string? FeeAmounts { get; set; }                 // nvarchar
        public string? FamilyRegNo { get; set; }                // nvarchar
        public string? FamilyHeadName { get; set; }             // nvarchar
        public string? FeeTitle { get; set; }
        public List<prodDetails> prodDetails { get; set; }
    }

}
