using BeatSaberMarkupLanguage.GameplaySetup;
using Claws.Modifiers;
using Claws.Views;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Claws
{
    internal class Gamemode
    {
        readonly SaberGrip _saberGrip = new SaberGrip();
        readonly SaberLength _saberLength = new SaberLength();
        GamemodeSettingsViewController _gamemodeSettingsView;

        string CurrentCustomSaber
        {
            get => CustomSaber.Settings.Configuration.CurrentlySelectedSaber;
            set
            {
                var sabers = CustomSaber.Utilities.SaberAssetLoader.CustomSabers.ToList();
                var saberIndex = sabers.FindIndex(saber => saber.FileName.Equals(value, StringComparison.InvariantCultureIgnoreCase));

                if (saberIndex != -1)
                    typeof(CustomSaber.Utilities.SaberAssetLoader).SetStaticMember("SelectedSaber", saberIndex);

                typeof(CustomSaber.Settings.Configuration).SetStaticMember("CurrentlySelectedSaber", value);
            }
        }

        internal Gamemode()
        {
            SceneManager.sceneLoaded += OnSceneManagerSceneLoaded;
        }

        void OnSceneManagerSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            if (loadedScene.name == "GameCore") OnGameLoaded(loadedScene);
            if (loadedScene.name == "MenuCore") OnMenuLoaded();
        }

        void OnGameLoaded(Scene loadedScene)
        {
            Preferences.Invalidate();

            var gameCore = loadedScene.GetRootGameObjects().First();

            _saberGrip.ApplyToGameCore(gameCore);
            _saberLength.ApplyToGameCore(gameCore);

            UpdateLength();
        }

        void OnMenuLoaded()
        {
            _gamemodeSettingsView = new GamemodeSettingsViewController();
            _gamemodeSettingsView.IsEnabledChanged += OnGamemodeToggled;

            GameplaySetup.instance.AddTab(
                "Claws",
                GamemodeSettingsViewController.Resource,
                _gamemodeSettingsView
            );

            UpdateCapability();
        }

        void OnGamemodeToggled(object sender, EventArgs e)
        {
            SwitchSaber();
            UpdateLength();
            UpdateCapability();
        }

        void UpdateLength()
        {
            if (Plugin.IsEnabled)
                _saberLength.SetLength(Preferences.Length);
            else
                _saberLength.SetLength(1.0f);
        }

        void SwitchSaber()
        {
            if (Plugin.IsEnabled)
            {
                Plugin.Log.Info($"Switching sabers from '{CurrentCustomSaber}' to {Plugin.ClawsSaberName}!");

                Preferences.LastCustomSaber = CurrentCustomSaber;
                CurrentCustomSaber = Plugin.ClawsSaberName;
            }
            else
            {
                if (CurrentCustomSaber != Plugin.ClawsSaberName) return; // Don't reset unless it's our saber.

                var lastSaber = !string.IsNullOrEmpty(Preferences.LastCustomSaber)
                    ? Preferences.LastCustomSaber
                    : Plugin.DefaultSaberName;

                Plugin.Log.Info($"Switching sabers back to '{lastSaber}' from {Plugin.ClawsSaberName}!");

                CurrentCustomSaber = lastSaber;
            }
        }

        void UpdateCapability()
        {
            if (Plugin.IsEnabled)
                SongCore.Collections.RegisterCapability(Plugin.CapabilityName);
            else
                SongCore.Collections.DeregisterizeCapability(Plugin.CapabilityName);
        }
    }
}
