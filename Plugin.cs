using Harmony;
using IllusionPlugin;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

[assembly: AssemblyTitle("Claws")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2019 Steffan Donal")]

[assembly: Guid("a563479b-6b8d-41f0-9a23-cdc396dd9cf0")]

namespace Claws
{
    public class Plugin : IPlugin
    {
        const string IsEnabledPreference = @"Claws.Plugin.IsEnabled";

        static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;


        /// <summary>
        /// True if the mod is enabled in-game.
        /// </summary>
        public static bool IsEnabled { get; internal set; }


        static bool _isInitialized;


        void IPlugin.OnApplicationStart()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"Plugin had {nameof(IPlugin.OnApplicationStart)} called more than once! Critical failure.");

            _isInitialized = true;

            try
            {
                var harmony = HarmonyInstance.Create("com.github.steffandonal.claws");
                harmony.PatchAll(Assembly);
            }
            catch (Exception e)
            {
                Log("This plugin requires Harmony. Make sure you installed the plugin properly, as the Harmony DLL should have been installed with it.");
                Log(e.ToString());

                return;
            }

            RestorePlayerPrefs();
            Preferences.Invalidate();

            Log($"v{Version} loaded!");
        }

        void IPlugin.OnApplicationQuit()
        {
            StorePlayerPrefs();
        }


        static void StorePlayerPrefs()
        {
            PlayerPrefs.SetInt(IsEnabledPreference, IsEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
        static void RestorePlayerPrefs()
        {
            IsEnabled = PlayerPrefs.GetInt(IsEnabledPreference, 0) != 0;
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
