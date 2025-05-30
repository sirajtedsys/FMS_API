using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FMS_API.Data.Class;
using FMS_API.Repositry;
using static JwtService;
using System.Runtime.ConstrainedExecution;
using Microsoft.Extensions.Configuration;

namespace FMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly CommonRepositry comrep;
        private readonly JwtHandler jwtHandler;

        private readonly IConfiguration Iconfiguration;

        public CommonController(CommonRepositry _comrep, JwtHandler _jwthand, IConfiguration _iconf)
        {
            comrep = _comrep;
            jwtHandler = _jwthand;

            Iconfiguration = _iconf;
        }


        [HttpPost("CheckLogin")]
        public async Task<dynamic> LoginCheck(Login log)
        {
            if (log != null)
            {
                var resp = await comrep.LoginCheck(log.Username,log.Password);
                return resp;
            }
            else
            {
                return BadRequest();
            }
         }

        [HttpGet("GetAllUserBranches")]
        public async Task<dynamic> GetAllUserBranches()
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
                    return await comrep.GetAllUserBranches(decodedToken);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in UserDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        //userwanted and allowed repotrs
        [HttpGet("GetAppMenuAsync")]
        public async Task<dynamic> GetAppMenuAsync()
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
                    return await comrep.GetAppMenuAsync(decodedToken.AUSR_ID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppMenuAsync: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetCreditbill")]
        public async Task<dynamic> GetCreditbill(string fromDate, string toDate, string custid = null)
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
                    return await comrep.GetCreditbill(fromDate, toDate, custid);
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


        [HttpGet("GetCreditbillsettled")]
        public async Task<dynamic> GetCreditbillsettled(string fromDate, string toDate, string custid = null)
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
                    return await comrep.GetCreditbillsettled(fromDate, toDate, custid);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetCreditbillsettled: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }




        [HttpGet("GetUserDetails")]
        public async Task<dynamic> GetUserDetails()
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
                    return await comrep.GetUserDetails(decodedToken);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppEmpWiseSct: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetAppEmpWiseSct")]
        public async Task<dynamic> GetAppEmpWiseSct()
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
                    return await comrep.GetAppEmpWiseSct(decodedToken.USERNAME,decodedToken.PASSWORD);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppEmpWiseSct: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetCreditbillSettings")]
        public async Task<dynamic> GetCreditbillSettings()
        {
            var section = Iconfiguration.GetSection("CreditbillSettlement")
                           .GetChildren()
                           .ToDictionary(x => x.Key, x => x.Value);

            dynamic creditSettings = new System.Dynamic.ExpandoObject();
            foreach (var kvp in section)
            {
                ((IDictionary<string, object>)creditSettings)[kvp.Key] = kvp.Value;
            }

            return new DefaultMessage.Message3
            {
                Status = 200,
                Data = new { Creditbill = creditSettings }
            };
        }




        #region Credit Bill settlemnet

        [HttpGet("GetAppFinYear")]
        public async Task<dynamic> GetAppFinYear()
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
                    return await comrep.GetAppFinYear();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppFinYear: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetAppCustomer")]
        public async Task<dynamic> GetAppCustomer()
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
                    return await comrep.GetAppCustomer();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppEmpWiseSct: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetAppCustBranch")]
        public async Task<dynamic> GetAppCustBranch(string mainCustId)
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
                    return await comrep.GetAppCustBranch(mainCustId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppCustBranch: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetAppCustFlexFill")]
        public async Task<dynamic> GetAppCustFlexFill(string sctId, string filterCust)
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
                    return await comrep.GetAppCustFlexFill(sctId,filterCust);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppCustFlexFill: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet("GetManualInvoiceFlexFill")]
        public async Task<dynamic> GetManualInvoiceFlexFill(string sctId, string manualInvNo, int finYearID)
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
                    return await comrep.GetManualInvoiceFlexFill(sctId, manualInvNo, finYearID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetManualInvoiceFlexFill: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet("GetAppBillNoFlexFill")]
        public async Task<dynamic> GetAppBillNoFlexFill(string sctId, string BillNO, int finYearID)
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
                    //return Ok(new { Message = "User details retrieved BillNO", UserDetails = decodedToken });
                    return await comrep.GetAppBillNoFlexFill(sctId, BillNO, finYearID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppBillNoFlexFill: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        


        [HttpGet("GetCompanyFinYearDetails")]
        public async Task<dynamic> GetCompanyFinYearDetails()
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
                    return await comrep.GetCompanyFinYearDetails();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppFinYear: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GenerateCRBillRefNo")]
        public async Task<dynamic> GenerateCRBillRefNo(int compID, int finYearID, string compCode, string finYearCode, int sctID)
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
                    return await comrep.GenerateCRBillRefNo(compID,finYearID,compCode,finYearCode,sctID);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GenerateCRBillRefNo: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetAppBank")]
        public async Task<dynamic> GetAppBank()
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
                    return await comrep.GetAppBank();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppBank: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet("GetAppWallet")]
        public async Task<dynamic> GetAppWallet()
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
                    return await comrep.GetAppWallet();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetAppWallet: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        

        [HttpGet("GetCRBCPrefix")]
        public async Task<dynamic> GetCRBCPrefix()
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
                    return await comrep.GetCRBCPrefix();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetCRBCPrefix: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpGet("GetSectionCounterDetails")]
        public async Task<dynamic> GetSectionCounterDetails(long sectionId)
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
                    return await comrep.GetSectionCounterDetails(sectionId);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetSectionCounterDetails: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost("SaveAppCrBillMaster")]
        public async Task<dynamic> SaveAppCrBillMaster(CrBill cb)
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
                    return await comrep.SaveAppCrBillMaster(cb,decodedToken );
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SaveAppCrBillMaster: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }






        #endregion Credit Bill settlement


    }


}

