using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace dotnet8_keycloak_sample.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;

            _logger.LogInformation("Account controller created");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        [HttpPost("/[controller]/login-callback")]
        [HttpGet("/[controller]/login-callback")]
        public async Task<IActionResult> LoginCallback()
        {
            _logger.LogInformation("Login callback invoked");
            var authResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (authResult?.Succeeded != true)
            {
                // Handle failed authentication
                _logger.LogError("Authentication failed: {0}", authResult.Failure.Message);
                return RedirectToAction("Index");
            }

            // Get the access token and refresh token
            var accessToken = authResult.Properties.GetTokenValue("access_token");
            var refreshToken = authResult.Properties.GetTokenValue("refresh_token");
            _logger.LogInformation("Access token: {0}", accessToken);
            _logger.LogInformation("Refresh token: {0}", refreshToken);

            // Redirect the user to the desired page
            return RedirectToAction("Home", "Privacy");
        }

        public async Task<IActionResult> Login()
        {
            _logger.LogInformation("Logging in...");
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);

            _logger.LogInformation("Redirecting to Privacy...");
            return RedirectToAction("Privacy", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logging out...");
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
