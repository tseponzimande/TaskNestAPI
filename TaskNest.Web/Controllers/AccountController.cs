namespace TaskNest.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

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
