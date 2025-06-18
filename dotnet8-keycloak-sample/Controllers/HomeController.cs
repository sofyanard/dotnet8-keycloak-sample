using System.Diagnostics;
using dotnet8_keycloak_sample.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet8_keycloak_sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            _logger.LogInformation("Login callback invoked");
            string accessToken = string.Empty;
            string refreshToken = string.Empty;

            try
            {
                var authResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

                if (authResult?.Succeeded != true)
                {
                    // Get the access token and refresh token
                    accessToken = authResult.Properties.GetTokenValue("access_token");
                    refreshToken = authResult.Properties.GetTokenValue("refresh_token");
                    _logger.LogInformation("Access token: {0}", accessToken);
                    _logger.LogInformation("Refresh token: {0}", refreshToken);
                }
                else
                {
                    // Handle failed authentication
                    _logger.LogError("Authentication failed: {0}", authResult.Failure.Message);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication check");
                return RedirectToAction("Index");
            }

            

            ViewData["access_token"] = accessToken;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
