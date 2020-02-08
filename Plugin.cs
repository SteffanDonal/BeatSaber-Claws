using Harmony;
using IPA;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

[assembly: AssemblyTitle("Claws")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2019 Steffan Donal")]

[assembly: Guid("a563479b-6b8d-41f0-9a23-cdc396dd9cf0")]

namespace Claws
{
    public class Plugin : IBeatSaberPlugin
    {
        internal const string CapabilityName = @"Claws";
        internal const string ClawsSaberName = "Claws.saber";
        internal const string DefaultSaberName = "DefaultSabers";

        static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;


        /// <summary>
        /// True if the mod is enabled in-game.
        /// </summary>
        public static bool IsEnabled => Preferences.IsEnabled;

        public static IPALogger Log { get; internal set; }


        static bool _isInitialized;

        static Gamemode _gamemode;

        public void Init(object _, IPALogger log)
        {
            Log = log;
        }

        void IBeatSaberPlugin.OnApplicationStart()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"Plugin had {nameof(IBeatSaberPlugin.OnApplicationStart)} called more than once! Critical failure.");

            _isInitialized = true;

            try
            {
                var harmony = HarmonyInstance.Create("com.github.steffandonal.claws");
                harmony.PatchAll(Assembly);
            }
            catch (Exception e)
            {
                Log.Error("This plugin requires Harmony. Make sure you installed the plugin properly, as the Harmony DLL should have been installed with it.");
                Log.Error(e.ToString());

                return;
            }

            Preferences.Restore();
            Preferences.Invalidate();

            _gamemode = new Gamemode();

            Log.Info($"v{Version} loaded!");
        }

        void IBeatSaberPlugin.OnApplicationQuit()
        {
            Preferences.Store();
        }


        #region Unused IPlugin Members

        void IBeatSaberPlugin.OnUpdate() { }
        void IBeatSaberPlugin.OnFixedUpdate() { }
        void IBeatSaberPlugin.OnActiveSceneChanged(Scene from, Scene to) { }
        void IBeatSaberPlugin.OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
        void IBeatSaberPlugin.OnSceneUnloaded(Scene scene) { }

        #endregion
    }
}
