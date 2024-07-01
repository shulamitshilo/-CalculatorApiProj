using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IO.Swagger.Services;
using Newtonsoft.Json.Linq;

namespace IO.Swagger.Controllers
{
    /// <summary>
    /// Controller handling operations for a calculator API.
    /// </summary>
    [ApiController]
    [Route("v1")]
    public class CalculatorApiController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public CalculatorApiController(TokenService tokenService)
        {
            _tokenService = tokenService;
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

            if (login.Username == "test" && login.Password == "password")
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
        public IActionResult CalculatePost([FromBody] CalculateModel body, [FromHeader][Required] string operation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            decimal result = 0;

            try
            {
                switch (operation.ToLower())
                {
                    case "add":
                        result = body.Number1 + body.Number2;
                        break;
                    case "subtract":
                        result = body.Number1 - body.Number2;
                        break;
                    case "multiply":
                        result = body.Number1 * body.Number2;
                        break;
                    case "divide":
                        if (body.Number2 != 0)
                        {
                            result = body.Number1 / body.Number2;
                        }
                        //Division by zero
                        else
                        {
                            return BadRequest("Division by zero is not allowed.");
                        }
                        break;
                    //if the operator doesn't valid
                    default:
                        return BadRequest("Invalid operation specified.");
                }
            }
            catch (Exception ex) 
            {
                return BadRequest("One or both numbers are out of valid range.");

            }
            return Ok(result);
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
