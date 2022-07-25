using UnityEngine;

namespace Claws.Modifiers
{
    [DefaultExecutionOrder(-1)]
    class ClawVisibilityTrackingBehaviour : MonoBehaviour
    {
        SaberType? _forSaberType;
        public SaberType? ForSaberType
        {
            get => _forSaberType;
            set
            {
                _forSaberType = value;
                UpdateVisibilityState();
            }
        }

        void OnDisable() => UpdateVisibilityState();
        void OnEnable() => UpdateVisibilityState();

        void UpdateVisibilityState()
        {
            switch (ForSaberType)
            {
                case SaberType.SaberA:
                    SaberGrip.IsLeftVisible = gameObject.activeInHierarchy;
                    break;

                case SaberType.SaberB:
                    SaberGrip.IsRightVisible = gameObject.activeInHierarchy;
                    break;
            }
        }
    }
}
