using System.ComponentModel.DataAnnotations;

namespace FMS_API.Data.DbModel
{
    public class PRMMASTER_Hrm_Employee
    {
        [Key]
        public int EMP_ID { get; set; }
        public string EMP_NAME { get; set; } = null!;
        public string? EMP_CODE { get; set; }
        public char? EMP_GENDER { get; set; }
        public DateTime? EMP_DOB { get; set; }
        public DateTime? EMP_DOJ { get; set; }
        public string? EMP_ADDRESS { get; set; }
        public int? PLACE_ID { get; set; }
        public string? ZIP_CODE { get; set; }
        public int? DSG_ID { get; set; }
        public int? SCT_ID { get; set; }
        public string? PHONE_NO { get; set; }
        public string? MOBILE_NO { get; set; }
        public string? E_MAIL { get; set; }
        public char? SW_USER { get; set; }
        public string? SW_LOGIN_NAME { get; set; }
        public string? SW_PASSWORD { get; set; }
        public char? SW_SUPER_USER { get; set; }
        public char? MANUAL_ENTRY { get; set; }
        public char? ACTIVE_STATUS { get; set; }
        public int? CREATE_USER { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public int? UPDATE_USER { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public int? FIN_YEAR_ID { get; set; }
        public int? COMP_ID { get; set; }
        public string? EMP_LEVEL_STRING { get; set; }
        public char? USER_TYPE { get; set; }
        public char? EMP_ALLOW_EDITING { get; set; }
        public string? EMP_BNK_ACC_NO { get; set; }
        public int? BANK_ID { get; set; }
        public char? EMP_PAY_MODE { get; set; }
        public DateTime? EMP_RETIREMENT_DT { get; set; }
        public string? EMP_ESI_IP_NO { get; set; }
        public DateTime? EMP_PF_START_DT { get; set; }
        public string? EMP_PF_ACC_NO { get; set; }
        public decimal? EMP_EXTRA_PF_PERC { get; set; }
        public decimal? EMP_EXTRA_PF_AMT { get; set; }
        public char? EMP_LEAVE_ALLOW { get; set; }
        public decimal? EMP_CUR_DA { get; set; }
        public decimal? EMP_CUR_BASIC { get; set; }
        public string? EMPT_ID { get; set; }
        public string? EMPG_ID { get; set; }
        public string? EMP_GUARDIAN { get; set; }
        public char? SUSP_CASH_PAYABLE { get; set; }
        public char? PDA_PORT_STATUS { get; set; }
        public decimal? SALES_MAN_PERC { get; set; }
        public string? PASSPORT_NO { get; set; }
        public DateTime? PASSPORT_ISS_DATE { get; set; }
        public DateTime? PASSPORT_EXP_DATE { get; set; }
        public string? RESPERMIT_NO { get; set; }
        public DateTime? RESPERMIT_ISS_DATE { get; set; }
        public DateTime? RESPERMIT_EXP_DATE { get; set; }
        public string? DRLICENSE_NO { get; set; }
        public DateTime? DRLICENSE_ISS_DATE { get; set; }
        public DateTime? DRLICENSE_EXP_DATE { get; set; }
        public string? MUNCARD_NO { get; set; }
        public DateTime? MUNCARD_ISS_DATE { get; set; }
        public DateTime? MUNCARD_EXP_DATE { get; set; }
        public string? HEALTHCARD_NO { get; set; }
        public DateTime? HEALTHCARD_ISS_DATE { get; set; }
        public DateTime? HEALTHCARD_EXP_DATE { get; set; }
        public string? OTHER_NO { get; set; }
        public DateTime? OTHER_ISS_DATE { get; set; }
        public DateTime? OTHER_EXP_DATE { get; set; }
        public decimal? MAX_DISC_ALLOW { get; set; }
        public char? SHOW_MARGIN { get; set; }
        public char? SENT_SMS { get; set; }
        public int? PRVG_CARD_ID { get; set; }
        public string? PRVG_CARD_BARCODE { get; set; }
        public char SUPERVISORY_USER { get; set; } = 'N';
        public char TAKEN_BY_STATUS { get; set; } = 'N';
        public char IS_DELIVERY_MAN { get; set; } = 'N';
        public char IS_EMPLOYEE { get; set; } = 'N';
        public char SALES_MAN_STS { get; set; } = 'N';
        public int? ACC_MASTER_ID { get; set; }
        public char? IS_ADV_ALLOWED { get; set; }
    }



}

