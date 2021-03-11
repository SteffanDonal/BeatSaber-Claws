using Claws.Modifiers;
using SiraUtil.Interfaces;
using Zenject;

namespace Claws.Installers
{
    internal class SaberModelInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (!Plugin.IsEnabled)
                return;

            Container.Bind<IModelProvider>().To<ClawsSaberModelProvider>().AsSingle();
        }
    }
}
