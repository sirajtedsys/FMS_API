namespace FMS_API.Data.Class
{
    public class CrBill
    {

        public string? CompPrefixCode { get; set; }
        public string? SctId { get; set; }
        public string? CustId { get; set; }
        public string? PartyName { get; set; }
        public decimal? CrbcAmount { get; set; }
        public string? CounterId { get; set; }
        public string? LoginEmpId { get; set; }
        public string? PayChqNo { get; set; }
        public string? PayChqDate { get; set; }
        public string? PayChqAmt { get; set; }
        public string? PayChqBankId { get; set; }
        public string? PayCcNo { get; set; }
        public string? PayCcAmt { get; set; }
        public string? PayCcBankId { get; set; }
        public string? PayCashAmt { get; set; }
        public string? PayWalletId { get; set; }
        public string? PayWalletTransNo { get; set; }
        public string? PayWalletDate { get; set; }
        public string? PayWalletAmt { get; set; }
        public string? PayWalletBankId { get; set; }
        public string? PayBankTransRefNo { get; set; }
        public string? PayBankTransDate { get; set; }
        public string? PayFromTransBankId { get; set; }
        public string? PayToTransBankId { get; set; }
        public string? PayBankTransAmt { get; set; }
        public string? PayToChequeBankId { get; set; }
        public string? CrbcDiscAmt { get; set; }
        public string? CrbcDiscType { get; set; }
        public string? CrbcSettledAmt { get; set; }
        public string? CrbcTotalAmt { get; set; }

        public List<CBillDetails> crbilDetails { get; set; }
    }
}
