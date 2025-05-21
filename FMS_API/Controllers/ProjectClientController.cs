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
	public class ProjectClientController : Controller
	{
		private readonly AttendenceRepositry comrep;
		private readonly JwtHandler jwtHandler;

		public ProjectClientController(AttendenceRepositry _comrep, JwtHandler _jwthand)
		{
			comrep = _comrep;
			jwtHandler = _jwthand;
		}

		private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Project");

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



	}
}
