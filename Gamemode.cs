using BeatSaberMarkupLanguage.GameplaySetup;
using Claws.Views;

namespace Claws
{
    internal class Gamemode
    {
        readonly GamemodeSettingsViewController _gamemodeSettingsView;

        internal Gamemode()
        {
            _gamemodeSettingsView = new GamemodeSettingsViewController();

            GameplaySetup.instance.AddTab(
                "Claws",
                GamemodeSettingsViewController.Resource,
                _gamemodeSettingsView
            );
        }
    }
}
