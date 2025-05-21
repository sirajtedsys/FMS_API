using FMS_API.Class;
using FMS_API.Data.Class;
using FMS_API.Data.DbModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Dynamic;
using static JwtService;
using System.Globalization;


namespace FMS_API.Repositry
{
    public class ExpenseTrackerRepositry
    {

            private readonly AppDbContext _DbContext;

            private readonly JwtHandler jwthand;
            private readonly string _con;
        private readonly IConfiguration Iconfiguration;

            public ExpenseTrackerRepositry(AppDbContext dbContext, JwtHandler _jwthand,IConfiguration _iconf)
            {
                _DbContext = dbContext;
                jwthand = _jwthand;
                _con = _DbContext.Database.GetConnectionString();
                Iconfiguration = _iconf;
            }

        public class Employee
        {
            public string EMP_ID { get; set; }
            public string EMP_NAME { get; set; }
            public string SW_LOGIN_NAME { get; set; }
            public string SW_PASSWORD { get; set; }
            public string ACC_MASTER_ID { get; set; }
            public string IS_ADV_ALLOWED { get; set; }
            public string EMP_CODE { get; set; }
            public string EMP_GENDER { get; set; }
        }


        public async Task<dynamic> CheckLogin(string userName, string password)
        {
            try
            {
                var da = await GetEmployeeDetails(userName, password);
                if(da.Status==200)
                {
                    var data = da.Data;

                    if (data != null && data.Count > 0)
                    {
                        var userdat = new UserTocken
                        {
                            AUSR_ID = data[0].EMP_ID,
                            USERNAME = data[0].SW_LOGIN_NAME,
                            PASSWORD = data[0].SW_PASSWORD
                        };

                        var token = jwthand.GenerateToken(userdat);
                        var dat = await _DbContext.APP_LOGIN_SETTINGS
                                                  .Where(x => x.EMP_ID == userdat.AUSR_ID)
                                                  .ToListAsync();

                        var existingDATA = dat.FirstOrDefault();

                        if (existingDATA != null)
                        {
                            existingDATA.TOKEN = token;
                            existingDATA.CREATE_ON = DateTime.Now;
                            await _DbContext.SaveChangesAsync();
                        }
                        else
                        {
                            var newLogin = new PRMMASTER_LoginSettings
                            {
                                EMP_ID = userdat.AUSR_ID,
                                TOKEN = token,
                                CREATE_ON = DateTime.Now
                            };

                            await _DbContext.APP_LOGIN_SETTINGS.AddAsync(newLogin);
                            await _DbContext.SaveChangesAsync();
                        }

                        return new DefaultMessage.Message1 { Status = 200, Message = token };
                    }
                    else
                    {
                        return new DefaultMessage.Message1 { Status = 600, Message = "Invalid Username and Password" };
                    }
                }
                else
                {

                    return new DefaultMessage.Message1 { Status = da.Status, Message = da.Message };
                }
             
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }
        }

        public async  Task<dynamic> GetActiveProjects()
        {
            List<dynamic> projects = new List<dynamic>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    conn.Open();
                    string query = "SELECT PROJECT_ID, PROJECT_NAME FROM PRMMASTER.INV_PROJECT_MASTER WHERE NVL(ACTIVE_STATUS, 'A') = 'A'";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                projects.Add(new 
                                {
                                    ProjectId = reader.GetInt32(0),
                                    ProjectName = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }


            return new DefaultMessage.Message3 { Status = 200, Data = projects };
        }

        public async Task<dynamic> GetEmployeeDetails(string userName, string password)
        {
            List<Employee> employees = new List<Employee>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT EMP_ID, EMP_NAME, SW_LOGIN_NAME, SW_PASSWORD, ACC_MASTER_ID, 
                       IS_ADV_ALLOWED, EMP_CODE, 
                       DECODE(NVL(EMP_GENDER,'M'),'M','Male','Female') AS EMP_GENDER
                FROM PRMMASTER.HRM_EMPLOYEE 
                WHERE 
                   NVL(ACTIVE_STATUS, 'A')='A' 
                  AND UPPER(SW_LOGIN_NAME) = UPPER(:userName)  
                  AND SW_PASSWORD = :password", conn))
                    {
                        cmd.Parameters.Add(":userName", OracleDbType.Varchar2).Value = userName;
                        cmd.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees.Add(new Employee
                                {
                                    EMP_ID = reader["EMP_ID"]?.ToString(),
                                    EMP_NAME = reader["EMP_NAME"]?.ToString(),
                                    SW_LOGIN_NAME = reader["SW_LOGIN_NAME"]?.ToString(),
                                    SW_PASSWORD = reader["SW_PASSWORD"]?.ToString(),
                                    ACC_MASTER_ID = reader["ACC_MASTER_ID"]?.ToString(),
                                    IS_ADV_ALLOWED = reader["IS_ADV_ALLOWED"]?.ToString(),
                                    EMP_CODE = reader["EMP_CODE"]?.ToString(),
                                    EMP_GENDER = reader["EMP_GENDER"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }


            return new DefaultMessage.Message3 { Status = 200, Data = employees };
            //return employees;
        }


        public async Task<dynamic> GetAccountMastersAsync()
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT ACC_MASTER_ID, ACC_MASTER_NAME, G_TYPE_SECTION 
                FROM PRMMASTER.ACC_ACCOUNT_MASTER 
                WHERE NVL(LIST_IN_EMP_EXPEN_FORM,'N')='Y' 
                  AND NVL(ACC_ACTIVE_STATUS, 'I')='A'", conn))
                    {
                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>
                        {
                            { "ACC_MASTER_ID", reader["ACC_MASTER_ID"]?.ToString() },
                            { "ACC_MASTER_NAME", reader["ACC_MASTER_NAME"]?.ToString() },
                            { "G_TYPE_SECTION", reader["G_TYPE_SECTION"]?.ToString() }
                        };
                                resultList.Add(row);
                            }
                        }
                    }
                }

                // Return successful response
                return new { Status = 200, Data = resultList };
            }
            catch (OracleException ex)
            {
                // Return Oracle-specific error message
                return new { Status = 500, Message = "Database error: " + ex.Message };
            }
            catch (Exception ex)
            {
                // Return general error message
                return new { Status = 500, Message = "An error occurred: " + ex.Message };
            }
        }


        public async Task<dynamic> GetEmployeeExpensesAsync(string strAccountId, string strFrom, string strTo)
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand(@"
      SELECT NVL(E.CONFIRM_STATUS, 'N') AS CONFIRM_STATUS, 
       D.EXP_DTLS_SLNO, 
       A.ACC_MASTER_NAME, 
       AD.ACC_MASTER_NAME AS ACC_EXPENCE, 
       D.EXP_DTLS_AMT, 
       D.EXP_DTLS_REMARK, 
       E.EMP_EXP_ID, 
       E.EMP_EXP_REF_NO, 
       E.EMP_EXP_DATE, 
       E.EXP_AMT,
       PM.PROJECT_ID,
       PM.PROJECT_NAME
FROM PRMTRANS.ACC_EMP_EXPENCE E
INNER JOIN PRMMASTER.ACC_ACCOUNT_MASTER A ON E.ACC_MASTER_ID = A.ACC_MASTER_ID 
INNER JOIN PRMTRANS.ACC_EMP_EXPENCE_DTLS D ON D.EMP_EXP_ID = E.EMP_EXP_ID
INNER JOIN PRMMASTER.ACC_ACCOUNT_MASTER AD ON D.ACC_MASTER_ID = AD.ACC_MASTER_ID 
INNER JOIN PRMMASTER.INV_PROJECT_MASTER PM ON E.PROJECT_ID = PM.PROJECT_ID
WHERE NVL(D.ACTIVE_STATUS, 'A') = 'A' 
  AND E.ACC_MASTER_ID = :strAccountId
  AND NVL(E.CANCEL_STATUS, 'N') = 'N'
                  AND EMP_EXP_DATE BETWEEN 
    TRUNC(TO_DATE(:strFrom, 'DD/MM/YY')) 
    AND TRUNC(TO_DATE(:strTo, 'DD/MM/YY')) + INTERVAL '1' DAY - INTERVAL '1' SECOND

                ORDER BY E.EMP_EXP_ID, EXP_DTLS_SLNO ASC", conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.Add(":strAccountId", OracleDbType.Varchar2).Value = strAccountId;
                        cmd.Parameters.Add(":strFrom", OracleDbType.Varchar2).Value = strFrom;
                        cmd.Parameters.Add(":strTo", OracleDbType.Varchar2).Value = strTo;

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>
                        {
                            { "CONFIRM_STATUS", reader["CONFIRM_STATUS"]?.ToString() },
                            { "EXP_DTLS_SLNO", reader["EXP_DTLS_SLNO"]?.ToString() },
                            { "ACC_MASTER_NAME", reader["ACC_MASTER_NAME"]?.ToString() },
                            { "ACC_EXPENCE", reader["ACC_EXPENCE"]?.ToString() },
                            { "EXP_DTLS_AMT", reader["EXP_DTLS_AMT"]?.ToString() },
                            { "EXP_DTLS_REMARK", reader["EXP_DTLS_REMARK"]?.ToString() },
                            { "EMP_EXP_ID", reader["EMP_EXP_ID"]?.ToString() },
                            { "EMP_EXP_REF_NO", reader["EMP_EXP_REF_NO"]?.ToString() },
                            { "EMP_EXP_DATE", reader["EMP_EXP_DATE"]?.ToString() },
                            { "EXP_AMT", reader["EXP_AMT"]?.ToString() },
                            { "PROJECT_ID", reader["PROJECT_ID"]?.ToString() },
                            { "PROJECT_NAME", reader["PROJECT_NAME"]?.ToString() }
                        };
                                resultList.Add(row);
                            }
                        }
                    }
                }

                // Return successful response
                return new { Status = 200, Data = resultList };
            }
            catch (OracleException ex)
            {
                return new { Status = 500, Message = "Database error: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = "An error occurred: " + ex.Message };
            }
        }

        public async Task<dynamic> GetEmployeeExpenseDetailsAsync(string strExpId)
        {
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT NVL(CONFIRM_STATUS, 'N') AS CONFIRM_STATUS, 
                       EXP_DTLS_SLNO, 
                       A.ACC_MASTER_NAME, 
                       AD.ACC_MASTER_NAME AS ACC_EXPENCE, 
                       EXP_DTLS_AMT, 
                       EXP_DTLS_REMARK, 
                       E.EMP_EXP_ID, 
                       EMP_EXP_REF_NO, 
                       EMP_EXP_DATE, 
                       EXP_AMT,
                       PM.PROJECT_ID,
                       PM.PROJECT_NAME
                FROM PRMTRANS.ACC_EMP_EXPENCE E
                INNER JOIN PRMMASTER.ACC_ACCOUNT_MASTER A ON E.ACC_MASTER_ID = A.ACC_MASTER_ID 
                INNER JOIN PRMTRANS.ACC_EMP_EXPENCE_DTLS D ON D.EMP_EXP_ID = E.EMP_EXP_ID
                INNER JOIN PRMMASTER.ACC_ACCOUNT_MASTER AD ON D.ACC_MASTER_ID = AD.ACC_MASTER_ID 
                INNER JOIN PRMMASTER.INV_PROJECT_MASTER PM ON E.PROJECT_ID = PM.PROJECT_ID
                WHERE E.EMP_EXP_ID = :strExpId 
                ORDER BY EXP_DTLS_SLNO ASC", conn))
                    {
                        // Add parameter to prevent SQL injection
                        cmd.Parameters.Add(":strExpId", OracleDbType.Varchar2).Value = strExpId;

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>
                        {
                            { "CONFIRM_STATUS", reader["CONFIRM_STATUS"]?.ToString() },
                            { "EXP_DTLS_SLNO", reader["EXP_DTLS_SLNO"]?.ToString() },
                            { "ACC_MASTER_NAME", reader["ACC_MASTER_NAME"]?.ToString() },
                            { "ACC_EXPENCE", reader["ACC_EXPENCE"]?.ToString() },
                            { "EXP_DTLS_AMT", reader["EXP_DTLS_AMT"]?.ToString() },
                            { "EXP_DTLS_REMARK", reader["EXP_DTLS_REMARK"]?.ToString() },
                            { "EMP_EXP_ID", reader["EMP_EXP_ID"]?.ToString() },
                            { "EMP_EXP_REF_NO", reader["EMP_EXP_REF_NO"]?.ToString() },
                            { "EMP_EXP_DATE", reader["EMP_EXP_DATE"]?.ToString() },
                            { "EXP_AMT", reader["EXP_AMT"]?.ToString() },
                            { "PROJECT_ID", reader["PROJECT_ID"]?.ToString() },
                            { "PROJECT_NAME", reader["PROJECT_NAME"]?.ToString() }
                        };
                                resultList.Add(row);
                            }
                        }
                    }
                }

                // Return successful response
                return new { Status = 200, Data = resultList };
            }
            catch (OracleException ex)
            {
                return new { Status = 500, Message = "Database error: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = "An error occurred: " + ex.Message };
            }
        }

        public async Task<dynamic> UpdateEmployeeExpense(string strUserId, string strRemarks, string expId)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    conn.Open();

                    string query = @"UPDATE PRMTRANS.ACC_EMP_EXPENCE SET UPDATE_USER = :userId, 
                                 CANCEL_STATUS = 'Y', CANCEL_DATE = SYSDATE, 
                                 CANCELLED_REMARKS = :remarks WHERE EMP_EXP_ID = :expId";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // Adding parameters to prevent SQL injection
                        cmd.Parameters.Add(new OracleParameter("userId", strUserId));
                        cmd.Parameters.Add(new OracleParameter("remarks", strRemarks));
                        cmd.Parameters.Add(new OracleParameter("expId", expId));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {

                            return new { Status = 200, Message = "Expence Cancelled Successfully" };
                        }
                        else
                        {

                            return new { Status = 404, Message = "Expence not found" };
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                return new { Status = 500, Message = ex.Message };
                // Log detailed Oracle error information if needed
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
                // Log detailed general error information if needed
            }
        }



        public async Task<dynamic> SaveEmployeeExpense(EmpExpence ex,UserTocken ut)
        {
            string retval = string.Empty;

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_APP_SAVE_ACC_EMP_EXPENCE", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Adding input parameters
                        cmd.Parameters.Add(new OracleParameter("P_ACC_MASTER_ID", OracleDbType.Varchar2)).Value = ex.accMasterId;
                        cmd.Parameters.Add(new OracleParameter("P_EMP_EXP_DATE", OracleDbType.Varchar2)).Value = ex.ExpDate; // You can modify the format based on your requirement
                        cmd.Parameters.Add(new OracleParameter("P_EXP_AMT", OracleDbType.Varchar2)).Value = ex.TotalAmt;
                        cmd.Parameters.Add(new OracleParameter("P_CREATE_USER", OracleDbType.Varchar2)).Value = ut.AUSR_ID;
                        cmd.Parameters.Add(new OracleParameter("P_PROJECT_ID", OracleDbType.Varchar2)).Value = ex.ProjectId;

                        

                        // Adding output parameter
                        var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 15)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        };
                        cmd.Parameters.Add(retvalParam);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();

                        // Get the output parameter value
                        retval = retvalParam.Value.ToString();

                        if (ex.EmpExpenceDetails == null || ex.EmpExpenceDetails.Count == 0)
                        {
                            return new { Status = 200, Message = "Expense saved successfully", RetVal = retval };
                        }

                        // Process details in parallel
                        var tasks = ex.EmpExpenceDetails.Select(detail => SaveEmployeeExpenseDetails(detail, ut, retval));
                        var results = await Task.WhenAll(tasks);

                        // Check if all are successful
                        var failedResult = results.FirstOrDefault(r => r.Status != 200);
                        return failedResult ?? results.Last();
                        //return 
                    }
                }
            }
            catch (OracleException exp)
            {
                return new { Status = 500, Message = exp.Message };
                // Handle or log Oracle-specific errors
            }
            catch (Exception exp)
            {
                return new { Status = 500, Message = exp.Message };
                // Handle or log general errors
            }

            //return retval;
        }


        public async Task<dynamic> SaveEmployeeExpenseDetails(EmpExpenceDetails parameters,UserTocken ut,string expid)
        {
            string retval = string.Empty;

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_APP_SAVE_ACC_EMP_EXP_DTLS", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Adding input parameters
                        cmd.Parameters.Add(new OracleParameter("P_EMP_EXP_ID", OracleDbType.Varchar2)).Value = expid;
                        cmd.Parameters.Add(new OracleParameter("P_ACC_MASTER_ID", OracleDbType.Varchar2)).Value = parameters.AccMasterId;
                        cmd.Parameters.Add(new OracleParameter("P_EXP_DTLS_AMT", OracleDbType.Varchar2)).Value = parameters.ExpDtlAmt;
                        cmd.Parameters.Add(new OracleParameter("P_EXP_DTLS_REMARK", OracleDbType.Varchar2)).Value = parameters.ExpDtlRemark;
                        cmd.Parameters.Add(new OracleParameter("P_EXP_DTLS_SLNO", OracleDbType.Varchar2)).Value = parameters.ExpDtlSlno;
                        cmd.Parameters.Add(new OracleParameter("P_ACTIVE_STATUS", OracleDbType.Varchar2)).Value = parameters.ActiveStatus;

                        // Adding output parameter
                        var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 15)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        };
                        cmd.Parameters.Add(retvalParam);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();

                        // Get the output parameter value
                        retval = retvalParam.Value.ToString();
                    }
                }
            }
            catch (OracleException ex)
            {
                return new { Status = 500, Message = ex.Message };
                // Handle or log Oracle-specific errors
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
                // Handle or log general errors
            }



            return new { Status = 200, Message = "Expense Added Successfully" };
        }




        public async Task<dynamic> GetAccountBalances(DateTime tdate, long YearID, long CompId,long AcMasterId)
        {
            var accounts = new List<dynamic>();

            string query = @"
    SELECT 
        ACC_MASTER_ID, 
        ACC_MASTER_NAME AS Name,  
        PRMTRANS.GET_CUR_BALANCE(
            :tdate1, :tdate2, :CompId, :YearID, ACC_MASTER_ID
        ) AS CurrentBalance
    FROM PRMMASTER.ACC_ACCOUNT_MASTER 
    WHERE ACC_MASTER_ID = :ACC_MASTER_ID AND NVL(EXPEN_ADV_LEDGER, 'N') = 'Y' 
    ORDER BY UPPER(ACC_MASTER_NAME)";

            using (var connection = new OracleConnection(_con))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    // ✅ Bind each parameter correctly
                    command.Parameters.Add(":tdate1", OracleDbType.Date).Value = tdate;
                    command.Parameters.Add(":tdate2", OracleDbType.Date).Value = tdate; // Second occurrence
                    command.Parameters.Add(":CompId", OracleDbType.Int64).Value = CompId;
                    command.Parameters.Add(":YearID", OracleDbType.Int64).Value = YearID;
                    command.Parameters.Add(":ACC_MASTER_ID", OracleDbType.Int64).Value = AcMasterId;

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                accounts.Add(new
                                {
                                    AccMasterId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    CurrentBalance = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
                                });
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        return new { Status = 500, Message = "Database Error: " + ex.Message };
                    }
                    catch (Exception ex)
                    {
                        return new { Status = 500, Message = "Error: " + ex.Message };
                    }
                }
            }

            return new { Status = 200, Data = accounts };
        }


        public async Task<dynamic> GetCompanySettingsAsync()
        {
            string query = @"
        SELECT COMP_ID, CUR_FIN_YEAR_ID, CUR_FIN_YEAR_CODE 
        FROM PRMADMIN.CMP_SETTINGS 
        WHERE ROWNUM = 1"; // Ensuring only one row is fetched

            using (OracleConnection conn = new OracleConnection(_con)) // Assuming _con is your connection string
            {
                try
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new
                                {
                                    Status = 200, // Success status
                                    Message = "Success",
                                    Data = new
                                    {
                                        CompanyId = reader.GetInt64(0), // Convert COMP_ID to Int64
                                        FinancialYearId = reader.GetInt64(1), // Convert CUR_FIN_YEAR_ID to Int64
                                        FinancialYearCode = reader.GetString(2) // CUR_FIN_YEAR_CODE is string
                                    }
                                };
                            }
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return new { Status = 500, Message = "Database Error: " + ex.Message };
                }
                catch (Exception ex)
                {
                    return new { Status = 500, Message = "Error: " + ex.Message };
                }
            }

            return new { Status = 404, Message = "No data found" };
        }

        public async Task<dynamic> GetTabs(long moduleId, long empId)
        {
            var tabs = new List<dynamic>();

            string query = @"
            SELECT T.TAB_ID, TAB_NAME, LINK, T.PRIORITY ,T.ICONS
            FROM PRMMASTER.HRM_EMPLOYEE E
            INNER JOIN PRMMASTER.WEB_MENU_GROUP_DTLS MGD ON MGD.GROUP_ID = E.MENU_GROUP_ID
            INNER JOIN PRMMASTER.WEB_TABS_VIEW_LINK T ON MGD.TAB_ID = T.TAB_ID AND MGD.MODULE_ID = T.MODULE_ID 
            WHERE MGD.MODULE_ID = :moduleId AND E.EMP_ID = :empId
            ORDER BY T.PRIORITY ASC";

            using (var connection = new OracleConnection(_con))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    // ✅ Binding Parameters
                    command.Parameters.Add(":moduleId", OracleDbType.Int64).Value = moduleId;
                    command.Parameters.Add(":empId", OracleDbType.Varchar2).Value = empId;

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tabs.Add(new
                                {
                                    TabId = reader.GetInt32(0),
                                    TabName = reader.GetString(1),
                                    Link = reader.GetString(2),
                                    Priority = reader.GetInt32(3),
                                    Icon = reader.GetString(4),
                                });
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        return new { Status = 500, Message = "Database Error: " + ex.Message };
                    }
                    catch (Exception ex)
                    {
                        return new { Status = 500, Message = "Error: " + ex.Message };
                    }
                }
            }

            return new { Status = 200, Data = tabs };
        }


        public async Task<dynamic> GetCompayId()
        {
            try
            {
                //var data = await _DbContext.EMR_ADMIN_USERS
                //    .Where(x => x.AUSR_USERNAME == username
                //    && x.AUSR_PWD == password
                //    && x.AUSR_STATUS != "D")
                //    .ToListAsync();


                var CompId = Iconfiguration["Acc_Comp_Id"];



                //var  comp =    { CompId = CompId };

                return new DefaultMessage.Message3 { Status = 200, Data = new { CompId = CompId } };




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

        public async Task<dynamic> GetAllWorkSheet(DateTime workDateF, DateTime workDateT)
        {
            var result = new List<Dictionary<string, object>>();

            using (OracleConnection conn = new OracleConnection(_con))
            {
                try
                {
                    await conn.OpenAsync(); // Open connection asynchronously

                    string query = @"
                SELECT DISTINCT DW.CREATE_DATE, 
                                WORK_REFNO, 
                                CALLED_DESCRIPTION, 
                                DW.WORK_SHEET_ID, 
                                DW.WORK_ID, 
                                DW.TOTAL_WORK_PERCENTAGE, 
                                DW.TOTAL_WORK_HOURS, 
                                DW.TOTAL_WORK_MINUTES, 
                                DW.WORK_DATE, 
                                DW.STATUS_ID, 
                                DW.PROGRESS_NOTE, 
                                WORK_STATUS,
                                W.CUST_ID, 
                                C.CUST_NAME, 
                                W.PROJECT_ID, 
                                P.PROJECT_NAME, 
                                W.MODULE_ID, 
                                M.MODULE_NAME, 
                                W.CALL_TYPE_ID, 
                                CT.CALL_TYPE,  
                                W.EMERGENCY_ID, 
                                E.EMERGENCY_TYPE, 
                                W.ERROR_TYPE_ID, 
                                ET.ERROR_TYPE, 
                                CALLED_BY,
                                EMP_NAME,
                                DW.CREATE_USER
                FROM PRMTRANS.DAILY_WORK_SHEET DW
                INNER JOIN PRMTRANS.INV_WORK W ON W.WORK_ID = DW.WORK_ID
                INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
                INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
                INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
                LEFT JOIN PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
                LEFT JOIN PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
                LEFT JOIN PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
                INNER JOIN PRMMASTER.INV_WORK_STATUS ST ON ST.WORK_STATUS_ID = DW.STATUS_ID
                INNER JOIN PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = DW.CREATE_USER
                WHERE TRUNC(DW.WORK_DATE) BETWEEN :WorkDateF AND :WorkDateT
                ORDER BY EMP_NAME ASC, DW.CREATE_DATE DESC";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":WorkDateF", OracleDbType.Date).Value = workDateF;
                        cmd.Parameters.Add(":WorkDateT", OracleDbType.Date).Value = workDateT;

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync()) // Execute query
                        {
                            //if (!reader.HasRows)
                            //{
                            //    return new DefaultMessage.Message3 { Status = 200, Data = [] };
                            //}

                            while (await reader.ReadAsync()) // Read each row
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }

                                result.Add(row);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    return new DefaultMessage.Message1
                    {
                        Status = 500,
                        Message = ex.Message
                    };
                }
            }

            return new DefaultMessage.Message3 { Status = 200, Data = result };
        }


        public async Task<dynamic> GetDailyWorkSheet(long UserId, DateTime workDate, DateTime workDate1)
        {
            var result = new List<Dictionary<string, object>>();

            using (OracleConnection conn = new OracleConnection(_con))
            {
                try
                {
                    await conn.OpenAsync();  // Use asynchronous method for opening connection

					//        string query = @"
					//SELECT DISTINCT DW.CREATE_DATE, 
					//                WORK_REFNO, 
					//                CALLED_DESCRIPTION, 
					//                DW.WORK_SHEET_ID, 
					//                DW.WORK_ID, 
					//                DW.TOTAL_WORK_PERCENTAGE, 
					//                DW.TOTAL_WORK_HOURS, 
					//                DW.TOTAL_WORK_MINUTES, 
					//                DW.WORK_DATE, 
					//                DW.STATUS_ID, 
					//                DW.PROGRESS_NOTE, 
					//                WORK_STATUS,
					//                W.CUST_ID, 
					//                C.CUST_NAME, 
					//                W.PROJECT_ID, 
					//                P.PROJECT_NAME, 
					//                W.MODULE_ID, 
					//                M.MODULE_NAME, 
					//                W.CALL_TYPE_ID, 
					//                CT.CALL_TYPE,  
					//                W.EMERGENCY_ID, 
					//                E.EMERGENCY_TYPE, 
					//                W.ERROR_TYPE_ID, 
					//                ET.ERROR_TYPE, 
					//                CALLED_BY 
					//FROM PRMTRANS.DAILY_WORK_SHEET DW
					//INNER JOIN PRMTRANS.INV_WORK W ON W.WORK_ID = DW.WORK_ID 
					//INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
					//INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
					//INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
					//LEFT JOIN PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
					//LEFT JOIN PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
					//LEFT JOIN PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
					//INNER JOIN PRMMASTER.INV_WORK_STATUS ST ON ST.WORK_STATUS_ID = DW.STATUS_ID
					//WHERE TO_DATE(DW.WORK_DATE, 'DD/MM/YY') = :workDate 
					//AND DW.CREATE_USER = :userId
					//ORDER BY DW.CREATE_DATE DESC";
					string query = @"
            SELECT DISTINCT DW.CREATE_DATE, 
                            WORK_REFNO, 
                            CALLED_DESCRIPTION, 
                            DW.WORK_SHEET_ID, 
                            DW.WORK_ID, 
                            DW.TOTAL_WORK_PERCENTAGE, 
                            DW.TOTAL_WORK_HOURS, 
                            DW.TOTAL_WORK_MINUTES, 
                            DW.WORK_DATE, 
                            DW.STATUS_ID, 
                            DW.PROGRESS_NOTE, 
                            WORK_STATUS,
                            W.CUST_ID, 
                            C.CUST_NAME, 
                            W.PROJECT_ID, 
                            P.PROJECT_NAME, 
                            W.MODULE_ID, 
                            M.MODULE_NAME, 
                            W.CALL_TYPE_ID, 
                            CT.CALL_TYPE,  
                            W.EMERGENCY_ID, 
                            E.EMERGENCY_TYPE, 
                            W.ERROR_TYPE_ID, 
                            ET.ERROR_TYPE, 
                            CALLED_BY 
            FROM PRMTRANS.DAILY_WORK_SHEET DW
            INNER JOIN PRMTRANS.INV_WORK W ON W.WORK_ID = DW.WORK_ID 
            INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
            INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
            INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
            LEFT JOIN PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
            LEFT JOIN PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
            LEFT JOIN PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
            INNER JOIN PRMMASTER.INV_WORK_STATUS ST ON ST.WORK_STATUS_ID = DW.STATUS_ID
            WHERE TO_DATE(DW.WORK_DATE, 'DD/MM/YY') between :workDate  and :workDate1
            AND DW.CREATE_USER = :userId
            ORDER BY DW.CREATE_DATE DESC";

					OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add(":workDate", OracleDbType.Date).Value = workDate;
					cmd.Parameters.Add(":workDate1", OracleDbType.Date).Value = workDate1;
					cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = UserId;

                    OracleDataReader reader = await cmd.ExecuteReaderAsync(); // Asynchronous execution of the query

                    while (await reader.ReadAsync())  // Asynchronous read of each row
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        result.Add(row);
                    }

                    await reader.CloseAsync(); // Close the reader asynchronously
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the database interaction
                    var msg1 = new DefaultMessage.Message1
                    {
                        Status = 500,
                        Message = ex.Message
                    };
                    return msg1;
                }
            }

            return new DefaultMessage.Message3 { Status = 200, Data = result };
        }

        public async Task<dynamic> GetOracleTimeAsync()
        {
            using (OracleConnection conn = new OracleConnection(_con))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT SYSTIMESTAMP FROM DUAL"; // Fetch Oracle timestamp

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        object result = await cmd.ExecuteScalarAsync();

                        // Convert to DateTimeOffset since SYSTIMESTAMP includes timezone
                        DateTimeOffset? oracleTime = result != DBNull.Value ? (DateTimeOffset)result : (DateTimeOffset?)null;

                        return new { Status = 200, Data = new { Date = oracleTime } };
                    }
                }
                catch (Exception ex)
                {
                    return new { Status = 500, Message = ex.Message };
                }
            }
        }



        public async Task<dynamic> SaveLeaveRequestAsync(LeaveRequest parameters,UserTocken ut)
        {
            try
            {
                using var connection = new OracleConnection(_con);
                using var command = new OracleCommand("PRMTRANS.SP_SAVE_APP_EMP_LEAVE_REQUEST", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var specificDate2 = DateTime.ParseExact(parameters.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var specificDate3 = DateTime.ParseExact(parameters.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                // Input parameters
                command.Parameters.Add("P_FROM_DATE", OracleDbType.Date).Value = specificDate2;
                command.Parameters.Add("P_TO_DATE", OracleDbType.Date).Value = specificDate3;
                command.Parameters.Add("P_STATUS_ID", OracleDbType.Varchar2).Value = parameters.StatusId;
                command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2).Value = ut.AUSR_ID;
                command.Parameters.Add("P_LEAVE_REASON", OracleDbType.Varchar2).Value = parameters.LeaveReason;
                //var today = DateTime.Today; // This gives "15/05/2025 00:00:00"
                var specificDate = DateTime.ParseExact(parameters.LeaveRequestDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                command.Parameters.Add("P_LEAVE_REQ_DATE", OracleDbType.Date).Value = specificDate;







                // Output parameter
                command.Parameters.Add("RETVAL", OracleDbType.Varchar2, 50).Direction = ParameterDirection.Output;

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                var a  =  command.Parameters["RETVAL"].Value.ToString();
                return new { Status = 200,Message="Leave request submitted successfully",Data= a};
            }
            catch (OracleException ex)
            {
                // Log Oracle-specific exceptions here if needed

                return new { Status = 500, Message = ex.Message };
            }
            catch (Exception ex)
            {
                // Log general exceptions

                return new { Status = 500, Message = ex.Message };
            }
        }

        public async Task<dynamic> GetLeaveRequestsByUserAsync(string createUser)
        {
            var results = new List<dynamic>();

            try
            {
                using var connection = new OracleConnection(_con);
                using var command = new OracleCommand(@"
                SELECT 
                    LEAVE_REQUEST_ID, 
                    FROM_DATE, 
                    TO_DATE,
                    LEAVE_REASON, 
                    LEAVE_REQ_NO,
                    L.STATUS_ID,
                    LEAVE_STATUS,
                    L.CREATE_DATE,
                    LEAVE_REQ_DATE
                FROM PRMTRANS.HRM_EMP_LEAVE_REQUEST L
                INNER JOIN PRMMASTER.HRM_LEAVE_STATUS S ON S.LEAVE_STATUS_ID = L.STATUS_ID
                WHERE L.CREATE_USER = :createUser", connection);

                command.Parameters.Add("createUser", OracleDbType.Varchar2).Value = createUser;

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    dynamic row = new ExpandoObject();
                    var dict = (IDictionary<string, object>)row;

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }

                    results.Add(row);
                }
            }
            catch (OracleException ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }

            return new { Status = 200, Data = results };
        }




        public async Task<dynamic> DeleteLeaveRequest(string leaveRequestId)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
                throw new ArgumentException("LeaveRequestId cannot be null or empty.");

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    string query = "DELETE FROM PRMTRANS.HRM_EMP_LEAVE_REQUEST WHERE LEAVE_REQUEST_ID = :LeaveRequestId";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add("LeaveRequestId", OracleDbType.Varchar2).Value = leaveRequestId;

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {

                            return new { Status = 200, Message = "Leave request deleted successfully" };
                        }
                        else
                        {

                            return new { Status = 400, Message = "Unable to delete leave request" };
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Handle Oracle-specific errors
                return new { Status = 500, Message = ex.Message };
            }
            catch (Exception ex)
            {
                // Handle general errors
                return new { Status = 500, Message = ex.Message };
            }

        }


        public async Task<dynamic> UpdateLeaveRequest(string leaveRequestId, DateTime fromDate, DateTime toDate, string leaveReason)
        {
            if (string.IsNullOrWhiteSpace(leaveRequestId))
                throw new ArgumentException("LeaveRequestId cannot be null or empty.");

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    string query = @"
                    UPDATE PRMTRANS.HRM_EMP_LEAVE_REQUEST
                    SET FROM_DATE = :FromDate,
                        TO_DATE = :ToDate,
                        LEAVE_REASON = :LeaveReason
                    WHERE LEAVE_REQUEST_ID = :LeaveRequestId";

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add("FromDate", OracleDbType.Date).Value = fromDate;
                        command.Parameters.Add("ToDate", OracleDbType.Date).Value = toDate;
                        command.Parameters.Add("LeaveReason", OracleDbType.Varchar2).Value = leaveReason;
                        command.Parameters.Add("LeaveRequestId", OracleDbType.Varchar2).Value = leaveRequestId;

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {

                            return new { Status = 200, Message = "Leave request Update successfully" };
                        }
                        else
                        {

                            return new { Status = 400, Message = "Unable to Update leave request" };
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Handle Oracle-specific errors
                return new { Status = 500, Message = ex.Message };
            }
            catch (Exception ex)
            {
                // Handle general errors
                return new { Status = 500, Message = ex.Message };
            }


            //return false;
        }

    
    
    
    }
}
