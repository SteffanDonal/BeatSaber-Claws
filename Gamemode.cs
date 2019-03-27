using Claws.Modifiers;
using CustomUI.GameplaySettings;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Claws
{
    internal class Gamemode
    {
        readonly SaberGrip _saberGrip = new SaberGrip();
        readonly SaberLength _saberLength = new SaberLength();

        ToggleOption _gamemodeToggle;

        Texture2D _gamemodeIconTexture;
        Sprite _gamemodeIconSprite;

        internal Gamemode()
        {
            SceneManager.sceneLoaded += OnSceneManagerSceneLoaded;

            _gamemodeIconTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            _gamemodeIconTexture.LoadImage(Plugin.LoadResource("GamemodeIcon.png"));

            _gamemodeIconSprite = Sprite.Create(
                _gamemodeIconTexture,
                new Rect(0, 0, _gamemodeIconTexture.width, _gamemodeIconTexture.height),
                Vector2.one * 0.5f
            );
        }

        void OnSceneManagerSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode)
        {
            if (loadedScene.name == "GameCore") OnGameLoaded(loadedScene);
            if (loadedScene.name == "MenuCore") OnMenuLoaded(loadedScene);
        }

        void OnGameLoaded(Scene loadedScene)
        {
            Preferences.Invalidate();

            var gameCore = loadedScene.GetRootGameObjects().First();

            _saberGrip.ApplyToGameCore(gameCore);
            _saberLength.ApplyToGameCore(gameCore);

            if (Plugin.IsEnabled)
            {
                _saberLength.SetLength(Preferences.Length);
            }
        }

        void OnMenuLoaded(Scene loadedScene)
        {
            if (_gamemodeToggle != null)
            {
                _gamemodeToggle.OnToggle -= OnGamemodeToggled;
            }

            _gamemodeToggle = GameplaySettingsUI.CreateToggleOption(
                GameplaySettingsPanels.ModifiersLeft,
                "Claws Mode",
                hintText: "Shortens saber hitboxes to 0.3m, and adjusts grip.",
                optionIcon: _gamemodeIconSprite
            );

            _gamemodeToggle.GetValue = Plugin.IsEnabled;
            _gamemodeToggle.OnToggle += OnGamemodeToggled;
        }

        void OnGamemodeToggled(bool isEnabled)
        {
            Plugin.IsEnabled = isEnabled;
        }
    }
}
