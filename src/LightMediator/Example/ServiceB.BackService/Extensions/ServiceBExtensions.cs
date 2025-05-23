
using System.Reflection;

namespace ServiceB.BackService.Extensions;

public static class ServiceBExtensions
{
    public static Assembly GetServiceAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}