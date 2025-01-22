namespace ServiceA.BackService.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddServiceA(this IServiceCollection services)
    { 
        services.AddHostedService<FirstService>(); 
        return services;
    } 
}
