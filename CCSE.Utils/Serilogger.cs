using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace CCSE.Utils
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureHostBuilder =>
          (context, configuration) =>
          {
              var logFile =
              context.Configuration.GetValue<string>("LogFileName");
              try
              {
                  configuration
                       .Enrich.FromLogContext()
                       .Enrich.WithMachineName()
                       .Enrich.WithExceptionDetails()
                       .WriteTo.Debug()
                       .WriteTo.Console()
                       .WriteTo.File(logFile)
                       .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                       .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                       .ReadFrom.Configuration(context.Configuration);
              }
              catch (Exception)
              {

              }
          };
    }
}
