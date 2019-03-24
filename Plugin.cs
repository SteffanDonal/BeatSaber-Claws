using IllusionPlugin;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Claws")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2019 Steffan Donal")]

[assembly: Guid("a563479b-6b8d-41f0-9a23-cdc396dd9cf0")]

namespace Claws
{
    public class Plugin : IPlugin
    {
        static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        void IPlugin.OnApplicationStart()
        {
            Log($"v{Version} loaded!");
        }

        void IPlugin.OnApplicationQuit()
        {

        }


        internal static void Log(string message)
        {
            Console.WriteLine($"[{Name}] {message}");
        }


        #region Unused IPlugin Members

        string IPlugin.Name => Name;
        string IPlugin.Version => Version;

        void IPlugin.OnUpdate() { }
        void IPlugin.OnFixedUpdate() { }
        void IPlugin.OnLevelWasLoaded(int level) { }
        void IPlugin.OnLevelWasInitialized(int level) { }

        #endregion
    }
}
