using FMS_API.Class;
using FMS_API.Data.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

//using FMS_API.Repositry;
using static JwtService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace FMS_API.Repositry
{
	public class SalesRepositry
	{
		private readonly AppDbContext _DbContext;

		private readonly JwtHandler jwthand;
		private readonly string _con;

		public SalesRepositry(AppDbContext dbContext, JwtHandler _jwthand)
		{
			_DbContext = dbContext;
			_con = _DbContext.Database.GetConnectionString();
			jwthand = _jwthand;
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
					cmd.Parameters.Add("P_PUNCH_RMKS", OracleDbType.Int32).Value = parameters.PunchRemarks;
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
		public async Task<dynamic> GetSalesDetails(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_SALE_BREAKUP", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate;

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

		public async Task<dynamic> GetOnlineSaleBreakup(string fromDate, string toDate)
		{
			string connString = _con;

			try
			{
				using (OracleConnection conn = new OracleConnection(connString))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_SALE_BREAKUP", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate;

					// Output parameter: ref cursor
					cmd.Parameters.Add("STROUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

					DataTable resultTable = new DataTable();
					await conn.OpenAsync();

					using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
					{
						resultTable.Load(reader);
					}


					return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
				}
			}
			catch (Exception ex)
			{
				// Log the error here if needed (e.g., using ILogger or Console.WriteLine)
				Console.WriteLine("Error in GetOnlineSaleBreakup: " + ex.Message);


				return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
			}
		}



		//Repositry
		public async Task<dynamic> GetBillwiseSale(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_SALE_BILLWISE", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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



		public async Task<dynamic> GetItemCategorywiseSale(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_SALE_ITMCATWISE", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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

		public async Task<dynamic> GetItemwiseSale(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_SALE_ITEMWISE", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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

		public async Task<dynamic> GetProfitBreakup(string fromDate, string toDate)
		{
			using (var conn = new OracleConnection(_con))
			using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_BREAKUP", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
				cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

				// Output cursor parameter
				var outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};

				cmd.Parameters.Add(outputCursor);

				var resultTable = new DataTable();

				try
				{
					conn.Open();

					using (var reader = cmd.ExecuteReader())
					{
						resultTable.Load(reader);
					}
				}
				catch (Exception ex)
				{
					// Handle error (logging, rethrow, etc.)

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}

				//return resultTable;


				return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
			}
		}



		public async Task<dynamic> GetStockBreakup()
		{
			using (var conn = new OracleConnection(_con))
			using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_STOCK_BREAKUP", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				// Output cursor parameter
				var outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};

				cmd.Parameters.Add(outputCursor);

				var resultTable = new DataTable();

				try
				{
					conn.Open();

					using (var reader = cmd.ExecuteReader())
					{
						resultTable.Load(reader);
					}
				}
				catch (Exception ex)
				{
					// Handle error (logging, rethrow, etc.)

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}

				//return resultTable;


				return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
			}
		}



		public async Task<dynamic> GetProitItemWise(string fromDate, string toDate)
		{
			using (var conn = new OracleConnection(_con))
			using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_ITEMWISE", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
				cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

				// Output cursor parameter
				var outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};

				cmd.Parameters.Add(outputCursor);

				var resultTable = new DataTable();

				try
				{
					conn.Open();

					using (var reader = cmd.ExecuteReader())
					{
						resultTable.Load(reader);
					}
				}
				catch (Exception ex)
				{
					// Handle error (logging, rethrow, etc.)

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}

				//return resultTable;


				return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
			}
		}

		public async Task<dynamic> GetStockItemWise()
		{
			using (var conn = new OracleConnection(_con))
			using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_STOCK_ITEMWISE", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				// Output cursor parameter
				var outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};

				cmd.Parameters.Add(outputCursor);

				var resultTable = new DataTable();

				try
				{
					conn.Open();

					using (var reader = cmd.ExecuteReader())
					{
						resultTable.Load(reader);
					}
				}
				catch (Exception ex)
				{
					// Handle error (logging, rethrow, etc.)

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}

				//return resultTable;


				return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
			}
		}




		public async Task<dynamic> GetCounterwiseCollection(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_DAILYCOLLN_SALE", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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


		public async Task<dynamic> GetPurchasewise(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_PUR_BILLWISE", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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

		public async Task<dynamic> GetPurchaseSummary(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_PUR_BREAKUP", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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


		public async Task<dynamic> GetPurchaseItemwise(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_PURCH_ITMWISE", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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


		public async Task<dynamic> GetPurchaseItemCategorywise(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_PURCH_ITMCATWISE", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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

		public async Task<dynamic> GetPurchasevendorwise(string fromDate, string toDate)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_PUR_VENDOR", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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

		public async Task<dynamic> GetOnlineProfitCategoryWise(string fromDate, string toDate)
		{
			using (var connection = new OracleConnection(_con))
			using (var command = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_CATWISE", connection))
			{
				command.CommandType = CommandType.StoredProcedure;

				// Input Parameters
				command.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
				command.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

				// Output parameter: SYS_REFCURSOR
				var outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};
				command.Parameters.Add(outputCursor);

				var dataTable = new DataTable();

				try
				{
					connection.Open();

					using (var reader = command.ExecuteReader())
					{
						dataTable.Load(reader);
					}


					return new DefaultMessage.Message3 { Status = 200, Data = dataTable };
				}
				catch (Exception ex)
				{

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}
			}
		}

		public async Task<dynamic> GetVendorWiseProfit(string fromDate, string toDate)
		{
			using (var conn = new OracleConnection(_con))
			using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_VENDORWISE", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
				cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

				// Output parameter: SYS_REFCURSOR
				var outputParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};
				cmd.Parameters.Add(outputParam);

				var resultTable = new DataTable();

				try
				{
					conn.Open();

					using (var reader = cmd.ExecuteReader())
					{
						resultTable.Load(reader);
					}


					return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
				}
				catch (Exception ex)
				{

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}
			}
		}








		public async Task<dynamic> GetCateWiseStock()
		{
			using (var conn = new OracleConnection(_con))
			using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_STOCK_CATWISE", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				// Output parameter: SYS_REFCURSOR
				var outputParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};
				cmd.Parameters.Add(outputParam);

				var resultTable = new DataTable();

				try
				{
					conn.Open();

					using (var reader = cmd.ExecuteReader())
					{
						resultTable.Load(reader);
					}


					return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
				}
				catch (Exception ex)
				{

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}
			}
		}

		public async Task<dynamic> GetVendorWiseStock()
		{
			using (var conn = new OracleConnection(_con))
			using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_STOCK_VENDORWISE", conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				// Output parameter: SYS_REFCURSOR
				var outputParam = new OracleParameter("STROUT", OracleDbType.RefCursor)
				{
					Direction = ParameterDirection.Output
				};
				cmd.Parameters.Add(outputParam);

				var resultTable = new DataTable();

				try
				{
					conn.Open();

					using (var reader = cmd.ExecuteReader())
					{
						resultTable.Load(reader);
					}


					return new DefaultMessage.Message3 { Status = 200, Data = resultTable };
				}
				catch (Exception ex)
				{

					return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
				}
			}
		}


		//public async Task<dynamic> GetItemwise(string fromDate, string toDate)
		//{
		//	try
		//	{


		//		using (OracleConnection conn = new OracleConnection(_con))
		//		using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ITEM_WISE_MONTHLY", conn))
		//		{
		//			cmd.CommandType = CommandType.StoredProcedure;

		//			// Input Parameters
		//			cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
		//			cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

		//			// Output Parameter (Cursor)
		//			OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
		//			{
		//				Direction = ParameterDirection.Output
		//			};
		//			cmd.Parameters.Add(outputCursor);

		//			// Execute Query
		//			OracleDataAdapter da = new OracleDataAdapter(cmd);
		//			DataTable dt = new DataTable();
		//			da.Fill(dt);

		//			return new DefaultMessage.Message3 { Status = 200, Data = dt };
		//		}
		//	}
		//	catch (Exception ex)
		//	{

		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}

		//}

		public async Task<dynamic> GetItemwise(string fromDate, string toDate)
		{
			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ITEM_WISE_MONTHLY", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2, 20).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("TODATE", OracleDbType.Varchar2, 20).Value = toDate ?? (object)DBNull.Value;

					// Output Parameter (RefCursor)
					OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
					{
						Direction = ParameterDirection.Output
					};
					cmd.Parameters.Add(outputCursor);

					// Open the connection
					await conn.OpenAsync();

					// Read data from cursor
					using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
					{
						DataTable dt = new DataTable();
						dt.Load(reader);

						return new DefaultMessage.Message3 { Status = 200, Data = dt };
					}
				}
			}
			catch (Exception ex)
			{
				return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
			}
		}

		//public async Task<dynamic> GetItemwiseMonth1(string fromDate, string toDate)
		//{
		//	DataTable dataTable = new DataTable();

		//	string sqlQuery = @"
  //      SELECT LISTAGG(''''' || SALE_DATE || '''''', ', ') 
  //      WITHIN GROUP (ORDER BY TO_DATE(SALE_DATE, 'MON-YY')) 
  //      FROM (
  //          SELECT DISTINCT TO_CHAR(SALE_DATE, 'MON-YY') SALE_DATE  
  //          FROM PRMTRANS.INV_SALES 
  //          WHERE SALE_DATE BETWEEN TO_DATE(:fromDate, 'DD/MM/YYYY') AND TO_DATE(:toDate, 'DD/MM/YYYY')
  //      )
  //  ";

		//	try
		//	{
		//		using (var connection = _DbContext.Database.GetDbConnection())
		//		{
		//			await connection.OpenAsync();
		//			using (var command = connection.CreateCommand())
		//			{
		//				command.CommandText = sqlQuery;

		//				var fromParam = command.CreateParameter();
		//				fromParam.ParameterName = "fromDate";
		//				fromParam.Value = fromDate;
		//				command.Parameters.Add(fromParam);

		//				var toParam = command.CreateParameter();
		//				toParam.ParameterName = "toDate";
		//				toParam.Value = toDate;
		//				command.Parameters.Add(toParam);

		//				using (DbDataReader reader = await command.ExecuteReaderAsync())
		//				{
		//					dataTable.Load(reader);
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		return new { Status = 500, Message = "Error: " + ex.Message };
		//	}

		//	return new { Status = 200, Data = dataTable };
		//}


		public async Task<dynamic> GetItemwiseMonth(DateTime fromDate, DateTime toDate,string SctId = null)
		{
			string result = string.Empty;
			string query;

			if (SctId != null)
			{

				query = @"
					SELECT LISTAGG('''' || SALE_DATE || '''', ', ') 
						   WITHIN GROUP (ORDER BY TO_DATE(SALE_DATE, 'MON-YY')) 
					FROM (
						SELECT DISTINCT TO_CHAR(SALE_DATE, 'MON-YY') SALE_DATE  
						FROM PRMTRANS.INV_SALES 
						WHERE SALE_DATE BETWEEN :fromDate AND :toDate and SCT_ID=:SctId
					)";
			}
			else
			{
				query = @"
					SELECT LISTAGG('''' || SALE_DATE || '''', ', ') 
						   WITHIN GROUP (ORDER BY TO_DATE(SALE_DATE, 'MON-YY')) 
					FROM (
						SELECT DISTINCT TO_CHAR(SALE_DATE, 'MON-YY') SALE_DATE  
						FROM PRMTRANS.INV_SALES 
						WHERE SALE_DATE BETWEEN :fromDate AND :toDate
					)";
			}

			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand(query, connection))
					{
						command.BindByName = true;
						command.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						command.Parameters.Add(new OracleParameter("SctId", OracleDbType.Varchar2)).Value = SctId;

						var resultObj = await command.ExecuteScalarAsync();
						if (resultObj != DBNull.Value && resultObj != null)
						{
							result = resultObj.ToString();
						}
						else
						{
							//Console.WriteLine("");
							return new { Status = 404, Message = "Query executed but returned NULL (no matching rows?)" };
						}
					}
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

			return new { Status = 200, Data = result };
		}


		public async Task<dynamic> GetItemwiseMonthPivot(DateTime fromDate, DateTime toDate, string monthList, string SctId = null)
		{
			var resultTable = new DataTable();
			string query;

			if (SctId != null)
			{

				 query = $@"
				SELECT * FROM (
					SELECT 
						D.ITEM_ID,
						ITM.ITEM_NAME,
						TO_CHAR(M.SALE_DATE, 'MON-YY') AS MONTH,
						D.SLDT_NET_AMT
					FROM PRMTRANS.INV_SALES_DTLS D
					JOIN PRMTRANS.INV_SALES M ON M.SALE_ID = D.SALE_ID
					JOIN PRMMASTER.INV_ITEM ITM ON D.ITEM_ID = ITM.ITEM_ID
					WHERE NVL(M.SALE_CANCEL, 'N') = 'N'
					AND M.SALE_DATE BETWEEN :fromDate AND :toDate AND M.SCT_ID=:SctId
				)
				PIVOT (
					SUM(SLDT_NET_AMT) FOR MONTH IN ({monthList})
				)
				ORDER BY ITEM_NAME";
			}
			else
			{
				 query = $@"
				SELECT * FROM (
					SELECT 
						D.ITEM_ID,
						ITM.ITEM_NAME,
						TO_CHAR(M.SALE_DATE, 'MON-YY') AS MONTH,
						D.SLDT_NET_AMT
					FROM PRMTRANS.INV_SALES_DTLS D
					JOIN PRMTRANS.INV_SALES M ON M.SALE_ID = D.SALE_ID
					JOIN PRMMASTER.INV_ITEM ITM ON D.ITEM_ID = ITM.ITEM_ID
					WHERE NVL(M.SALE_CANCEL, 'N') = 'N'
					AND M.SALE_DATE BETWEEN :fromDate AND :toDate 
				)
				PIVOT (
					SUM(SLDT_NET_AMT) FOR MONTH IN ({monthList})
				)
				ORDER BY ITEM_NAME";
			}

			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand(query, connection))
					{
						command.BindByName = true;

						command.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						command.Parameters.Add(new OracleParameter("SctId", OracleDbType.Varchar2)).Value = SctId;
						//command.Parameters.Add(new OracleParameter("monthslist", OracleDbType.Varchar2)).Value = monthList;

						using (var reader = await command.ExecuteReaderAsync())
						{
							resultTable.Load(reader);
						}
					}
				}
			}
			catch (OracleException ex)
			{
				Console.WriteLine("OracleException: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}

			return new { Status = 200, Data = resultTable };
		}



		public async Task<dynamic> GetItemwiseDetails(string fromDate, string toDate, string itemid)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ITEM_WISE_DETAILED", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRITEM_FILTER", OracleDbType.Varchar2).Value = itemid ?? (object)DBNull.Value;

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

		public async Task<dynamic> GetCustomerwiseMonth(DateTime fromDate, DateTime toDate, string SctId = null)
		{
			string result = string.Empty;
			string query;
			if (SctId != null)
			{
				query = @"
				SELECT LISTAGG('''' || SALE_DATE || '''', ', ') 
					   WITHIN GROUP (ORDER BY TO_DATE(SALE_DATE, 'MON-YY')) 
				FROM (
					SELECT DISTINCT TO_CHAR(SALE_DATE, 'MON-YY') SALE_DATE  
					FROM PRMTRANS.INV_SALES 
					WHERE SALE_DATE BETWEEN :fromDate AND :toDate AND  SCT_ID=:SctId
				)";
			}
			else
			{
				query = @"
				SELECT LISTAGG('''' || SALE_DATE || '''', ', ') 
					   WITHIN GROUP (ORDER BY TO_DATE(SALE_DATE, 'MON-YY')) 
				FROM (
					SELECT DISTINCT TO_CHAR(SALE_DATE, 'MON-YY') SALE_DATE  
					FROM PRMTRANS.INV_SALES 
					WHERE SALE_DATE BETWEEN :fromDate AND :toDate 
				)";
			}

			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand(query, connection))
					{
						command.BindByName = true;
						command.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						command.Parameters.Add(new OracleParameter("SctId", OracleDbType.Varchar2)).Value = SctId;

						var resultObj = await command.ExecuteScalarAsync();
						if (resultObj != DBNull.Value && resultObj != null)
						{
							result = resultObj.ToString();
						}
						else
						{
							//Console.WriteLine("");
							return new { Status = 404, Message = "Query executed but returned NULL (no matching rows?)" };
						}
					}
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

			return new { Status = 200, Data = result };
		}


		public async Task<dynamic> GetCustomerwiseMonthPivot(DateTime fromDate, DateTime toDate, string monthList, string SctId = null)
		{
			var resultTable = new DataTable();


			string query;
			if (SctId != null)
			{
				 query = $@"SELECT * FROM (
				SELECT CUST_ID, CUST_NAME, MONTH, SUM(SALE_NET_AMT) AMOUNT
				FROM (
					SELECT UPPER(TO_CHAR(S.SALE_DATE, 'MON-YY', 'NLS_DATE_LANGUAGE=ENGLISH')) MONTH,
						   C.CUST_NAME, C.CUST_ID, S.SALE_NET_AMT
					FROM PRMTRANS.INV_SALES S
					INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = S.SALE_CUSTOMER_ID
					WHERE NVL(SALE_CANCEL, 'N') = 'N'
					  AND S.SALE_DATE >= :fromDate
					  AND S.SALE_DATE <= :toDate AND S.SCT_ID=:SctId
				)
				GROUP BY CUST_ID, MONTH, CUST_NAME
				)
				PIVOT (
					SUM(AMOUNT) FOR MONTH IN ({monthList})
				)
				ORDER BY CUST_NAME";
			}
			else
			{
				query = $@"SELECT * FROM (
				SELECT CUST_ID, CUST_NAME, MONTH, SUM(SALE_NET_AMT) AMOUNT
				FROM (
					SELECT UPPER(TO_CHAR(S.SALE_DATE, 'MON-YY', 'NLS_DATE_LANGUAGE=ENGLISH')) MONTH,
						   C.CUST_NAME, C.CUST_ID, S.SALE_NET_AMT
					FROM PRMTRANS.INV_SALES S
					INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = S.SALE_CUSTOMER_ID
					WHERE NVL(SALE_CANCEL, 'N') = 'N'
					  AND S.SALE_DATE >= :fromDate
					  AND S.SALE_DATE <= :toDate 
				)
				GROUP BY CUST_ID, MONTH, CUST_NAME
				)
				PIVOT (
					SUM(AMOUNT) FOR MONTH IN ({monthList})
				)
				ORDER BY CUST_NAME";
			}
			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand(query, connection))
					{
						command.BindByName = true;

						command.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						command.Parameters.Add(new OracleParameter("SctId", OracleDbType.Varchar2)).Value = SctId;
						//command.Parameters.Add(new OracleParameter("monthslist", OracleDbType.Varchar2)).Value = monthList;

						using (var reader = await command.ExecuteReaderAsync())
						{
							resultTable.Load(reader);
						}
					}
				}
			}
			catch (OracleException ex)
			{
				Console.WriteLine("OracleException: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}

			return new { Status = 200, Data = resultTable };
		}


		public async Task<dynamic> GetGroupwiseMonth(DateTime fromDate, DateTime toDate,string SctId = null)
		{
			string result = string.Empty;
			string query;
			if (SctId != null)
			{
				query = @"
				SELECT LISTAGG('''' || SALE_DATE || '''', ', ') 
					   WITHIN GROUP (ORDER BY TO_DATE(SALE_DATE, 'MON-YY')) 
				FROM (
					SELECT DISTINCT TO_CHAR(SALE_DATE, 'MON-YY') SALE_DATE  
					FROM PRMTRANS.INV_SALES 
					WHERE SALE_DATE BETWEEN :fromDate AND :toDate  AND  SCT_ID=:SctId
				)";
			}
			else
			{
				query = @"
				SELECT LISTAGG('''' || SALE_DATE || '''', ', ') 
					   WITHIN GROUP (ORDER BY TO_DATE(SALE_DATE, 'MON-YY')) 
				FROM (
					SELECT DISTINCT TO_CHAR(SALE_DATE, 'MON-YY') SALE_DATE  
					FROM PRMTRANS.INV_SALES 
					WHERE SALE_DATE BETWEEN :fromDate AND :toDate
				)";
			}
			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand(query, connection))
					{
						command.BindByName = true;
						command.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						command.Parameters.Add(new OracleParameter("SctId", OracleDbType.Varchar2)).Value = SctId;

						var resultObj = await command.ExecuteScalarAsync();
						if (resultObj != DBNull.Value && resultObj != null)
						{
							result = resultObj.ToString();
						}
						else
						{
							//Console.WriteLine("");
							return new { Status = 404, Message = "Query executed but returned NULL (no matching rows?)" };
						}
					}
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

			return new { Status = 200, Data = result };
		}

		public async Task<dynamic> GetGroupwiseMonthPivot(DateTime fromDate, DateTime toDate, string monthList, string SctId = null)
		{
			var resultTable = new DataTable();


			string query;
			if (SctId != null)
			{
					query = $@"
				SELECT * FROM (
					SELECT ITGRP_ID, ITGRP_NAME AS ITEM_GROUP, MONTH, SUM(SLDT_NET_AMT) AS AMOUNT  
					FROM (
						SELECT 
							TO_CHAR(M.SALE_DATE, 'MON-YY') AS MONTH,
							ITM.ITGRP_ID,
							G.ITGRP_NAME,
							D.ITEM_ID,
							SLDT_QTY,
							SLDT_NET_AMT 
						FROM PRMTRANS.INV_SALES_DTLS D 
						INNER JOIN PRMTRANS.INV_SALES M ON M.SALE_ID = D.SALE_ID 
						INNER JOIN PRMMASTER.INV_ITEM ITM ON D.ITEM_ID = ITM.ITEM_ID
						INNER JOIN PRMMASTER.INV_ITEM_GROUP G ON G.ITGRP_ID = ITM.ITGRP_ID
						WHERE NVL(M.SALE_CANCEL, 'N') = 'N'
						  AND M.SALE_DATE >= :fromDate
						  AND M.SALE_DATE <= :toDate AND M.SCT_ID=:SctId
					)
					GROUP BY MONTH, ITGRP_ID, ITGRP_NAME
				)
				PIVOT (
					SUM(AMOUNT) FOR MONTH IN ({monthList})
				)
				ORDER BY ITEM_GROUP";
			}
			else
			{
				query = $@"
			SELECT * FROM (
				SELECT ITGRP_ID, ITGRP_NAME AS ITEM_GROUP, MONTH, SUM(SLDT_NET_AMT) AS AMOUNT  
				FROM (
					SELECT 
						TO_CHAR(M.SALE_DATE, 'MON-YY') AS MONTH,
						ITM.ITGRP_ID,
						G.ITGRP_NAME,
						D.ITEM_ID,
						SLDT_QTY,
						SLDT_NET_AMT 
					FROM PRMTRANS.INV_SALES_DTLS D 
					INNER JOIN PRMTRANS.INV_SALES M ON M.SALE_ID = D.SALE_ID 
					INNER JOIN PRMMASTER.INV_ITEM ITM ON D.ITEM_ID = ITM.ITEM_ID
					INNER JOIN PRMMASTER.INV_ITEM_GROUP G ON G.ITGRP_ID = ITM.ITGRP_ID
					WHERE NVL(M.SALE_CANCEL, 'N') = 'N'
					  AND M.SALE_DATE >= :fromDate
					  AND M.SALE_DATE <= :toDate
				)
				GROUP BY MONTH, ITGRP_ID, ITGRP_NAME
			)
			PIVOT (
				SUM(AMOUNT) FOR MONTH IN ({monthList})
			)
			ORDER BY ITEM_GROUP";
			}

			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand(query, connection))
					{
						command.BindByName = true;

						command.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						command.Parameters.Add(new OracleParameter("SctId", OracleDbType.Varchar2)).Value = SctId;
						//command.Parameters.Add(new OracleParameter("monthslist", OracleDbType.Varchar2)).Value = monthList;

						using (var reader = await command.ExecuteReaderAsync())
						{
							resultTable.Load(reader);
						}
					}
				}
			}
			catch (OracleException ex)
			{
				Console.WriteLine("OracleException: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}

			return new { Status = 200, Data = resultTable };
		}


		//public async Task<dynamic> GetCustomerwise(string fromDate, string toDate)
		//{
		//	try
		//	{


		//		using (OracleConnection conn = new OracleConnection(_con))
		//		using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_CUSTOMER_WISE_MONTHLY", conn))
		//		{
		//			cmd.CommandType = CommandType.StoredProcedure;

		//			// Input Parameters
		//			cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
		//			cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

		//			// Output Parameter (Cursor)
		//			OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
		//			{
		//				Direction = ParameterDirection.Output
		//			};
		//			cmd.Parameters.Add(outputCursor);

		//			// Execute Query
		//			OracleDataAdapter da = new OracleDataAdapter(cmd);
		//			DataTable dt = new DataTable();
		//			da.Fill(dt);

		//			return new DefaultMessage.Message3 { Status = 200, Data = dt };
		//		}
		//	}
		//	catch (Exception ex)
		//	{

		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}

		//}

		public async Task<dynamic> GetCustomerwiseDetails(string fromDate, string toDate, string customerid)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_CUSTOMER_WISE_DETAILED", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRITEM_FILTER", OracleDbType.Varchar2).Value = customerid ?? (object)DBNull.Value;

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


		//public async Task<dynamic> GetOnepartyitemwise(string fromDate, string toDate,string custid,string SctId=null)
		//{
		//	try
		//	{


		//		using (OracleConnection conn = new OracleConnection(_con))
		//		using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ONE_PARTY_ALL_ITEMS", conn))
		//		{
		//			cmd.CommandType = CommandType.StoredProcedure;

		//			// Input Parameters
		//			cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
		//			cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
		//			cmd.Parameters.Add("STRCUST_FILTER", OracleDbType.Varchar2).Value = custid ?? (object)DBNull.Value;
		//			cmd.Parameters.Add("STRBRANCH_FILTER", OracleDbType.Varchar2).Value = SctId ?? (object)DBNull.Value;
		//			// Output Parameter (Cursor)
		//			OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
		//			{
		//				Direction = ParameterDirection.Output
		//			};
		//			cmd.Parameters.Add(outputCursor);

		//			// Execute Query
		//			OracleDataAdapter da = new OracleDataAdapter(cmd);
		//			DataTable dt = new DataTable();
		//			da.Fill(dt);

		//			return new DefaultMessage.Message3 { Status = 200, Data = dt };
		//		}
		//	}
		//	catch (Exception ex)
		//	{

		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}

		//}

		public async Task<dynamic> GetOnepartyitemwise(string fromDate, string toDate, string custid , string SctId=null )
		{
			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ONE_PARTY_ALL_ITEMS", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? string.Empty;
					cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? string.Empty;
					cmd.Parameters.Add("STRCUST_FILTER", OracleDbType.Varchar2).Value = custid ?? string.Empty;
					//cmd.Parameters.Add("STRCUST_FILTER", OracleDbType.Varchar2).Value = custid ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRBRANCH_FILTER", OracleDbType.Varchar2).Value = SctId ?? (object)DBNull.Value;


					// Output Parameter (Cursor)
					OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
					{
						Direction = ParameterDirection.Output
					};
					cmd.Parameters.Add(outputCursor);

					// Open connection
					await conn.OpenAsync();

					// Execute and fetch data
					using (OracleDataAdapter da = new OracleDataAdapter(cmd))
					{
						DataTable dt = new DataTable();
						da.Fill(dt);

						return new DefaultMessage.Message3 { Status = 200, Data = dt };
					}
				}
			}
			catch (Exception ex)
			{
				return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
			}
		}


		public async Task<dynamic> GetOnepartyitemwiseDetails(string fromDate, string toDate, string custid,string itemid,string SctId)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ITEM_PARTY_SALES_DTLS", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRITEM_FILTER", OracleDbType.Varchar2).Value = itemid ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRCUST_FILTER", OracleDbType.Varchar2).Value = custid ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRBRANCH_FILTER", OracleDbType.Varchar2).Value = SctId ?? (object)DBNull.Value;

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


		//public async Task<dynamic> GetCustomer(string fromDate, string toDate)
		//{
		//	List<dynamic> customers = new List<dynamic>();

		//	try
		//	{
		//		using (OracleConnection conn = new OracleConnection(_con))
		//		{
		//			await conn.OpenAsync();

		//			//string query = @"
		//			//           select distinct CUST_ID,CUST_NAME  from PRMMASTER.GEN_CUSTOMERS where ACTIVE_STATUS='A'";
		//			string query = @"
		//             SELECT distinct CUST_ID,CUST_NAME
		//		 FROM  PRMTRANS.INV_SALES_DTLS D
		//		INNER JOIN PRMTRANS.INV_SALES S ON S.SALE_ID=D.SALE_ID
		//		INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID=S.SALE_CUSTOMER_ID
		//		WHERE TO_DATE(SALE_DATE)>=TO_DATE(:fromDate,'DD/MM/YYYY')
		//		AND TO_DATE(SALE_DATE)<=TO_DATE(:toDate,'DD/MM/YYYY')";

		//			using (OracleCommand cmd = new OracleCommand(query, conn))
		//			{

		//				using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
		//				{
		//					while (await reader.ReadAsync())
		//					{
		//						customers.Add(new
		//						{
		//							CUST_ID = reader.GetInt32(0),
		//							CUST_NAME = reader.GetString(1)
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

		//	return new DefaultMessage.Message3 { Status = 200, Data = customers };
		//}


		public async Task<dynamic> GetCustomer(DateTime fromDate, DateTime toDate)
		{
			List<dynamic> customers = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					await conn.OpenAsync();

					string query = @"
				SELECT DISTINCT C.CUST_ID, C.CUST_NAME
				FROM PRMTRANS.INV_SALES_DTLS D
				INNER JOIN PRMTRANS.INV_SALES S ON S.SALE_ID = D.SALE_ID
				INNER JOIN PRMMASTER.GEN_CUSTOMERS C ON C.CUST_ID = S.SALE_CUSTOMER_ID
				WHERE S.SALE_DATE>=:fromDate
				AND S.SALE_DATE<= :toDate";
					using (OracleCommand cmd = new OracleCommand(query, conn))
					{

						cmd.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						cmd.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								customers.Add(new
								{
									CUST_ID = reader.GetInt32(0),
									CUST_NAME = reader.GetString(1)
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

			return new DefaultMessage.Message3 { Status = 200, Data = customers };
		}


		public async Task<dynamic> GetItemGroupwise(string fromDate, string toDate)
		{
			try
			{




				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ITEM_GROUP_MONTHLY", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;

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

		//public async Task<dynamic> GetItemGroupwiseDetails(string fromDate, string toDate, string groupid)
		//{
		//	try
		//	{


		//		using (OracleConnection conn = new OracleConnection(_con))
		//		using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ITEM_GROUP_DETAILED", conn))
		//		{
		//			cmd.CommandType = CommandType.StoredProcedure;

		//			// Input Parameters
		//			cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
		//			cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
		//			cmd.Parameters.Add("STRITEMGRP_FILTER", OracleDbType.Varchar2).Value = groupid ?? (object)DBNull.Value;

		//			// Output Parameter (Cursor)
		//			OracleParameter outputCursor = new OracleParameter("STROUT", OracleDbType.RefCursor)
		//			{
		//				Direction = ParameterDirection.Output
		//			};
		//			cmd.Parameters.Add(outputCursor);

		//			// Execute Query
		//			OracleDataAdapter da = new OracleDataAdapter(cmd);
		//			DataTable dt = new DataTable();
		//			da.Fill(dt);

		//			return new DefaultMessage.Message3 { Status = 200, Data = dt };
		//		}
		//	}
		//	catch (Exception ex)
		//	{

		//		return new DefaultMessage.Message1 { Status = 500, Message = ex.Message };
		//	}

		//}




		public async Task<dynamic> GetItemGroupwiseDetails(DateTime fromDate, DateTime toDate, string groupid, string monthList)
		{
			var resultTable = new DataTable();


			//string query = $@"SELECT * FROM (SELECT ITGRP_ID,ITGRP_NAME ITEM_GROUP,ITEM_NAME,ITEM_ID,MONTH,SUM(SLDT_NET_AMT) AMOUNT  FROM(
			//SELECT TO_CHAR(M.SALE_DATE,''MON'') MONTH,ITM.ITGRP_ID,ITGRP_NAME,ITEM_NAME,D.ITEM_ID,SLDT_NET_AMT FROM PRMTRANS.INV_SALES_DTLS D 
			//INNER JOIN PRMTRANS.INV_SALES M ON M.SALE_ID=D.SALE_ID 
			//INNER JOIN PRMMASTER.INV_ITEM ITM ON D.ITEM_ID=ITM.ITEM_ID
			//INNER JOIN PRMMASTER.INV_ITEM_GROUP G ON G.ITGRP_ID=ITM.ITGRP_ID
			//WHERE NVL(M.SALE_CANCEL, 'N') = 'N'
			//		  AND M.SALE_DATE >= :fromDate
			//		  AND M.SALE_DATE <= :toDate AND ITM.ITGRP_ID IN ('||STRITEMGRP_FILTER||')
			//) GROUP BY MONTH ,ITGRP_ID,ITGRP_NAME,ITEM_NAME,ITEM_ID  ORDER BY ITGRP_NAME,MONTH
			//)PIVOT (SUM(AMOUNT) FOR MONTH  IN ({monthList})) ORDER BY  ITEM_GROUP,ITEM_NAME
			//";

			string query = $@"
SELECT * FROM (
    SELECT 
        G. ITGRP_ID,
        G.ITGRP_NAME AS ITEM_GROUP,
        ITM.ITEM_NAME,
        D.ITEM_ID,
        TO_CHAR(M.SALE_DATE, 'MON-YY') AS MONTH,
        SUM(SLDT_NET_AMT) AS AMOUNT  
    FROM PRMTRANS.INV_SALES_DTLS D 
    INNER JOIN PRMTRANS.INV_SALES M ON M.SALE_ID = D.SALE_ID 
    INNER JOIN PRMMASTER.INV_ITEM ITM ON D.ITEM_ID = ITM.ITEM_ID
    INNER JOIN PRMMASTER.INV_ITEM_GROUP G ON G.ITGRP_ID = ITM.ITGRP_ID
    WHERE NVL(M.SALE_CANCEL, 'N') = 'N'
      AND M.SALE_DATE >= :fromDate
      AND M.SALE_DATE <= :toDate 
      AND ITM.ITGRP_ID IN ({groupid})
    GROUP BY 
        TO_CHAR(M.SALE_DATE, 'MON-YY'), 
         G.ITGRP_ID, 
        G.ITGRP_NAME, 
        ITM.ITEM_NAME, 
        D.ITEM_ID
)
PIVOT (
    SUM(AMOUNT) 
    FOR MONTH IN ({monthList})
)
ORDER BY ITEM_GROUP, ITEM_NAME";



			try
			{
				using (var connection = new OracleConnection(_con))
				{
					await connection.OpenAsync();

					using (var command = new OracleCommand(query, connection))
					{
						command.BindByName = true;

						command.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;
						//command.Parameters.Add("groupid", OracleDbType.Varchar2).Value = groupid ?? (object)DBNull.Value;
						//command.Parameters.Add(new OracleParameter("monthslist", OracleDbType.Varchar2)).Value = monthList;

						using (var reader = await command.ExecuteReaderAsync())
						{
							resultTable.Load(reader);
						}
					}
				}
			}
			catch (OracleException ex)
			{
				Console.WriteLine("OracleException: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.Message);
				return new { Status = 500, Message = ex.Message };
			}

			return new { Status = 200, Data = resultTable };
		}



	
		public async Task<dynamic> GetSalesItemDetails(string fromDate, string toDate, string itemid)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ITEM_SALES_DTLS", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRITEM_FILTER", OracleDbType.Varchar2).Value = itemid ?? (object)DBNull.Value;

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

		public async Task<dynamic> GetOneitemAllparties(string fromDate, string toDate, string itemid)
		{
			try
			{


				using (OracleConnection conn = new OracleConnection(_con))
				using (OracleCommand cmd = new OracleCommand("PRMTRANS.SP_ONLN_ONE_ITEM_ALL_PARTIES", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					// Input Parameters
					cmd.Parameters.Add("FROMDATE", OracleDbType.Varchar2).Value = fromDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = toDate ?? (object)DBNull.Value;
					cmd.Parameters.Add("STRITEM_FILTER", OracleDbType.Varchar2).Value = itemid ?? (object)DBNull.Value;
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

		//public async Task<dynamic> GetItem()
		//{
		//	List<dynamic> customers = new List<dynamic>();

		//	try
		//	{
		//		using (OracleConnection conn = new OracleConnection(_con))
		//		{
		//			await conn.OpenAsync();

		//			string query = @"
		//              select ITEM_ID,ITEM_NAME from PRMMASTER.INV_ITEM where ACTIVE_STATUS='A'";

		//			using (OracleCommand cmd = new OracleCommand(query, conn))
		//			{

		//				using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
		//				{
		//					while (await reader.ReadAsync())
		//					{
		//						customers.Add(new
		//						{
		//							ITEM_ID = reader.GetInt32(0),
		//							ITEM_NAME = reader.GetString(1)
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

		//	return new DefaultMessage.Message3 { Status = 200, Data = customers };
		//}


		public async Task<dynamic> GetItem(DateTime fromDate, DateTime toDate)
		{
			List<dynamic> customers = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					await conn.OpenAsync();

					string query = @"
				 SELECT distinct D.ITEM_ID,ITM. ITEM_NAME
				 FROM  PRMTRANS.INV_SALES_DTLS D
				INNER JOIN PRMTRANS.INV_SALES S ON S.SALE_ID=D.SALE_ID
				INNER JOIN PRMMASTER.INV_ITEM ITM ON D.ITEM_ID=ITM.ITEM_ID
				WHERE S.SALE_DATE>=:fromDate
				AND S.SALE_DATE<= :toDate";

					using (OracleCommand cmd = new OracleCommand(query, conn))
					{
						cmd.Parameters.Add(new OracleParameter("fromDate", OracleDbType.Date)).Value = fromDate;
						cmd.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = toDate;

						using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								customers.Add(new
								{
									ITEM_ID = reader.GetInt32(0),
									ITEM_NAME = reader.GetString(1)
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

			return new DefaultMessage.Message3 { Status = 200, Data = customers };
		}

		public async Task<dynamic> GetDepartment()
		{
			List<dynamic> customers = new List<dynamic>();

			try
			{
				using (OracleConnection conn = new OracleConnection(_con))
				{
					await conn.OpenAsync();

					string query = @"
				select SCT_ID,SCT_NAME from PRMMASTER.HRM_DEPARTMENT_SECTIONS where ACTIVE_STATUS='A'";
					using (OracleCommand cmd = new OracleCommand(query, conn))
					{

						
						using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								customers.Add(new
								{
									SCT_ID = reader.GetInt32(0),
									SCT_NAME = reader.GetString(1)
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

			return new DefaultMessage.Message3 { Status = 200, Data = customers };
		}


	}


}

