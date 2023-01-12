using AuthServer.Config;
using AuthServer.Data;
using AuthServer.Models;
using CCSE.Utils;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var config = CustomExtensionsMethods.GetConfiguration();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddCustomMVC(builder.Configuration)
.AddCustomDbContext(builder.Configuration);

var app = builder.Build();
app.MigrateDbContext<PersistedGrantDbContext>((_, __) => { })
              .MigrateDbContext<UserDbContext>((context, services) =>
              {
                  var env = services.GetService<IWebHostEnvironment>();
                  var logger = services.GetService<ILogger<UserDbContextSeed>>();
                  var userManager = services.GetService<UserManager<AppUser>>();

                  new UserDbContextSeed()
                        .SeedAsync(context, env, userManager, logger)
                        .Wait();
              })
              .MigrateDbContext<ConfigurationDbContext>((context, services) =>
              {
                  new ConfigurationDbContextSeed()
                        .SeedAsync(context, config)
                        .Wait();
              });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AuthServer.Utilities.Constants.AllowedSpecificOriginsPolicyName);

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.RunAsync();


static class CustomExtensionsMethods
{
    internal static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        var config = builder.Build();
        return config;
    }
    public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        // get the allowed host as a string array
        var allowedOrigins = configuration.GetSection("UsedHostNames").GetChildren().Select(a => a.Value).ToArray();

        // enable cors only for specific origins
        services.AddCors(options =>
        {
            options.AddPolicy(AuthServer.Utilities.Constants.AllowedSpecificOriginsPolicyName,
                   builder => builder
                   .WithOrigins(allowedOrigins)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials());
        });

        // injects the http context accessor 
        services.AddHttpContextAccessor();

        services.AddMvc().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });

        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = ApiVersion.Default;
        });

        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var passwordOptions = configuration.GetSection("PasswordOptions").GetChildren().ToDictionary(x => x.Key, x => x.Value);
        var signInOptions = configuration.GetSection("SignInOptions").GetChildren().ToDictionary(x => x.Key, x => x.Value);

        services.AddIdentity<AppUser, AppRole>(opt =>
        {
            opt.Password.RequiredUniqueChars = int.Parse(passwordOptions["RequiredUniqueChars"]);
            opt.Password.RequireNonAlphanumeric = passwordOptions["RequireNonAlphanumeric"].Equals("1") ? true : false;
            opt.Password.RequiredLength = int.Parse(passwordOptions["RequiredLength"]);
            opt.Password.RequireDigit = passwordOptions["RequireDigit"].Equals("1") ? true : false;
            opt.Password.RequireUppercase = passwordOptions["RequireUppercase"].Equals("1") ? true : false;
            opt.Password.RequireLowercase = passwordOptions["RequireLowercase"].Equals("1") ? true : false;
            opt.User.RequireUniqueEmail = passwordOptions["RequireUniqueEmail"].Equals("1") ? true : false;

            opt.SignIn.RequireConfirmedEmail = signInOptions["RequireConfirmedEmail"].Equals("1") ? true : false;
            opt.SignIn.RequireConfirmedPhoneNumber = signInOptions["RequireConfirmedMobile"].Equals("1") ? true : false;
            opt.Tokens.EmailConfirmationTokenProvider = "EmailConfirmationTokenProvider";
            opt.Tokens.ChangePhoneNumberTokenProvider = "PhoneNumberConfirmationTokenProvider";
        })
        .AddSignInManager<CustomSignInManager>()
        .AddEntityFrameworkStores<UserDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailConfirmationTokenProvider<AppUser>>("EmailConfirmationTokenProvider")
        .AddTokenProvider<PhoneNumberConfirmationTokenProvider<AppUser>>("PhoneNumberConfirmationTokenProvider");

        var dataProtectionTokenProviderOptions =
            configuration.GetSection("DataProtectionTokenProviderOptions").GetChildren().ToDictionary(x => x.Key, x => x.Value);

        services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(
                int.Parse(dataProtectionTokenProviderOptions["TokenLifeSpanInHours"])));

        var emailConfirmationTokenProviderOptions =
            configuration.GetSection("EmailConfirmationTokenProviderOptions").GetChildren().ToDictionary(x => x.Key, x => x.Value);

        services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(
                int.Parse(emailConfirmationTokenProviderOptions["TokenLifeSpanInHours"])));

        services.AddTransient<EmailConfirmationTokenProvider<AppUser>>();
        services.AddTransient<PhoneNumberConfirmationTokenProvider<AppUser>>();

        var lockoutIdenityOptions =
          configuration.GetSection("LockoutIdenityOptions").GetChildren().ToDictionary(x => x.Key, x => x.Value);

        services.Configure<IdentityOptions>(options =>
        {
            options.Lockout.MaxFailedAccessAttempts = int.Parse(lockoutIdenityOptions["MaxFailedAccessAttempts"]);
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(
                 int.Parse(lockoutIdenityOptions["DefaultLockoutTimeSpanInMinutes"]));
        });

        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

        var identityUrl = configuration.GetValue<string>("IdentityUrl");

        string applicationDbContextConnectionString =
           configuration.GetSection("ConnectionStrings").GetValue<string>("ApplicationDbContext");

        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(applicationDbContextConnectionString,
                npgsqlOptionsAction: psqlOptions =>
                {
                    psqlOptions.EnableRetryOnFailure(
                         maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    psqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                })
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));
        });

        if (configuration.GetValue<bool>("Certificate:IsUsed") == false)
        {
            var eventsConfig =
                configuration.GetSection("Events").GetChildren().ToDictionary(x => x.Key, x => x.Value);

            services.AddIdentityServer(options =>
            {
                options.IssuerUri = identityUrl;
                options.Events.RaiseSuccessEvents = Convert.ToBoolean(eventsConfig["RaiseSuccessEvents"]);
                options.Events.RaiseInformationEvents = Convert.ToBoolean(eventsConfig["RaiseInformationEvents"]);
                options.Events.RaiseFailureEvents = Convert.ToBoolean(eventsConfig["RaiseFailureEvents"]);
                options.Events.RaiseErrorEvents = Convert.ToBoolean(eventsConfig["RaiseErrorEvents"]);
            })
            .AddDeveloperSigningCredential()
            .AddAspNetIdentity<AppUser>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                          builder.UseNpgsql(applicationDbContextConnectionString,
                              sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseNpgsql(applicationDbContextConnectionString,
                         sql => sql.MigrationsAssembly(migrationsAssembly));
            });
        }
        else
        {
            var cert = new Certificate()
            {
                Path = configuration.GetValue<string>("Certificate:Path"),
                Password = configuration.GetValue<string>("Certificate:Password")
            };
            services.AddIdentityServer(options =>
            {
                options.IssuerUri = identityUrl;
            })
              .AddSigningCredential(KestrelServerOptionsExtensions.LoadCertificate(cert))
              .AddAspNetIdentity<AppUser>()
              .AddConfigurationStore(options =>
              {
                  options.ConfigureDbContext = builder =>
                      builder.UseNpgsql(applicationDbContextConnectionString,
                          sql => sql.MigrationsAssembly(migrationsAssembly));
              })
              // this adds the operational data from DB (codes, tokens, consents)
              .AddOperationalStore(options =>
              {
                  options.ConfigureDbContext = builder =>
                      builder.UseNpgsql(applicationDbContextConnectionString,
                           sql => sql.MigrationsAssembly(migrationsAssembly));
              });
        }

        services.AddAccessTokenManagement();

        return services;
    }
}