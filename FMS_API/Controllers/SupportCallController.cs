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
	public class SupportCallController : Controller
	{

        string fpath;

        private readonly SupportCallRepository comrep;
		private readonly JwtHandler jwtHandler;
		public SupportCallController(SupportCallRepository _comrep, JwtHandler _jwthand)
		{
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
            comrep = _comrep;
			jwtHandler = _jwthand;
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
					return await comrep.GetEmployee();
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

		[HttpPost("SaveSupportCall")]
		public async Task<dynamic> SaveSupportCall(Calldet exp)
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
					return await comrep.SaveSupportCall(exp, decodedToken);
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


		[HttpGet("GetSupportRefNo")]
		public async Task<dynamic> GetSupportRefNo(string ondate)
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
					return await comrep.GetSupportRefNo(ondate);
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




        private readonly string _storagePathWorkStausUpdate = Path.Combine(Directory.GetCurrentDirectory(), "WorkStatusUpdateAtt");


        [HttpPost("uploadFilesOnWorkStatusUpdate")]
        public async Task<IActionResult> uploadFilesOnWorkStatusUpdate([FromForm] string daily, [FromForm] IFormFile file,  [FromForm] string CLNT_WORK_STATUS_ID)
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
                string sanitizedWrefno = SanitizeFileName(CLNT_WORK_STATUS_ID);

                // Create the directory path including the sanitized wrefno subfolder
                //var directoryPath = Path.Combine(_storagePath, "ProjectDocumentUpload", sanitizedWrefno);
                var directoryPath = Path.Combine(_storagePathWorkStausUpdate, sanitizedWrefno);

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
                await comrep.SaveProjectWorkStatusFileuploadAsync(sanitizedWrefno, fpath, decodedToken);

                // You can now use the dailyData object and the saved file path as needed
                return Ok(new { FilePath = filePath, DailyData = dailyData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "ProjectDocumentUpload");

        [HttpGet("{*fullPath}")]
        public IActionResult GetFile(string fullPath)
        {

            fullPath = fullPath.Replace("//", "/");
            // Remove 'ProjectDocumentUploadWorkStatusUpdateAtt if it exists at the start of the path from the frontend
            if (fullPath.StartsWith("WorkStatusUpdateAtt/"))
            {

                fullPath = fullPath.Substring("WorkStatusUpdateAtt/".Length);
            }

            // Now combine the cleaned-up path with the base storage path
            var filePath = Path.Combine(_storagePathWorkStausUpdate, fullPath);

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


    }
}
