
using System.Reflection;

namespace ServiceB.BackService.Extensions;

public static class ServiceExtensions
{
    public static Assembly GetServiceAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}