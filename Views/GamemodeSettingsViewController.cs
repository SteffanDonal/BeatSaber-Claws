using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace Claws.Views
{
    public class GamemodeSettingsViewController : BSMLResourceViewController
    {
        public const string Resource = "Claws.Views.GamemodeSettings.bsml";

        public override string ResourceName => Resource;

        public event EventHandler IsEnabledChanged;

        [UIValue("isEnabled")]
        public bool IsEnabled
        {
            get => Preferences.IsEnabled;
            set
            {
                Preferences.IsEnabled = value;
                IsEnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
