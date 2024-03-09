using UnityEngine;

namespace Claws
{
    internal static class Preferences
    {
        const string IsEnabledPreference = @"Claws.Plugin.IsEnabled";

        public const float Length = 0.3f;

        public static bool IsEnabled { get; internal set; }

        public static void Store()
        {
            Plugin.Log.Info("Storing plugin preferences...");

            PlayerPrefs.SetInt(IsEnabledPreference, IsEnabled ? 1 : 0);
            PlayerPrefs.Save();

            Plugin.Log.Info("Stored!");
        }

        public static void Restore()
        {
            Plugin.Log.Info("Loading plugin preferences...");

            IsEnabled = PlayerPrefs.GetInt(IsEnabledPreference, 0) != 0;

            var pluginState = Plugin.IsEnabled ? "enabled" : "disabled";
            Plugin.Log.Info($"Loaded! Plugin is {pluginState}.");
        }

        public static void Invalidate()
        {
        }
    }
}
