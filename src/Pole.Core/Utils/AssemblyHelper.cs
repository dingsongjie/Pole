using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Pole.Core.Utils
{
    public class AssemblyHelper
    {
        private static IList<Assembly> Assemblies;
        public static IList<Assembly> GetAssemblies(ILogger logger = default)
        {
            if (Assemblies != null)
            {
                return Assemblies;
            }
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && !lib.Name.StartsWith("Microsoft") && !lib.Name.StartsWith("System"));
            Assemblies = libs.Select(lib =>
              {
                  try
                  {
                      return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                  }
                  catch (Exception ex)
                  {
                      if (logger != default)
                          logger.LogWarning(ex, ex.Message);
                      return default;
                  }
              }).Where(assembly => assembly != default).ToList();
            return Assemblies;
        }
    }
}
