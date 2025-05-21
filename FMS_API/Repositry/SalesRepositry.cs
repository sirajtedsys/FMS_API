using FMS_API.Class;
using FMS_API.Data.Class;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;
//using FMS_API.Repositry;
using static JwtService;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace FMS_API.Repositry
{
	public class SalesRepositry
	{
		private readonly AppDbContext _DbContext;

		private readonly JwtHandler jwthand;
		private readonly string _con;

		public SalesRepositry( AppDbContext dbContext, JwtHandler _jwthand)
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
					cmd.Parameters.Add("STRFROMDATE", OracleDbType.Varchar2).Value = fromDate ;
					cmd.Parameters.Add("STRTODATE", OracleDbType.Varchar2).Value = toDate ;
					
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

        public async Task<dynamic> GetProfitBreakup()
        {
            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_BREAKUP", conn))
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

        

              public async Task<dynamic> GetProitItemWise()
        {
            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_ITEMWISE", conn))
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

        public async Task<dynamic> GetOnlineProfitCategoryWise()
        {
            using (var connection = new OracleConnection(_con))
            using (var command = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_CATWISE", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

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

        public async Task<dynamic> GetVendorWiseProfit()
        {
            using (var conn = new OracleConnection(_con))
            using (var cmd = new OracleCommand("PRMTRANS.SP_ONLN_PROFIT_VENDORWISE", conn))
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


    }


}

