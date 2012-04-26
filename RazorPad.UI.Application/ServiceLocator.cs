using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace RazorPad
{
    public static class ServiceLocator
    {
        internal static CompositionContainer Container { get; private set; }

        public static void Initialize()
        {
            if(Container != null)
                return;

            var assemblies = new AggregateCatalog(
                    new AssemblyCatalog(Assembly.Load("RazorPad.Core")),
                    new AssemblyCatalog(Assembly.Load("RazorPad.UI")),
                    new AssemblyCatalog(Assembly.Load("RazorPad"))
                );

            var plugins = GetPluginsCatalog();

            Container = new CompositionContainer(new AggregateCatalog(assemblies, plugins));
        }

        public static TService Get<TService>(string name = null)
        {
            if (name != null)
                return Container.GetExportedValue<TService>(name);

            return Container.GetExportedValue<TService>();
        }

        public static IEnumerable<TService> GetMany<TService>(string name = null)
        {
            if (name != null)
                return Container.GetExportedValues<TService>(name);

            return Container.GetExportedValues<TService>();
        }

        public static void Inject(object target)
        {
            Container.ComposeParts(target);
        }

        private static DirectoryCatalog GetPluginsCatalog()
        {
            string pluginPath = Path.Combine(Environment.CurrentDirectory, "plugins");

            if (!Directory.Exists(pluginPath))
                Directory.CreateDirectory(pluginPath);

            var plugins = new DirectoryCatalog(pluginPath);
            return plugins;
        }
    }
}
