using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Utilities;
using Claws.Modifiers;
using Claws.Views;
using SongCore;

namespace Claws
{
    internal class Gamemode
    {
        readonly GamemodeSettingsViewController _gamemodeSettingsView;

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
        }

        void UpdateCapability()
        {
            if (Plugin.IsEnabled)
                Collections.RegisterCapability(Plugin.CapabilityName);
            else
                Collections.DeregisterizeCapability(Plugin.CapabilityName);
        }
    }
}
