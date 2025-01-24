using System.Reflection;

namespace LightMediator;

// Options class for configuring mediator services
public class LightMediatorOptions
{
    public bool IgnoreNamespaceInAssemblies { get; set; }
    public bool IgnoreNotificationDifferences { get; set; } = true;
    public bool RegisterNotificationsByAssembly { get; set; } = true;
    public Assembly[] Assemblies { get; set; } = Array.Empty<Assembly>();
}
