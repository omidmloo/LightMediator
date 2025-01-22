namespace ServiceB.BackService.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddServiceB(this IServiceCollection services)
    { 
        services.AddHostedService<SecondService>(); 
        return services;
    }
}
