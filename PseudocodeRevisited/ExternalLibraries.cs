using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PseudocodeRevisited
{
    public partial class ExecutionState
    {
        private Dictionary<string, Library> LoadedLibraries;
        /// <summary>
        /// Gets a loaded library from cache or loads it from a file.
        /// </summary>
        public Library LoadLibrary(string path)
        {
            Library result;
            if (LoadedLibraries.TryGetValue(path, out result))
                return result;
            if (path == "builtins")
            {
                result = CreateBuiltInsLibrary();
            }
            else
            {
                try
                {
                    Assembly loadedAssembly = Assembly.LoadFile(path);
                    result = new Library();
                    var candidates = loadedAssembly.GetExportedTypes()
                        .Where((t) => typeof(IExternalLibrary).IsAssignableFrom(t));
                    foreach (var candidateType in candidates)
                    {
                        IExternalLibrary libMaker = (IExternalLibrary)Activator.CreateInstance(candidateType);
                        libMaker.Populate(result);
                    }
                }
                catch (Exception)
                {
                    throw new RuntimeException("Unable to load library " + path);
                }
            }
            LoadedLibraries.Add(path, result);
            return result;
        }
    }
    public interface IExternalLibrary
    {
        void Populate(Library lib);
    }
}
