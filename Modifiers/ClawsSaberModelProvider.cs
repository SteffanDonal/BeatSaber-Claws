using System;
using SiraUtil.Interfaces;
using UnityEngine;
using Logger = IPA.Logging.Logger;

namespace Claws.Modifiers
{
    internal class ClawsSaberModelProvider : IModelProvider
    {
        public Type Type => typeof(ClawsSaberModelController);
        public int Priority => int.MaxValue;
    }

    internal class ClawsSaberModelController : SaberModelController, IColorable
    {
        Color? _color;

        GameObject _saber;
        ClawTrail trail;

        public override void Init(Transform parent, Saber saber)
        {
            if (saber.saberType == SaberType.SaberA)
            {
                _saber = Instantiate(Plugin.LeftSaber, parent, false);
            }
            else
            {
                _saber = Instantiate(Plugin.RightSaber, parent, false);
            }

            _saber.transform.localScale = new Vector3(1, 1, Preferences.Length / 0.30f);

            SetColor(_color ?? _colorManager.ColorForSaberType(saber.saberType));

            trail = _saber.AddComponent<ClawTrail>();
            trail.RegisterPrefab(_saberTrail.GetPrivateField<SaberTrailRenderer>("_trailRendererPrefab"));
            trail.Setup(_color ?? _colorManager.ColorForSaberType(saber.saberType), saber.movementData);
            Plugin.Log.Log(Logger.Level.Debug, "_saberTrail has been activated");
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
