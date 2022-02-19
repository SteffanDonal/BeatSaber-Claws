using SiraUtil.Interfaces;
using UnityEngine;
using Logger = IPA.Logging.Logger;

namespace Claws.Modifiers
{
    internal class ClawsModelController : SaberModelController, IColorable
    {
        const float ClawsModelScale = 0.3f;

        #region Saber Asset Management
        internal static GameObject LeftSaber;
        internal static GameObject RightSaber;

        internal static void LoadSaberAsset()
        {
            GameObject sabers;

            using (var stream = Plugin.Assembly.GetManifestResourceStream(Plugin.ClawsSaberResourceName))
            {
                sabers = AssetBundle.LoadFromStream(stream).LoadAsset<GameObject>("_CustomSaber");
            }

            foreach (Transform t in sabers.transform)
            {
                if (t.name == "LeftSaber")
                    LeftSaber = t.gameObject;
                else if (t.name == "RightSaber")
                    RightSaber = t.gameObject;

                if (LeftSaber != null && ClawsModelController.RightSaber != null)
                    break;
            }
        }
        #endregion

        Color _color;
        public Color Color
        {
            get => _color;
            set => SetColor(value);
        }

        Saber _saber;
        GameObject _saberContainer;
        ClawTrail _trail;
        ClawVisibilityTrackingBehaviour _visibilityTracking;

        public override void Init(Transform parent, Saber saber)
        {
            _saber = saber;

            _saberContainer = Instantiate(
                _saber.saberType == SaberType.SaberA
                    ? LeftSaber
                    : RightSaber,
                parent, false);
            gameObject.transform.SetParent(parent);

            _saberContainer.transform.localScale = new Vector3(1, 1, Preferences.Length / ClawsModelScale);

            _trail = _saberContainer.AddComponent<ClawTrail>();
            _trail.RegisterPrefab(_saberTrail.GetPrivateField<SaberTrailRenderer>("_trailRendererPrefab"));

            _visibilityTracking = _saberContainer.AddComponent<ClawVisibilityTrackingBehaviour>();
            _visibilityTracking.ForSaberType = _saber.saberType;

            Color = _colorManager.ColorForSaberType(_saber.saberType);

            ApplyHitboxMod();

            var saberHand = _saber.saberType == SaberType.SaberA ? "Left" : "Right";
            Plugin.Log.Log(Logger.Level.Debug, $"{saberHand} claw initialized.");
        }

        void SetColor(Color color)
        {
            _color = color;

            _trail.Setup((_color * _initData.trailTintColor).linear, _saber.movementData);

            foreach (var renderer in _saberContainer.GetComponentsInChildren<Renderer>())
                foreach (var material in renderer.materials)
                    if (material.HasProperty("_Glow") && material.GetFloat("_Glow") > 0 ||
                        material.HasProperty("_Bloom") && material.GetFloat("_Bloom") > 0)
                        material.color = _color;
        }

        void ApplyHitboxMod()
        {
            var saberTop = _saber.GetPrivateField<Transform>("_saberBladeTopTransform");
            var saberBottom = _saber.GetPrivateField<Transform>("_saberBladeBottomTransform");

            saberTop.localPosition = new Vector3(saberTop.localPosition.x, saberTop.localPosition.y, saberBottom.localPosition.z + Preferences.Length);
        }
    }
}
