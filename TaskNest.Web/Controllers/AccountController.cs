namespace TaskNest.Web.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]

    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;


        /// <summary>
        /// Register a new user
        /// </summary>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var result = await _accountService.RegisterAsync(model);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("User registered successfully.");
        }


        /// <summary>
        /// Login a user and return a JWT token
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var token = await _accountService.LoginAsync(model);
            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { Token = token });
        }


    }
}
