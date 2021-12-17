using SiraUtil.Interfaces;
using UnityEngine;
using Logger = IPA.Logging.Logger;

namespace Claws.Modifiers
{
    internal class ClawsModelController : SaberModelController, IColorable
    {
        public Color Color { get; set; }

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

            Color = _colorManager.ColorForSaberType(saber.saberType);
            SetColor();

            _trail = _saber.AddComponent<ClawTrail>();
            _trail.RegisterPrefab(_saberTrail.GetPrivateField<SaberTrailRenderer>("_trailRendererPrefab"));
            _trail.Setup((Color * _initData.trailTintColor).linear, saber.movementData);
            Plugin.Log.Log(Logger.Level.Debug, "_saberTrail has been activated");
        }

        void SetColor()
        {
            foreach (var renderer in _saber.GetComponentsInChildren<Renderer>())
            {
                foreach (var material in renderer.materials)
                {
                    if (material.HasProperty("_Glow") && material.GetFloat("_Glow") > 0 || material.HasProperty("_Bloom") && material.GetFloat("_Bloom") > 0)
                    {
                        material.color = Color;
                    }
                }
            }
        }
    }
}
