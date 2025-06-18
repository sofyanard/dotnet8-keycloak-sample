namespace dotnet8_keycloak_sample
{
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    public class Program
    {
        public static void Main(string[] args)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger("Program");
            logger.LogInformation("Hello World! Logging is {Description}.", "fun");

            var builder = WebApplication.CreateBuilder(args);

            // Add OpenID Connect authentication
            builder.Services
                .AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                })
                .AddOpenIdConnect(options =>
                {
                    options.Authority = builder.Configuration["OpenIDConnect:Authority"];
                    options.ClientId = builder.Configuration["OpenIDConnect:ClientId"];
                    options.ClientSecret = builder.Configuration["OpenIDConnect:ClientSecret"];
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                    options.SaveTokens = true;
                    options.CallbackPath = "/login-callback"; // Update callback path
                    options.SignedOutCallbackPath = "/logout-callback"; // Update signout callback path
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "preferred_username",
                        RoleClaimType = "roles"
                    };
                    options.RequireHttpsMetadata = false;
                    options.SkipUnrecognizedRequests = true;
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                    };
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Add routes for callback handling
            app.Map("/login-callback", loginCallbackApp =>
            {
                loginCallbackApp.Run(async context =>
                {
                    // Handle the callback from Keycloak after successful authentication
                    // await context.Response.WriteAsync("Authentication successful");
                    logger.LogInformation("Authentication successful");
                    Results.Redirect("/home/privacy");
                });
            });

            app.Map("/logout-callback", logoutCallbackApp =>
            {
                logoutCallbackApp.Run(async context =>
                {
                    // Handle the callback from Keycloak after sign-out
                    await context.Response.WriteAsync("Sign-out successful");
                });
            });

            app.MapControllerRoute(
                name: "login-callback",
                pattern: "login-callback",
                defaults: new { controller = "Account", action = "LoginCallback" });

            app.Run();
        }
    }
}
