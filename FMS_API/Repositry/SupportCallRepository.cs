using FMS_API.Class;
using FMS_API.Data.Class;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Globalization;
using static JwtService;
namespace FMS_API.Repositry
{
	public class SupportCallRepository
	{
		private readonly AppDbContext _DbContext;

		private readonly JwtHandler jwthand;
		private readonly string _con;
		public SupportCallRepository(AppDbContext dbContext, JwtHandler _jwthand)
		{
			_DbContext = dbContext;
			jwthand = _jwthand;
			_con = _DbContext.Database.GetConnectionString();
		}


		public async Task<dynamic> GetEmergency()
		{
			List<dynamic> emergency = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					conn.Open();
					string query = "SELECT EMERGENCY_ID,EMERGENCY_TYPE \"EMERGENCY\" FROM PRMMASTER.INV_WORK_EMERGENCY  WHERE  NVL(ACTIVE_STATUS,'A')= 'A' order by upper(EMERGENCY_TYPE)";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						using (OracleDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								emergency.Add(new
								{
									EmergencyId = reader.GetInt32(0),
									Emergency = reader.GetString(1)
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


			return new DefaultMessage.Message3 { Status = 200, Data = emergency };
		}

		//public async Task<dynamic> GetActiveProjects(long clientId)
		//{
		//    List<dynamic> projects = new List<dynamic>();

		//    try
		//    {
		//        using (OracleConnection conn = new OracleConnection(_con))
		//        {
		//            conn.Open();
		//            //string query = "SELECT PROJECT_ID, PROJECT_NAME FROM PRMMASTER.INV_PROJECT_MASTER WHERE NVL(ACTIVE_STATUS, 'A') = 'A'";
		//            string query = " Select distinct P.PROJECT_ID ,P.PROJECT_NAME ,PRJ.CUST_ID FROM PRMTRANS.INV_PROJECT_ENTRY PRJ INNER Join PRMMASTER.GEN_CUSTOMERS CUST ON PRJ.CUST_ID=CUST.CUST_ID  INNER Join PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID=PRJ.PROJECT_ID WHERE PRJ.CUST_ID=:clientId ORDER BY UPPER(P.PROJECT_NAME)";

		//            using (OracleCommand cmd = new OracleCommand(query, conn))
		//            {
		//                cmd.Parameters.Add(new OracleParameter("clientId", clientId));
		//                using (OracleDataReader reader = cmd.ExecuteReader())
		//                {
		//                    while (reader.Read())
		//                    {
		//                        projects.Add(new
		//                        {
		//                            ProjectId = reader.GetInt32(0),
		//                            ProjectName = reader.GetString(1)
		//                        });
		//                    }
		//                }
		//            }
		//        }
		//    }
		//    catch (OracleException ex)
		//    {
		//        return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//    }
		//    catch (Exception ex)
		//    {
		//        return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//    }


		//    return new DefaultMessage.Message3 { Status = 200, Data = projects };
		//}


		public async Task<dynamic> GetActiveProjects(long empId)
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
                AND E.EMP_ID = :empId 
                ORDER BY UPPER(P.PROJECT_NAME)";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						cmd.Parameters.Add(new OracleParameter("empId", empId));
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


		public async Task<dynamic> GetErrorTypes()
		{
			List<dynamic> errortypes = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					conn.Open();
					// Query to retrieve error type details
					string query = "SELECT ERROR_TYPE_ID, ERROR_TYPE \"Error Type\", NVL(EMP_ASSIGN_STS, 'N') EMP_ASSIGN_STS " +
								   "FROM PRMMASTER.INV_WORK_ERROR_TYPE " +
								   "WHERE NVL(ACTIVE_STATUS, 'A') = 'A' ORDER BY UPPER(ERROR_TYPE)";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						using (OracleDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								errortypes.Add(new
								{
									ErrorTypeId = reader.GetInt32(0),
									ErrorType = reader.GetString(1),
									EmpAssignSts = reader.GetString(2)  // Now includes EMP_ASSIGN_STS
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

			return new DefaultMessage.Message3 { Status = 200, Data = errortypes };
		}


		public async Task<dynamic> GetCallTypes()
		{
			List<dynamic> calltypes = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					conn.Open();
					string query = "SELECT CALL_TYPE_ID,CALL_TYPE \"Call Type\" FROM PRMMASTER.INV_WORK_CALL_TYPE  WHERE  NVL(ACTIVE_STATUS,'A')= 'A' order by upper(CALL_TYPE)";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						using (OracleDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								calltypes.Add(new
								{
									CallTypeId = reader.GetInt32(0),
									CallType = reader.GetString(1)
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


			return new DefaultMessage.Message3 { Status = 200, Data = calltypes };
		}

		//public async Task<dynamic> GetModules()
		//{
		//	List<dynamic> modules = new List<dynamic>();

		//	try
		//	{
		//		using (OracleConnection conn = new OracleConnection(_con))
		//		{
		//			conn.Open();
		//			string query = "SELECT MODULE_ID,MODULE_NAME \"Module\" FROM PRMMASTER.INV_WORK_MODULE  WHERE  NVL(ACTIVE_STATUS,'A')= 'A' order by upper(MODULE_NAME)";

		//			using (OracleCommand cmd = new OracleCommand(query, conn))
		//			{
		//				using (OracleDataReader reader = cmd.ExecuteReader())
		//				{
		//					while (reader.Read())
		//					{
		//						modules.Add(new
		//						{
		//							ModuleId = reader.GetInt32(0),
		//							Module = reader.GetString(1)
		//						});
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (OracleException ex)
		//	{
		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}
		//	catch (Exception ex)
		//	{
		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}


		//	return new DefaultMessage.Message3 { Status = 200, Data = modules };
		//}



		public async Task<dynamic> GetModules(long empId)
		{
			List<dynamic> modules = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					await conn.OpenAsync();

					string query = @"
                SELECT DISTINCT 
                    M.MODULE_ID, 
                    M.MODULE_NAME 
                FROM PRMMASTER.HRM_EMPLOYEE E
                INNER JOIN PRMMASTER.WEB_MODULE_GROUP_DTLS D ON D.GROUP_ID = E.MODULE_GROUP_ID
                INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = D.MODULE_ID
                INNER JOIN PRMMASTER.WEB_MODULE_GROUP G ON D.GROUP_ID = E.MODULE_GROUP_ID
                WHERE NVL(M.ACTIVE_STATUS, 'A') = 'A' 
                AND NVL(G.ACTIVE_STATUS, 'A') = 'A' 
                AND E.EMP_ID = :empId 
                ORDER BY MODULE_NAME ASC";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						cmd.Parameters.Add(new OracleParameter("empId", empId));

						using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								modules.Add(new
								{
									ModuleId = reader.GetInt32(0),
									ModuleName = reader.GetString(1)
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

			return new DefaultMessage.Message3 { Status = 200, Data = modules };
		}



		public async Task<dynamic> GetEmployee()
		{
			List<dynamic> employee = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					conn.Open();
					string query = " select EMP_ID,EMP_NAME  from  PRMMASTER.HRM_EMPLOYEE  WHERE  NVL(ACTIVE_STATUS,'A')= 'A' order by upper(EMP_NAME)";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						using (OracleDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								employee.Add(new
								{
									EmployeeId = reader.GetInt32(0),
									EmployeeName = reader.GetString(1)
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


			return new DefaultMessage.Message3 { Status = 200, Data = employee };
		}

		//public async Task<dynamic> GetClients()
		//{
		//	List<dynamic> clients = new List<dynamic>();

		//	try
		//	{
		//		using (OracleConnection conn = new OracleConnection(_con))
		//		{
		//			conn.Open();
		//			string query = "select  CUST_ID,CUST_NAME  from PRMMASTER.GEN_CUSTOMERS  WHERE  NVL(ACTIVE_STATUS,'A')= 'A' order by upper(CUST_NAME)";

		//			using (OracleCommand cmd = new OracleCommand(query, conn))
		//			{
		//				using (OracleDataReader reader = cmd.ExecuteReader())
		//				{
		//					while (reader.Read())
		//					{
		//						clients.Add(new
		//						{
		//							CustomerId = reader.GetInt32(0),
		//							CustomerName = reader.GetString(1)
		//						});
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (OracleException ex)
		//	{
		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}
		//	catch (Exception ex)
		//	{
		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}


		//	return new DefaultMessage.Message3 { Status = 200, Data = clients };
		//}


		public async Task<dynamic> GetClients(long empId, long projectId)
		{
			List<dynamic> clients = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					await conn.OpenAsync();

					string query = @"
                SELECT DISTINCT C.CUST_ID,C.CUST_NAME ,PRJ.PROJECT_ID
                FROM PRMMASTER.HRM_EMPLOYEE E
                INNER JOIN PRMMASTER.WEB_CUST_GROUP_DTLS D ON D.GROUP_ID = E.CUST_GROUP_ID
                INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = D.CUST_ID
                INNER JOIN PRMMASTER.WEB_CUST_GROUP G ON D.GROUP_ID = E.CUST_GROUP_ID
                 INNER JOIN PRMTRANS.INV_PROJECT_ENTRY PRJ ON   PRJ.CUST_ID = D.CUST_ID
                WHERE NVL(C.ACTIVE_STATUS, 'A') = 'A' 
                AND NVL(G.ACTIVE_STATUS, 'A') = 'A' 
                 AND E.EMP_ID = :empId
                AND  PRJ.PROJECT_ID=:projectId
                ORDER BY C.CUST_NAME ASC";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						cmd.Parameters.Add(new OracleParameter("empId", empId));
						cmd.Parameters.Add(new OracleParameter("projectId", projectId));

						using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								clients.Add(new
								{
									CustomerId = reader.GetInt32(0),
									CustomerName = reader.GetString(1)
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

			return new DefaultMessage.Message3 { Status = 200, Data = clients };
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
			string query = @"SELECT nvl(MAX(WORK_NO),0)+1 NEXT_NO  from prmtrans.INV_WORK WHERE FIN_YEAR_ID= :finId";

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


		public async Task<dynamic> GetSupportRefNo(string ondate)
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
			string query = "SELECT 'SW' || LPAD(MAX(NVL(:Num, 0)), 6, '0') || '-' || " +
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



		public async Task<dynamic> SaveWorkAssignmentAsync(string workId, long? empId, string createUser)
		{
			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand("PRMTRANS.SP_SAVE_APP_WORK_ASSIGN_DTLS", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						// Input Parameters
						command.Parameters.Add("P_WORK_ID", OracleDbType.Varchar2, workId, ParameterDirection.Input);
						command.Parameters.Add("P_EMP_ID", OracleDbType.Varchar2, empId, ParameterDirection.Input);
						command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2, createUser, ParameterDirection.Input);

						// Output Parameter
						var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 500)
						{
							Direction = ParameterDirection.Output
						};
						command.Parameters.Add(retvalParam);

						await command.ExecuteNonQueryAsync();

						// Ensure retvalParam.Value is not null before calling ToString()
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


		public async Task<dynamic> GetWorkIdAsync(string workRefNo)
		{
			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					string query = "SELECT WORK_ID FROM prmtrans.INV_WORK WHERE WORK_REFNO = :workrefno";

					using (var command = new OracleCommand(query, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.Add("workrefno", OracleDbType.Varchar2, workRefNo, ParameterDirection.Input);

						var result = await command.ExecuteScalarAsync();

						//return result?.ToString() ?? "No work ID found";

						if (result != null)
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

				return new { Status = 500, Message = ex.Message };
			}
		}

        public async Task<dynamic> SaveSupportCall(Calldet ex, UserTocken ut)
        {
            string retval = string.Empty;
            //var refno = await GetRefNo();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_SAVE_APP_SUPPORT_WORK", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Adding input parameters
                        //new OracleParameter("P_WORK_ASSIGN_DATE", OracleDbType.Date)
                        //{
                        //	Value = string.IsNullOrEmpty(ex.WorkAssignDate) ? (object)DBNull.Value : (object)DateTime.Parse(ex.WorkAssignDate)
                        //};
                        cmd.Parameters.Add(new OracleParameter("P_WORK_DATE", OracleDbType.Date)).Value = string.IsNullOrEmpty(ex.WorkAssignDate) ? (object)DBNull.Value : (object)DateTime.Parse(ex.WorkAssignDate);

                        cmd.Parameters.Add(new OracleParameter("P_WORK_REFNO", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.WorkRefNo) ? DBNull.Value : ex.WorkRefNo;

                        cmd.Parameters.Add(new OracleParameter("P_CUST_ID", OracleDbType.Int32)).Value = ex.CustomerId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_PROJECT_ID", OracleDbType.Int32)).Value = ex.ProjectId ?? (object)DBNull.Value;
                        //cmd.Parameters.Add(new OracleParameter("P_EMP_ASSIGN_ID", OracleDbType.Int32)).Value = ex.EmployeeId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_MODULE_ID", OracleDbType.Int32)).Value = ex.ModuleId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_CALL_TYPE_ID", OracleDbType.Int32)).Value = ex.CallTypeId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_EMERGENCY_ID", OracleDbType.Int32)).Value = ex.EmergencyId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_ERROR_ID", OracleDbType.Int32)).Value = ex.ErrorTypeId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_CALLED_BY", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.CalledBy) ? DBNull.Value : ex.CalledBy;
                        cmd.Parameters.Add(new OracleParameter("P_SCT_ID", OracleDbType.Varchar2)).Value = "1";
                        //cmd.Parameters.Add(new OracleParameter("P_LOGIN_EMP_ID", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.LoginEmpId) ? DBNull.Value : ex.LoginEmpId;
                        cmd.Parameters.Add(new OracleParameter("P_LOGIN_EMP_ID", OracleDbType.Varchar2)).Value = ut.AUSR_ID;
                        cmd.Parameters.Add(new OracleParameter("P_CALLED_DESC", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.CalledDesc) ? DBNull.Value : ex.CalledDesc;
                        //cmd.Parameters.Add(new OracleParameter("P_ATTACH_FILE", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.AttachFile) ? DBNull.Value : ex.AttachFile;
                        string sta;
                        if (ex.EmployeeId>0 )
                        {
                            sta = "2";
                        }
                        else
                        {
                            sta = "1";
                        }
                        cmd.Parameters.Add(new OracleParameter("P_WORK_STATUS_ID", OracleDbType.Varchar2)).Value = sta;

                        // Adding user reference
                        //cmd.Parameters.Add(new OracleParameter("P_CREATE_USER", OracleDbType.Varchar2)).Value = ut.AUSR_ID;

                        // Adding output parameter
                        var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 15)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        };
                        cmd.Parameters.Add(retvalParam);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();
                        var workid = await GetWorkIdAsync(ex.WorkRefNo);

                        // Ensure retvalParam.Value is not null before calling ToString()
                        string result = retvalParam.Value?.ToString() ?? "";

                        //if (result == "1")
                        //{
                        return new { Status = 200, Message = "Support Call Details saved Successfully", Data = workid.Data };
                        //}
                        //else
                        //{
                        //	return new { Status = 400, Message = "Error Support Call" };
                        //}


                    }
                }
            }
            catch (OracleException exp)
            {
                return new { Status = 500, Message = exp.Message };
            }
            catch (Exception exp)
            {
                return new { Status = 500, Message = exp.Message };
            }
        }

        public async Task<dynamic> SaveFileuploadAsync(string? workId, string attachfilepath, UserTocken ut)
        {
            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    await connection.OpenAsync();

                    using (var command = new OracleCommand("PRMTRANS.SP_SAVE_APP_WORK_ATTACH_FILE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input Parameters
                        command.Parameters.Add("P_WORK_ID", OracleDbType.Varchar2, workId, ParameterDirection.Input);
                        command.Parameters.Add("P_ATTACH_FILE_PATH", OracleDbType.Varchar2, attachfilepath, ParameterDirection.Input);
                        command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2, ut.AUSR_ID, ParameterDirection.Input);

                        // Output Parameter
                        var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(retvalParam);

                        await command.ExecuteNonQueryAsync();

                        // Ensure retvalParam.Value is not null before calling ToString()
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

        public async Task<dynamic> SaveProjectWorkStatusFileuploadAsync(string? P_CLNT_WORK_STATUS_ID, string attachfilepath, UserTocken ut)
        {
            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    await connection.OpenAsync();

                    using (var command = new OracleCommand("PRMTRANS.SP_SAVE_PW_ATTACH_FILE_UPD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input Parameters
                        command.Parameters.Add("P_CLNT_WORK_STATUS_ID", OracleDbType.Varchar2, P_CLNT_WORK_STATUS_ID, ParameterDirection.Input);
                        command.Parameters.Add("P_ATTACH_FILE_PATH", OracleDbType.Varchar2, attachfilepath, ParameterDirection.Input);
                        command.Parameters.Add("P_CREATE_USER", OracleDbType.Varchar2, ut.AUSR_ID, ParameterDirection.Input);

                        // Output Parameter
                        var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(retvalParam);

                        await command.ExecuteNonQueryAsync();

                        // Ensure retvalParam.Value is not null before calling ToString()
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

       

        private string SanitizeFileName(string input)
        {
            // List of invalid characters in file names
            char[] invalidChars = Path.GetInvalidFileNameChars();

            // Remove invalid characters from wrefno
            foreach (char c in invalidChars)
            {
                input = input.Replace(c.ToString(), string.Empty);
            }

            return input;
        }
    }
}
