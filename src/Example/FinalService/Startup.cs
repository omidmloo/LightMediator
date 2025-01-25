
using Microsoft.AspNetCore.Hosting;

namespace FinalService;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Use hosting extension of services to configure 
        services.AddServiceA();
        services.AddServiceB();
        services.AddServiceC();


        services.AddHostedService<Worker>();

       

        // Add Mediator to the Dependency Injection container
        services.AddLightMediator(options =>
         {
             // Configure options for the mediator
             options.IgnoreNamespaceInAssemblies = true;
             options.IgnoreNotificationDifferences = true;
             options.RegisterNotificationsByAssembly = true;

             // specify the assemblies to scan for notifications
             options.Assemblies = new[]
             {
                Assembly.GetExecutingAssembly(),
                ServiceA.BackService.Extensions.ServiceExtensions.GetServiceAssembly(),
                ServiceB.BackService.Extensions.ServiceExtensions.GetServiceAssembly(),
                ServiceC.BackService.Extensions.ServiceExtensions.GetServiceAssembly()
             };
         });


        services.AddControllers()
             .AddApplicationPart(typeof(FinalService.Controllers.ServicesController).Assembly)
             .AddApplicationPart(ServiceA.BackService.Extensions.ServiceExtensions.GetServiceAssembly())
             .AddApplicationPart(ServiceB.BackService.Extensions.ServiceExtensions.GetServiceAssembly())
             //.AddApplicationPart(typeof(ServiceC.BackService.Controllers.AppsController).Assembly)
             .AddControllersAsServices();

    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    { 

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {

            endpoints.MapControllers();
        }); 
    }
}
