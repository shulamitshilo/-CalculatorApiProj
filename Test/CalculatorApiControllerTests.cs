using System.Net;
using System.Security.Claims;
using IO.Swagger.Controllers;
using IO.Swagger.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IO.Swagger.Tests
{
    public class CalculatorApiControllerTests
    {
        private readonly Mock<TokenService> _mockTokenService;
        private readonly CalculatorApiController _controller;
 


        public CalculatorApiControllerTests()
        {
            _mockTokenService = new Mock<TokenService>();
            _controller = new CalculatorApiController(_mockTokenService.Object);
         

        }


        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorizedResult()
        {
            var loginModel = new LoginModel { Username = "wrong", Password = "wrong" };
            var result = _controller.Login(loginModel);
            Assert.IsType<UnauthorizedResult>(result);
        }
        private void SetUserToken(string token)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "test") };
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = claimsPrincipal }
            };
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer " + token;


        }
            [Fact]
        //test for add operator
        public void CalculatePost_AddOperation_ReturnsOkResult()
        {
            var calculateModel = new CalculateModel { Number1 = 10, Number2 = 5 };
            var result = _controller.CalculatePost(calculateModel, "add") as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(15m, result.Value);
        }


        [Fact]
        //test for subtrct operator
        public void CalculatePost_SubtractOperation_ReturnsCorrectResult()
        {
            var calculateModel = new CalculateModel { Number1 = 10, Number2 = 5 };

            var result = _controller.CalculatePost(calculateModel, "subtract") as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(5m, result.Value);
        }

        [Fact]
        //test for multiply operator
        public void CalculatePost_MultiplyOperation_ReturnsCorrectResult()
        {
            var calculateModel = new CalculateModel { Number1 = 10, Number2 = 5 };
            var result = _controller.CalculatePost(calculateModel, "multiply") as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(50m, result.Value);
        }

        [Fact]
        //test for divide operator
        public void CalculatePost_DivideOperation_ReturnsCorrectResult()
        {
            var calculateModel = new CalculateModel { Number1 = 10, Number2 = 5 };
            var result = _controller.CalculatePost(calculateModel, "divide") as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(2m, result.Value);
        }

        [Fact]
        //Division by zero test
        public void CalculatePost_DivideByZero_ReturnsBadRequest()
        {
            var calculateModel = new CalculateModel { Number1 = 10, Number2 = 0 };
            var result = _controller.CalculatePost(calculateModel, "divide") as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Division by zero is not allowed.", result.Value);
        }

        [Fact]
        //Invalid operation test
        public void CalculatePost_InvalidOperation_ReturnsBadRequest()
        {
            var calculateModel = new CalculateModel { Number1 = 10, Number2 = 5 };
            var result = _controller.CalculatePost(calculateModel, "invalid") as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Invalid operation specified.", result.Value);
        }
        //Add 1 to the Maxvalue
        [Fact]
        public void CalculatePost_MaxValueAddition_ReturnsBadRequest()
        {
            var calculateModel = new CalculateModel { Number1 = decimal.MaxValue, Number2 = 1 };
            var result = _controller.CalculatePost(calculateModel, "add") as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("One or both numbers are out of valid range.", result.Value.ToString());
        }

        //Substract 1 from the Minvalue
        [Fact]
        public void CalculatePost_MinValueSubtraction_ReturnsBadRequest()
        {
 
            var calculateModel = new CalculateModel { Number1 = decimal.MinValue, Number2 = 1 };
            var result = _controller.CalculatePost(calculateModel, "subtract") as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("One or both numbers are out of valid range.", result.Value.ToString());
        }

        [Fact]
        //Invalid Username test
public void Login_InvalidModel_ReturnsBadRequest()
{
    _controller.ModelState.AddModelError("Username", "Required");
    var loginModel = new LoginModel { Username = "", Password = "password" };
    var result = _controller.Login(loginModel) as BadRequestObjectResult;
    Assert.NotNull(result);
    Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
}


    }
}
