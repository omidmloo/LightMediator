

using System.Reflection;

namespace ServiceA.BackService.Extensions;

public static class ServiceExtensions
{
    public static Assembly GetServiceAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}