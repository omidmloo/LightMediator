

using System.Reflection;

namespace ServiceA.BackService.Extensions;

public static class ServiceAExtensions
{
    public static Assembly GetServiceAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}