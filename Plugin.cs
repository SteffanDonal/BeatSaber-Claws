using Harmony;
using IPA;
using IPALogger = IPA.Logging.Logger;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: AssemblyTitle("Claws")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2019 Steffan Donal")]

[assembly: Guid("a563479b-6b8d-41f0-9a23-cdc396dd9cf0")]

namespace Claws
{
    public class Plugin : IBeatSaberPlugin
    {
        const string IsEnabledPreference = @"Claws.Plugin.IsEnabled";

        internal const string CapabilityName = @"Claws";

        static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;


        /// <summary>
        /// True if the mod is enabled in-game.
        /// </summary>
        public static bool IsEnabled { get; internal set; }


        static bool _isInitialized;

        static Gamemode _gamemode;

        internal static Sprite IconSprite { get; private set; }

        static Texture2D _iconTexture;
        public static IPALogger Log { get; internal set; }

        public void Init(object thisIsNull, IPALogger log)
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

            LoadIcon();

            RestorePlayerPrefs();
            Preferences.Invalidate();

            _gamemode = new Gamemode();

            Log.Info($"v{Version} loaded!");
        }

        void IBeatSaberPlugin.OnApplicationQuit()
        {
            StorePlayerPrefs();
        }


        static void StorePlayerPrefs()
        {
            Log.Info("Storing plugin preferences...");

            PlayerPrefs.SetInt(IsEnabledPreference, IsEnabled ? 1 : 0);
            PlayerPrefs.Save();

            Log.Info("Stored!");
        }
        static void RestorePlayerPrefs()
        {
            Log.Info("Loading plugin preferences...");

            IsEnabled = PlayerPrefs.GetInt(IsEnabledPreference, 0) != 0;

            var pluginState = IsEnabled ? "enabled" : "disabled";
            Log.Info($"Loaded! Plugin is {pluginState}.");
        }

        static void LoadIcon()
        {
            _iconTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            _iconTexture.LoadImage(LoadResource("GamemodeIcon.png"));

            IconSprite = Sprite.Create(
                _iconTexture,
                new Rect(0, 0, _iconTexture.width, _iconTexture.height),
                Vector2.one * 0.5f
            );
        }

        internal static byte[] LoadResource(string resourceName)
        {
            resourceName = @"Claws.Resources." + resourceName;

            Log.Info($"Loading embedded resource: {resourceName}");

            using (var resourceStream = Assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null) return null;

                var resource = new byte[resourceStream.Length];

                resourceStream.Read(resource, 0, resource.Length);

                Log.Info($"Loaded {resourceName}");

                return resource;
            }
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
