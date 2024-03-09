using Claws.Modifiers;
using IPA;
using SiraUtil.Sabers;
using SiraUtil.Zenject;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using IPALogger = IPA.Logging.Logger;

[assembly: AssemblyTitle("Claws")]
[assembly: AssemblyFileVersion("1.10.2")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2022 Steffan Donal")]

[assembly: Guid("a563479b-6b8d-41f0-9a23-cdc396dd9cf0")]

namespace Claws
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal const string CapabilityName = @"Claws";
        internal const string ClawsSaberResourceName = "Claws.Claws.saber";

        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

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

            ClawsModelController.LoadSaberAsset();

            zenjector.Install(Location.Player, container =>
            {
                if (!IsEnabled) return;
                container.BindInstance(SaberModelRegistration.Create<ClawsModelController>(int.MaxValue));
            });
        }

        [OnStart]
        public void OnStart()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"Plugin had {nameof(OnStart)} called more than once! Critical failure.");

            _isInitialized = true;

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
