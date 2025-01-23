
using System.Reflection;

namespace ServiceC.BackService.Extensions;

public static class ServiceExtensions
{
    public static Assembly GetServiceAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}