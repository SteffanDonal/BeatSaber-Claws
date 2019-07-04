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
        string LastSelectedSaber;
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
            Plugin.IsEnabled = isEnabled;

            SwitchSaber();
            UpdateLength();
            UpdateCapability();
        }

        void UpdateLength()
        {
            if(Plugin.IsEnabled)
               _saberLength.SetLength(Preferences.Length);
            else
               _saberLength.SetLength(1.0f);
        }

        void SwitchSaber()
        {
            if (Plugin.IsEnabled)
            { 
                Plugin.Log.Info("switching sabers from" + CustomSaber.Plugin._currentSaberPath + "to Claws! ");
                LastSelectedSaber = CustomSaber.Plugin._currentSaberPath;
                CustomSaber.Plugin._currentSaberPath = Plugin.ClawsSaberPath;
            }
            else if (!string.IsNullOrEmpty(LastSelectedSaber) &&                  // If we have the previous saber
                Plugin.ClawsSaberPath == CustomSaber.Plugin._currentSaberPath)    // And we're the owners of the state (Claws is selected)
                    CustomSaber.Plugin._currentSaberPath = LastSelectedSaber;     // Change the saber back
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
