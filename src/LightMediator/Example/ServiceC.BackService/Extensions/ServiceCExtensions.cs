
using System.Reflection;

namespace ServiceC.BackService.Extensions;

public static class ServiceCExtensions
{
    public static Assembly GetServiceAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}