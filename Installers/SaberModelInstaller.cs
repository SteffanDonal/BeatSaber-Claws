using Claws.Modifiers;
using SiraUtil.Interfaces;
using SiraUtil.Sabers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace Claws.Installers
{
    class SaberModelInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (!Plugin.IsEnabled)
                return;

            Container.Bind<IModelProvider>().To<ClawsSaberModelProvider>().AsSingle();
        }
    }
}
