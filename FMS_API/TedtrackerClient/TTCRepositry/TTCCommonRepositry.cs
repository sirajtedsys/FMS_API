using FMS_API.Class;
using FMS_API.Data.Class;
using FMS_API.Data.DbModel;
using FMS_API.TedtrackerClient.TTCClass;
using FMS_API.TedtrackerClient.TTCDbNodels;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Globalization;
using static FMS_API.Repositry.ExpenseTrackerRepositry;
using static JwtService;
using static JwtServiceProject;

namespace FMS_API.TedtrackerClient.TTCRepositry
{
    public class TTCCommonRepositry
    {

        private readonly AppDbContext _DbContext;

        private readonly JwtHandlerProject jwthand;
        private readonly string _con;
        public TTCCommonRepositry(AppDbContext dbContext, JwtHandlerProject _jwthand)
        {
            _DbContext = dbContext;
            jwthand = _jwthand;
            _con = _DbContext.Database.GetConnectionString();
        }



        public async Task<dynamic> CheckProjectLogin(string userName, string password)
        {
            try
            {
                var da = await GetUserProject(userName, password);
                if (da.Status == 200)
                {
                    var data = da.Data;

                    if (data != null && data.Count > 0)
                    {
                        var userdat = new ProjectUserTocken
                        {
                            AUSR_ID = data[0].PROJECT_ID,
                            USERNAME = userName,
                            PASSWORD = password
                        };

                        var token = jwthand.GenerateToken(userdat);
                        var dat = await _DbContext.APP_PROJECT_LOGIN_SETTINGS
                                                  .Where(x => x.PROJ_ID == userdat.AUSR_ID)
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
                            var newLogin = new PRMMASTER_ProjectLoginSettings
                            {
                                PROJ_ID = userdat.AUSR_ID,
                                TOKEN = token,
                                CREATE_ON = DateTime.Now
                            };

                            await _DbContext.APP_PROJECT_LOGIN_SETTINGS.AddAsync(newLogin);
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


        public async Task<dynamic> GetUserProject(string userName, string password)
        {
            string connectionString = _con;
            var resultList = new List<dynamic>();

            string query = @"
        SELECT PROJECT_ID, PROJECT_NAME, ACTIVE_STATUS 
        FROM PRMMASTER.INV_PROJECT_MASTER 
        WHERE NVL(ACTIVE_STATUS, 'A') = 'A' 
          AND UPPER(USER_NAME) = UPPER(:userName) 
          AND PASSWORD = :password";

            try
            {
                using (var connection = new OracleConnection(connectionString))
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("userName", userName));
                    command.Parameters.Add(new OracleParameter("password", password));

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dynamic row = new System.Dynamic.ExpandoObject();
                            row.PROJECT_ID = reader["PROJECT_ID"]?.ToString();
                            row.PROJECT_NAME = reader["PROJECT_NAME"]?.ToString();
                            row.ACTIVE_STATUS = reader["ACTIVE_STATUS"]?.ToString();
                            resultList.Add(row);
                        }
                    }
                }

                return new DefaultMessage.Message3
                {
                    Status = 200,
                    Data = resultList
                };
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1
                {
                    Status = 500,
                    Message = ex.Message
                };
            }
        }


        public async Task<dynamic> GetModulesByProjectId(string projectId)
        {
            DataTable dataTable = new DataTable();
            //string connectionString = ;con

            string query = @"
        SELECT DISTINCT M.MODULE_ID, MODULE_NAME
        FROM PRMMASTER.INV_PROJECT_MASTER E
        INNER JOIN PRMMASTER.WEB_MODULE_GROUP_DTLS D ON D.GROUP_ID = E.MODULE_GROUP_ID
        INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = D.MODULE_ID
        INNER JOIN PRMMASTER.WEB_MODULE_GROUP G ON D.GROUP_ID = E.MODULE_GROUP_ID
        WHERE NVL(M.ACTIVE_STATUS, 'A') = 'A' 
          AND NVL(G.ACTIVE_STATUS, 'A') = 'A' 
          AND PROJECT_ID = :projectId
        ORDER BY UPPER(MODULE_NAME) ASC";

            try
            {
                using (var connection = new OracleConnection(_con))
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("projectId", projectId));

                    using (var adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)

                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };

                // Optionally rethrow or handle gracefully
                // throw; // uncomment if you want to propagate the exception
            }
            return new DefaultMessage.Message3 { Status = 200, Data =dataTable };
        }

        public async Task<dynamic> SaveAppProjectWork(ProjectWorkAddCall input, ProjectUserTocken ut)
        {
            string connectionString = _con;
            string result = string.Empty;
			try
            {
                using (var connection = new OracleConnection(connectionString))
                using (var command = new OracleCommand("PRMTRANS.SP_SAVE_APP_PROJECT_WORK", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    command.Parameters.Add("P_PROJECT_WORK_DATE", OracleDbType.Date).Value = input.ProjectWorkDate;
                    command.Parameters.Add("P_PROJECT_ID", OracleDbType.Varchar2).Value = ut.AUSR_ID;
                    command.Parameters.Add("P_MODULE_ID", OracleDbType.Varchar2).Value = input.ModuleId;
                    command.Parameters.Add("P_CONTACT_NAME", OracleDbType.Varchar2).Value = input.ContactName;
                    command.Parameters.Add("P_CONTACT_NO", OracleDbType.Varchar2).Value = input.ContactNo;
                    command.Parameters.Add("P_CALLED_DESCRIPTION", OracleDbType.Varchar2).Value = input.CalledDescription;
                    command.Parameters.Add("P_PROJECT_WORK_STATUS_ID", OracleDbType.Int32).Value = input.ProjectWorkStatusId;

                    // Output parameter
                    command.Parameters.Add("RETVAL", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();

                    result = command.Parameters["RETVAL"].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };

                // Optionally rethrow or handle gracefully
                // throw; // uncomment if you want to propagate the exception
            }
            return new  { Status = 200, Message = "Call Added Successfully",Data = result };
        }

		//public async Task<dynamic> SaveFileuploadAsync(string? workId, string attachfilepath, ProjectUserTocken ut)
		//{
		//	try
		//	{
		//		using (var connection = new OracleConnection(_con))
		//		{
		//			await connection.OpenAsync();

		//			using (var command = new OracleCommand("PRMTRANS.SP_SAVE_PW_ATTACH_FILE", connection))
		//			{
		//				command.CommandType = CommandType.StoredProcedure;

		//				// Input Parameters
		//				command.Parameters.Add("P_WORK_ID", OracleDbType.Varchar2, workId, ParameterDirection.Input);
		//				command.Parameters.Add("P_ATTACH_FILE_PATH", OracleDbType.Varchar2, attachfilepath, ParameterDirection.Input);
		//				command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2, ut.AUSR_ID, ParameterDirection.Input);

		//				// Output Parameter
		//				var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 500)
		//				{
		//					Direction = ParameterDirection.Output
		//				};
		//				command.Parameters.Add(retvalParam);

		//				await command.ExecuteNonQueryAsync();

		//				// Ensure retvalParam.Value is not null before calling ToString()
		//				string result = retvalParam.Value?.ToString() ?? "";

		//				if (result == "1")
		//				{
		//					return new DefaultMessage.Message3 { Status = 200, Data = result };
		//				}
		//				else
		//				{
		//					return new DefaultMessage.Message3 { Status = 400, Data = result };
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		return new DefaultMessage.Message1 { Status = 400, Message = ex.Message };
		//	}
		//}

		public async Task<dynamic> SaveFileuploadAsync(string? workId, string attachfilepath, ProjectUserTocken ut)
		{
			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand("PRMTRANS.SP_SAVE_PW_ATTACH_FILE", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						long w = long.Parse(workId);

						// Input Parameters
						command.Parameters.Add("P_WORK_ID", OracleDbType.Varchar2).Value = workId ?? ""; // Avoid null
						command.Parameters.Add("P_ATTACH_FILE_PATH", OracleDbType.Varchar2).Value = attachfilepath ?? "";
						command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2).Value = ut?.AUSR_ID ?? "";

						// Output Parameter (size matters in VARCHAR2 OUT)
						var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 500)
						{
							Direction = ParameterDirection.Output
						};
						command.Parameters.Add(retvalParam);

						await command.ExecuteNonQueryAsync();

						string result = retvalParam.Value?.ToString() ?? "";

						if (result == "1")
						{
							return new DefaultMessage.Message3 { Status = 200, Data = result };
						}
						else
						{
							return new DefaultMessage.Message3 { Status = 400, Data = result };
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new DefaultMessage.Message1 { Status = 400, Message = ex.Message };
			}
		}


		public async Task<dynamic> GetFinYearId()
		{
			string connectionString = _DbContext.Database.GetConnectionString();
			string query = @"SELECT CUR_FIN_YEAR_ID FROM  PRMADMIN.CMP_SETTINGS WHERE 1=1";

			using (var connection = new OracleConnection(connectionString))
			{
				await connection.OpenAsync();

				using (var command = new OracleCommand(query, connection))
				{
					// Add the dt1 parameter to the command to avoid SQL injection
					//command.Parameters.Add(new OracleParameter(":dt1", OracleDbType.Varchar2, dt1, ParameterDirection.Input));

					var result = await command.ExecuteScalarAsync();

					// If the result is null, return a default value
					//return result?.ToString() ?? "No result"; // Return a default if no result is found
					return new DefaultMessage.Message3 { Status = 200, Data = result?.ToString() };
				}
			}
		}

		public async Task<dynamic> GetNextNo(int finId)
		{

			string connectionString = _DbContext.Database.GetConnectionString();
			//string query = @"SELECT nvl(MAX(PROJECT_WORK_NO),0)+1 NEXT_NO  from prmtrans.INV_PROJECT_WORK WHERE FIN_YEAR_ID= :finId";
			string query = @"SELECT nvl(MAX(PROJECT_WORK_NO),0)+1 NEXT_NO  from prmtrans.INV_PROJECT_WORK ";

			using (var connection = new OracleConnection(connectionString))
			{
				await connection.OpenAsync();

				using (var command = new OracleCommand(query, connection))
				{
					// Add the dt1 parameter to the command to avoid SQL injection
					command.Parameters.Add(new OracleParameter(":finId", OracleDbType.Int16, finId, ParameterDirection.Input));

					var result = await command.ExecuteScalarAsync();

					// If the result is null, return a default value
					//return result?.ToString() ?? "No result"; // Return a default if no result is found
					return new DefaultMessage.Message3 { Status = 200, Data = result?.ToString() };
				}
			}
		}


		public async Task<dynamic> GetRefNo(string ondate)
		{
			try
			{
				var finId1 = await GetFinYearId();
				int finId = int.Parse(finId1.Data);

				var NextN = await GetNextNo(finId);
				int? Num = int.Parse(NextN.Data);

				if (!DateTime.TryParseExact(ondate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
				{
					return new DefaultMessage.Message3 { Status = 400, Data = "Invalid date format. Use DD/MM/YYYY." };
				}

				return await GetFormattedString(Num, parsedDate);

			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		public async Task<dynamic> GetFormattedString(int? num, DateTime dt1)
		{
			string result = string.Empty;
			string query = "SELECT 'PW' || LPAD(MAX(NVL(:Num, 0)), 6, '0') || '-' || " +
						   "TO_CHAR(:dt1, 'MM') || '/' || TO_CHAR(:dt1, 'YYYY') FROM dual";

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						cmd.Parameters.Add(new OracleParameter("Num", num ?? (object)DBNull.Value));
						cmd.Parameters.Add(new OracleParameter("dt1", OracleDbType.Date)).Value = dt1;

						conn.Open();
						object resultObj = cmd.ExecuteScalar();
						if (resultObj != null)
						{
							result = resultObj.ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{

				return new DefaultMessage.Message1 { Status = 200, Message = ex.Message };
			}
			//return result;

			return new DefaultMessage.Message3 { Status = 200, Data = result };
		}

		public async Task<dynamic> Getworklist(string projectId)
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
    NVL(WAT.ATTACH_FILE_COUNT, 0) AS ATTACH_FILE_COUNT,
    CLIENT_WORK_STATUS,
    NVL(STS.ROW_COUNT, 0) AS STATUS_ROW_COUNT

FROM PRMTRANS.INV_PROJECT_WORK W 

INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID 
INNER JOIN PRMMASTER.INV_CLIENT_WORK_STATUS S ON S.CLIENT_WORK_STATUS_ID = W.PROJECT_WORK_STATUS_ID

-- Attach file count per work ID
LEFT JOIN (
    SELECT 
        COUNT(ATTACH_FILE_ID) AS ATTACH_FILE_COUNT,
        WORK_ID 
    FROM PRMTRANS.INV_PROJECTWORK_ATTACH_FILE 
    GROUP BY WORK_ID
) WAT ON WAT.WORK_ID = W.PROJECT_WORK_ID

-- Status row count per project work ID
LEFT JOIN (
    SELECT 
        A.PROJECT_WORK_ID,
        COUNT(*) AS ROW_COUNT
    FROM PRMMASTER.INV_CLIENT_WORK_STS_UPD A
    INNER JOIN PRMMASTER.INV_CLIENT_WORK_STATUS B ON A.CLIENT_WORK_STATUS_ID = B.CLIENT_WORK_STATUS_ID
    INNER JOIN PRMMASTER.HRM_EMPLOYEE C ON C.EMP_ID = A.CREATE_USER
    INNER JOIN PRMTRANS.INV_PROJECT_WORK D ON D.PROJECT_WORK_ID = A.PROJECT_WORK_ID

    -- Join with attachment table based on work ID and client work status ID
    LEFT JOIN PRMTRANS.INV_PRJT_WORK_ATTACHMENT ATT 
        ON ATT.CLNT_WORK_STATUS_ID = A.CLNT_WORK_STATUS_ID

    WHERE A.EXTERNAL_REMARKS IS NOT NULL
      or ATT.CLNT_WORK_STATUS_ID IS NOT NULL -- Ensures only rows with attachments are counted

    GROUP BY A.PROJECT_WORK_ID
) STS ON STS.PROJECT_WORK_ID = W.PROJECT_WORK_ID


WHERE  
    W.PROJECT_ID = :projectId 
    AND TRUNC(W.PROJECT_WORK_DATE) = TRUNC(SYSDATE)

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
						command.Parameters.Add(new OracleParameter(":projectId", projectId));
						

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


		public async Task<dynamic> Getworklistproject(string projectId)
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
    NVL(WAT.ATTACH_FILE_COUNT, 0) AS ATTACH_FILE_COUNT,
    CLIENT_WORK_STATUS,
    NVL(STS.ROW_COUNT, 0) AS STATUS_ROW_COUNT
FROM PRMTRANS.INV_PROJECT_WORK W 

INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID 
INNER JOIN PRMMASTER.INV_CLIENT_WORK_STATUS S ON S.CLIENT_WORK_STATUS_ID = W.PROJECT_WORK_STATUS_ID

-- Count of attachments per work ID
LEFT JOIN (
    SELECT 
        COUNT(ATTACH_FILE_ID) AS ATTACH_FILE_COUNT,
        WORK_ID 
    FROM PRMTRANS.INV_PROJECTWORK_ATTACH_FILE 
    GROUP BY WORK_ID
) WAT ON WAT.WORK_ID = W.PROJECT_WORK_ID

-- Count of status updates per project work ID
LEFT JOIN (
    SELECT 
        A.PROJECT_WORK_ID,
        COUNT(*) AS ROW_COUNT
    FROM PRMMASTER.INV_CLIENT_WORK_STS_UPD A
    INNER JOIN PRMMASTER.INV_CLIENT_WORK_STATUS B ON A.CLIENT_WORK_STATUS_ID = B.CLIENT_WORK_STATUS_ID
    INNER JOIN PRMMASTER.HRM_EMPLOYEE C ON C.EMP_ID = A.CREATE_USER
    INNER JOIN PRMTRANS.INV_PROJECT_WORK D ON D.PROJECT_WORK_ID = A.PROJECT_WORK_ID

    -- Join with attachment table based on work ID and client work status ID
    LEFT JOIN PRMTRANS.INV_PRJT_WORK_ATTACHMENT ATT 
        ON ATT.CLNT_WORK_STATUS_ID = A.CLNT_WORK_STATUS_ID

    WHERE A.EXTERNAL_REMARKS IS NOT NULL
      or ATT.CLNT_WORK_STATUS_ID IS NOT NULL -- Ensures only rows with attachments are counted

    GROUP BY A.PROJECT_WORK_ID
) STS ON STS.PROJECT_WORK_ID = W.PROJECT_WORK_ID

WHERE W.PROJECT_ID = :projectId 

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
						command.Parameters.Add(new OracleParameter(":projectId", projectId));


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

		public async Task<dynamic> GetAttach(string workid)
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


        public async Task<dynamic> DeleteProjectWorkAsync(long workId)
        {
            try
            {
                using (var connection = _DbContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    BEGIN
                        DELETE FROM PRMTRANS.INV_PROJECTWORK_ATTACH_FILE WHERE WORK_ID = :workId;
                        DELETE FROM PRMTRANS.INV_PROJECT_WORK WHERE PROJECT_WORK_ID = :workId;
                    END;";
                        command.CommandType = CommandType.Text;

                        // Add parameter
                        var param = command.CreateParameter();
                        param.ParameterName = ":workId";
                        param.Value = workId;
                        command.Parameters.Add(param);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return new { Status = 200, Message = "Deleted successfully." };
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<dynamic> UpdateProjectWorkDetails(ProjectWorkAddCall pw)
        {
            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    await connection.OpenAsync();

                    string sql = @"
                UPDATE PRMTRANS.INV_PROJECT_WORK 
                SET MODULE_ID = :modid, 
                    CONTACT_NAME = :contname, 
                    CONTACT_NO = :contno, 
                    CALLED_DESCRIPTION = :calldesc 
                WHERE PROJECT_WORK_ID = :projectWorkId";

                    using (var command = new OracleCommand(sql, connection))
                    {
                        // Add the parameter
                        command.Parameters.Add(new OracleParameter("modid", OracleDbType.Varchar2)).Value = pw.ModuleId;
                        command.Parameters.Add(new OracleParameter("contname", OracleDbType.Varchar2)).Value = pw.ContactName;
                        command.Parameters.Add(new OracleParameter("contno", OracleDbType.Varchar2)).Value = pw.ContactNo;
                        command.Parameters.Add(new OracleParameter("calldesc", OracleDbType.Varchar2)).Value = pw.CalledDescription;
                        command.Parameters.Add(new OracleParameter("projectWorkId", OracleDbType.Varchar2)).Value = pw.Project_WorkId;

                        // Execute the update query
                        int rowsUpdated = await command.ExecuteNonQueryAsync();

                        if (rowsUpdated > 0)
                        {
                            return new { Status = 200, Message = "Project work details updated successfully." };
                        }
                        else
                        {
                            return new { Status = 400, Message = "No rows updated. Check if the Project Work ID exists." };
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                return new { Status = 500, Message = "Oracle error: " + ex.Message };
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = "Unexpected error: " + ex.Message };
            }
        }

        public async Task<dynamic> DeleteAttachFileAsync(long fileId)
        {
            string sql = @"DELETE FROM PRMTRANS.INV_PROJECTWORK_ATTACH_FILE 
                       WHERE ATTACH_FILE_ID = :fileId";

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    await connection.OpenAsync();

                    using (var command = new OracleCommand(sql, connection))
                    {
                        command.Parameters.Add(new OracleParameter("fileId", fileId));

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if(rowsAffected > 0)
                        {

                            return new { Status = 200, Message = "Deleted successfully." };
                        }
                        else
                        {

                            return new { Status = 400, Message = "Error on Deletion." };
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return new { Status = 500, Message = $"Error: {ex.Message}" };
            }
        }






        public async Task<dynamic> UpdateProjectWorkConfirmation(string projectWorkId, string confirmedBy, string confirmedRemarks, string CurrentTaskStatus)
        {
            string query = @"
            UPDATE PRMTRANS.INV_PROJECT_WORK 
            SET 
                CONFIRMED_BY = :confirmedBy, 
                CONFIRMED_ON = SYSDATE, 
                CONFIRMED_STATUS = 'Y', 
                CONFIRMED_REMARKS = :confirmedRemarks 
            WHERE 
                PROJECT_WORK_ID = :projectWorkId";

            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("confirmedBy", confirmedBy));
                cmd.Parameters.Add(new OracleParameter("confirmedRemarks", confirmedRemarks));
                cmd.Parameters.Add(new OracleParameter("projectWorkId", projectWorkId));

                try
                {
                    conn.Open();
                    int rowsUpdated = cmd.ExecuteNonQuery();
                    if (rowsUpdated > 0)
                    {
                        if (CurrentTaskStatus == "1")
                        {
                            var a = await UpdateStatusAndAppendRemarks(projectWorkId, confirmedRemarks);
                            if (a != null && a.Status == 200)
                            {

                                return new { Status = 200, Message = "Confirmation Successfull" };

                            }
                            else
                            {
                                return a;
                            }
                        }
                        else
                        {

                            return new { Status = 200, Message = "Confirmation Successfull" };

                        }

                    }
                    else
                    {
                        return new { Status = 400, Message = "Error while confirmation" };
                    }
                }
                catch (Exception ex)
                {

                    return new { Status = 500, Message = ex.Message };
                }
            }
        }


        public async Task<dynamic> UpdateStatusAndAppendRemarks(string projectWorkId, string newRemark)
        {
            string query = @"
            UPDATE PRMTRANS.INV_PROJECT_WORK 
            SET 
                PROJECT_WORK_STATUS_ID = '4', 
                UPDATE_REMARKS = NVL(UPDATE_REMARKS, '') || ' ' || :newRemark 
            WHERE 
                PROJECT_WORK_ID = :projectWorkId";

            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("newRemark", newRemark));
                cmd.Parameters.Add(new OracleParameter("projectWorkId", projectWorkId));

                try
                {
                    conn.Open();
                    int rowsUpdated = cmd.ExecuteNonQuery();
                    if (rowsUpdated > 0)
                    {

                        return new { Status = 200, Message = "Confirmation Successfull" };
                    }
                    else
                    {
                        return new { Status = 400, Message = "Error while confirmation" };
                    }
                }
                catch (Exception ex)
                {

                    return new { Status = 500, Message = ex.Message };
                }
            }
        }


        public async Task<dynamic> GetClientWorkStatusesAsync()
        {
            var resultList = new List<dynamic>();

            using (var conn = new OracleConnection(_con))
            {
                try
                {
                    await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        SELECT CLIENT_WORK_STATUS_ID, CLIENT_WORK_STATUS, ACTIVE_STATUS, FOR_COLOR_CODE 
                        FROM PRMMASTER.INV_CLIENT_WORK_STATUS 
                        WHERE NVL(SHOW_CLIENT,'N') = 'Y' 
                          AND NVL(ACTIVE_STATUS,'A') = 'A'";

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                dynamic row = new ExpandoObject();
                                row.CLIENT_WORK_STATUS_ID = reader["CLIENT_WORK_STATUS_ID"]?.ToString();
                                row.CLIENT_WORK_STATUS = reader["CLIENT_WORK_STATUS"]?.ToString();
                                row.ACTIVE_STATUS = reader["ACTIVE_STATUS"]?.ToString();
                                row.FOR_COLOR_CODE = reader["FOR_COLOR_CODE"]?.ToString();

                                resultList.Add(row);
                            }
                        }
                    }

                    return new { Status = 200,Data = resultList };
                }
                catch (Exception ex)
                {

                    return new { Status = 500, Message = ex.Message };
                }
            }
        }
   
    
    
    }
}
