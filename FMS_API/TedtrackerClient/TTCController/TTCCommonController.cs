using FMS_API.Data.Class;
using FMS_API.Repositry;
using FMS_API.TedtrackerClient.TTCClass;
using FMS_API.TedtrackerClient.TTCRepositry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Oracle.ManagedDataAccess.Client;
using static JwtService;
using static JwtServiceProject;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace FMS_API.TedtrackerClient.TTCController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TTCCommonController : ControllerBase

    {
        private readonly TTCCommonRepositry comrep;
        private readonly JwtHandlerProject jwtHandler;
		private readonly AttendenceRepositry atrep;
		string fpath;

		public TTCCommonController(TTCCommonRepositry _comrep, JwtHandlerProject _jwthand,AttendenceRepositry _atrep)
        {
            comrep = _comrep;
            jwtHandler = _jwthand;
			atrep = _atrep;
			if (!Directory.Exists(_storagePath))
			{
				Directory.CreateDirectory(_storagePath);
			}
		}

        [HttpPost("CheckProjectLogin")]
        public async Task<dynamic> LoginCheck(Login log)
        {
            if (log != null)
            {
                var resp = await comrep.CheckProjectLogin(log.Username, log.Password);
                return resp;
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("SaveAppProjectWork")]
        public async Task<dynamic> SaveAppProjectWork(ProjectWorkAddCall exp)
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
				ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await comrep.SaveAppProjectWork(exp,decodedToken);
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


        [HttpGet("GetProjectDetails")]
        public async Task<dynamic> GetProjectDetails()
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await comrep.GetUserProject(decodedToken.USERNAME, decodedToken.PASSWORD);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetEmployeeDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }



        [HttpGet("GetModulesByProjectId")]
        public async Task<dynamic> GetModulesByProjectId()
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await comrep.GetModulesByProjectId(decodedToken.AUSR_ID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetModulesByProjectId: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

		public class DailyData
		{
			public string Task { get; set; }
			public string Date { get; set; }
			//public string wrefno { get; set; }
		}
		private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Project");




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
				ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
				ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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


		[HttpGet("Getworklist")]
		public async Task<dynamic> Getworklist(string projectid)
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
				ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
					return await comrep.Getworklist(decodedToken.AUSR_ID);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetCalllist: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}

		[HttpGet("Getworklistproject")]
		public async Task<dynamic> Getworklistproject(string projectid)
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
				ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
					return await comrep.Getworklistproject(decodedToken.AUSR_ID);
				}
				else
				{
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				// Log the GetActiveProjects
				Console.WriteLine($"Error in GetCalllist: {ex.Message}");
				return StatusCode(500, "Internal server error");
			}

		}


		[HttpGet("GetAttach")]
		public async Task<dynamic> GetAttach(string ProjectWorkId)
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
				ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
					return await comrep.GetAttach(ProjectWorkId);
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


		private readonly string _storagePath1 = Path.Combine(Directory.GetCurrentDirectory(), "Project");

		[HttpGet("{*fullPath}")]
		public IActionResult GetFile(string fullPath)
		{

			fullPath = fullPath.Replace("//", "/");
			// Remove 'ProjectDocumentUpload/' if it exists at the start of the path from the frontend
			if (fullPath.StartsWith("Project/"))
			{

				fullPath = fullPath.Substring("Project/".Length);
			}

			// Now combine the cleaned-up path with the base storage path
			var filePath = Path.Combine(_storagePath1, fullPath);

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
        
		
		
		
		[HttpGet("DeleteProjectWorkAsync")]
        public async Task<dynamic> DeleteProjectWorkAsync(long workId)
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await comrep.DeleteProjectWorkAsync(workId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DeleteProjectWorkAsync: {ex.Message}");
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await atrep.GetActiveClientWorkStatuses();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetActiveClientWorkStatuses: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost("UpdateProjectWorkDetails")]
        public async Task<dynamic> UpdateProjectWorkDetails(ProjectWorkAddCall pw)
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await comrep.UpdateProjectWorkDetails(pw);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in UpdateProjectWorkDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetClientWorkStatusByProjectIdAsync")]
        public async Task<dynamic> GetClientWorkStatusByProjectIdAsync(int projectWorkId)
        {
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {     // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await atrep.GetClientWorkStatusByProjectIdAsync(projectWorkId);
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

        [HttpGet("DeleteAttachFileAsync")]
        public async Task<dynamic> DeleteAttachFileAsync(long fileId)
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await comrep.DeleteAttachFileAsync(fileId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DeleteProjectWorkAsync: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetWorkStatusUpAttach")]
        public async Task<dynamic> GetWorkStatusUpAttach(string P_CLNT_WORK_STATUS_ID)
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await atrep.GetWorkStatusUpAttach(P_CLNT_WORK_STATUS_ID);
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




        [HttpGet("UpdateProjectWorkConfirmation")]
        public async Task<dynamic> UpdateProjectWorkConfirmation(string projectWorkId, string confirmedRemarks, string CurrentTaskStatus)
        {
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {     // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.UpdateProjectWorkConfirmation(projectWorkId, decodedToken.AUSR_ID, confirmedRemarks, CurrentTaskStatus);
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


        [HttpGet("GetClientWorkStatusesAsync")]
        public async Task<dynamic> GetClientWorkStatusesAsync()
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

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
                    return await comrep.GetClientWorkStatusesAsync();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetActiveClientWorkStatuses: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("UpdateProjectWorkStatus")]
        public async Task<dynamic> UpdateProjectWorkStatus(string projectWorkId, string projectWorkStatusId, string updateRemarks)
        {
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
                ProjectUserTocken decodedToken = jwtHandler.DecriptTocken(authorizationHeader);

                if (decodedToken == null)
                {
                    return Unauthorized();
                }

                // Validate token
                var isValid = await jwtHandler.ValidateToken(token);

                if (isValid)
                {     // Return user details or appropriate response
                    //return Ok(new { Message = "User details retrieved successfully", UserDetails = decodedToken });
                    return await comrep.UpdateProjectWorkStatus(projectWorkId, projectWorkStatusId, updateRemarks);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in UpdateProjectWorkStatus: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


    }
}
