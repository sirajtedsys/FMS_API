using FMS_API.Class;
using FMS_API.Data.Class;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

using System.Data.Common;
using System.Globalization;
using static JwtService;
namespace FMS_API.Repositry
{
	public class AddCallRepository
	{
		private readonly AppDbContext _DbContext;

		private readonly JwtHandler jwthand;
		private readonly string _con;
        //private readonly AttendenceRepositry Ar;

		public AddCallRepository(AppDbContext dbContext, JwtHandler _jwthand)
		{
			_DbContext = dbContext;
			jwthand = _jwthand;
			_con = _DbContext.Database.GetConnectionString();
            //Ar = _ar;
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

		//, long clientId
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
					                //AND PRJ.CUST_ID = :clientId 

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



  //      public async Task<dynamic> GetEmployee()
		//{
		//	List<dynamic> employee = new List<dynamic>();

		//	try
		//	{
		//		using (OracleConnection conn = new OracleConnection(_con))
		//		{
		//			conn.Open();
		//			string query = " select EMP_ID,EMP_NAME  from  PRMMASTER.HRM_EMPLOYEE  WHERE  NVL(ACTIVE_STATUS,'A')= 'A' order by upper(EMP_NAME)";

		//			using (OracleCommand cmd = new OracleCommand(query, conn))
		//			{
		//				using (OracleDataReader reader = cmd.ExecuteReader())
		//				{
		//					while (reader.Read())
		//					{
		//						employee.Add(new
		//						{
		//							EmployeeId = reader.GetInt32(0),
		//							EmployeeName = reader.GetString(1)
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


		//	return new DefaultMessage.Message3 { Status = 200, Data = employee };
		//}

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



        public async Task<dynamic> GetClients(long empId,long projectId)
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

        public async Task<dynamic> GetEmployee(string empId)
        {
            List<dynamic> employee = new List<dynamic>();

            try
            {
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    conn.Open();

                    // Updated query to match the provided query
                    string query = @"
                SELECT 
                    D.EMP_ID,
                    INITCAP(EMP.EMP_NAME) AS EMP_NAME
                FROM 
                    PRMMASTER.HRM_EMPLOYEE E
                INNER JOIN  
                    PRMMASTER.WEB_EMP_GROUP_DTLS D ON D.GROUP_ID = E.EMP_GROUP_ID
                INNER JOIN 
                    PRMMASTER.WEB_EMP_GROUP G ON G.GROUP_ID = D.GROUP_ID
                INNER JOIN 
                    PRMMASTER.HRM_EMPLOYEE EMP ON D.EMP_ID = EMP.EMP_ID
                WHERE 
                    E.EMP_ID = :empId 
                    AND NVL(EMP.ACTIVE_STATUS, 'A') = 'A' 
                    AND NVL(G.ACTIVE_STATUS, 'A') = 'A'
                ORDER BY 
                    UPPER(EMP_NAME) ASC";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        // Add the parameter for EMP_ID to the query
                        cmd.Parameters.Add(new OracleParameter(":empId", empId));

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map the result to the dynamic object
                                employee.Add(new
                                {
                                    EmployeeId = reader.GetString(0), // EMP_ID
                                    EmployeeName = reader.GetString(1) // EMP_NAME
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
            string query = "SELECT 'W' || LPAD(MAX(NVL(:Num, 0)), 6, '0') || '-' || " +
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
                            return new  { Status = 200, Data = result ,Message= "Work ASsigned Sucessfully"};
                        }
                        else
                        {
                            return new  { Status = 400, Data = result,Message="Error While Assigning work to Employee" };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 400, Message = ex.Message };
            }
        }


        public async Task<dynamic> SaveWorkAssignmentFromCallList(string workId, long? empId, string createUser)
        {
            try
            {
                var saveworkAssignment = await SaveWorkAssignmentAsync(workId, empId, createUser);
                if (saveworkAssignment.Status == 200) 
                {
                    var UpdateWorkStatus = await UpdateWorkStatusAsync(long.Parse(workId), 2);
                    if(UpdateWorkStatus.Status==200)
                    {
                        return new { Status = 200, Message = "Work Assigned To Employee Successfully" };
                    }
                    else
                    {
                        return UpdateWorkStatus;
                    }
                }
                else
                {
                    return saveworkAssignment;
                }
            }
            catch (Exception ex)
            {
                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
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

        //  public async Task<dynamic> SaveAddCall(Calldet ex, UserTocken ut)
        //  {
        //      string retval = string.Empty;
        //      //var refno = await GetRefNo();

        //      try
        //      {
        //          using (OracleConnection conn = new OracleConnection(_con))
        //          {
        //              conn.Open();
        //              using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_SAVE_APP_WORK", conn))
        //              {
        //                  cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //                  // Adding input parameters
        //                  //new OracleParameter("P_WORK_ASSIGN_DATE", OracleDbType.Date)
        //                  //{
        //                  //	Value = string.IsNullOrEmpty(ex.WorkAssignDate) ? (object)DBNull.Value : (object)DateTime.Parse(ex.WorkAssignDate)
        //                  //};
        //                  cmd.Parameters.Add(new OracleParameter("P_WORK_DATE", OracleDbType.Date)).Value = string.IsNullOrEmpty(ex.WorkAssignDate) ? (object)DBNull.Value : (object)DateTime.Parse(ex.WorkAssignDate);

        //                  cmd.Parameters.Add(new OracleParameter("P_WORK_REFNO", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.WorkRefNo) ? DBNull.Value : ex.WorkRefNo;

        //                  cmd.Parameters.Add(new OracleParameter("P_CUST_ID", OracleDbType.Int32)).Value = ex.CustomerId ?? (object)DBNull.Value;
        //                  cmd.Parameters.Add(new OracleParameter("P_PROJECT_ID", OracleDbType.Int32)).Value = ex.ProjectId ?? (object)DBNull.Value;
        //                  cmd.Parameters.Add(new OracleParameter("P_EMP_ASSIGN_ID", OracleDbType.Int32)).Value = ex.EmployeeId ?? (object)DBNull.Value;
        //                  cmd.Parameters.Add(new OracleParameter("P_MODULE_ID", OracleDbType.Int32)).Value = ex.ModuleId ?? (object)DBNull.Value;
        //                  cmd.Parameters.Add(new OracleParameter("P_CALL_TYPE_ID", OracleDbType.Int32)).Value = ex.CallTypeId ?? (object)DBNull.Value;
        //                  cmd.Parameters.Add(new OracleParameter("P_EMERGENCY_ID", OracleDbType.Int32)).Value = ex.EmergencyId ?? (object)DBNull.Value;
        //                  cmd.Parameters.Add(new OracleParameter("P_ERROR_ID", OracleDbType.Int32)).Value = ex.ErrorTypeId ?? (object)DBNull.Value;
        //                  cmd.Parameters.Add(new OracleParameter("P_CALLED_BY", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.CalledBy) ? DBNull.Value : ex.CalledBy;
        //                  cmd.Parameters.Add(new OracleParameter("P_SCT_ID", OracleDbType.Varchar2)).Value = "1";
        //                  //cmd.Parameters.Add(new OracleParameter("P_LOGIN_EMP_ID", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.LoginEmpId) ? DBNull.Value : ex.LoginEmpId;
        //                  cmd.Parameters.Add(new OracleParameter("P_LOGIN_EMP_ID", OracleDbType.Varchar2)).Value = ut.AUSR_ID;
        //                  cmd.Parameters.Add(new OracleParameter("P_CALLED_DESC", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.CalledDesc) ? DBNull.Value : ex.CalledDesc;
        //                  cmd.Parameters.Add(new OracleParameter("P_ATTACH_FILE", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.AttachFile) ? DBNull.Value : ex.AttachFile;
        //                  string sta;
        //                  if (ex.EmployeeId>0)
        //                  {
        //                      sta = "2";
        //                  }
        //                  else
        //                  {
        //                      sta = "1";
        //                  }
        //                  cmd.Parameters.Add(new OracleParameter("P_WORK_STATUS_ID", OracleDbType.Varchar2)).Value = sta;

        //                  // Adding user reference
        //                  //cmd.Parameters.Add(new OracleParameter("P_CREATE_USER", OracleDbType.Varchar2)).Value = ut.AUSR_ID;

        //                  // Adding output parameter
        //                  var retvalParam = new OracleParameter("RETVAL", OracleDbType.Varchar2, 15)
        //                  {
        //                      Direction = System.Data.ParameterDirection.Output
        //                  };
        //                  cmd.Parameters.Add(retvalParam);

        //                  // Execute the stored procedure
        //                  cmd.ExecuteNonQuery();


        //var workid = await GetWorkIdAsync(ex.WorkRefNo);
        //if(workid.Status==200)
        //{
        //                      if (ex.EmployeeId > 0)
        //                      {
        //                          var d = await SaveWorkAssignmentAsync(workid.Data.ToString(), (long?)ex.EmployeeId, ut.AUSR_ID);

        //                          if (d.Status == 200)
        //                          {

        //                              return new { Status = 200, Message = "Call Details saved Successfully" };
        //                          }
        //                          else
        //                          {

        //                              return new { Status = 400, Message = "Error while call assigning to Employee" };
        //                          }

        //                      }
        //                      else
        //                      {

        //                          return new { Status = 200, Message = "Call Details saved Successfully" };
        //                      }


        //                  }
        //else
        //{
        //	return new DefaultMessage.Message1 { Status = 200, Message = "error while fetching work ref no" };
        //}

        //                  //if(sta=="1")
        //                  //{


        //                  //}

        //                  // Get the output parameter value
        //                  //retval = retvalParam.Value.ToString();

        //                  //if (string.IsNullOrEmpty(retval))
        //                  //{
        //                  //	return new { Status = 500, Message = "Error while saving the call details" };
        //                  //}

        //              }
        //          }
        //      }
        //      catch (OracleException exp)
        //      {
        //          return new { Status = 500, Message = exp.Message };
        //      }
        //      catch (Exception exp)
        //      {
        //          return new { Status = 500, Message = exp.Message };
        //      }
        //  }

        public async Task<dynamic> SaveAddCall(Calldet ex, UserTocken ut)
        {
            string retval = string.Empty;
            //var refno = await GetRefNo();

            try
            {

                //var specificDate2 = DateTime.ParseExact(DateTime.Now, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime specificDate = DateTime.Today;
                string specificDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                var refno = await GetRefNo(specificDate);
                ex.WorkRefNo = refno.Data;
                using (OracleConnection conn = new OracleConnection(_con))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_SAVE_APP_WORK", conn))
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
                        cmd.Parameters.Add(new OracleParameter("P_EMP_ASSIGN_ID", OracleDbType.Int32)).Value = ex.EmployeeId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_MODULE_ID", OracleDbType.Int32)).Value = ex.ModuleId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_CALL_TYPE_ID", OracleDbType.Int32)).Value = ex.CallTypeId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_EMERGENCY_ID", OracleDbType.Int32)).Value = ex.EmergencyId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_ERROR_ID", OracleDbType.Int32)).Value = ex.ErrorTypeId ?? (object)DBNull.Value;
                        cmd.Parameters.Add(new OracleParameter("P_CALLED_BY", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.CalledBy) ? DBNull.Value : ex.CalledBy;
                        cmd.Parameters.Add(new OracleParameter("P_SCT_ID", OracleDbType.Varchar2)).Value = "1";
                        //cmd.Parameters.Add(new OracleParameter("P_LOGIN_EMP_ID", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.LoginEmpId) ? DBNull.Value : ex.LoginEmpId;
                        cmd.Parameters.Add(new OracleParameter("P_LOGIN_EMP_ID", OracleDbType.Varchar2)).Value = ut.AUSR_ID;
                        cmd.Parameters.Add(new OracleParameter("P_CALLED_DESC", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.CalledDesc) ? DBNull.Value : ex.CalledDesc;
                        cmd.Parameters.Add(new OracleParameter("P_ATTACH_FILE", OracleDbType.Varchar2)).Value = string.IsNullOrEmpty(ex.AttachFile) ? DBNull.Value : ex.AttachFile;
                        string sta;
                        if (ex.EmployeeId > 0)
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
                        if (workid.Status == 200)
                        {
                            if (ex.EmployeeId > 0)
                            {
                                var d = await SaveWorkAssignmentAsync(workid.Data.ToString(), (long?)ex.EmployeeId, ut.AUSR_ID);

                                if (d.Status == 200)
                                {


                                    return new { Status = 200, Message = "Call Details saved Successfully", Data = workid.Data };
                                }
                                else
                                {

                                    return new { Status = 400, Message = "Error while call assigning to Employee" };
                                }

                            }
                            else
                            {

                                return new { Status = 200, Message = "Call Details saved Successfully", Data = workid.Data };
                            }


                        }
                        else
                        {
                            return new DefaultMessage.Message1 { Status = 200, Message = "error while fetching work ref no" };
                        }

                        //if(sta=="1")
                        //{


                        //}

                        // Get the output parameter value
                        //retval = retvalParam.Value.ToString();

                        //if (string.IsNullOrEmpty(retval))
                        //{
                        //	return new { Status = 500, Message = "Error while saving the call details" };
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


        public async Task<dynamic> UpdateWorkStatusAsync(long workId, int workStatusId)
        {
            if (workId == 0)
            {
                return new { Status = 400, Message = "Invalid Work ID" };
            }

            string query = "UPDATE PRMTRANS.INV_WORK SET WORK_STATUS_ID = :workStatusId WHERE WORK_ID = :workId";

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    await connection.OpenAsync();

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(":workStatusId", OracleDbType.Int32).Value = workStatusId;
                        command.Parameters.Add(":workId", OracleDbType.Int64).Value = workId;

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return new { Status = 200, Message = "Work status updated successfully" };
                        else
                            return new { Status = 404, Message = "Work ID not found" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }

        public async Task<dynamic> UpdateWorkAssignmentStatusAsync(long workDtlsId, long workId, int workStatusId)
        {
            if (workDtlsId == 0 || workId == 0)
            {
                return new { Status = 400, Message = "Invalid Work Details ID or Work ID" };
            }

            string query = @"
            UPDATE PRMTRANS.INV_WORK_ASSIGN_DTLS 
            SET WORK_STATUS_ID = :workStatusId 
            WHERE WORK_DTLS_ID = :workDtlsId AND WORK_ID = :workId";

            try
            {
                using (var connection = new OracleConnection(_con))
                {
                    await connection.OpenAsync();

                    using (var command = new OracleCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(":workStatusId", OracleDbType.Int32).Value = workStatusId;
                        command.Parameters.Add(":workDtlsId", OracleDbType.Int64).Value = workDtlsId;
                        command.Parameters.Add(":workId", OracleDbType.Int64).Value = workId;

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return new { Status = 200, Message = "Work assignment status updated successfully" };
                        else
                            return new { Status = 404, Message = "Work Details ID or Work ID not found" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }
    
        //below funtion is using for two cases one for closing call from call list and another closing call from inbox
        //while closing call from inbox workdtls id will be not zero and both function inside updatework status will run and retuen else only updateworkstatusasync function will work and workdtlsid is 0 as per it is from call list

        public async Task<dynamic>UpdateWorkStatus(long workDtlsId, long workId, int workStatusId)
        {
            var statusupdte = await UpdateWorkStatusAsync(workId, workStatusId);
            if (statusupdte.Status == 200 && workDtlsId!=0) 
            { 
                var sts2 = await UpdateWorkAssignmentStatusAsync(workDtlsId, workId, workStatusId);
                if(sts2.Status ==200)
                {
                    return new { Status = 200, Message = "Work Status Updated" };
                }
                else
                {
                    return sts2;
                }
            }
            else

            {
                return statusupdte;
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
                return new DefaultMessage.Message3 { Status = 200, Data = result };
            }
            catch (Exception ex)
            {

                return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
            }
        }

        public async Task<dynamic> GetWorkDetailsAsync(string empId, string? whereClause = "")
        {
            DataTable dataTable = new DataTable();

            // Add additional filters only if provided
            string additionalWhere = string.IsNullOrWhiteSpace(whereClause) ? "" : $" AND {whereClause}";

            // Query with proper placement of WHERE and ORDER BY
            string sqlQuery = $@"
    SELECT *
    FROM (
        SELECT DISTINCT 
            W.WORK_TYPE, 
            W.WORK_STATUS_ID, 
            ST.WORK_STATUS, 
            W.WORK_ID, 
            W.WORK_REFNO, 
            W.WORK_DATE, 
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
            W.CALLED_BY, 
            W.CALLED_DESCRIPTION, 
            EMP.EMP_NAME, 
            W.CREATE_DATE,
            NVL(WAT.ATTACH_FILE_COUNT, 0) AS ATTACH_FILE_COUNT
        FROM PRMTRANS.INV_WORK W
        INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = W.CUST_ID
        INNER JOIN PRMMASTER.INV_PROJECT_MASTER P ON P.PROJECT_ID = W.PROJECT_ID
        INNER JOIN PRMMASTER.INV_WORK_MODULE M ON M.MODULE_ID = W.MODULE_ID
        LEFT JOIN PRMMASTER.INV_WORK_EMERGENCY E ON E.EMERGENCY_ID = W.EMERGENCY_ID
        LEFT JOIN PRMMASTER.INV_WORK_ERROR_TYPE ET ON ET.ERROR_TYPE_ID = W.ERROR_TYPE_ID
        LEFT JOIN PRMMASTER.INV_WORK_CALL_TYPE CT ON CT.CALL_TYPE_ID = W.CALL_TYPE_ID
        INNER JOIN PRMMASTER.INV_WORK_STATUS ST ON ST.WORK_STATUS_ID = W.WORK_STATUS_ID
        INNER JOIN PRMMASTER.HRM_EMPLOYEE EMP ON EMP.EMP_ID = W.CREATE_USER
        LEFT JOIN (
            SELECT WORK_ID, COUNT(ATTACH_FILE_ID) AS ATTACH_FILE_COUNT
            FROM PRMTRANS.INV_WORK_ATTACH_FILE 
            GROUP BY WORK_ID
        ) WAT ON WAT.WORK_ID = W.WORK_ID
        INNER JOIN PRMTRANS.INV_EMP_WORK_MAPPING MP 
            ON W.WORK_TYPE = MP.WORK_TYPE AND MP.EMP_ID = :empId
        WHERE NVL(W.CLOSE_STATUS, 'N') = 'N' {additionalWhere}
    )
    ORDER BY WORK_DATE DESC";

            try
            {
                using (var connection = _DbContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlQuery;

                        // OracleParameter for EMP_ID
                        command.Parameters.Add(new OracleParameter(":empId", empId));

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

        public async Task<dynamic> OpenCallBySelfFromCAllList(string WorkId, string EmpId)
        {
            try
            {
                var SaveWorkAssignment = await InsertWorkAssignmentAsync(WorkId,EmpId,"3", EmpId);
                if(SaveWorkAssignment.Status==200)
                {
                    var updateWorkStatus = await UpdateWorkStatusAsync(long.Parse(WorkId), 3);
                    if (updateWorkStatus.Status == 200)
                    {
                        return new { Status = 200, Message = "Work Open Successfull" };
                    }
                    else
                    {
                        return updateWorkStatus;
                    }
                }
                else
                {
                    return SaveWorkAssignment;
                }
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }


		public async Task<dynamic> SaveCallInform(CallInformation ex, UserTocken ut)
		{

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					conn.Open();
					using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_SAVE_APP_WORK_CALL_SHEET", conn))
					{
						cmd.CommandType = System.Data.CommandType.StoredProcedure;
						cmd.Parameters.Add(new OracleParameter("P_WORK_ID", OracleDbType.Varchar2)).Value = ex.P_WORK_ID  ?? (object)DBNull.Value;
						cmd.Parameters.Add(new OracleParameter("P_CALL_ID", OracleDbType.Varchar2)).Value = ex.P_CALL_ID ?? (object)DBNull.Value;
						cmd.Parameters.Add(new OracleParameter("P_EMP_ID", OracleDbType.Varchar2)).Value = ex.P_EMP_ID ?? (object)DBNull.Value;
						cmd.Parameters.Add(new OracleParameter("P_CREATE_USER", OracleDbType.Varchar2)).Value = ut.AUSR_ID;
						cmd.Parameters.Add(new OracleParameter("P_CALL_NOTE", OracleDbType.Varchar2)).Value = ex.P_CALL_NOTE ?? (object)DBNull.Value;
						
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

						return new { Status = 200, Message = "Save Call Information" };
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

        public async Task<dynamic> RejectASsignedWork(long WorkId,long WorkdtlsId)
        {
            try
            {
                var WorkASsigndtlscahnge = await UpdateWorkAssignmentStatusAsync(WorkdtlsId, WorkId, 9);
                if (WorkASsigndtlscahnge.Status == 200) 
                {
                    var workstatuschange = await UpdateWorkStatusAsync(WorkId, 1);
                    if (workstatuschange.Status == 200) {
                        return new { Status = 200, Mesage = "Rejected Successfully" };
                    }
                    else
                    {
                        return workstatuschange;
                    }
                }
                else
                {
                    return WorkASsigndtlscahnge;
                }
            }
            catch (Exception ex)
            {
                return new { Status = 500, Message = ex.Message };
            }
        }

	}
}
