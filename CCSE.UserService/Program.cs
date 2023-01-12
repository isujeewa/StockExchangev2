using AuthServer.Config;
using AuthServer.Data;
using AuthServer.Models;
using CCSE.Utils;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Reflection;
using UserService.API.IRepositories;
using UserService.API.IServices;
using UserService.API.Repositories;
using UserService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog(SeriLogger.ConfigureHostBuilder);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCustomMVC(builder.Configuration)
           .AddCustomDbContext(builder.Configuration)
            .AddCustomIntegrations(builder.Configuration)
           .AddCustomAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AuthServer.Utilities.Constants.AllowedSpecificOriginsPolicyName);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.RunAsync();

static class CustomExtensionsMethods
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var identityUrl = configuration.GetValue<string>("IdentityUrl");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer("Bearer", options =>
        {
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "AuthService";
        });

        services.AddAuthorization();

        return services;
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
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "User Service API",
                Description = "API for User service",
                Contact = new OpenApiContact
                {
                    Name = "CCSE",
                }
            });
        });
        return services;
    }
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var identityUrl = configuration.GetValue<string>("IdentityUrl");

        string applicationDbContextConnectionString =
           configuration.GetSection("ConnectionStrings").GetValue<string>("ApplicationDbContext");

        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

        // User db context
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(applicationDbContextConnectionString,
                npgsqlOptionsAction: psqlOptions =>
                {
                    psqlOptions.EnableRetryOnFailure(
                         maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    psqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                });
        });

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
            opt.Tokens.EmailConfirmationTokenProvider = "EmailConfirmationTokenProvider";
            opt.Tokens.ChangePhoneNumberTokenProvider = "PhoneNumberConfirmationTokenProvider";
        })
        .AddSignInManager<CustomSignInManager>()
        .AddEntityFrameworkStores<UserDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailConfirmationTokenProvider<AppUser>>("EmailConfirmationTokenProvider")
        .AddTokenProvider<PhoneNumberConfirmationTokenProvider<AppUser>>("PhoneNumberConfirmationTokenProvider");

        services.AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = builder => builder.UseNpgsql(applicationDbContextConnectionString,
                 npgsqlOptionsAction: psqlOptions =>
                 {
                     psqlOptions.EnableRetryOnFailure(
                          maxRetryCount: 10,
                     maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                     psqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                 });
        });

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

        services.AddSingleton<IEventService, IdentityEventService>();

        return services;
    }

    /// <summary>
    /// Add configuration store
    /// </summary>
    /// <param name="services"></param>
    /// <param name="storeOptionsAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfigurationStore(this IServiceCollection services,
    Action<ConfigurationStoreOptions> storeOptionsAction = null)
    {
        return services.AddConfigurationStore<ConfigurationDbContext>(storeOptionsAction);
    }

    /// <summary>
    /// Register configuration db context
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="storeOptionsAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfigurationStore<TContext>(this IServiceCollection services,
    Action<ConfigurationStoreOptions> storeOptionsAction = null)
    where TContext : DbContext, IConfigurationDbContext
    {
        var options = new ConfigurationStoreOptions();
        services.AddSingleton(options);
        storeOptionsAction?.Invoke(options);

        if (options.ResolveDbContextOptions != null)
        {
            services.AddDbContext<TContext>(options.ResolveDbContextOptions);
        }
        else
        {
            services.AddDbContext<TContext>(dbCtxBuilder =>
            {
                options.ConfigureDbContext?.Invoke(dbCtxBuilder);
            });
        }
        services.AddScoped<IConfigurationDbContext, TContext>();

        return services;
    }

    public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();

        IdentityModelEventSource.ShowPII = true;
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        //services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
        //    sp => (DbConnection c) => new IntegrationEventLogService(c));

        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddTransient<IUserService, UserService.API.Services.UserService>((serviceProvider) =>
        {
            var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
            return new UserService.API.Services.UserService(unitOfWork);
        });


        services.AddTransient<IApiResourceRepository, ApiResourceRepository>((serviceProvider) =>
        {
            var configurationDbContext = serviceProvider.GetService<ConfigurationDbContext>();
            return new ApiResourceRepository(configurationDbContext);
        });

        services.AddTransient<IApiResourceService, ApiResourceService>((serviceProvider) =>
        {
            var repo = serviceProvider.GetService<IApiResourceRepository>();
            return new ApiResourceService(repo);
        });

        services.AddTransient<IApiScopeRepository, ApiScopeRepository>((serviceProvider) =>
        {
            var configurationDbContext = serviceProvider.GetService<ConfigurationDbContext>();
            return new ApiScopeRepository(configurationDbContext);
        });

        services.AddTransient<IApiScopeService, ApiScopeService>((serviceProvider) =>
        {
            var repo = serviceProvider.GetService<IApiScopeRepository>();
            return new ApiScopeService(repo);
        });

        services.AddTransient<IClientRepository, ClientRepository>((serviceProvider) =>
        {
            var configurationDbContext = serviceProvider.GetService<ConfigurationDbContext>();
            return new ClientRepository(configurationDbContext);
        });

        services.AddTransient<IClientService, ClientService>((serviceProvider) =>
        {
            var repo = serviceProvider.GetService<IClientRepository>();
            return new ClientService(repo);
        });

        services.ConfigureNonBreakingSameSiteCookies();

        return services;
    }
}
