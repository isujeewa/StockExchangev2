using AuthServer.Models;
using AuthServer.ViewModels;
using IdentityModel.Client;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserService.API.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> loginManager;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService userService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<Account> logger;

        public Account(UserManager<AppUser> _userManager,
            SignInManager<AppUser> _loginManager,
            ILogger<Account> _logger,
            IConfiguration _config,
            IHttpContextAccessor _contextAccessor,
            IUserService _userService,
            IHttpClientFactory _httpClientFactory)
        {
            userManager = _userManager;
            loginManager = _loginManager;
            logger = _logger;
            config = _config;
            contextAccessor = _contextAccessor;
            userService = _userService;
            httpClientFactory = _httpClientFactory;
        }

        [Route("login")]
        [HttpPost]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AdminLogin([FromBody] LoginViewModel credentials)
        {
            logger.LogInformation("Entered login API");
            if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest();
            }

            try
            {
                var user = await userManager.FindByEmailAsync(credentials.Username);
                if (user == null)
                {
                    return BadRequest();
                }

                var result = await loginManager.PasswordSignInAsync(credentials.Username, credentials.Password, false, true);
                if (result.IsLockedOut || result.IsNotAllowed)
                {
                    return Unauthorized();
                }
                else
                {
                    var client = httpClientFactory.CreateClient();
                    var identityClientRegistration = config.GetSection("IdentityClientRegistration").GetChildren();
                    var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        Address = identityClientRegistration.Where(a => a.Key == "TokenEndpoint").First().Value,
                        ClientId = identityClientRegistration.Where(a => a.Key == "ClientId").First().Value,
                        ClientSecret = identityClientRegistration.Where(a => a.Key == "ClientSecret").First().Value,
                        UserName = credentials.Username,
                        Password = credentials.Password
                    });

                    if (response.HttpResponse.IsSuccessStatusCode)
                    {
                        TokenViewModel tokenViewModel = JsonConvert.DeserializeObject<TokenViewModel>(response.Json.ToString());
                        tokenViewModel.Role = user.UserRoleValue;
                        tokenViewModel.UserId = user.Id;
                        tokenViewModel.FullName = user.FullName;
                        tokenViewModel.PhoneNumber = user.PhoneNumber;
                        tokenViewModel.Email = user.Email;

                        return Ok(tokenViewModel);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
            }
            catch (Exception ex)
            {
                //string exceptionData = string.Format(Constants.ExceptionDataMsg,
                //  Constants.ExceptionMessages.AdminLoginMethod,
                //    ex.Message,
                //    ex.InnerException,
                //    ex.StackTrace);
                //logger.LogError(exceptionData);

                //string errorText = string.Format(stringLocalizer[Constants.ErrorCodeString],
                //    (int)ErroCodes.InternalServerError,
                //    stringLocalizer[Helper.ToEnumString(ErroCodes.InternalServerError)]);
                return StatusCode(500, "InternalServerError");
            }
        }

        // GET: api/<Account>
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Account>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Account>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Account>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Account>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
