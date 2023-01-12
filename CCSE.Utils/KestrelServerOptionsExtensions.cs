using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace CCSE.Utils
{
    public static class KestrelServerOptionsExtensions
    {
        public static void ConfigureEndpoints(this KestrelServerOptions options)
        {
            Console.WriteLine("Setting up Kestrel Server");
            var configuration = options.ApplicationServices.GetRequiredService<IConfiguration>();
            var environment = options.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            var endpoints = configuration.GetSection("KestrelServer:Endpoints")
                .GetChildren()
                .ToDictionary(section => section.Key, section =>
                {
                    var endpoint = new EndpointConfiguration();
                    section.Bind(endpoint);
                    return endpoint;
                });

            foreach (var endpoint in endpoints)
            {
                var ipAddresses = new List<IPAddress>();

                var config = endpoint.Value;
                if (IPAddress.TryParse(config.Host, out var ipAddress))
                {
                    ipAddresses.Add(ipAddress);
                }

                foreach (var address in ipAddresses)
                {
                    Console.WriteLine($"Adding address: {address}");

                    if (config.Scheme == "https")
                    {
                        Console.WriteLine($"Loading Certificate...");

                        var certificate = LoadCertificate(config.Certificate);

                        Console.WriteLine($"Loading Certificate... done.");

                        Console.WriteLine($"Executing port listen on- Address: {address} and Port: {config.Port.Value}");

                        options.Listen(address, config.Port.Value, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });

                        Console.WriteLine($"Executing port listening completed.");
                    }
                    else
                    {
                        options.Listen(IPAddress.Any, config.Port.Value, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                    }
                }
            }
        }

        public static X509Certificate2 LoadCertificate(Certificate config)
        {
            Console.WriteLine($"Cert config.Path: {config.Path}");
            Console.WriteLine($"Cert config.Password: {config.Password}");

            if (config.Path != null && config.Password != null)
            {
                return new X509Certificate2(config.Path, config.Password);
            }

            throw new InvalidOperationException("No valid certificate configuration found for the current endpoint.");
        }
    }

    public class EndpointConfiguration
    {
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Scheme { get; set; }
        public Certificate Certificate { get; set; }

    }

    public class Certificate
    {
        public string Path { get; set; }
        public string Password { get; set; }
    }
}
