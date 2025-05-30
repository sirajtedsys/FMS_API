using Microsoft.EntityFrameworkCore;
using FMS_API.Class;
using FMS_API.Data.Class;
using FMS_API.Data.DbModel;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Dynamic;
using static JwtService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace FMS_API.Repositry
{
    public class CommonRepositry
    {
        private readonly AppDbContext _DbContext;

        private readonly JwtHandler jwthand;
        private readonly string _con;

        public CommonRepositry(AppDbContext dbContext, JwtHandler _jwthand)
        {
            _DbContext = dbContext;
            jwthand = _jwthand;
            _con = _DbContext.Database.GetConnectionString();
        }

        public async Task<dynamic> LoginCheck(string username, string password)
        {
            try
            {
                //var data = await _DbContext.EMR_ADMIN_USERS
                //    .Where(x => x.AUSR_USERNAME == username
                //    && x.AUSR_PWD == password
                //    && x.AUSR_STATUS != "D")
                //    .ToListAsync();
                var data = await _DbContext.HRM_EMPLOYEE
                   .Where(e => e.ACTIVE_STATUS == 'A' &&
                               e.SW_LOGIN_NAME == username &&
                               e.SW_PASSWORD == password)
                   .Select(e => new
                   {
                       e.EMP_ID,
                       e.EMP_NAME,
                       e.SW_LOGIN_NAME,
                       e.SW_PASSWORD
                   })
                   .ToListAsync();

                //return data/*;*/

                //return userdata;

                //SELECT B.BRANCH_ID, B.BRCH_NAME FROM UCHMASTER.HRM_BRANCH B
                //                INNER JOIN UCHEMR.EMR_ADMIN_USERS_BRANCH_LINK BL ON BL.BRANCH_ID = B.BRANCH_ID
                //                INNER JOIN UCHEMR.EMR_ADMIN_USERS USR ON USR.AUSR_ID = BL.AUSR_ID
                //                WHERE NVL(B.ACTIVE_STATUS, 'A')= 'A' AND ausr_username = 'tedsys' AND ausr_pwd = 'ted@123' ORDER BY B.BRANCH_ID



                if (data != null && data.Count > 0)
                {
                    var userdat = new UserTocken
                    {
                        AUSR_ID = data[0].EMP_ID.ToString(),
                        USERNAME = data[0].SW_LOGIN_NAME,
                        PASSWORD = data[0].SW_PASSWORD
                    };

                    var token = jwthand.GenerateToken(userdat);

                    //check exiting userdetail in loginsettings
                    var dat = await _DbContext.APP_LOGIN_SETTINGS.Where(x => x.EMP_ID == data[0].EMP_ID.ToString()).ToListAsync();
                    var existingDATA = new PRMMASTER_LoginSettings();

                    if (dat.Count > 0)
                    {

                        existingDATA = dat[0];
                    }
                    else
                    {
                        existingDATA = null;
                    }
                    //if data exist then edit the table

                    if (existingDATA != null)
                    {
                        existingDATA.TOKEN = token;
                        existingDATA.CREATE_ON = DateTime.Now;
                        _DbContext.SaveChanges();

                    }
                    //if no existing data exist then add new data to teh tablw
                    else
                    {
                        var newlogin = new PRMMASTER_LoginSettings
                        {
                            EMP_ID = userdat.AUSR_ID,
                            TOKEN = token,
                            CREATE_ON = DateTime.Now,
                        };

                        _DbContext.APP_LOGIN_SETTINGS.Add(newlogin);
                        _DbContext.SaveChanges();
                    }

                    var msgsuccsess = new DefaultMessage.Message1
                    {
                        Status = 200,
                        Message = token
                    };
                    return msgsuccsess;


                }
                else
                {
                    var msg = new DefaultMessage.Message1
                    {
                        Status = 600,
                        Message = "Invalid Username and Password"

                    };
                    return msg;
                }




            }
            catch (Exception ex)
            {
                var msg1 = new DefaultMessage.Message1
                {
                    Status = 500,
                    Message = ex.Message
                };
                return msg1;
            }

        }

        public async Task<dynamic> GetAllUserBranches(UserTocken ut)
        {
            try
            {


                //var result = (from b in _DbContext.HRM_BRANCH
                //              join bl in _DbContext.HRM_EMPLOYEE_BRANCH_LINK on b.BRANCH_ID equals bl.BRANCH_ID
                //              join hr in _DbContext.HRM_EMPLOYEE_HR on bl.EMP_ID equals hr.EMP_ID
                //              join emp in _DbContext.HRM_EMPLOYEE on hr.EMP_ID equals emp.hr
                //              where b.ACTIVE_STATUS  == "A"
                //                    && emp.EMP_LOGIN_NAME == ut.USERNAME
                //                    && emp.EMP_PASSWORD == ut.PASSWORD
                //              orderby b.BRANCH_ID
                //              select new
                //              {
                //                  b.BRANCH_ID,
                //                  b.BRCH_NAME
                //              }).ToList();
                //return result;
                return null;

            }
            catch (Exception ex)
            {
                var msg1 = new DefaultMessage.Message1
                {
                    Status = 500,
                    Message = ex.Message
                };
                return msg1;

            }
        }
        public async Task<dynamic> GetUserDetails(UserTocken ut)
        {
            try
            {

                var result = (from emp in _DbContext.HRM_EMPLOYEE
                                  //join doc in _DbContext.OPN_DOCTOR on emp.EMP_ID equals doc.EMPLOYEE_ID into docgroup
                                  //from doc in docgroup.DefaultIfEmpty()
                              where emp.EMP_ID == int.Parse(ut.AUSR_ID)
                              select new
                              {
                                  emp.EMP_ID,
                                  emp.EMP_NAME,
                                  //emp.EMP_CONT_PER_PHONE,
                                  //doc.LEVEL_ID,
                                  //doc.BRANCH_ID,
                                  //doc.DEF_PAGE
                              }).ToList();
                return result;

            }
            catch (Exception ex)
            {
                var msg1 = new DefaultMessage.Message1
                {
                    Status = 500,
                    Message = ex.Message
                };
                return msg1;

            }
        }


        //userwntedreports
        public async Task<dynamic> GetAppMenuAsync(string userId)
        {
            try
            {
                // Define the SQL statement to call the stored procedure
                var sql = "BEGIN UCHTRANS.SP_MIS_APP_MENU(:USER_ID, :STROUT); END;";

                // Define the parameters
                var userIdParam = new OracleParameter("USER_ID", OracleDbType.Varchar2) { Value = userId };
                var strOutParam = new OracleParameter("STROUT", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };

                // Execute the stored procedure
                using (var cmd = _DbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;

                    // Add parameters to the command
                    cmd.Parameters.Add(userIdParam);
                    cmd.Parameters.Add(strOutParam);

                    await _DbContext.Database.GetDbConnection().OpenAsync();

                    using (var reader = (OracleDataReader)await cmd.ExecuteReaderAsync())
                    {
                        var results = new List<dynamic>();

                        while (await reader.ReadAsync())
                        {
                            var result = new
                            {
                                TabName = reader["TAB_NAME"] != DBNull.Value ? reader["TAB_NAME"].ToString() : null,
                                Link = reader["LINK"] != DBNull.Value ? reader["LINK"].ToString() : null,
                                Priority = reader["PRIORITY"] != DBNull.Value ? Convert.ToInt32(reader["PRIORITY"]) : (int?)null
                            };

                            results.Add(result);
                        }

                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg1 = new DefaultMessage.Message1
                {
                    Status = 500,
                    Message = ex.Message
                };
                return msg1;
            }
        }

        public async Task<DataTable> GetAppEmpWiseSct(string loginName, string password)
        {
            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand("PRMTRANS.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_EMP_WISE_SCT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input Parameters
                    cmd.Parameters.Add(new OracleParameter("STR_LOGIN_NAME", OracleDbType.Varchar2) { Value = loginName });
                    cmd.Parameters.Add(new OracleParameter("STR_PWD", OracleDbType.Varchar2) { Value = password });

                    // Output Parameter
                    OracleParameter outParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable resultTable = new DataTable();
                            resultTable.Load(reader);
                            return resultTable;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetAppEmpWiseSct: {ex.Message}");
                        throw;
                    }
                }
            }
        }




        public async Task<dynamic> GetCreditbill(string fromDate, string toDate, string custid = null)
        {
            try
            {


                using (OracleConnection conn = new OracleConnection(_con))
                using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_CREDIT_BILL_PENDING", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input Parameters
                    cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
                    //cmd.Parameters.Add("STRITEM_FILTER", OracleDbType.Varchar2).Value = itemid ?? (object)DBNull.Value;
                    cmd.Parameters.Add("STRCUST_FILTER", OracleDbType.Varchar2).Value = custid ?? (object)DBNull.Value;


                    // Output Parameter (Cursor)
                    OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputCursor);

                    // Execute Query
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return new DefaultMessage.Message3 { Status = 200, Data = dt };
                }
            }
            catch (Exception ex)
            {

                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }

        }




        #region Credit bill settlement


        public async Task<DataTable> GetAppFinYear()
        {
            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand("prmtrans.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_FIN_YEAR", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Output Parameter
                    OracleParameter outParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable resultTable = new DataTable();
                            resultTable.Load(reader);
                            return resultTable;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetAppFinYear: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<DataTable> GetAppCustomer()
        {
            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand("prmtrans.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_CUSTOMER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Output Parameter
                    OracleParameter outParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable resultTable = new DataTable();
                            resultTable.Load(reader);
                            return resultTable;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetAppCustomer: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<DataTable> GetAppCustBranch(string mainCustId)
        {
            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand("prmtrans.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_CUST_BRANCH", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input Parameter
                    cmd.Parameters.Add(new OracleParameter("StrMainCustID", OracleDbType.Varchar2) { Value = mainCustId });

                    // Output Parameter
                    OracleParameter outParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable resultTable = new DataTable();
                            resultTable.Load(reader);
                            return resultTable;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetAppCustBranch: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<DataTable> GetAppCustFlexFill(string sctId, string filterCust)
        {
            //if (!Regex.IsMatch(filterCust, @"^'(\d+)'(,'\d+')*$"))
            //{
            //    throw new ArgumentException("Invalid format for filterCust. It must be a comma-separated list of quoted numeric IDs.");
            //}

            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand("BEGIN prmtrans.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_CUST_FLEX_FILL(:Str_SCT_ID, :StrFilterCust, :STROUT); END;", conn))
                {
                    cmd.CommandType = CommandType.Text;

                    // Input Parameters
                    cmd.Parameters.Add(new OracleParameter("Str_SCT_ID", OracleDbType.Varchar2) { Value = sctId });

                    // Pass the filterCust as is
                    cmd.Parameters.Add(new OracleParameter("StrFilterCust", OracleDbType.Varchar2) { Value = filterCust });

                    // Output Parameter
                    OracleParameter outParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outParam);

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable resultTable = new DataTable();
                            resultTable.Load(reader);
                            return resultTable;
                        }
                    }
                    catch (OracleException oe)
                    {
                        Console.WriteLine($"Error in GetAppCustFlexFill: {oe.Message}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetAppCustFlexFill: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<DataTable> GetCompanyFinYearDetails()
        {
            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand("PRMTRANS.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_COMP_FINYR_DETAILS", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("STROUT", OracleDbType.RefCursor, ParameterDirection.Output);

                var dt = new DataTable();
                try
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                return dt;
            }
        }

        public async Task<dynamic> GenerateCRBillRefNo(int compID, int finYearID, string compCode, string finYearCode, int sctID)
        {
            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand("PRMTRANS.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_CRBILL_REFNO", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("IntCompID", OracleDbType.Int32).Value = compID;
                cmd.Parameters.Add("intFinYearID", OracleDbType.Int32).Value = finYearID;
                cmd.Parameters.Add("StrCompCode", OracleDbType.Varchar2).Value = compCode;
                cmd.Parameters.Add("StrFinYearCode", OracleDbType.Varchar2).Value = finYearCode;
                cmd.Parameters.Add("IntSCT_ID", OracleDbType.Int32).Value = sctID;
                cmd.Parameters.Add("STROUT", OracleDbType.RefCursor, ParameterDirection.Output);

                List<string> result = new List<string>(); // Corrected list initialization

                try
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) // Use ReadAsync() for async operations
                        {
                            result.Add(reader["REFNO"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                return result; // Returning a dynamic list of strings
            }
        }

        public async Task<DataTable> GetManualInvoiceFlexFill(string sctID, string manualInvNo, int finYearID)
        {
            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand("PRMTRANS.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_MANUAL_INVNO_FLEX_FILL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("Str_SCT_ID", OracleDbType.Varchar2).Value = sctID;
                cmd.Parameters.Add("strManualInvNo", OracleDbType.Varchar2).Value = manualInvNo;
                cmd.Parameters.Add("lngFinYearID", OracleDbType.Int32).Value = finYearID;
                cmd.Parameters.Add("STROUT", OracleDbType.RefCursor, ParameterDirection.Output);

                var dt = new DataTable();
                try
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                return dt;
            }
        }

        public async Task<DataTable> GetAppBank()
        {
            DataTable bankTable = new DataTable();

            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand("PRMTRANS.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_BANK", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.Parameters.Add(outputCursor);

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            bankTable.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return bankTable;
        }

        public async Task<DataTable> GetAppWallet()
        {
            DataTable bankTable = new DataTable();

            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand("PRMTRANS.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_WALLET", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.Parameters.Add(outputCursor);

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            bankTable.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return bankTable;
        }

        public async Task<DataTable> GetCRBCPrefix()
        {
            DataTable dataTable = new DataTable();
            string query = "SELECT CRBC_PREFIX FROM PRMADMIN.INV_SETTINGS";

            using (OracleConnection conn = new OracleConnection(_con))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            using (OracleDataAdapter adapter = new OracleDataAdapter(cmd)) // Use DataAdapter for DataTable
            {
                cmd.CommandType = CommandType.Text;

                try
                {
                    await conn.OpenAsync();
                    adapter.Fill(dataTable); // Fill DataTable with query result
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        await conn.CloseAsync();
                }
            }

            return dataTable;
        }


        public async Task<DataTable> GetSectionCounterDetails(long sectionId)
        {
            DataTable dataTable = new DataTable();
            string query = @"SELECT HRM_SECTION_COUNTER.COUNTER_ID, HRM_SECTION_COUNTER.COUNTER_NAME,
                                HRM_SECTION_COUNTER.COUNTER_CODE, HRM_DEPARTMENT_SECTIONS.SCT_ID,
                                HRM_SECTION_COUNTER.RRNT_TYPE, HRM_SECTION_COUNTER.SLRTN_PRNT_TYPE,
                                HRM_DEPARTMENT_SECTIONS.SCT_NAME, HRM_DEPARTMENT_SECTIONS.SCT_CODE 
                         FROM prmmaster.HRM_SECTION_COUNTER 
                         INNER JOIN prmmaster.HRM_DEPARTMENT_SECTIONS 
                         ON HRM_SECTION_COUNTER.SCT_ID = HRM_DEPARTMENT_SECTIONS.SCT_ID 
                         WHERE HRM_DEPARTMENT_SECTIONS.SCT_ID = :SectionId";

            using (OracleConnection conn = new OracleConnection(_con))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":SectionId", sectionId));

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return dataTable;
        }
        public async Task<dynamic> SaveAppCrBillMaster(CrBill parameters, UserTocken ut)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_SAVE_APP_CRBILL_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_COMP_PREFIX_CODE", OracleDbType.Varchar2).Value = parameters.CompPrefixCode;
                    cmd.Parameters.Add("P_SCT_ID", OracleDbType.Varchar2).Value = parameters.SctId;
                    cmd.Parameters.Add("P_CUST_ID", OracleDbType.Varchar2).Value = parameters.CustId;
                    cmd.Parameters.Add("P_PARTY_NAME", OracleDbType.Varchar2).Value = parameters.PartyName;
                    cmd.Parameters.Add("P_CRBC_AMOUNT", OracleDbType.Decimal).Value = parameters.CrbcAmount;
                    cmd.Parameters.Add("P_COUNTER_ID", OracleDbType.Varchar2).Value = parameters.CounterId;
                    cmd.Parameters.Add("P_LOGIN_EMP_ID", OracleDbType.Varchar2).Value = ut.AUSR_ID;
                    cmd.Parameters.Add("P_PAY_CHQ_NO", OracleDbType.Varchar2).Value = parameters.PayChqNo;
                    cmd.Parameters.Add("P_PAY_CHQ_DATE", OracleDbType.Varchar2).Value = parameters.PayChqDate;
                    cmd.Parameters.Add("P_PAY_CHQ_AMT", OracleDbType.Varchar2).Value = parameters.PayChqAmt;
                    cmd.Parameters.Add("P_PAY_CHQ_BANK_ID", OracleDbType.Varchar2).Value = parameters.PayChqBankId;
                    cmd.Parameters.Add("P_PAY_CC_NO", OracleDbType.Varchar2).Value = parameters.PayCcNo;
                    cmd.Parameters.Add("P_PAY_CC_AMT", OracleDbType.Varchar2).Value = parameters.PayCcAmt;
                    cmd.Parameters.Add("P_PAY_CC_BANK_ID", OracleDbType.Varchar2).Value = parameters.PayCcBankId;
                    cmd.Parameters.Add("P_PAY_CASH_AMT", OracleDbType.Varchar2).Value = parameters.PayCashAmt;
                    cmd.Parameters.Add("P_PAY_WALLET_ID", OracleDbType.Varchar2).Value = parameters.PayWalletId;
                    cmd.Parameters.Add("P_PAY_WALLET_TRANS_NO", OracleDbType.Varchar2).Value = parameters.PayWalletTransNo;
                    cmd.Parameters.Add("P_PAY_WALLET_DATE", OracleDbType.Varchar2).Value = parameters.PayWalletDate;
                    cmd.Parameters.Add("P_PAY_WALLET_AMT", OracleDbType.Varchar2).Value = parameters.PayWalletAmt;
                    cmd.Parameters.Add("P_PAY_WALLET_BANK_ID", OracleDbType.Varchar2).Value = parameters.PayWalletBankId;
                    cmd.Parameters.Add("P_PAY_BANK_TRANS_REF_NO", OracleDbType.Varchar2).Value = parameters.PayBankTransRefNo;
                    cmd.Parameters.Add("P_PAY_BANK_TRANS_DATE", OracleDbType.Varchar2).Value = parameters.PayBankTransDate;
                    cmd.Parameters.Add("P_PAY_FROM_TRANS_BANK_ID", OracleDbType.Varchar2).Value = parameters.PayFromTransBankId;
                    cmd.Parameters.Add("P_PAY_TO_TRANS_BANK_ID", OracleDbType.Varchar2).Value = parameters.PayToTransBankId;
                    cmd.Parameters.Add("P_PAY_BANK_TRANS_AMT", OracleDbType.Varchar2).Value = parameters.PayBankTransAmt;
                    cmd.Parameters.Add("P_PAY_TO_CHEQUE_BANK_ID", OracleDbType.Varchar2).Value = parameters.PayToChequeBankId;
                    cmd.Parameters.Add("P_CRBC_DISC_AMT", OracleDbType.Varchar2).Value = parameters.CrbcDiscAmt;
                    cmd.Parameters.Add("P_CRBC_DISC_TYPE", OracleDbType.Varchar2).Value = parameters.CrbcDiscType;
                    cmd.Parameters.Add("P_CRBC_SETTLED_AMT", OracleDbType.Varchar2).Value = parameters.CrbcSettledAmt;
                    cmd.Parameters.Add("P_CRBC_TOTAL_AMT", OracleDbType.Varchar2).Value = parameters.CrbcTotalAmt;

                    OracleParameter retVal = new OracleParameter("RETVAL", OracleDbType.Varchar2, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(retVal);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    long crbillid = long.TryParse(retVal.Value.ToString(), out long result) ? result : 0;

                    if (crbillid>0)
                    {
                      var dat =  await SaveAppCrBillDetails(parameters.crbilDetails, crbillid);
                        return dat;
                    }
                    else
                    {
                        var msg1 = new DefaultMessage.Message1
                        {
                            Status = 501,
                            Message = "Error while Insertion"
                        };
                        return msg1;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg1 = new DefaultMessage.Message1
                {
                    Status = 500,
                    Message = ex.Message
                };
                return msg1;
            }
        }
        public async Task<dynamic> SaveAppCrBillDetails(List<CBillDetails> records,long crbillid)
        {
            using (OracleConnection conn = new OracleConnection(_con))
            {
                conn.Open();
                using (OracleTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_SAVE_APP_CRBILL_DETAILS", conn))
                        {
                            cmd.Transaction = transaction; // Set transaction separately
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add parameters
                            cmd.Parameters.Add("P_CRBC_ID", OracleDbType.Int64);
                            cmd.Parameters.Add("P_CRBC_DTLS_SECTION", OracleDbType.Varchar2);
                            cmd.Parameters.Add("P_CRBC_DTLS_TRANS_ID", OracleDbType.Varchar2);
                            cmd.Parameters.Add("P_CRBC_DTLS_TRANS_NO", OracleDbType.Varchar2);
                            cmd.Parameters.Add("P_CRBC_DTLS_TRANS_DATE", OracleDbType.Varchar2);
                            cmd.Parameters.Add("P_CRBC_DTLS_SET_AMT", OracleDbType.Decimal);
                            cmd.Parameters.Add("P_PREV_COL_AMT", OracleDbType.Decimal);
                            cmd.Parameters.Add("P_CRBC_DTLS_CUST_ID", OracleDbType.Int64);
                            cmd.Parameters.Add("P_CRBC_DTLS_DISC_AMT", OracleDbType.Decimal);
                            cmd.Parameters.Add("P_CRBC_DTLS_TRANS_AMT", OracleDbType.Decimal);

                            // Output parameter
                            OracleParameter retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 10)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(retvalParam);

                            foreach (var record in records)
                            {
                                cmd.Parameters["P_CRBC_ID"].Value = crbillid;
                                cmd.Parameters["P_CRBC_DTLS_SECTION"].Value = record.Section;
                                cmd.Parameters["P_CRBC_DTLS_TRANS_ID"].Value = record.TransId;
                                cmd.Parameters["P_CRBC_DTLS_TRANS_NO"].Value = record.TransNo;
                                cmd.Parameters["P_CRBC_DTLS_TRANS_DATE"].Value = record.TransDate;
                                cmd.Parameters["P_CRBC_DTLS_SET_AMT"].Value = record.SetAmt;
                                cmd.Parameters["P_PREV_COL_AMT"].Value = record.PrevColAmt;
                                cmd.Parameters["P_CRBC_DTLS_CUST_ID"].Value = record.CustId;
                                cmd.Parameters["P_CRBC_DTLS_DISC_AMT"].Value = record.DiscAmt;
                                cmd.Parameters["P_CRBC_DTLS_TRANS_AMT"].Value = record.TransAmt;

                                cmd.ExecuteNonQuery();

                                if (int.TryParse(retvalParam.Value.ToString(), out int result) && result == -1)
                                {
                                    throw new Exception("Failed to insert a record.");
                                }
                            }
                        }
                        transaction.Commit();
                        var msg1 = new DefaultMessage.Message1
                        {
                            Status =200,
                            Message = "Submitted Successfully"
                        };
                        return msg1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var msg1 = new DefaultMessage.Message1
                        {
                            Status = 500,
                            Message = ex.Message
                        };
                        return msg1;
                    }
                }
            }
        }

        public async Task<DataTable> GetAppBillNoFlexFill(string strSctId, string strSaleBillNo, int lngFinYearID)
        {
            DataTable resultTable = new DataTable();

            try
            {
                using (OracleConnection connection = new OracleConnection(_con))
                {
                    using (OracleCommand command = new OracleCommand("PRMTRANS.PKG_APP_CREDIT_BILL_SETTLEMENT.GET_APP_BILL_NO_FLEX_FILL", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.Add("Str_SCT_ID", OracleDbType.Varchar2).Value = strSctId;
                        command.Parameters.Add("strSaleBillNo", OracleDbType.Varchar2).Value = strSaleBillNo;
                        command.Parameters.Add("lngFinYearID", OracleDbType.Int32).Value = lngFinYearID;

                        // Output cursor
                        OracleParameter outCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outCursor);

                        connection.Open();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            resultTable.Load(reader);
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }

            return resultTable;
        }


        public async Task<dynamic> GetCreditbillsettled(string fromDate, string toDate, string custid = null)
        {
            try
            {


                using (OracleConnection conn = new OracleConnection(_con))
                using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_CR_BILL_SETTLE_DTLS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input Parameters
                    cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
                    //cmd.Parameters.Add("STRITEM_FILTER", OracleDbType.Varchar2).Value = itemid ?? (object)DBNull.Value;
                    cmd.Parameters.Add("STRCUST_FILTER", OracleDbType.Varchar2).Value = custid ?? (object)DBNull.Value;


                    // Output Parameter (Cursor)
                    OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputCursor);

                    // Execute Query
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return new DefaultMessage.Message3 { Status = 200, Data = dt };
                }
            }
            catch (Exception ex)
            {

                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }

        }








        #endregion Credit bill settlement
    }
}
