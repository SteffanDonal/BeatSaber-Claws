using SiraUtil.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Claws.Modifiers
{
    class ClawsSaberModelProvider : IModelProvider
    {
        public Type Type => typeof(ClawsSaberModelController);
        public int Priority => int.MaxValue;
    }

    class ClawsSaberModelController : SaberModelController, IColorable
    {
        private Color? _color;

        private GameObject _saber;
        private ClawTrail trail;

        public override void Init(Transform parent, Saber saber)
        {
            if (saber.saberType == SaberType.SaberA)
            {
                _saber = UnityEngine.Object.Instantiate(Plugin.LeftSaber, parent, false);
            }
            else
            {
                _saber = UnityEngine.Object.Instantiate(Plugin.RightSaber, parent, false);
            }

            _saber.transform.localScale = new Vector3(1, 1, Preferences.Length / 0.30f);

            SetColor(_color ?? _colorManager.ColorForSaberType(saber.saberType));

            this.trail = _saber.AddComponent<ClawTrail>();
            this.trail.RegisterPrefab(this._saberTrail.GetPrivateField<SaberTrailRenderer>("_trailRendererPrefab"));
            this.trail.Setup(_color ?? _colorManager.ColorForSaberType(saber.saberType), saber.movementData);
            Plugin.Log.Log(IPA.Logging.Logger.Level.Debug, "_saberTrail has been activated");
        }

        public void SetColor(Color color)
        {
            _color = color;

            foreach (var renderer in _saber.GetComponentsInChildren<Renderer>())
            {
                foreach (var material in renderer.materials)
                {
                    if (material.HasProperty("_Glow") && material.GetFloat("_Glow") > 0 || material.HasProperty("_Bloom") && material.GetFloat("_Bloom") > 0)
                    {
                        material.color = color;
                    }
                }
            }
        }

        public Color Color => _color.GetValueOrDefault();
    }
}
