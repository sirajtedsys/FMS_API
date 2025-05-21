using FMS_API.Class;
using FMS_API.Data.Class;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;
//using FMS_API.Repositry;
using static JwtService;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data.Common;
using System.Dynamic;


namespace FMS_API.Repositry
{
    public class AttendenceRepositry
    {

        private readonly AppDbContext _DbContext;

        private readonly JwtHandler jwthand;
        private readonly string _con;
        //private readonly AddCallRepository acp;
        private readonly AddCallRepository acp;
        public AttendenceRepositry(AddCallRepository _acp,AppDbContext dbContext, JwtHandler _jwthand)
        {
            _DbContext = dbContext;
            jwthand = _jwthand;
            _con = _DbContext.Database.GetConnectionString();
            acp = _acp;
        }



      
        public async Task<dynamic> GetPunchDetails(PunchDetails parameters)
        {
            try
            {


                using (OracleConnection conn = new OracleConnection(_con))
                using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_HRM_PUNCH_DTLS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    // Input Parameters
                    cmd.Parameters.Add("P_OPR_ID", OracleDbType.Int64).Value = parameters.P_OPR_ID;
                    cmd.Parameters.Add("P_PUNCH_ID", OracleDbType.Int64).Value = parameters.PunchId;
                    cmd.Parameters.Add("P_PUNCH_DATE", OracleDbType.Date).Value = parameters.PunchDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PUMCH_TYPE", OracleDbType.Int32).Value = parameters.PunchType;
                    cmd.Parameters.Add("P_EMP_ID", OracleDbType.Int64).Value = parameters.EmpId;
                    cmd.Parameters.Add("P_WHERE1", OracleDbType.Varchar2).Value = parameters.Where1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_WHERE", OracleDbType.Varchar2).Value = parameters.Where ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_Hdr", OracleDbType.Int32).Value = parameters.Hdr;
                    cmd.Parameters.Add("strSDate", OracleDbType.Varchar2).Value = parameters.StrSDate;
                    cmd.Parameters.Add("strFDate", OracleDbType.Varchar2).Value = parameters.StrFDate;
					cmd.Parameters.Add("P_PUNCH_FROM", OracleDbType.Int32).Value = parameters.PunchFrom;
					cmd.Parameters.Add("P_PROJECT_ID", OracleDbType.Int32).Value = parameters.ProjectId;
					cmd.Parameters.Add("P_PUNCH_RMKS", OracleDbType.Varchar2).Value = parameters.PunchRemarks;
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



        public async Task<dynamic> GetEmployeePunchDetails(long empId, DateTime punchDate)
        {
            DataTable resultTable = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand(@"
                SELECT 
                    PUNCH_ID, 
                    PUMCH_TYPE, 
                    EMP_ID, 
                    PUNCH_DATE,
                    CASE 
                        WHEN PUMCH_TYPE = 0 THEN 'CHECK IN'
                        WHEN PUMCH_TYPE = 1 THEN 'CHECK OUT'
                        WHEN PUMCH_TYPE = 2 THEN 'BREAK IN'
                        WHEN PUMCH_TYPE = 3 THEN 'BREAK OUT'
                        ELSE 'UNKNOWN'
                    END AS PUMCH_TYPE_NAME,PUNCH_RMKS
                FROM PRMTRANS.HRM_PUNCH_DTLS 
                WHERE EMP_ID = :empid 
                AND TRUNC(PUNCH_DATE) = TRUNC(:punchDate)", conn)) // FIXED HERE
                    {
                        // Bind parameters
                        cmd.Parameters.Add(new OracleParameter(":empid", OracleDbType.Int64)).Value = empId;
                        cmd.Parameters.Add(new OracleParameter(":punchDate", OracleDbType.Date)).Value = punchDate; // FIXED HERE

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = "Database Error: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }

            return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
        }


        public async Task<dynamic> GetOpenedWorkOfEmpkoyee(long empId)
        {
            DataTable workAssignmentsTable = new DataTable();

            string query = @"
    SELECT WORK_STATUS, 
       a.EMP_ID,
       WORK_DTLS_ID, 
       w.WORK_ID,
       WORK_REFNO, 
       WORK_DATE, 
       W.CUST_ID, 
       CUST_NAME, 
       W.PROJECT_ID, 
       PROJECT_NAME, 
       W.MODULE_ID, 
       MODULE_NAME, 
       W.WORK_STATUS_ID,
       W.CALL_TYPE_ID, 
       CALL_TYPE,  
       W.EMERGENCY_ID, 
       EMERGENCY_TYPE, 
       W.ERROR_TYPE_ID, 
       ERROR_TYPE,  
       CALLED_BY, 
       CALLED_DESCRIPTION, 
       EMP.EMP_NAME
      , NVL(ATTACH_FILE_COUNT,0)ATTACH_FILE_COUNT
FROM PRMTRANS.INV_WORK_ASSIGN_DTLS A
INNER JOIN PRMTRANS.INV_WORK W ON W.WORK_ID = A.WORK_ID
INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
LEFT JOIN PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
LEFT JOIN PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
LEFT JOIN PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
INNER JOIN PRMMASTER.INV_WORK_STATUS ST ON ST.WORK_STATUS_ID = A.WORK_STATUS_ID
INNER JOIN PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = W.CREATE_USER
LEFT JOIN (  SELECT COUNT (ATTACH_FILE_ID) AS ATTACH_FILE_COUNT,WORK_ID FROM PRMTRANS.INV_WORK_ATTACH_FILE GROUP BY WORK_ID )WAT ON WAT.WORK_ID = A.WORK_ID
WHERE A.WORK_STATUS_ID IN ( 3,7) AND A.EMP_ID = :EmpId
ORDER BY A.CREATE_DATE DESC";

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    var command = new OracleCommand(query, connection);
                    // Adding the dynamic EMP_ID parameter to the query
                    command.Parameters.Add(new OracleParameter(":EmpId", empId));

                    var adapter = new OracleDataAdapter(command);

                    // Fill the DataTable with the result set
                    adapter.Fill(workAssignmentsTable);
                }

                // Return the populated DataTable with status
                return new DefaultMessage.Message3 { Status = 200, Data = workAssignmentsTable };
            }
            catch (OracleException sqlEx)
            {
                // Handle Oracle exceptions
                return new DefaultMessage.Message1 { Status = 500, Message = "Oracle Error: " + sqlEx.Message };
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }
        }

        public async Task<dynamic> GetEmployeePunchDet(long empId, DateTime punchStartDate, DateTime punchEndDate)
        {
            DataTable resultTable = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();
                    using (OracleCommand cmd = new OracleCommand(@"
            SELECT 
                PUNCH_DATE AS ""Punch Time"", 
                CASE 
                    WHEN B.PUMCH_TYPE = 0 THEN 'Check In' 
                    WHEN B.PUMCH_TYPE = 2 THEN 'Break In' 
                    WHEN B.PUMCH_TYPE = 3 THEN 'Break Out' 
                    ELSE 'Check Out' 
                END AS ""Att. Type"",
                CASE 
                    WHEN B.PUNCH_STATUS IS NULL THEN 'PUNCH' 
                    WHEN B.PUNCH_STATUS = 1 THEN 'NEW' 
                    WHEN B.PUNCH_STATUS = 5 THEN 'UPDATED' 
                    ELSE 'DELETED' 
                END AS ""Status"", 
                B.PUNCH_STATUS, 
                B.EMP_ID, 
                B.PUNCH_ID, 
                B.PUMCH_TYPE,B.PUNCH_RMKS
            FROM PRMTRANS.HRM_PUNCH_DTLS B
            WHERE B.EMP_ID = :empid 
            AND TRUNC(B.PUNCH_DATE) BETWEEN :startDate AND :endDate
            ORDER BY B.PUNCH_DATE", conn))
                    {
                        // Bind parameters
                        cmd.Parameters.Add(new OracleParameter(":empid", OracleDbType.Int64)).Value = empId;
                        cmd.Parameters.Add(new OracleParameter(":startDate", OracleDbType.Date)).Value = punchStartDate;
                        cmd.Parameters.Add(new OracleParameter(":endDate", OracleDbType.Date)).Value = punchEndDate;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = "Database Error: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }

            // Handle empty result case
            if (resultTable.Rows.Count == 0)
            {
                return new DefaultMessage.Message3 { Status = 200, Data = "No records found" };
            }

            return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
        }


        public async Task<dynamic> GetWorkAssignments(long empId)
        {
            DataTable workAssignmentsTable = new DataTable();

            string query = @"
                   SELECT WORK_STATUS, a.EMP_ID,WORK_DTLS_ID, w.WORK_ID,WORK_REFNO, WORK_DATE, W.CUST_ID,CUST_NAME, W.PROJECT_ID, PROJECT_NAME,W.MODULE_ID,MODULE_NAME, W.WORK_STATUS_ID,
           W.CALL_TYPE_ID,CALL_TYPE,  W.EMERGENCY_ID,EMERGENCY_TYPE, 
           W.ERROR_TYPE_ID,ERROR_TYPE,  CALLED_BY, CALLED_DESCRIPTION,EMP.EMP_NAME,EMP.EMP_NAME ASSIGNED_BY,
              NVL(ATTACH_FILE_COUNT,0)ATTACH_FILE_COUNT
           FROM PRMTRANS.INV_WORK_ASSIGN_DTLS A
           INNER JOIN PRMTRANS.INV_WORK W ON W.WORK_ID=A.WORK_ID
           INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID=W.CUST_ID
           INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID=W.PROJECT_ID
           INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID=W.MODULE_ID
           LEFT JOIN PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID=W.EMERGENCY_ID
           LEFT JOIN PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID=W.ERROR_TYPE_ID
           LEFT JOIN PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID=W.CALL_TYPE_ID
           inner join PRMMASTER.INV_WORK_STATUS st on st.WORK_STATUS_ID=a.WORK_STATUS_ID
           INNER JOIN PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = W.CREATE_USER
           INNER JOIN PRMMASTER.HRM_EMPLOYEE EMPA ON EMPA.EMP_ID = A.CREATE_USER
           LEFT JOIN (  SELECT COUNT (ATTACH_FILE_ID) AS ATTACH_FILE_COUNT,WORK_ID FROM PRMTRANS.INV_WORK_ATTACH_FILE GROUP BY WORK_ID )WAT ON WAT.WORK_ID = A.WORK_ID
           where w.WORK_STATUS_ID in (2,3,5,7)  AND A.EMP_ID = :EmpId ORDER BY A.CREATE_DATE DESC";

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    var command = new OracleCommand(query, connection);
                    // Adding the dynamic EMP_ID parameter to the query
                    command.Parameters.Add(new OracleParameter(":EmpId", empId));

                    var adapter = new OracleDataAdapter(command);

                    // Fill the DataTable with the result set
                    adapter.Fill(workAssignmentsTable);
                }

                // Return the populated DataTable with status
                return new DefaultMessage.Message3 { Status = 200, Data = workAssignmentsTable };
            }
            catch (OracleException sqlEx)
            {
                // Handle Oracle exceptions
                return new DefaultMessage.Message1 { Status = 500, Message = "Oracle Error: " + sqlEx.Message };
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }
        }


		public async Task<dynamic> GetAttendenceReport(DateTime strFDate, DateTime strTDate, long Empid)
		{
			try
			{
				// Define the strWhere condition
				string strWhere = "";

				
				if (strFDate != DateTime.MinValue && strTDate != DateTime.MinValue)
				{

					//strWhere += $" AND (TRUNC(A.DT) BETWEEN TO_DATE('{strFDate:dd/MM/yyyy}', 'dd/MM/yyyy') AND TO_DATE('{strTDate:dd/MM/yyyy}', 'dd/MM/yyyy') + (1/24))";
					strWhere += $" AND (TRUNC(B.PUNCH_DATE) BETWEEN TO_DATE('{strFDate:dd/MM/yyyy}', 'dd/MM/yyyy') AND TO_DATE('{strTDate:dd/MM/yyyy}', 'dd/MM/yyyy') + (1/24))";
				}

				// Create Oracle Connection and Command
				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_HRM_PUNCH_DTLS", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("P_OPR_ID", OracleDbType.Int64).Value = 100;
					cmd.Parameters.Add("P_PUNCH_ID", OracleDbType.Int64).Value = 0;
					cmd.Parameters.Add("P_PUNCH_DATE", OracleDbType.Date).Value = null;
					cmd.Parameters.Add("P_PUNCH_TYPE", OracleDbType.Int32).Value = 0;
					cmd.Parameters.Add("P_EMP_ID", OracleDbType.Int64).Value = 0;
					cmd.Parameters.Add("P_WHERE1", OracleDbType.Varchar2).Value = null;
					cmd.Parameters.Add("P_WHERE", OracleDbType.Varchar2).Value = strWhere ?? (object)DBNull.Value;
					cmd.Parameters.Add("P_Hdr", OracleDbType.Int32).Value = 3;
			
					cmd.Parameters.Add("strTDate", OracleDbType.Varchar2).Value = strTDate.ToString("dd/MM/yyyy"); 
					cmd.Parameters.Add("strFDate", OracleDbType.Varchar2).Value = strFDate.ToString("dd/MM/yyyy");

					cmd.Parameters.Add("P_PUNCH_FROM", OracleDbType.Int32).Value = null;
					cmd.Parameters.Add("P_PROJECT_ID", OracleDbType.Int32).Value = 0;
					cmd.Parameters.Add("P_PUNCH_RMKS", OracleDbType.Int32).Value = null;
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

		public async Task<dynamic> GetAttenReport(DateTime strFDate, DateTime strTDate, long Empid, bool blnType)
		{
			try
			{
				//Define the strWhere condition
				string strWhere = "";

				if (Empid != 0)
				{
					strWhere += $" AND " + (blnType ? " B.EMP_ID" : " A.EMP_ID") + "=" + Empid;
				}
				if (strFDate != DateTime.MinValue && strTDate != DateTime.MinValue)
				{
					strWhere += $" AND TRUNC(" + (blnType ? "B.PUNCH_DATE" : "A.DT") + ")";
					strWhere += $" BETWEEN TO_DATE('{strFDate:dd/MM/yyyy}', 'dd/MM/yyyy') AND TO_DATE('{strTDate:dd/MM/yyyy}', 'dd/MM/yyyy')";
				}


				//Create Oracle Connection and Command
				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_HRM_PUNCH_DTLS", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					//Input Parameters
					cmd.Parameters.Add("P_OPR_ID", OracleDbType.Int64).Value = 100;
					cmd.Parameters.Add("P_PUNCH_ID", OracleDbType.Int64).Value = 0;
					cmd.Parameters.Add("P_PUNCH_DATE", OracleDbType.Date).Value = null;
					cmd.Parameters.Add("P_PUNCH_TYPE", OracleDbType.Int32).Value = 0;
					cmd.Parameters.Add("P_EMP_ID", OracleDbType.Int64).Value = Empid;
					cmd.Parameters.Add("P_WHERE1", OracleDbType.Varchar2).Value = null;
					cmd.Parameters.Add("P_WHERE", OracleDbType.Varchar2).Value = strWhere ?? (object)DBNull.Value;
					if (blnType == false)
					{
						cmd.Parameters.Add("P_Hdr", OracleDbType.Int32).Value = 4;
					}
					else
					{
						cmd.Parameters.Add("P_Hdr", OracleDbType.Int32).Value = 3;
					}
					cmd.Parameters.Add("strTDate", OracleDbType.Varchar2).Value = strTDate.ToString("dd/MM/yyyy"); 
					cmd.Parameters.Add("strFDate", OracleDbType.Varchar2).Value = strFDate.ToString("dd/MM/yyyy"); 

					cmd.Parameters.Add("P_PUNCH_FROM", OracleDbType.Int32).Value = null;
					cmd.Parameters.Add("P_PROJECT_ID", OracleDbType.Int32).Value = 0;
					cmd.Parameters.Add("P_PUNCH_RMKS", OracleDbType.Int32).Value = null;


					//Output Parameter(Cursor)
					OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
					{
						Direction = ParameterDirection.Output
					};
					cmd.Parameters.Add(outputCursor);

					//Execute Query
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



	
        public async Task<dynamic> SaveAppDailyWorkSheetAsync(DailyWorkSheet parameters,UserTocken ut)
        {
            string retval = string.Empty;

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    await connection.OpenAsync();
                    var formattedDate = DateTime.ParseExact(parameters.WorkDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");


                    using (var command = new OracleCommand("PRMTRANS.SP_SAVE_APP_DAILY_WORK_SHEET", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                    
                        // Adding input parameters
                        command.Parameters.Add("P_WORK_ID", OracleDbType.Varchar2).Value = parameters.WorkId;
                        command.Parameters.Add("P_TOTAL_WORK_PERCENTAGE", OracleDbType.Varchar2).Value = parameters.TotalWorkPercentage;
                        command.Parameters.Add("P_TOTAL_WORK_HOURS", OracleDbType.Varchar2).Value = parameters.TotalWorkHours;
                        command.Parameters.Add("P_TOTAL_WORK_MINUTES", OracleDbType.Varchar2).Value = parameters.TotalWorkMinutes;
                        command.Parameters.Add("P_WORK_DATE", OracleDbType.Varchar2).Value = formattedDate;
                        command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2).Value = ut.AUSR_ID;
                        command.Parameters.Add("P_STATUS_ID", OracleDbType.Varchar2).Value = parameters.StatusId;
                        command.Parameters.Add("P_PROGRESS_NOTE", OracleDbType.Varchar2).Value = parameters.ProgressNote;

                        // Adding output parameter for RETVAL
                        var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 10)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(retvalParam);

                        // Execute the stored procedure
                        await command.ExecuteNonQueryAsync();

                        // Get the output parameter value
                        retval = retvalParam.Value.ToString();
                    }
                }

                if (retval == "1")
                {
                    var res = await acp.UpdateWorkStatus(long.Parse(parameters.WorkDtlsId), long.Parse(parameters.WorkId), int.Parse(parameters.StatusId));

                    if (res.Status==200)
                    {

                        return new { Status = 200, Message = "Work sheet saved successfully" };
                    }
                    else
                    {

                        return new { Status = 400, Message = "Error While Changing Work Status" };
                    }


                }
                else 
                {
                    return new { Status = 400, Message = "Failed to save work sheet" };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }

        public async Task<dynamic> CloseCallFromCAllList(DailyWorkSheet p, UserTocken ut)
        {
            try
            {
                var InsertWorkAssignmnet = await InsertWorkAssignmentAsync(p.WorkId, ut.AUSR_ID, p.StatusId, ut.AUSR_ID);
                if(InsertWorkAssignmnet.Status==200)
                {
                    //var WorkASsign = await acp.SaveWorkAssignmentAsync(p.WorkId,long.Parse( ut.AUSR_ID), ut.AUSR_ID);
                    //if(WorkASsign.Status==200)
                    //{

                    
                    var savedailyworksheet = await SaveAppDailyWorkSheetAsync(p, ut);
                    if (savedailyworksheet.Status == 200) 
                    {
                        return new { Status = 200,Message="Call Closed Successfully" };
                    }
                    else
                    {
                        return savedailyworksheet;
                    }
                    //}
                    //else
                    //{
                    //    return WorkASsign;
                    //}
                }
                else
                {
                    return InsertWorkAssignmnet;
                }
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }


        public async Task<dynamic> InsertWorkAssignmentAsync(string workId, string empId, string workStatusId, string createUser)
        {
            try
            {
                var query = @"
            INSERT INTO PRMTRANS.INV_WORK_ASSIGN_DTLS(WORK_ID, EMP_ID, WORK_STATUS_ID, CREATE_USER)
            VALUES (:P_WORK_ID, :P_EMP_ID, :P_WORK_STATUS_ID, :P_CREATE_USER)";

                var parameters = new[]
                {
            new OracleParameter("P_WORK_ID", workId),
            new OracleParameter("P_EMP_ID", empId),
            new OracleParameter("P_WORK_STATUS_ID", workStatusId),
            new OracleParameter("P_CREATE_USER", createUser)
        };

                int result = await _DbContext.Database.ExecuteSqlRawAsync(query, parameters);
                //return result; // Returns the number of rows affected (should be 1 for a successful insert)
                return new DefaultMessage.Message3 { Status = 200,Data=result };
            }
            catch (Exception ex)
            {

                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }
        }

        public async Task<dynamic> DeleteDailyWorkSheetAsync(string workSheetId)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_con))  // Ensure '_con' is your connection string.
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM PRMTRANS.DAILY_WORK_SHEET WHERE WORK_SHEET_ID = :workSheetId";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        cmd.Parameters.Add(":workSheetId", OracleDbType.Varchar2).Value = workSheetId;

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();  // Execute the query asynchronously

                        if (rowsAffected > 0)
                        {
                            return new { Status = 200, Message = "Record deleted successfully." };
                        }
                        else
                        {
                            return new { Status = 404, Message = "No record found with the specified Work_Sheet_ID." };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return new { Status = 500, Message = "Error: " + ex.Message };
            }
        }

        public async Task<dynamic> ReassignTonewEmployeeFromInbox(DailyWorkSheet workSheet,UserTocken ut)
        {
            try
            {

                //adding first employee progress note and setting first employee status as closing
                workSheet.StatusId = "4";
                var FirstEmployeeClose = await SaveAppDailyWorkSheetAsync(workSheet,ut);
                if(FirstEmployeeClose.Status ==200)
                {
                    //change the work status to 2 as it for to status assing
                    var ChangeTheWorkStatusToAssign = await acp.UpdateWorkStatusAsync(long.Parse(workSheet.WorkId), 2);
                    if (ChangeTheWorkStatusToAssign.Status == 200) 
                    {

                        //now assign the task to new employee
                        var AssignWorkToNewEmployee = await acp.SaveWorkAssignmentAsync(workSheet.WorkId, workSheet.EmpId, ut.AUSR_ID);
                        if (AssignWorkToNewEmployee.Status == 200) 
                        {
                            return new { Status = 200, Message = "Work Sheet Updated and Assigned to Employee Successfully" };
                        }
                        else
                        {
                            return AssignWorkToNewEmployee;
                        }

                    }
                    else
                    {
                        return ChangeTheWorkStatusToAssign;
                    }
                }
                else
                {
                    return FirstEmployeeClose;
                }

            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }

        public async Task<dynamic> GetAttach(string workid)
        {
            DataTable FileAttachTable = new DataTable();


            string query = @"
       select ATTACH_FILE_ID,WORK_ID,ATTACH_FILE_PATH,CREATE_USER  from PRMTRANS.INV_WORK_ATTACH_FILE where WORK_ID=:workid";
            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    var command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter(":workid", workid));

                    var adapter = new OracleDataAdapter(command);

                    // Fill the DataTable with the result set
                    adapter.Fill(FileAttachTable);
                }

                // Return the populated DataTable
                //return workAssignmentsTable;

                return new DefaultMessage.Message3 { Status = 200, Data = FileAttachTable };
            }
            catch (OracleException sqlEx)
            {
                // Log the error or handle it accordingly
                //Console.WriteLine($"Oracle Error: {sqlEx.Message}");

                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + sqlEx.Message };
                // Optionally, you can throw or return an empty DataTable
                //return new DataTable(); // Return an empty DataTable if the query fails
            }
            catch (Exception ex)
            {
                // Log the error or handle it accordingly

                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
                // Optionally, you can throw or return an empty DataTable
                //return new DataTable(); // Return an empty DataTable if an unexpected error occurs
            }
        }

		public async Task<dynamic> GetEmployeeWorkdasboard(long DepartmentId)
		{
			var workStatusData = new List<dynamic>();

			string query = @"
    SELECT * FROM (
        SELECT SCT_NAME, S.SCT_ID, E.EMP_ID, EMP_NAME, A.WORK_STATUS_ID
        FROM PRMMASTER.HRM_EMPLOYEE E
        LEFT JOIN PRMTRANS.INV_WORK_ASSIGN_DTLS A ON E.EMP_ID = A.EMP_ID
        LEFT JOIN PRMMASTER.INV_WORK_STATUS st ON st.WORK_STATUS_ID = A.WORK_STATUS_ID
        LEFT JOIN PRMTRANS.INV_WORK W ON W.WORK_ID = A.WORK_ID
        INNER JOIN PRMMASTER.HRM_DEPARTMENT_SECTIONS S ON S.SCT_ID = E.SCT_ID
    )
    PIVOT (
        COUNT(WORK_STATUS_ID)
        FOR WORK_STATUS_ID IN (
            1 AS Task_Created, 
            2 AS Task_Assigned, 
            3 AS Task_Opened, 
            7 AS Work_Progressing, 
            5 AS Task_Paused, 
            4 AS Task_Closed, 
            6 AS Development_Completed
        )
    ) 
    WHERE 1 = 1 and SCT_ID =:DepartmentId
    ORDER BY SCT_NAME, EMP_NAME ASC";

			using (var connection = new OracleConnection(_con))
			{
				await connection.OpenAsync();

				using (var command = new OracleCommand(query, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add(":DepartmentId", OracleDbType.Varchar2).Value = DepartmentId;

					try
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								workStatusData.Add(new
								{
									SectionName = reader.GetString(reader.GetOrdinal("SCT_NAME")),
									SectionId = reader.GetInt32(reader.GetOrdinal("SCT_ID")),
									EmpId = reader.GetInt64(reader.GetOrdinal("EMP_ID")),
									EmpName = reader.GetString(reader.GetOrdinal("EMP_NAME")),
									TaskCreated = reader.IsDBNull(reader.GetOrdinal("Task_Created")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Created")),
									TaskAssigned = reader.IsDBNull(reader.GetOrdinal("Task_Assigned")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Assigned")),
									TaskOpened = reader.IsDBNull(reader.GetOrdinal("Task_Opened")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Opened")),
									WorkProgressing = reader.IsDBNull(reader.GetOrdinal("Work_Progressing")) ? 0 : reader.GetInt32(reader.GetOrdinal("Work_Progressing")),
									TaskPaused = reader.IsDBNull(reader.GetOrdinal("Task_Paused")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Paused")),
									TaskClosed = reader.IsDBNull(reader.GetOrdinal("Task_Closed")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Closed")),
									DevelopmentCompleted = reader.IsDBNull(reader.GetOrdinal("Development_Completed")) ? 0 : reader.GetInt32(reader.GetOrdinal("Development_Completed")),
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

			return new { Status = 200, Data = workStatusData };
		}

		public async Task<dynamic> GetEmployeeWorkdasboarddet(string empid,int id)
		{
			var workDetailsData = new List<dynamic>();

			// New query as per the user's request
			string query = @"
    SELECT 
        WORK_STATUS, 
        A.EMP_ID, 
        WORK_DTLS_ID, 
        W.WORK_ID, 
        WORK_REFNO, 
        WORK_DATE, 
        W.CUST_ID, 
        C.CUST_NAME, 
        W.PROJECT_ID, 
        P.PROJECT_NAME, 
        W.MODULE_ID, 
        M.MODULE_NAME, 
        W.WORK_STATUS_ID,
        W.CALL_TYPE_ID, 
        CT.CALL_TYPE,  
        W.EMERGENCY_ID, 
        E.EMERGENCY_TYPE, 
        W.ERROR_TYPE_ID, 
        ET.ERROR_TYPE,  
        W.CALLED_BY, 
        W.CALLED_DESCRIPTION,
        EMP.EMP_NAME
    FROM 
        PRMTRANS.INV_WORK_ASSIGN_DTLS A
    INNER JOIN 
        PRMTRANS.INV_WORK W ON W.WORK_ID = A.WORK_ID
    INNER JOIN 
        PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
    INNER JOIN 
        PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
    INNER JOIN 
        PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
    LEFT JOIN 
        PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
    LEFT JOIN 
        PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
    LEFT JOIN 
        PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
    INNER JOIN 
        PRMMASTER.INV_WORK_STATUS st ON st.WORK_STATUS_ID = A.WORK_STATUS_ID
    INNER JOIN 
        PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = W.CREATE_USER
    WHERE 
        W.WORK_STATUS_ID = :id  
        AND A.EMP_ID = :empid 
    ORDER BY 
        A.CREATE_DATE DESC";

			using (var connection = new OracleConnection(_con))
			{
				await connection.OpenAsync();

				using (var command = new OracleCommand(query, connection))
				{
					command.CommandType = CommandType.Text;

					// Adding the parameters
					command.Parameters.Add(new OracleParameter(":id", id));
					command.Parameters.Add(new OracleParameter(":empid", empid));

					try
					{
						using (var reader = await command.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								workDetailsData.Add(new
								{
									WorkStatus = reader.IsDBNull(reader.GetOrdinal("WORK_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_STATUS")),
									EmpId = reader.GetInt64(reader.GetOrdinal("EMP_ID")),
									WorkDetailsId = reader.GetInt32(reader.GetOrdinal("WORK_DTLS_ID")),
									WorkId = reader.GetInt64(reader.GetOrdinal("WORK_ID")),
									WorkRefNo = reader.IsDBNull(reader.GetOrdinal("WORK_REFNO")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_REFNO")),
									WorkDate = reader.GetDateTime(reader.GetOrdinal("WORK_DATE")),
									CustId = reader.GetInt32(reader.GetOrdinal("CUST_ID")),
									CustName = reader.GetString(reader.GetOrdinal("CUST_NAME")),
									ProjectId = reader.GetInt32(reader.GetOrdinal("PROJECT_ID")),
									ProjectName = reader.GetString(reader.GetOrdinal("PROJECT_NAME")),
									ModuleId = reader.GetInt32(reader.GetOrdinal("MODULE_ID")),
									ModuleName = reader.GetString(reader.GetOrdinal("MODULE_NAME")),
									WorkStatusId = reader.GetInt32(reader.GetOrdinal("WORK_STATUS_ID")),
									CallTypeId = reader.GetInt32(reader.GetOrdinal("CALL_TYPE_ID")),
									CallType = reader.IsDBNull(reader.GetOrdinal("CALL_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALL_TYPE")),
									EmergencyId = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("EMERGENCY_ID")),
									EmergencyType = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("EMERGENCY_TYPE")),
									ErrorTypeId = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ERROR_TYPE_ID")),
									ErrorType = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("ERROR_TYPE")),
									CalledBy = reader.IsDBNull(reader.GetOrdinal("CALLED_BY")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_BY")),
									CalledDescription = reader.IsDBNull(reader.GetOrdinal("CALLED_DESCRIPTION")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_DESCRIPTION")),
									EmpName = reader.GetString(reader.GetOrdinal("EMP_NAME"))
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

			return new { Status = 200, Data = workDetailsData };
		}


		public async Task<dynamic> GetDepartment()
		{
			List<dynamic> department = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					await conn.OpenAsync();

					string query = @"
                 select SCT_ID,SCT_NAME  from PRMMASTER.HRM_DEPARTMENT_SECTIONS where ACTIVE_STATUS='A'";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						//cmd.Parameters.Add(new OracleParameter("empId", empId));

						using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								department.Add(new
								{
									DepartmentId = reader.GetInt32(0),
									DepartmentName = reader.GetString(1)
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

			return new DefaultMessage.Message3 { Status = 200, Data = department };
		}

        public async Task<dynamic> GetEmployeeWorkdasboard(long DepartmentId, string EmployeeId)
        {
            var workStatusData = new List<dynamic>();

            //string query = @"
            // SELECT * FROM (
            //     SELECT SCT_NAME, S.SCT_ID, E.EMP_ID, EMP_NAME, A.WORK_STATUS_ID
            //     FROM PRMMASTER.HRM_EMPLOYEE E
            //     LEFT JOIN PRMTRANS.INV_WORK_ASSIGN_DTLS A ON E.EMP_ID = A.EMP_ID
            //     LEFT JOIN PRMMASTER.INV_WORK_STATUS st ON st.WORK_STATUS_ID = A.WORK_STATUS_ID
            //     LEFT JOIN PRMTRANS.INV_WORK W ON W.WORK_ID = A.WORK_ID
            //     INNER JOIN PRMMASTER.HRM_DEPARTMENT_SECTIONS S ON S.SCT_ID = E.SCT_ID
            // )
            // PIVOT (
            //     COUNT(WORK_STATUS_ID)
            //     FOR WORK_STATUS_ID IN (
            //         1 AS Task_Created, 
            //         2 AS Task_Assigned, 
            //         3 AS Task_Opened, 
            //         7 AS Work_Progressing, 
            //         5 AS Task_Paused, 
            //         4 AS Task_Closed, 
            //         6 AS Development_Completed
            //     )
            // ) 
            // WHERE 1 = 1 and SCT_ID =:DepartmentId
            // ORDER BY SCT_NAME, EMP_NAME ASC";

            string query = @"
    SELECT * FROM (
SELECT SCT_NAME,S.SCT_ID, E.EMP_ID,EMP_NAME,A.WORK_STATUS_ID    FROM PRMMASTER.HRM_EMPLOYEE E
LEFT JOIN PRMTRANS.INV_WORK_ASSIGN_DTLS A ON E.EMP_ID=A.EMP_ID
LEFT join PRMMASTER.INV_WORK_STATUS st on st.WORK_STATUS_ID=a.WORK_STATUS_ID
LEFT JOIN PRMTRANS.INV_WORK W ON W.WORK_ID=A.WORK_ID
INNER JOIN PRMMASTER.HRM_DEPARTMENT_SECTIONS S ON S.SCT_ID=E.SCT_ID 
INNER JOIN (SELECT D.EMP_ID   
FROM PRMMASTER.HRM_EMPLOYEE E
INNER JOIN  PRMMASTER.WEB_EMP_GROUP_DTLS D ON D.GROUP_ID=E.EMP_GROUP_ID 
WHERE E.EMP_ID=:EmployeeId) EMP_ASS ON EMP_ASS.EMP_ID=A.EMP_ID
)
PIVOT (COUNT(WORK_STATUS_ID)   
FOR WORK_STATUS_ID IN ( 1 AS Task_Created, 2 AS Task_Assigned, 3 AS Task_Opened, 7 AS Work_Progressing, 5 AS Task_Paused, 4 AS Task_Closed, 6 AS Development_Completed
 )) WHERE 1=1 and SCT_ID =:DepartmentId ORDER BY SCT_NAME,EMP_NAME ASC";

            using (var connection = new OracleConnection(_con))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(":EmployeeId", OracleDbType.Varchar2).Value = EmployeeId;
                    command.Parameters.Add(":DepartmentId", OracleDbType.Varchar2).Value = DepartmentId;


                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                workStatusData.Add(new
                                {
                                    SectionName = reader.GetString(reader.GetOrdinal("SCT_NAME")),
                                    SectionId = reader.GetInt32(reader.GetOrdinal("SCT_ID")),
                                    EmpId = reader.GetInt64(reader.GetOrdinal("EMP_ID")),
                                    EmpName = reader.GetString(reader.GetOrdinal("EMP_NAME")),
                                    TaskCreated = reader.IsDBNull(reader.GetOrdinal("Task_Created")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Created")),
                                    TaskAssigned = reader.IsDBNull(reader.GetOrdinal("Task_Assigned")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Assigned")),
                                    TaskOpened = reader.IsDBNull(reader.GetOrdinal("Task_Opened")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Opened")),
                                    WorkProgressing = reader.IsDBNull(reader.GetOrdinal("Work_Progressing")) ? 0 : reader.GetInt32(reader.GetOrdinal("Work_Progressing")),
                                    TaskPaused = reader.IsDBNull(reader.GetOrdinal("Task_Paused")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Paused")),
                                    TaskClosed = reader.IsDBNull(reader.GetOrdinal("Task_Closed")) ? 0 : reader.GetInt32(reader.GetOrdinal("Task_Closed")),
                                    DevelopmentCompleted = reader.IsDBNull(reader.GetOrdinal("Development_Completed")) ? 0 : reader.GetInt32(reader.GetOrdinal("Development_Completed")),
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

            return new { Status = 200, Data = workStatusData };
        }

        public async Task<dynamic> GetWorkStatus()
        {
            List<dynamic> workstatus = new List<dynamic>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();

                    string query = @"
             select WORK_STATUS_ID,WORK_STATUS from PRMMASTER.INV_WORK_STATUS where ACTIVE_STATUS='A'";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        //cmd.Parameters.Add(new OracleParameter("empId", empId));

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                workstatus.Add(new
                                {
                                    WorkStatusId = reader.GetInt32(0),
                                    WorkStatusName = reader.GetString(1)
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

            return new DefaultMessage.Message3 { Status = 200, Data = workstatus };
        }


        public async Task<dynamic> GetEmployeeWorkdetails(string empid)
        {
            var workDetailsData = new List<dynamic>();

            // New query as per the user's request
            string query = @"
SELECT 
    CALLED_DESCRIPTION,
    WORK_STATUS,
    EMP.EMP_NAME AS CREATE_USER,
    ASS.EMP_NAME AS ASSIGN_EMP_NAME,
    A.CREATE_DATE AS ASSIGN_DATE,
    WORK_REFNO,
    WORK_DATE AS CREATED_DATE,
    A.WORK_ID,
    A.EMP_ID,
    WORK_DTLS_ID,
    W.CUST_ID,
    C.CUST_NAME,
    W.PROJECT_ID,
    PROJECT_NAME,
    W.MODULE_ID,
    MODULE_NAME,
    W.WORK_STATUS_ID,
    W.CALL_TYPE_ID,
    CALL_TYPE,
    W.EMERGENCY_ID,
    EMERGENCY_TYPE,
    W.ERROR_TYPE_ID,
    ERROR_TYPE,
    CALLED_BY
FROM 
    PRMTRANS.INV_WORK_ASSIGN_DTLS A
INNER JOIN 
    PRMTRANS.INV_WORK W ON W.WORK_ID = A.WORK_ID
INNER JOIN 
    PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
INNER JOIN 
    PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
INNER JOIN 
    PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
INNER JOIN 
    PRMMASTER.INV_WORK_STATUS st ON st.WORK_STATUS_ID = A.WORK_STATUS_ID
INNER JOIN 
    PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = W.CREATE_USER
INNER JOIN 
    PRMMASTER.HRM_EMPLOYEE ASS ON ASS.EMP_ID = A.EMP_ID
INNER JOIN 
    (SELECT D.EMP_ID   
     FROM PRMMASTER.HRM_EMPLOYEE E
     INNER JOIN  PRMMASTER.WEB_EMP_GROUP_DTLS D ON D.GROUP_ID = E.EMP_GROUP_ID 
     WHERE E.EMP_ID = :empid) EMP_ASS ON EMP_ASS.EMP_ID = A.EMP_ID
WHERE 
    W.WORK_STATUS_ID IN (2, 3, 5, 6, 7, 8)
ORDER BY 
    ASS.EMP_NAME ASC, A.CREATE_DATE DESC";

            using (var connection = new OracleConnection(_con))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Adding the parameters
                    command.Parameters.Add(new OracleParameter(":empid", empid));

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                workDetailsData.Add(new
                                {
                                    CalledDescription = reader.IsDBNull(reader.GetOrdinal("CALLED_DESCRIPTION")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_DESCRIPTION")),
                                    WorkStatus = reader.IsDBNull(reader.GetOrdinal("WORK_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_STATUS")),
                                    CreateUser = reader.GetString(reader.GetOrdinal("CREATE_USER")),
                                    AssignEmpName = reader.GetString(reader.GetOrdinal("ASSIGN_EMP_NAME")),
                                    AssignDate = reader.GetDateTime(reader.GetOrdinal("ASSIGN_DATE")),
                                    WorkRefNo = reader.IsDBNull(reader.GetOrdinal("WORK_REFNO")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_REFNO")),
                                    WorkDate = reader.GetDateTime(reader.GetOrdinal("CREATED_DATE")),
                                    WorkId = reader.GetInt64(reader.GetOrdinal("WORK_ID")),
                                    EmpId = reader.GetInt64(reader.GetOrdinal("EMP_ID")),
                                    WorkDetailsId = reader.GetInt32(reader.GetOrdinal("WORK_DTLS_ID")),
                                    CustId = reader.GetInt32(reader.GetOrdinal("CUST_ID")),
                                    CustName = reader.GetString(reader.GetOrdinal("CUST_NAME")),
                                    ProjectId = reader.GetInt32(reader.GetOrdinal("PROJECT_ID")),
                                    ProjectName = reader.GetString(reader.GetOrdinal("PROJECT_NAME")),
                                    ModuleId = reader.GetInt32(reader.GetOrdinal("MODULE_ID")),
                                    ModuleName = reader.GetString(reader.GetOrdinal("MODULE_NAME")),
                                    WorkStatusId = reader.GetInt32(reader.GetOrdinal("WORK_STATUS_ID")),
                                    CallTypeId = reader.GetInt32(reader.GetOrdinal("CALL_TYPE_ID")),
                                    CallType = reader.IsDBNull(reader.GetOrdinal("CALL_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALL_TYPE")),
                                    EmergencyId = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("EMERGENCY_ID")),
                                    EmergencyType = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("EMERGENCY_TYPE")),
                                    ErrorTypeId = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ERROR_TYPE_ID")),
                                    ErrorType = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("ERROR_TYPE")),
                                    CalledBy = reader.IsDBNull(reader.GetOrdinal("CALLED_BY")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_BY"))
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

            return new { Status = 200, Data = workDetailsData };
        }


        public async Task<dynamic> GetEmployeeWorkdetailsEmp(string EmployeeId, string empid)
        {
            var workDetailsDataemp = new List<dynamic>();

            // New query as per the user's request
            string query = @"
SELECT 
    CALLED_DESCRIPTION,
    WORK_STATUS,
    EMP.EMP_NAME AS CREATE_USER,
    ASS.EMP_NAME AS ASSIGN_EMP_NAME,
    A.CREATE_DATE AS ASSIGN_DATE,
    WORK_REFNO,
    WORK_DATE AS CREATED_DATE,
    A.WORK_ID,
    A.EMP_ID,
    WORK_DTLS_ID,
    W.CUST_ID,
    C.CUST_NAME,
    W.PROJECT_ID,
    PROJECT_NAME,
    W.MODULE_ID,
    MODULE_NAME,
    W.WORK_STATUS_ID,
    W.CALL_TYPE_ID,
    CALL_TYPE,
    W.EMERGENCY_ID,
    EMERGENCY_TYPE,
    W.ERROR_TYPE_ID,
    ERROR_TYPE,
    CALLED_BY
FROM 
    PRMTRANS.INV_WORK_ASSIGN_DTLS A
INNER JOIN 
    PRMTRANS.INV_WORK W ON W.WORK_ID = A.WORK_ID
INNER JOIN 
    PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
INNER JOIN 
    PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
INNER JOIN 
    PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
INNER JOIN 
    PRMMASTER.INV_WORK_STATUS st ON st.WORK_STATUS_ID = A.WORK_STATUS_ID
INNER JOIN 
    PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = W.CREATE_USER
INNER JOIN 
    PRMMASTER.HRM_EMPLOYEE ASS ON ASS.EMP_ID = A.EMP_ID
INNER JOIN 
    (SELECT D.EMP_ID   
     FROM PRMMASTER.HRM_EMPLOYEE E
     INNER JOIN  PRMMASTER.WEB_EMP_GROUP_DTLS D ON D.GROUP_ID = E.EMP_GROUP_ID 
     WHERE E.EMP_ID = :EmployeeId) EMP_ASS ON EMP_ASS.EMP_ID = A.EMP_ID
WHERE 
    W.WORK_STATUS_ID IN (2, 3, 5, 6, 7, 8) and A.EMP_ID=:empid
ORDER BY 
    ASS.EMP_NAME ASC, A.CREATE_DATE DESC";

            using (var connection = new OracleConnection(_con))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Adding the parameters
                    command.Parameters.Add(new OracleParameter(":EmployeeId", EmployeeId));
                    command.Parameters.Add(new OracleParameter(":empid", empid));

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                workDetailsDataemp.Add(new
                                {
                                    CalledDescription = reader.IsDBNull(reader.GetOrdinal("CALLED_DESCRIPTION")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_DESCRIPTION")),
                                    WorkStatus = reader.IsDBNull(reader.GetOrdinal("WORK_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_STATUS")),
                                    CreateUser = reader.GetString(reader.GetOrdinal("CREATE_USER")),
                                    AssignEmpName = reader.GetString(reader.GetOrdinal("ASSIGN_EMP_NAME")),
                                    AssignDate = reader.GetDateTime(reader.GetOrdinal("ASSIGN_DATE")),
                                    WorkRefNo = reader.IsDBNull(reader.GetOrdinal("WORK_REFNO")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_REFNO")),
                                    WorkDate = reader.GetDateTime(reader.GetOrdinal("CREATED_DATE")),
                                    WorkId = reader.GetInt64(reader.GetOrdinal("WORK_ID")),
                                    EmpId = reader.GetInt64(reader.GetOrdinal("EMP_ID")),
                                    WorkDetailsId = reader.GetInt32(reader.GetOrdinal("WORK_DTLS_ID")),
                                    CustId = reader.GetInt32(reader.GetOrdinal("CUST_ID")),
                                    CustName = reader.GetString(reader.GetOrdinal("CUST_NAME")),
                                    ProjectId = reader.GetInt32(reader.GetOrdinal("PROJECT_ID")),
                                    ProjectName = reader.GetString(reader.GetOrdinal("PROJECT_NAME")),
                                    ModuleId = reader.GetInt32(reader.GetOrdinal("MODULE_ID")),
                                    ModuleName = reader.GetString(reader.GetOrdinal("MODULE_NAME")),
                                    WorkStatusId = reader.GetInt32(reader.GetOrdinal("WORK_STATUS_ID")),
                                    CallTypeId = reader.GetInt32(reader.GetOrdinal("CALL_TYPE_ID")),
                                    CallType = reader.IsDBNull(reader.GetOrdinal("CALL_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALL_TYPE")),
                                    EmergencyId = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("EMERGENCY_ID")),
                                    EmergencyType = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("EMERGENCY_TYPE")),
                                    ErrorTypeId = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ERROR_TYPE_ID")),
                                    ErrorType = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("ERROR_TYPE")),
                                    CalledBy = reader.IsDBNull(reader.GetOrdinal("CALLED_BY")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_BY"))
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

            return new { Status = 200, Data = workDetailsDataemp };
        }

        public async Task<dynamic> GetEmployeeWorkdetailsEmpStatus(string EmployeeId, long empid, string workstatusid)
        {
            var workDetailsDataempStatus = new List<dynamic>();

            // New query as per the user's request
            string query = @"
SELECT 
    CALLED_DESCRIPTION,
    WORK_STATUS,
    EMP.EMP_NAME AS CREATE_USER,
    ASS.EMP_NAME AS ASSIGN_EMP_NAME,
    A.CREATE_DATE AS ASSIGN_DATE,
    WORK_REFNO,
    WORK_DATE AS CREATED_DATE,
    A.WORK_ID,
    A.EMP_ID,
    WORK_DTLS_ID,
    W.CUST_ID,
    C.CUST_NAME,
    W.PROJECT_ID,
    PROJECT_NAME,
    W.MODULE_ID,
    MODULE_NAME,
    W.WORK_STATUS_ID,
    W.CALL_TYPE_ID,
    CALL_TYPE,
    W.EMERGENCY_ID,
    EMERGENCY_TYPE,
    W.ERROR_TYPE_ID,
    ERROR_TYPE,
    CALLED_BY
FROM 
    PRMTRANS.INV_WORK_ASSIGN_DTLS A
INNER JOIN 
    PRMTRANS.INV_WORK W ON W.WORK_ID = A.WORK_ID
INNER JOIN 
    PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
INNER JOIN 
    PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
INNER JOIN 
    PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
LEFT JOIN 
    PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
INNER JOIN 
    PRMMASTER.INV_WORK_STATUS st ON st.WORK_STATUS_ID = A.WORK_STATUS_ID
INNER JOIN 
    PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = W.CREATE_USER
INNER JOIN 
    PRMMASTER.HRM_EMPLOYEE ASS ON ASS.EMP_ID = A.EMP_ID
INNER JOIN 
    (SELECT D.EMP_ID   
     FROM PRMMASTER.HRM_EMPLOYEE E
     INNER JOIN  PRMMASTER.WEB_EMP_GROUP_DTLS D ON D.GROUP_ID = E.EMP_GROUP_ID 
     WHERE E.EMP_ID = :EmployeeId) EMP_ASS ON EMP_ASS.EMP_ID = A.EMP_ID
WHERE 
  W.WORK_STATUS_ID IN (2, 3, 5, 6, 7, 8) 
    AND (:empid1 = 0 OR A.EMP_ID = :empid2)
AND (:workstatusid1 = 0 OR A.WORK_STATUS_ID = :workstatusid2)
ORDER BY 
    ASS.EMP_NAME ASC, A.CREATE_DATE DESC";

            using (var connection = new OracleConnection(_con))
            {
                await connection.OpenAsync();

                using (var command = new OracleCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Adding the parameters
                    //command.Parameters.Add(new OracleParameter(":EmployeeId", EmployeeId));
                    //command.Parameters.Add(new OracleParameter(":empid", empid));
                    //command.Parameters.Add(new OracleParameter(":workstatusid", workstatusid));
                    command.Parameters.Add("EmployeeId", OracleDbType.Int64).Value = Convert.ToInt64(EmployeeId ?? "0");

                    command.Parameters.Add("empid1", OracleDbType.Int64).Value = empid;
                    command.Parameters.Add("empid2", OracleDbType.Int64).Value = empid;

                    int workStatusId = Convert.ToInt32(workstatusid ?? "0");
                    command.Parameters.Add("workstatusid1", OracleDbType.Int32).Value = workStatusId;
                    command.Parameters.Add("workstatusid2", OracleDbType.Int32).Value = workStatusId;


                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                workDetailsDataempStatus.Add(new
                                {
                                    CalledDescription = reader.IsDBNull(reader.GetOrdinal("CALLED_DESCRIPTION")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_DESCRIPTION")),
                                    WorkStatus = reader.IsDBNull(reader.GetOrdinal("WORK_STATUS")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_STATUS")),
                                    CreateUser = reader.GetString(reader.GetOrdinal("CREATE_USER")),
                                    AssignEmpName = reader.GetString(reader.GetOrdinal("ASSIGN_EMP_NAME")),
                                    AssignDate = reader.GetDateTime(reader.GetOrdinal("ASSIGN_DATE")),
                                    WorkRefNo = reader.IsDBNull(reader.GetOrdinal("WORK_REFNO")) ? string.Empty : reader.GetString(reader.GetOrdinal("WORK_REFNO")),
                                    WorkDate = reader.GetDateTime(reader.GetOrdinal("CREATED_DATE")),
                                    WorkId = reader.GetInt64(reader.GetOrdinal("WORK_ID")),
                                    EmpId = reader.GetInt64(reader.GetOrdinal("EMP_ID")),
                                    WorkDetailsId = reader.GetInt32(reader.GetOrdinal("WORK_DTLS_ID")),
                                    CustId = reader.GetInt32(reader.GetOrdinal("CUST_ID")),
                                    CustName = reader.GetString(reader.GetOrdinal("CUST_NAME")),
                                    ProjectId = reader.GetInt32(reader.GetOrdinal("PROJECT_ID")),
                                    ProjectName = reader.GetString(reader.GetOrdinal("PROJECT_NAME")),
                                    ModuleId = reader.GetInt32(reader.GetOrdinal("MODULE_ID")),
                                    ModuleName = reader.GetString(reader.GetOrdinal("MODULE_NAME")),
                                    WorkStatusId = reader.GetInt32(reader.GetOrdinal("WORK_STATUS_ID")),
                                    CallTypeId = reader.GetInt32(reader.GetOrdinal("CALL_TYPE_ID")),
                                    CallType = reader.IsDBNull(reader.GetOrdinal("CALL_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALL_TYPE")),
                                    EmergencyId = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("EMERGENCY_ID")),
                                    EmergencyType = reader.IsDBNull(reader.GetOrdinal("EMERGENCY_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("EMERGENCY_TYPE")),
                                    ErrorTypeId = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ERROR_TYPE_ID")),
                                    ErrorType = reader.IsDBNull(reader.GetOrdinal("ERROR_TYPE")) ? string.Empty : reader.GetString(reader.GetOrdinal("ERROR_TYPE")),
                                    CalledBy = reader.IsDBNull(reader.GetOrdinal("CALLED_BY")) ? string.Empty : reader.GetString(reader.GetOrdinal("CALLED_BY"))
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

            return new { Status = 200, Data = workDetailsDataempStatus };
        }



        public async Task<dynamic> SavePunchRemarks(PunchDetails parameters)
        {
            string retval = string.Empty;

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    //await connection.OpenAsync();
                    //var formattedDate = DateTime.ParseExact(parameters.WorkDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");

                    using (OracleConnection conn = new OracleConnection(_con))
                    using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_HRM_PUNCH_DTLS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        //Input Parameters
                        cmd.Parameters.Add("P_OPR_ID", OracleDbType.Int64).Value = 4;
                        cmd.Parameters.Add("P_PUNCH_ID", OracleDbType.Int64).Value = parameters.PunchId;
                        //cmd.Parameters.Add("P_PUNCH_DATE", OracleDbType.Date).Value = null;
                        cmd.Parameters.Add("P_PUNCH_DATE", OracleDbType.Date).Value = parameters.PunchDate;
                        cmd.Parameters.Add("P_PUNCH_TYPE", OracleDbType.Int32).Value = 0;
                        cmd.Parameters.Add("P_EMP_ID", OracleDbType.Int64).Value = 0;
                        cmd.Parameters.Add("P_WHERE1", OracleDbType.Varchar2).Value = null;
                        cmd.Parameters.Add("P_WHERE", OracleDbType.Varchar2).Value = null;

                        cmd.Parameters.Add("P_Hdr", OracleDbType.Int32).Value = 1;

                        cmd.Parameters.Add("strTDate", OracleDbType.Varchar2).Value = null;
                        cmd.Parameters.Add("strFDate", OracleDbType.Varchar2).Value = null;

                        cmd.Parameters.Add("P_PUNCH_FROM", OracleDbType.Int32).Value = null;
                        cmd.Parameters.Add("P_PROJECT_ID", OracleDbType.Int32).Value = 0;
                        cmd.Parameters.Add("P_PUNCH_RMKS", OracleDbType.Varchar2).Value = parameters.PunchRemarks;

                        //var retvalParam = new OracleParameter("STROUT", OracleDbType.Varchar2, 10)
                        //{
                        //	Direction = ParameterDirection.Output
                        //};
                        //cmd.Parameters.Add(retvalParam);

                        OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputCursor);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        // Get the output parameter value
                        retval = outputCursor.Value.ToString();

                    }

                }

                //if (retval == "1")
                //{
                return new { Status = 200, Message = "Punch Remarks saved successfully" };

                //}
                //else
                //{
                //    return new { Status = 400, Message = "Failed to save Remarks" };
                //}
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }



        public async Task<dynamic> Getprojectclientlist()
        {
            DataTable dataTable = new DataTable();

            string sqlQuery = @"SELECT DISTINCT  
    PROJECT_NAME,  
    MODULE_NAME,
    W.PROJECT_WORK_ID, 
    PROJECT_WORK_NO, 
    PROJECT_WORK_REFNO, 
    PROJECT_WORK_DATE, 
    W.PROJECT_ID, 
    W.MODULE_ID, 
    W.CONTACT_NAME, 
    S.FOR_COLOR_CODE,
    W.CONTACT_NO, 
    W.CREATE_DATE, 
    CALLED_DESCRIPTION, 
    PROJECT_WORK_STATUS_ID,  
    NVL(ATTACH_FILE_COUNT, 0) AS ATTACH_FILE_COUNT,
    CLIENT_WORK_STATUS,
    NVL(STS.ROW_COUNT, 0) AS STATUS_ROW_COUNT
FROM PRMTRANS.INV_PROJECT_WORK W 
INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID 
INNER JOIN PRMMASTER.INV_CLIENT_WORK_STATUS S ON S.CLIENT_WORK_STATUS_ID = W.PROJECT_WORK_STATUS_ID
LEFT JOIN (
    SELECT 
        A.PROJECT_WORK_ID,
        COUNT(*) AS ROW_COUNT
    FROM PRMMASTER.INV_CLIENT_WORK_STS_UPD A
    INNER JOIN PRMMASTER.INV_CLIENT_WORK_STATUS B ON A.CLIENT_WORK_STATUS_ID = B.CLIENT_WORK_STATUS_ID
    INNER JOIN PRMMASTER.HRM_EMPLOYEE C ON C.EMP_ID = A.CREATE_USER
    INNER JOIN PRMTRANS.INV_PROJECT_WORK D ON D.PROJECT_WORK_ID = A.PROJECT_WORK_ID
    GROUP BY A.PROJECT_WORK_ID
) STS ON STS.PROJECT_WORK_ID = W.PROJECT_WORK_ID
LEFT JOIN (
    SELECT 
        COUNT(ATTACH_FILE_ID) AS ATTACH_FILE_COUNT,
        WORK_ID 
    FROM PRMTRANS.INV_PROJECTWORK_ATTACH_FILE 
    GROUP BY WORK_ID
) WAT ON WAT.WORK_ID = W.PROJECT_WORK_ID
ORDER BY W.CREATE_DATE DESC
";

            try
            {
                using (var connection = _DbContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlQuery;
                        //command.Parameters.Add(new OracleParameter(":projectId", projectId));


                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = "Error: " + ex.Message };
            }

            return new { Status = 200, Data = dataTable };
        }

        public async Task<dynamic> GetAttachProject(string workid)
        {
            DataTable FileAttachTable = new DataTable();


            string query = @"
       select ATTACH_FILE_ID,WORK_ID,ATTACH_FILE_PATH,CREATE_USER  from PRMTRANS.INV_PROJECTWORK_ATTACH_FILE where WORK_ID=:workid";
            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    var command = new OracleCommand(query, connection);
                    command.Parameters.Add(new OracleParameter(":workid", workid));

                    var adapter = new OracleDataAdapter(command);

                    // Fill the DataTable with the result set
                    adapter.Fill(FileAttachTable);
                }

                // Return the populated DataTable
                //return workAssignmentsTable;

                return new DefaultMessage.Message3 { Status = 200, Data = FileAttachTable };
            }
            catch (OracleException sqlEx)
            {
                // Log the error or handle it accordingly
                //Console.WriteLine($"Oracle Error: {sqlEx.Message}");

                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + sqlEx.Message };
                // Optionally, you can throw or return an empty DataTable
                //return new DataTable(); // Return an empty DataTable if the query fails
            }
            catch (Exception ex)
            {
                // Log the error or handle it accordingly

                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
                // Optionally, you can throw or return an empty DataTable
                //return new DataTable(); // Return an empty DataTable if an unexpected error occurs
            }
        }

        public async Task<dynamic> GetActiveProjects()
        {
            List<dynamic> projects = new List<dynamic>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    await conn.OpenAsync();

                    string query = @"
                SELECT DISTINCT 
                 P.PROJECT_ID, 
                 P.PROJECT_NAME AS PROJECT, 
                 PRJ.CUST_ID 
             FROM PRMMASTER.HRM_EMPLOYEE E
             INNER JOIN PRMMASTER.WEB_PROJECT_GROUP_DTLS D ON D.GROUP_ID = E.PROJECT_GROUP_ID
             INNER JOIN PRMTRANS.INV_PROJECT_ENTRY PRJ ON D.PROJECT_ID = PRJ.PROJECT_ID AND PRJ.CUST_ID = D.CUST_ID
             INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = PRJ.PROJECT_ID
             INNER JOIN PRMMASTER.WEB_PROJECT_GROUP G ON D.GROUP_ID = E.PROJECT_GROUP_ID
             WHERE NVL(P.ACTIVE_STATUS, 'A') = 'A' 
             AND NVL(G.ACTIVE_STATUS, 'A') = 'A' 

             ORDER BY UPPER(P.PROJECT_NAME)";
                    //AND PRJ.CUST_ID = :clientId 

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        //cmd.Parameters.Add(new OracleParameter("empId", empId));
                        //cmd.Parameters.Add(new OracleParameter("clientId", clientId));

                        using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                projects.Add(new
                                {
                                    ProjectId = reader.GetInt32(0),
                                    ProjectName = reader.GetString(1),
                                    ClientId = reader.GetInt64(2)
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


        public async Task<dynamic> GetActiveClientWorkStatuses()
        {
            var result = new List<dynamic>();
            string query = @"
            SELECT CLIENT_WORK_STATUS_ID, CLIENT_WORK_STATUS
            FROM PRMMASTER.INV_CLIENT_WORK_STATUS
            WHERE ACTIVE_STATUS = 'A'";

            try
            {
                using (var connection = new OracleConnection(_con))
                using (var command = new OracleCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic item = new ExpandoObject();
                            item.CLIENT_WORK_STATUS_ID = reader["CLIENT_WORK_STATUS_ID"];
                            item.CLIENT_WORK_STATUS = reader["CLIENT_WORK_STATUS"];
                            result.Add(item);
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // You can log the error or rethrow as needed
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }

            return new DefaultMessage.Message3 { Status = 200, Data = result };
        }



        public async Task<dynamic> SaveProjectWorkStatus(string projectWorkId, string clientWorkStatusId, string createUser, string remarks, DateTime P_WORK_STATUS_DATE,string ex_remarks)
        {
            string result = null;

            try
            {
                using (var connection = new OracleConnection(_con))
                using (var command = new OracleCommand("prmtrans.SP_SAVE_APP_PROJECT_WORK_STS", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Input parameters
                    command.Parameters.Add("P_PROJECT_WORK_ID", OracleDbType.Varchar2).Value = projectWorkId;
                    command.Parameters.Add("P_CLIENT_WORK_STATUS_ID", OracleDbType.Varchar2).Value = clientWorkStatusId;
                    command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2).Value = createUser;
                    command.Parameters.Add("P_REMARKS", OracleDbType.Varchar2).Value = remarks;
                    command.Parameters.Add("P_EXTERNAL_REMARKS", OracleDbType.Varchar2).Value = ex_remarks;
                    command.Parameters.Add("P_WORK_STATUS_DATE", OracleDbType.Date).Value = P_WORK_STATUS_DATE;
                    

                    // Output parameter
                    command.Parameters.Add("RETVAL", OracleDbType.Varchar2, 100).Direction = System.Data.ParameterDirection.Output;

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    result = command.Parameters["RETVAL"].Value?.ToString();
                }

                // Safely parse and update
                if (int.TryParse(clientWorkStatusId, out int statusId) && int.TryParse(projectWorkId, out int projId))
                {
                    try
                    {
                        UpdateProjectWorkStatus(statusId, projId);
                    }
                    catch (Exception ex)
                    {
                        return new DefaultMessage.Message1 { Status = 500, Message = "Error updating project work status: " + ex.Message };
                    }
                }
                else
                {
                    return new DefaultMessage.Message1 { Status = 400, Message = "Invalid numeric values for status or project ID." };
                }
            }
            catch (OracleException ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = "Oracle Error: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }

            return new { Status = 200, Message = "Work Status Updated", Data = result };
        }

        public void UpdateProjectWorkStatus(int givenId, int projectWorkId)
        {
            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    connection.Open();

                    string sql = @"
            UPDATE prmtrans.inv_project_work 
            SET PROJECT_WORK_STATUS_ID = :givenid 
            WHERE PROJECT_WORK_ID = :project";

                    using (var command = new OracleCommand(sql, connection))
                    {
                        command.Parameters.Add(new OracleParameter("givenid", givenId));
                        command.Parameters.Add(new OracleParameter("project", projectWorkId));

                        int rowsUpdated = command.ExecuteNonQuery();

                        if (rowsUpdated == 0)
                        {
                            throw new Exception("No rows were updated. Check if the provided Project Work ID is valid.");
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Database error while updating project work status: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error during project work status update: " + ex.Message);
            }
        }


        public async Task<dynamic> GetClientWorkStatusByProjectIdAsync(int projectWorkId)
        {
            try
            {
                var sql = @"
            SELECT 
                D.PROJECT_WORK_NO, 
                D.PROJECT_WORK_REFNO, 
                D.PROJECT_WORK_DATE, 
                D.CALLED_DESCRIPTION, 
                C.EMP_NAME, 
                B.CLIENT_WORK_STATUS, 
                B.CLIENT_WORK_STATUS_ID, 
                A.PROJECT_WORK_ID, 
                A.CLIENT_WORK_STATUS_ID, 
                A.CREATE_USER, 
                A.CREATE_DATE, 
                A.UPDATE_USER, 
                A.UPDATE_DATE, 
                A.REMARKS, 
                A.EXTERNAL_REMARKS,
                B.FOR_COLOR_CODE
            FROM PRMMASTER.INV_CLIENT_WORK_STS_UPD A
            INNER JOIN PRMMASTER.INV_CLIENT_WORK_STATUS B ON A.CLIENT_WORK_STATUS_ID = B.CLIENT_WORK_STATUS_ID
            INNER JOIN PRMMASTER.HRM_EMPLOYEE C ON C.EMP_ID = A.CREATE_USER
            INNER JOIN PRMTRANS.INV_PROJECT_WORK D ON D.PROJECT_WORK_ID = A.PROJECT_WORK_ID
          
            WHERE A.PROJECT_WORK_ID = :projectWorkId
            ORDER BY B.CLIENT_WORK_STATUS_ID DESC";

                using var command = _DbContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.Parameters.Add(new OracleParameter("projectWorkId", projectWorkId));

                await _DbContext.Database.OpenConnectionAsync();

                var results = new List<dynamic>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    dynamic row = new ExpandoObject();
                    var dict = (IDictionary<string, object>)row;

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dict[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                    }

                    results.Add(row);
                }

                return new DefaultMessage.Message3 { Status = 200, Data = results };
            }
            catch (Exception ex)
            {
                // You can log the error or rethrow as needed
                return new DefaultMessage.Message1 { Status = 500, Message = "Unexpected Error: " + ex.Message };
            }
        }












    }















}



