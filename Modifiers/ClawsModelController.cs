using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using Logger = IPA.Logging.Logger;

namespace Claws.Modifiers
{
    internal class ClawsModelController : SaberModelController, IColorable, IPreSaberModelInit
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

                if (LeftSaber != null && RightSaber != null)
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

        [InjectOptional] readonly InitData _initData = new InitData();
        [Inject] readonly ColorManager _colorManager;

        MaterialPropertyBlock _materialPropertyBlock;

        public bool PreInit(Transform parent, Saber saber)
        {
            _saber = saber;

            _saberContainer = Instantiate(
                _saber.saberType == SaberType.SaberA
                    ? LeftSaber
                    : RightSaber,
                parent, false);
            gameObject.transform.SetParent(parent);

            _saberContainer.transform.localScale = new Vector3(1, 1, Preferences.Length / ClawsModelScale);

            var saberTrail = this.GetPrivateField<SaberTrail>(typeof(SaberModelController), "_saberTrail");

            _trail = _saberContainer.AddComponent<ClawTrail>();
            _trail.RegisterPrefab(saberTrail.GetPrivateField<SaberTrailRenderer>("_trailRendererPrefab"));

            Color = _colorManager.ColorForSaberType(_saber.saberType);

            ApplyHitboxMod();

            var saberHand = _saber.saberType == SaberType.SaberA ? "Left" : "Right";
            Plugin.Log.Log(Logger.Level.Debug, $"{saberHand} claw initialized.");

            return false;
        }

        void SetColor(Color color)
        {
            _color = color;

            _trail.Setup((_color * _initData.trailTintColor).linear, _saber.movementData);

            if (_materialPropertyBlock == null)
                _materialPropertyBlock = new MaterialPropertyBlock();

            _materialPropertyBlock.SetColor("_Color", color);

            foreach (var renderer in _saberContainer.GetComponentsInChildren<Renderer>())
                renderer.SetPropertyBlock(_materialPropertyBlock);
        }

        void ApplyHitboxMod()
        {
            var saberTop = _saber.GetPrivateField<Transform>("_saberBladeTopTransform");
            var saberBottom = _saber.GetPrivateField<Transform>("_saberBladeBottomTransform");

            saberTop.localPosition = new Vector3(saberTop.localPosition.x, saberTop.localPosition.y, saberBottom.localPosition.z + Preferences.Length);
        }
    }
}
