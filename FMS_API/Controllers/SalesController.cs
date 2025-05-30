using FMS_API.Data.Class;
using FMS_API.Repositry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static JwtService;

namespace FMS_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SalesController : Controller
	{
		string fpath;
		private readonly SalesRepositry comrep;
		private readonly JwtHandler jwtHandler;

		public SalesController(SalesRepositry _comrep, JwtHandler _jwthand)
		{
			comrep = _comrep;
			jwtHandler = _jwthand;
		}


		[HttpGet("GetSalesDetails")]
		public async Task<dynamic> GetSalesDetails(string fromDate, string toDate)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetOnlineSaleBreakup(fromDate,toDate);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetSalesDetails: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


        //Controller

        [HttpGet("GetBillwiseSale")]
        public async Task<dynamic> GetBillwiseSale(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetBillwiseSale(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetBillwiseSale: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        
        [HttpGet("GetItemCategorywiseSale")]
        public async Task<dynamic> GetItemCategorywiseSale(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetItemCategorywiseSale(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetItemwiseSale: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetItemwiseSale")]
public async Task<dynamic> GetItemwiseSale(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetItemwiseSale(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetItemwiseSale: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }



		[HttpGet("GetItemwiseMonth")]
		public async Task<dynamic> GetItemwiseMonth(DateTime fromDate, DateTime toDate,string SctId = null)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetItemwiseMonth(fromDate, toDate, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetItemwiseMonth: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		


			[HttpGet("GetItemwiseMonthPivot")]
		public async Task<dynamic> GetItemwiseMonthPivot(DateTime fromDate, DateTime toDate,string MonthList,string SctId = null)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetItemwiseMonthPivot(fromDate, toDate,MonthList, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetCounterwiseCollection: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


		[HttpGet("GetCustomerwiseMonth")]
		public async Task<dynamic> GetCustomerwiseMonth(DateTime fromDate, DateTime toDate, string SctId = null)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetCustomerwiseMonth(fromDate, toDate, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetCustomerwiseMonth: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}




		[HttpGet("GetCustomerwiseMonthPivot")]
		public async Task<dynamic> GetCustomerwiseMonthPivot(DateTime fromDate, DateTime toDate, string MonthList, string SctId = null)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetCustomerwiseMonthPivot(fromDate, toDate, MonthList, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetCustomerwiseMonthPivot: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


		[HttpGet("GetGroupwiseMonth")]
		public async Task<dynamic> GetGroupwiseMonth(DateTime fromDate, DateTime toDate,string SctId = null)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetGroupwiseMonth(fromDate, toDate, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetGroupwiseMonth: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		[HttpGet("GetGroupwiseMonthPivot")]
		public async Task<dynamic> GetGroupwiseMonthPivot(DateTime fromDate, DateTime toDate, string MonthList, string SctId = null)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetGroupwiseMonthPivot(fromDate, toDate, MonthList, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetGroupwiseMonthPivot: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


		[HttpGet("GetCounterwiseCollection")]
        public async Task<dynamic> GetCounterwiseCollection(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetCounterwiseCollection(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetCounterwiseCollection: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetPurchaseSummary")]
        public async Task<dynamic> GetPurchaseSummary(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetPurchaseSummary(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPurchaseSummary: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetPurchasewise")]
    public async Task<dynamic> GetPurchasewise(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetPurchasewise(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPurchasewise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
       


        [HttpGet("GetPurchaseItemwise")]
        public async Task<dynamic> GetPurchaseItemwise(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetPurchaseItemwise(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPurchaseItemwise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }



        [HttpGet("GetPurchaseItemCategorywise")]
        public async Task<dynamic> GetPurchaseItemCategorywise(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetPurchaseItemCategorywise(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPurchaseItemCategorywise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        
                [HttpGet("GetProfitBreakup")]
        public async Task<dynamic> GetProfitBreakup(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetProfitBreakup(fromDate,toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPurchasevendorwise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetStockBreakup")]
        public async Task<dynamic> GetStockBreakup()
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetStockBreakup();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetStockBreakup: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        

             [HttpGet("GetProitItemWise")]
        public async Task<dynamic> GetProitItemWise(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetProitItemWise(fromDate,toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetProitItemWise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetStockItemWise")]
        public async Task<dynamic> GetStockItemWise()
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetStockItemWise();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetStockItemWise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        

        [HttpGet("GetPurchasevendorwise")]
        public async Task<dynamic> GetPurchasevendorwise(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetPurchasevendorwise(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPurchasevendorwise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetVendorWiseProfit")]
        public async Task<dynamic> GetVendorWiseProfit(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetVendorWiseProfit(fromDate,toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetVendorWiseProfit: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }



        [HttpGet("GetOnlineProfitCategoryWise")]
        public async Task<dynamic> GetOnlineProfitCategoryWise(string fromDate, string toDate)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetOnlineProfitCategoryWise(fromDate, toDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetOnlineProfitCategoryWise: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetVendorWiseStock")]
        public async Task<dynamic> GetVendorWiseStock()
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetVendorWiseStock();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetVendorWiseStock: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetCateWiseStock")]
        public async Task<dynamic> GetCateWiseStock()
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetCateWiseStock();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetCateWiseStock: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


		//[HttpGet("GetItemwise")]
		//public async Task<dynamic> GetItemwise(string fromDate, string toDate)
		//{
		//	try
		//	{
		//		// Retrieve token from Authorization header
		//		string authorizationHeader = Request.Headers["Authorization"];

		//		if (string.IsNullOrEmpty(authorizationHeader))
		//		{
		//			return Unauthorized();
		//		}

		//		// Extract token from header (remove "Bearer " prefix)
		//		string token = authorizationHeader.Replace("Bearer ", "");

		//		// Decode token (not decrypt, assuming DecriptTocken is for decoding)
		//		UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

		//		if (decodedToken == null)
		//		{
		//			return Unauthorized();
		//		}

		//		// Validate token
		//		var isValid = await jwtHandler.ValidateToken(token);

		//		if (isValid)
		//		{
		//			// Return user details or appropriate response
		//			//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
		//			return await comrep.GetItemwise(fromDate, toDate);
		//		}
		//		else
		//		{
		//			return Unauthorized();
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log the exception
		//		Console.WriteLine($"Error in GetItemwise: {ex.Message}");
		//		return StatusCode(500, "Internal server error");
		//	}

		//}

		[HttpGet("GetItemwiseDetails")]
		public async Task<dynamic> GetItemwiseDetails(string fromDate, string toDate, string itemid)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetItemwiseDetails(fromDate, toDate, itemid);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetItemwise: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


        //[HttpGet("GetCustomerwise")]
        //public async Task<dynamic> GetCustomerwise(string fromDate, string toDate)
        //{
        //	try
        //	{
        //		// Retrieve token from Authorization header
        //		string authorizationHeader = Request.Headers["Authorization"];

        //		if (string.IsNullOrEmpty(authorizationHeader))
        //		{
        //			return Unauthorized();
        //		}

        //		// Extract token from header (remove "Bearer " prefix)
        //		string token = authorizationHeader.Replace("Bearer ", "");

        //		// Decode token (not decrypt, assuming DecriptTocken is for decoding)
        //		UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

        //		if (decodedToken == null)
        //		{
        //			return Unauthorized();
        //		}

        //		// Validate token
        //		var isValid = await jwtHandler.ValidateToken(token);

        //		if (isValid)
        //		{
        //			// Return user details or appropriate response
        //			//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
        //			return await comrep.GetCustomerwise(fromDate, toDate);
        //		}
        //		else
        //		{
        //			return Unauthorized();
        //		}
        //	}
        //	catch (Exception ex)
        //	{
        //		// Log the exception
        //		Console.WriteLine($"Error in GetCustomerise: {ex.Message}");
        //		return StatusCode(500, "Internal server error");
        //	}

        //}

        [HttpGet("GetCustomerwiseDetails")]
        public async Task<dynamic> GetCustomerwiseDetails(string fromDate, string toDate, string customerid)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetCustomerwiseDetails(fromDate, toDate, customerid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetCustomerwiseDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }



        [HttpGet("GetOnepartyitemwise")]
		public async Task<dynamic> GetOnepartyitemwise(string fromDate, string toDate,string custid, string SctId=null)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetOnepartyitemwise(fromDate, toDate,custid, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetOnepartyitemwise: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		

		[HttpGet("GetOnepartyitemwiseDetails")]
		public async Task<dynamic> GetOnepartyitemwiseDetails(string fromDate, string toDate, string custid,string itemid, string SctId)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetOnepartyitemwiseDetails(fromDate, toDate, custid,itemid, SctId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetOnepartyitemwiseDetails: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


		[HttpGet("GetCustomer")]
		public async Task<dynamic> GetCustomer(DateTime fromDate, DateTime toDate)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetCustomer(fromDate,toDate);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetCustomer: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		[HttpGet("GetItemGroupwise")]
		public async Task<dynamic> GetItemGroupwise(string fromDate, string toDate)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetItemGroupwise(fromDate, toDate);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetItemGroupwise: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		[HttpGet("GetItemGroupwiseDetails")]
		public async Task<dynamic> GetItemGroupwiseDetails(DateTime fromDate, DateTime toDate, string groupid,string monthList)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetItemGroupwiseDetails(fromDate, toDate, groupid, monthList);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetGetItemGroupwiseDetails: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		[HttpGet("GetSalesItemDetails")]
		public async Task<dynamic> GetSalesItemDetails(string fromDate, string toDate, string itemid)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetSalesItemDetails(fromDate, toDate, itemid);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetSalesItemDetails: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


        [HttpGet("GetOneitemAllparties")]
        public async Task<dynamic> GetOneitemAllparties(string fromDate, string toDate, string itemid = null, string sctid = null)
        {
            try
            {
                // Retrieve token from Authorization header
                string authorizationHeader = Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return Unauthorized();
                }

                // Extract token from header (remove "Bearer " prefix)
                string token = authorizationHeader.Replace("Bearer ", "");

                // Decode token (not decrypt, assuming DecriptTocken is for decoding)
                UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {
                    // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.GetOneitemAllparties(fromDate, toDate, itemid, sctid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetOneitemwAllparties: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        //[HttpGet("GetOneitemAllparties")]
        //public async Task<dynamic> GetOneitemAllparties(string fromDate, string toDate, string itemid)
        //{
        //    try
        //    {
        //        // Retrieve token from Authorization header
        //        string authorizationHeader = Request.Headers["Authorization"];

        //        if (string.IsNullOrEmpty(authorizationHeader))
        //        {
        //            return Unauthorized();
        //        }

        //        // Extract token from header (remove "Bearer " prefix)
        //        string token = authorizationHeader.Replace("Bearer ", "");

        //        // Decode token (not decrypt, assuming DecriptTocken is for decoding)
        //        UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

        //        if (decodedToken == null)
        //        {
        //            return Unauthorized();
        //        }

        //        // Validate token
        //        var isValid = await jwtHandler.ValidateToken(token);

        //        if (isValid)
        //        {
        //            // Return user details or appropriate response
        //            //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
        //            return await comrep.GetOneitemAllparties(fromDate, toDate, itemid);
        //        }
        //        else
        //        {
        //            return Unauthorized();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine($"Error in GetOneitemwAllparties: {ex.Message}");
        //        return StatusCode(500, "Internal server error");
        //    }

        //}


        [HttpGet("GetItem")]
		public async Task<dynamic> GetItem(DateTime fromDate, DateTime toDate)
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetItem(fromDate,toDate);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetItem: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		[HttpGet("GetDepartment")]
		public async Task<dynamic> GetDepartment()
		{
			try
			{
				// Retrieve token from Authorization header
				string authorizationHeader = Request.Headers["Authorization"];

				if (string.IsNullOrEmpty(authorizationHeader))
				{
					return Unauthorized();
				}

				// Extract token from header (remove "Bearer " prefix)
				string token = authorizationHeader.Replace("Bearer ", "");

				// Decode token (not decrypt, assuming DecriptTocken is for decoding)
				UserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

				if (decodedToken == null)
				{
					return Unauthorized();
				}

				// Validate token
				var isValid = await jwtHandler.ValidateToken(token);

				if (isValid)
				{
					// Return user details or appropriate response
					//return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
					return await comrep.GetDepartment();
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetCustomer: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


	}
}
