using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureKestrel(options =>
        {
            options.ConfigureHttpsDefaults(https =>
            {
                var password = new ConfigurationBuilder()
                    .AddJsonFile("secrets.json")
                    .Build()
                    .GetSection("secret")["password"];
                https.ServerCertificate = new X509Certificate2("certs/mycerts.pfx", password);
            });
            options.Listen(IPAddress.Any, 9621, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
                listenOptions.UseHttps();
            });
        })
        .UseKestrel()
        .ConfigureServices(services =>
        {
            services.AddSingleton<IConfiguration>(configuration);
            //services.AddHostedService<RemoteSer>();
            services.AddGrpc();
        })
        .Configure(app =>
        {
            //if (app.E)
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGrpcService<RemoteService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        });
    }).Build().Run();
