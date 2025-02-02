using ServiceC.BackService.Application.CommandHandlers;
using ServiceC.BackService.Application.Commands;

namespace ServiceC.BackService.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddServiceC(this IServiceCollection services)
    {  
        services.AddHostedService<ThirdService>();  
        return services;
    }
}
