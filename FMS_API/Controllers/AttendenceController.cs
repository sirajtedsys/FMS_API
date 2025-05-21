using FMS_API.Data.Class;
using FMS_API.Repositry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using static JwtService;

namespace FMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendenceController : ControllerBase
    {
        private readonly AttendenceRepositry comrep;
        private readonly JwtHandler jwtHandler;

        public AttendenceController(AttendenceRepositry _comrep, JwtHandler _jwthand)
        {
            comrep = _comrep;
            jwtHandler = _jwthand;
        }



        [HttpPost("GetPunchDetails")]
        public async Task<dynamic> GetPunchDetails(PunchDetails exp)
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
                    return await comrep.GetPunchDetails(exp);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPunchDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


  //      [HttpPost("SavePunchRemarks")]
		//public async Task<dynamic> SavePunchRemarks(PunchDetails parameters)
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
		//			return await comrep.SavePunchRemarks(parameters);
		//		}
		//		else
		//		{
		//			return Unauthorized();
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log the exception
		//		Console.WriteLine($"Error in Save Punch Remarks: {ex.Message}");
		//		return StatusCode(500, "Internal server error");
		//	}

		//}


                [HttpGet("GetEmployeePunchDetails")]
        public async Task<dynamic> GetEmployeePunchDetails(long empid, DateTime datee)
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
                    return await comrep.GetEmployeePunchDetails(empid ,datee);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetEmployeePunchDetails(empid ,datee): {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetOpenedWorkOfEmpkoyee")]
        public async Task<dynamic> GetOpenedWorkOfEmpkoyee(long empid)
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
                    return await comrep.GetOpenedWorkOfEmpkoyee(empid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetWorkAssignments(empid ,datee): {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

       

        [HttpGet("GetWorkAssignments")]
        public async Task<dynamic> GetWorkAssignments(long empid)
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
                    return await comrep.GetWorkAssignments(empid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetWorkAssignments(empid ,datee): {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }



		[HttpGet("GetAttendenceReport")]
		public async Task<dynamic> GetAttendenceReport(DateTime strFDate, DateTime strTDate, string Empid)
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
					return await comrep.GetAttendenceReport(strFDate, strTDate, long.Parse(decodedToken.AUSR_ID));
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetAttendenceReport(empid ,datee): {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


		[HttpGet("GetAttenReport")]
		public async Task<dynamic> GetAttenReport(DateTime strFDate, DateTime strTDate, string Empid, bool bltype)
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
					return await comrep.GetAttenReport(strFDate, strTDate, long.Parse(decodedToken.AUSR_ID), bltype);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetAttendenceReport(empid ,datee): {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}





		[HttpPost("SaveAppDailyWorkSheetAsync")]
        public async Task<dynamic> SaveAppDailyWorkSheetAsync(DailyWorkSheet exp)
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
                    return await comrep.SaveAppDailyWorkSheetAsync(exp,decodedToken );
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPunchDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost("CloseCallFromCAllList")]
        public async Task<dynamic> CloseCallFromCAllList(DailyWorkSheet exp)
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
                    return await comrep.CloseCallFromCAllList(exp, decodedToken);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPunchDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        

  [HttpGet("DeleteDailyWorkSheetAsync")]
        public async Task<dynamic> DeleteDailyWorkSheetAsync(string WorkSheetId)
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
                    return await comrep.DeleteDailyWorkSheetAsync(WorkSheetId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DeleteDailyWorkSheetAsync(empid ,datee): {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpPost("ReassignTonewEmployeeFromInbox")]
        public async Task<dynamic> ReassignTonewEmployeeFromInbox(DailyWorkSheet exp)
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
                    return await comrep.ReassignTonewEmployeeFromInbox(exp, decodedToken);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in ReassignTonewEmployeeFromInbox: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetAttach")]
        public async Task<dynamic> GetAttach(string workid)
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
                    return await comrep.GetAttach(workid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAttach: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }



        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "ProjectDocumentUpload");

        [HttpGet("{*fullPath}")]
        public IActionResult GetFile(string fullPath)
        {

            fullPath = fullPath.Replace("//", "/");
            // Remove 'ProjectDocumentUpload/' if it exists at the start of the path from the frontend
            if (fullPath.StartsWith("ProjectDocumentUpload/"))
            {

                fullPath = fullPath.Substring("ProjectDocumentUpload/".Length);
            }

            // Now combine the cleaned-up path with the base storage path
            var filePath = Path.Combine(_storagePath, fullPath);

            // Ensure the file exists before serving it
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Get MIME type based on file extension
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream"; // Default for unknown types
            }

            return PhysicalFile(filePath, contentType);
        }




        [HttpGet("GetEmployeePunchDet")]
            public async Task<dynamic> GetEmployeePunchDet(long empId, DateTime punchStartDate, DateTime punchEndDate)
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
                    return await comrep.GetEmployeePunchDet(empId, punchStartDate, punchEndDate);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAttendenceReport(empid ,datee): {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

		//[HttpGet("GetEmployeeWorkdasboard")]
		//public async Task<dynamic> GetEmployeeWorkdasboard(long DepartmentId)
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
		//			return await comrep.GetEmployeeWorkdasboard(DepartmentId);
		//		}
		//		else
		//		{
		//			return Unauthorized();
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log the exception
		//		Console.WriteLine($"Error in GetEmployeeWorkdasboard: {ex.Message}");
		//		return StatusCode(500, "Internal server error");
		//	}

		//}

		[HttpGet("GetEmployeeWorkdasboarddet")]
		public async Task<dynamic> GetEmployeeWorkdasboarddet(string empid,int id)
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
					return await comrep.GetEmployeeWorkdasboarddet(empid,id);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in GetEmployeeWorkdasboard: {ex.Message}");
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
				Console.WriteLine($"Error in GetModules: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


        [HttpGet("GetEmployeeWorkdasboard")]
        public async Task<dynamic> GetEmployeeWorkdasboard(long DepartmentId, long empid)
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
                    return await comrep.GetEmployeeWorkdasboard(DepartmentId, decodedToken.AUSR_ID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetEmployeeWorkdasboard: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet("GetWorkStatus")]
        public async Task<dynamic> GetWorkStatus()
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
                    return await comrep.GetWorkStatus();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in GetModules: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetEmployeeWorkdetails")]
        public async Task<dynamic> GetEmployeeWorkdetails(string EmployeeId)
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
                    return await comrep.GetEmployeeWorkdetails(decodedToken.AUSR_ID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetEmployeeWorkdeails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet("GetEmployeeWorkdetailsEmp")]
        public async Task<dynamic> GetEmployeeWorkdetailsEmp(string EmployeeId, string empid)
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
                    return await comrep.GetEmployeeWorkdetailsEmp(decodedToken.AUSR_ID, empid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetEmployeeWorkdeails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetEmployeeWorkdetailsEmpStatus")]
        public async Task<dynamic> GetEmployeeWorkdetailsEmpStatus(string EmployeeId, string empid, string workstatusid)
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
                    return await comrep.GetEmployeeWorkdetailsEmpStatus(decodedToken.AUSR_ID, long.Parse(empid), workstatusid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetEmployeeWorkdeails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


		

			 [HttpPost("SavePunchRemarks")]
		public async Task<dynamic> SavePunchRemarks(PunchDetails parameters)
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
					return await comrep.SavePunchRemarks(parameters);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in Save Punch Remarks: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

        [HttpGet("Getprojectclientlist")]
        public async Task<dynamic> Getprojectclientlist()
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
                    return await comrep.Getprojectclientlist();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in Getprojectclientlist: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetAttachProject")]
        public async Task<dynamic> GetAttachProject(string WorkId)
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
                    return await comrep.GetAttachProject(WorkId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAttach: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetActiveProjects")]
        public async Task<dynamic> GetActiveProjects()
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
                    return await comrep.GetActiveProjects();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in GetActiveProjects: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetActiveClientWorkStatuses")]
        public async Task<dynamic> GetActiveClientWorkStatuses()
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
                    return await comrep.GetActiveClientWorkStatuses();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in GetActiveClientWorkStatuses: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("SaveProjectWorkStatus")]
        public async Task<dynamic> SaveProjectWorkStatus(string projectWorkId, string clientWorkStatusId, string remarks,DateTime P_WORK_STATUS_DATE,string ex_remarks= null)
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
                    return await comrep.SaveProjectWorkStatus(projectWorkId,clientWorkStatusId,decodedToken.AUSR_ID,remarks,P_WORK_STATUS_DATE,ex_remarks);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in GetActiveClientWorkStatuses: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        



                [HttpGet("GetClientWorkStatusByProjectIdAsync")]
        public async Task<dynamic> GetClientWorkStatusByProjectIdAsync(int projectWorkId)
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
                    return await comrep.GetClientWorkStatusByProjectIdAsync(projectWorkId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetClientWorkStatusByProjectIdAsync: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

    }
}
