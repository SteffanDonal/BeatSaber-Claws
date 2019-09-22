using Claws.Modifiers;
using CustomUI.GameplaySettings;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Claws
{
    internal class Gamemode
    {
        readonly SaberGrip _saberGrip = new SaberGrip();
        readonly SaberLength _saberLength = new SaberLength();
        string _lastSelectedSaber;
        ToggleOption _gamemodeToggle;

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
            if (_gamemodeToggle != null)
            {
                _gamemodeToggle.OnToggle -= OnGamemodeToggled;
            }

            _gamemodeToggle = GameplaySettingsUI.CreateToggleOption(
                GameplaySettingsPanels.ModifiersLeft,
                "Claws Mode",
                hintText: "Shortens saber hitboxes to 0.3m, and adjusts grip.",
                optionIcon: Plugin.IconSprite
            );

            _gamemodeToggle.GetValue = Plugin.IsEnabled;
            _gamemodeToggle.OnToggle += OnGamemodeToggled;

            UpdateCapability();
        }

        void OnGamemodeToggled(bool isEnabled)
        {
            Preferences.IsEnabled = isEnabled;

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
                Plugin.Log.Info("Switching sabers from '" + CustomSaber.Plugin._currentSaberName + "' to Claws! ");

                _lastSelectedSaber = CustomSaber.Plugin._currentSaberName;
                CustomSaber.Plugin._currentSaberName = Plugin.ClawsSaberName;
            }
            else
            {
                if (string.IsNullOrEmpty(_lastSelectedSaber)) return; // Don't reset to nothing.
                if (CustomSaber.Plugin._currentSaberName != Plugin.ClawsSaberName) return; // Don't reset unless it's our saber.

                Plugin.Log.Info("Switching sabers back to '" + _lastSelectedSaber + "' from Claws! ");

                CustomSaber.Plugin._currentSaberName = _lastSelectedSaber;
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
