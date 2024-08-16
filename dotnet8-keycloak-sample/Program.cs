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
                    options.Authority = "http://localhost:8080/realms/myrealm";
                    options.ClientId = "myclient";
                    options.ClientSecret = "6WB0tAWZGm1m9j4HEpF4ucE7zwWcRiYM";
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                    options.SaveTokens = true;
                    options.CallbackPath = "/account/login-callback"; // Update callback path
                    options.SignedOutCallbackPath = "/account/logout-callback"; // Update signout callback path
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "preferred_username",
                        RoleClaimType = "roles"
                    };
                    options.RequireHttpsMetadata = false;
                    options.SkipUnrecognizedRequests = true;
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

            app.Run();
        }
    }
}
