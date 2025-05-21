using FMS_API.Data.Class;
using FMS_API.Repositry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static JwtService;

namespace FMS_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AddCallController : Controller
	{

        string fpath;
        private readonly AddCallRepository comrep;
		private readonly JwtHandler jwtHandler;

		public AddCallController(AddCallRepository _comrep, JwtHandler _jwthand)
		{
			comrep = _comrep;
			jwtHandler = _jwthand;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }
	
		[HttpGet("GetActiveProjects")]
		public async Task<dynamic> GetActiveProjects(long clientId)
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
					return await comrep.GetActiveProjects(long.Parse(decodedToken.AUSR_ID));
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

		[HttpGet("GetEmergency")]
		public async Task<dynamic> GetEmergency()
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
					return await comrep.GetEmergency();
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
		[HttpGet("GetErrorTypes")]
		public async Task<dynamic> GetErrorTypes()
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
					return await comrep.GetErrorTypes();
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetErrorTypes: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}
		[HttpGet("GetCallTypes")]
		public async Task<dynamic> GetCallTypes()
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
					return await comrep.GetCallTypes();
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetCalltypes: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}
		[HttpGet("GetModules")]
		public async Task<dynamic> GetModules()
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
					return await comrep.GetModules(long.Parse(decodedToken.AUSR_ID));
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
		[HttpGet("GetClients")]
		public async Task<dynamic> GetClients(long projectId)
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
					return await comrep.GetClients(long.Parse(decodedToken.AUSR_ID), projectId);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetClients: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


		[HttpGet("GetEmployee")]
		public async Task<dynamic> GetEmployee()
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
					return await comrep.GetEmployee(decodedToken.AUSR_ID);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetEmployee: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		[HttpPost("SaveAddCall")]
		public async Task<dynamic> SaveAddCall(Calldet exp)
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
					return await comrep.SaveAddCall(exp, decodedToken);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in SaveAddCall: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		
			[HttpGet("GetRefNo")]
		public async Task<dynamic> GetRefNo(string ondate)
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
					return await comrep.GetRefNo(ondate);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetRefNo: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


        [HttpGet("UpdateWorkStatus")]
        public async Task<dynamic> UpdateWorkStatus(long workDtlsId, long workId, int workStatusId)
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
                    return await comrep.UpdateWorkStatus(workDtlsId, workId, workStatusId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in GetRefNo: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        

               [HttpGet("SaveWorkAssignmentFromCallList")]
        public async Task<dynamic> SaveWorkAssignmentFromCallList(string WorkId, long empId)
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
                    return await comrep.SaveWorkAssignmentFromCallList(WorkId, empId,decodedToken.AUSR_ID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in SaveWorkAssignmentFromCallList: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("OpenCallBySelfFromCAllList")]
        public async Task<dynamic> OpenCallBySelfFromCAllList(string WorkId)
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
                    return await comrep.OpenCallBySelfFromCAllList(WorkId, decodedToken.AUSR_ID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in OpenCallBySelfFromCAllList: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        

           [HttpGet("RejectASsignedWork")]
        public async Task<dynamic> RejectASsignedWork(long WorkId, long WorkdtlsId)
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
                    return await comrep.RejectASsignedWork(WorkId,WorkdtlsId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in SaveWorkAssignmentFromCallList: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetWorkDetailsAsync")]
        public async Task<dynamic> GetWorkDetailsAsync(string whereClause="")
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
                    return await comrep.GetWorkDetailsAsync(decodedToken.AUSR_ID,whereClause);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the GetActiveProjects
                Console.WriteLine($"Error in GetWorkDetailsAsync: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        public class DailyData
        {
            public string Task { get; set; }
            public string Date { get; set; }
            //public string wrefno { get; set; }
        }
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "ProjectDocumentUpload");



        [HttpPost("FileUpload")]
        public async Task<IActionResult> FileUpload([FromForm] string daily, [FromForm] IFormFile file, [FromForm] string wrefno, [FromForm] string workid)
        {
            // Check if the file is null or empty
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
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



                // Deserialize the daily JSON data into an object
                var dailyData = Newtonsoft.Json.JsonConvert.DeserializeObject<DailyData>(daily);

                //var directoryPath = Path.Combine(_storagePath, "ProjectDocumentUpload", wrefno);
                string sanitizedWrefno = SanitizeFileName(wrefno);

                // Create the directory path including the sanitized wrefno subfolder
                //var directoryPath = Path.Combine(_storagePath, "ProjectDocumentUpload", sanitizedWrefno);
                var directoryPath = Path.Combine(_storagePath, sanitizedWrefno);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var filePath = Path.Combine(directoryPath, file.FileName);
                // Save the file to the server
                //var filePath = Path.Combine(_storagePath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                int lastBackslashIndex = directoryPath.LastIndexOf('\\');
                int secondLastBackslashIndex = directoryPath.LastIndexOf('\\', lastBackslashIndex - 1);

                if (secondLastBackslashIndex >= 0)
                {
                    // Extract the part of the path after the second last backslash
                    string relativePath = directoryPath.Substring(secondLastBackslashIndex + 1);
                    fpath = relativePath + '\\' + file.FileName;
                    Console.WriteLine(relativePath); // Outputs: ProjectDocumentUpload\W000009-032025
                }
                else
                {
                    Console.WriteLine("Path structure is incorrect.");
                }
                await comrep.SaveFileuploadAsync(workid, fpath, decodedToken);

                // You can now use the dailyData object and the saved file path as needed
                return Ok(new { FilePath = filePath, DailyData = dailyData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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



		[HttpPost("SaveCallInform")]
		public async Task<dynamic> SaveCallInform(CallInformation exp)
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
					return await comrep.SaveCallInform(exp, decodedToken);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error in SaveCallInform: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

	}
}
