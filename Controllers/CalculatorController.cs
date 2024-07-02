using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CalculatorApi.Services;
using Newtonsoft.Json.Linq;

namespace CalculatorApi.Controllers
{
    /// <summary>
    /// Controller handling operations for a calculator API.
    /// </summary>
    [ApiController]
    [Route("v1")]
    public class CalculatorApiController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly CalculationService _calculationService;

        public CalculatorApiController(TokenService tokenService, CalculationService calculationService)
        {
            _tokenService = tokenService;
            _calculationService = calculationService;

        }


        // Authenticates the user and generates a JWT token.
        //Login credentials is username = test and password = password for testing

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (login.Username.Equals("test") && login.Password.Equals("password"))
            {
                var token = _tokenService.GenerateToken(login.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

      
        /// Performs a calculation based on the provided operation and numbers.
        [HttpPost("calculate")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [SwaggerResponse(200, "A JSON object containing the result and the description of the operation performed.")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(500, "Internal server error")]
        public IActionResult Calculate([FromBody] CalculateModel body, [FromHeader][Required] string operation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = _calculationService.PerformCalculation(body.Number1, body.Number2, operation);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
           
          
        }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

    public class CalculateModel
    {
        [Required(ErrorMessage = "Number1 is required.")]
        [Range(typeof(decimal), "-79228162514264337593543950335", "79228162514264337593543950335", ErrorMessage = "Number1 must be a valid number.")]
        public decimal Number1 { get; set; }

        [Required(ErrorMessage = "Number2 is required.")]
        [Range(typeof(decimal), "-79228162514264337593543950335", "79228162514264337593543950335", ErrorMessage = "Number2 must be a valid number.")]
        public decimal Number2 { get; set; }
    }

}
