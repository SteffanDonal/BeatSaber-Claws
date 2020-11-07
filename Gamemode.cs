using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Utilities;
using Claws.Modifiers;
using Claws.Views;
using System;

namespace Claws
{
    internal class Gamemode
    {
        GamemodeSettingsViewController _gamemodeSettingsView;

        internal Gamemode()
        {
            _gamemodeSettingsView = new GamemodeSettingsViewController();
            _gamemodeSettingsView.IsEnabledChanged += OnGamemodeToggled;

            GameplaySetup.instance.AddTab(
                "Claws",
                GamemodeSettingsViewController.Resource,
                _gamemodeSettingsView
            );

            BSEvents.OnLoad();
            BSEvents.gameSceneActive += OnGameActivated;
            BSEvents.menuSceneActive += OnMenuActivated;
        }

        void OnMenuActivated()
        {
            UpdateCapability();
            SaberGrip.IsInGame = false;
        }

        void OnGameActivated()
        {
            Preferences.Invalidate();
            SaberGrip.IsInGame = true;
        }

        void OnGamemodeToggled(object sender, EventArgs e)
        {
            UpdateCapability();
            SaberLength.ApplyToSabers();
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
