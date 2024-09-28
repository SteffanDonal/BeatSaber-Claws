using BeatSaberMarkupLanguage.GameplaySetup;
using Claws.Views;
using Zenject;

namespace Claws
{
    internal class Gamemode : IInitializable
    {
        readonly GamemodeSettingsViewController _gamemodeSettingsView;

        internal Gamemode()
        {
            _gamemodeSettingsView = new GamemodeSettingsViewController();
        }

        public void Initialize()
        {
            GameplaySetup.Instance.AddTab(
                "Claws",
                GamemodeSettingsViewController.Resource,
                _gamemodeSettingsView
            );
        }
    }
}
