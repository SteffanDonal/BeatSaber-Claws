using Claws.Installers;
using HarmonyLib;
using IPA;
using SiraUtil.Zenject;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

[assembly: AssemblyTitle("Claws")]
[assembly: AssemblyFileVersion("1.3.1")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2020 Steffan Donal")]

[assembly: Guid("a563479b-6b8d-41f0-9a23-cdc396dd9cf0")]

namespace Claws
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal const string CapabilityName = @"Claws";
        internal const string ClawsSaberName = "Claws.Claws.saber";
        internal const string DefaultSaberName = "DefaultSabers";

        static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        static internal GameObject LeftSaber;
        static internal GameObject RightSaber;

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;


        /// <summary>
        /// True if the mod is enabled in-game.
        /// </summary>
        public static bool IsEnabled => Preferences.IsEnabled;

        public static IPALogger Log { get; internal set; }


        static bool _isInitialized;

        static Gamemode _gamemode;

        [Init]
        public void Init(object _, IPALogger log, Zenjector zenjector)
        {
            Log = log;

            GameObject sabers;

            using (var stream = Assembly.GetManifestResourceStream(ClawsSaberName))
            {
                sabers = AssetBundle.LoadFromStream(stream).LoadAsset<GameObject>("_CustomSaber");
            }

            foreach (Transform t in sabers.transform)
            {
                if (t.name == "LeftSaber")
                    LeftSaber = t.gameObject;
                else if (t.name == "RightSaber")
                    RightSaber = t.gameObject;
                if (LeftSaber != null && RightSaber != null)
                    break;
            }

            zenjector.OnGame<SaberModelInstaller>();
        }

        [OnStart]
        public void OnStart()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"Plugin had {nameof(OnStart)} called more than once! Critical failure.");

            _isInitialized = true;

            try
            {
                var harmony = new Harmony("com.github.steffandonal.claws");
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

        [OnExit]
        public void OnExit()
        {
            Preferences.Store();
        }
    }
}
