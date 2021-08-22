using SiraUtil.Interfaces;
using System;
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
        ClawTrail _trail;

        public override void Init(Transform parent, Saber saber)
        {
            _saber = Instantiate(
                saber.saberType == SaberType.SaberA
                    ? Plugin.LeftSaber
                    : Plugin.RightSaber,
                parent, false);

            _saber.transform.localScale = new Vector3(1, 1, Preferences.Length / 0.30f);

            SetColor(_color ?? _colorManager.ColorForSaberType(saber.saberType));

            _trail = _saber.AddComponent<ClawTrail>();
            _trail.RegisterPrefab(_saberTrail.GetPrivateField<SaberTrailRenderer>("_trailRendererPrefab"));
            _trail.Setup(((_color ?? _colorManager.ColorForSaberType(saber.saberType)) * _initData.trailTintColor).linear, saber.movementData);
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
