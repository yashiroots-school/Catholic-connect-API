using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolManagement.Website.Models
{
    public class PaymentViewModels
    {

        public string Name { get; set; }
        public string StudentId { get; set; }
        public string Class { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string POB { get; set; }
        public string Nationality { get; set; }
        public string MotherTongue { get; set; }
        public string Religion { get; set; }
        public string Category { get; set; }
        public string Caste { get; set; }
        public string BloodGroup { get; set; }
        public bool IsApproved { get; set; }
        public string Batch { get; set; }
        public string RoleNumber { get; set; }
        public string TCBal { get; set; }
        public string FeeHeadings { get; set; }
        public string Feeheadingamt { get; set; }
        public string ApplicationNumber { get; set; }
        public float ConcessionAmt { get; set; }
        public float Concession { get; set; }
        public string DueFee { get; set; }
        public string Email { get; set; }
        public string Section { get; set; }

    }

    public class PaymentResultModels
    {
        public string StudentName { get; set; }
        public string FatherName { get; set; }
        public string Contact { get; set; }
        public string Class { get; set; }
        public string Category { get; set; }
        public string RoleNumber { get; set; }
        public string TCBal { get; set; }
        //public string Batch { get; set; }
        public string studentid { get; set; }
        public string FeeHeadings { get; set; }
        public string Feeheadingamt { get; set; }
        public string ApplicationNumber { get; set; }
        public float Concession { get; set; }
        public float ConcessionAmt { get; set; }
        public string Key { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string OrdedrId { get; set; }
        public string Email { get; set; }
        public string paymentid { get; set; }

        public int classdetails { get; set; }
        public string AccountType { get; set; }
        public string MobileNO { get; set; }
        public string SectionName { get; set; }

    }

    public class PaymentTransactionDetails
    {
        public string TransactionStatus { get; set; }
        public string TransactionError { get; set; }
        public string ReferenceNo { get; set; }
        public string TrackId { get; set; }
        public string PaymentId { get; set; }
    }


    public class InputPayment
    {
        public string OrderId { get; set; }
        public string Mid { get; set; }
        public string Enckey { get; set; }
        public string MeTransReqType { get; set; }
        public string TrnAmt { get; set; }
        public string ResponseUrl { get; set; }
        public string TrnRemarks { get; set; }
        public string TrnCurrency { get; set; }
        public string AddField1 { get; set; }
        public string AddField2 { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
        public string AddField9 { get; set; }
        public string AddField10 { get; set; }
    }

    public class InputPaymentResult
    {
        public string OrderId { get; set; }
        public string Mid { get; set; }
        public string Enckey { get; set; }
        public string MeTransReqType { get; set; }
        public string TrnAmt { get; set; }
        public string ResponseUrl { get; set; }
        public string TrnRemarks { get; set; }
        public string TrnCurrency { get; set; }

        public string AddField9 { get; set; }
        public string AuthZCode { get; set; }
        public string PgMeTrnRefNo { get; set; }
        public string TrnReqDate { get; set; }
        public string Rrn { get; set; }
        public string StatusCode { get; set; }
        public string StatusDesc { get; set; }
        public string ResponseCode { get; set; }
        public string AddField1 { get; set; }
        public string AddField2 { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
        public string AddField10 { get; set; }
    }

    public class ResultPayment
    {
        public string StatusDesc { get; set; }
        public string ReqMsg { get; set; }

    }

    public class PaymentStatus
    {
        public string OrderId { get; set; }
        public string Mid { get; set; }
        public string Enckey { get; set; }

        public string pgTrnRefNo { get; set; }

        public string AddField1 { get; set; }
        public string AddField2 { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
    }
    public class ResultPaymentstatus
    {
        public string StatusCode { get; set; }
        public string ReqMsg { get; set; }

    }

    public class CancelPayment
    {
        public string OrderId { get; set; }
        public string Mid { get; set; }
        public string Enckey { get; set; }

        public string pgTrnRefNo { get; set; }

        public string AddField1 { get; set; }
        public string AddField2 { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
    }

    public class ResultCancelPayment
    {
        public string OrderId { get; set; }

        public string pgTrnRefNo { get; set; }

        public string StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string AddField1 { get; set; }
        public string AddField2 { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
    }

    public class RefundPayment
    {
        public string OrderId { get; set; }
        public string Mid { get; set; }
        public string Enckey { get; set; }
        public string Amount { get; set; }
        public string pgTrnRefNo { get; set; }

        public string AddField1 { get; set; }
        public string AddField2 { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
    }

    public class ResultRefundPayment
    {
        public string OrderId { get; set; }

        public string pgTrnRefNo { get; set; }

        public string StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string AddField1 { get; set; }
        public string AddField2 { get; set; }
        public string AddField3 { get; set; }
        public string AddField4 { get; set; }
        public string AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
    }

    public class PaymentResultSummary
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Category { get; set; }
        public string Course { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionID { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string Last_Name { get; set; }
    }
}