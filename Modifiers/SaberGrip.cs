using Harmony;
using UnityEngine;
using UnityEngine.XR;

namespace Claws.Modifiers
{
    internal class SaberGrip
    {
        internal static VRController LeftSaber;
        internal static VRController RightSaber;

        internal void ApplyToGameCore(GameObject gameCore)
        {
            LeftSaber = null;
            RightSaber = null;

            Plugin.Log.Info("Setting up grip adjustments...");

            var saberManagerObj = gameCore.transform
                .Find("Origin")
                ?.Find("VRGameCore")
                ?.Find("SaberManager");

            if (saberManagerObj == null)
            {
                Plugin.Log.Critical("Couldn't find SaberManager, bailing!");
                return;
            }

            var saberManager = saberManagerObj.GetComponent<SaberManager>();

            LeftSaber = saberManager.GetPrivateField<Saber>("_leftSaber").GetComponent<VRController>();
            RightSaber = saberManager.GetPrivateField<Saber>("_rightSaber").GetComponent<VRController>();

            if (LeftSaber is null || RightSaber is null)
            {
                Plugin.Log.Critical("Sabers couldn't be found. Bailing!");
                return;
            }

            Plugin.Log.Info("Grip adjustments ready!");
        }
    }

    [HarmonyPatch(typeof(VRController))]
    [HarmonyPatch("UpdatePositionAndRotation")]
    class SaberGripVRControllerPatch
    {
        static bool Prefix(VRController __instance)
        {
            if (!Plugin.IsEnabled) return true;
            if (!ReferenceEquals(__instance, SaberGrip.LeftSaber) &&
                !ReferenceEquals(__instance, SaberGrip.RightSaber)) return true;

            Vector3 translation;
            Vector3 rotation;

            if (ReferenceEquals(__instance, SaberGrip.LeftSaber))
            {
                translation = Preferences.LeftTranslation;
                rotation = Preferences.LeftRotation;
            }
            else
            {
                translation = Preferences.RightTranslation;
                rotation = Preferences.RightRotation;
            }

            // Begin Default Behaviour

            var node = __instance.GetPrivateField<XRNode>("_node");
            var lastTrackedPosition = __instance.GetPrivateField<Vector3>("_lastTrackedPosition");

            var localPosition = InputTracking.GetLocalPosition(node);
            var localRotation = InputTracking.GetLocalRotation(node);
            if (localPosition == Vector3.zero)
            {
                if (lastTrackedPosition != Vector3.zero)
                {
                    localPosition = lastTrackedPosition;
                }
                else if (node == XRNode.LeftHand)
                {
                    localPosition = new Vector3(-0.2f, 0.05f, 0f);
                }
                else if (node == XRNode.RightHand)
                {
                    localPosition = new Vector3(0.2f, 0.05f, 0f);
                }
            }
            else
            {
                __instance.SetPrivateField("_lastTrackedPosition", localPosition);
            }

            __instance.transform.localPosition = localPosition;
            __instance.transform.localRotation = localRotation;

            // End Default Behaviour

            __instance.transform.Translate(translation);
            __instance.transform.Rotate(rotation);

            return false;
        }
    }
}
