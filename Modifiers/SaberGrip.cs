using HarmonyLib;
using UnityEngine;

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
    [HarmonyPatch("Update")]
    class SaberGripVRControllerPatch
    {
        static void Postfix(VRController __instance)
        {
            if (!Plugin.IsEnabled) return;
            if (!ReferenceEquals(__instance, SaberGrip.LeftSaber) &&
                !ReferenceEquals(__instance, SaberGrip.RightSaber)) return;

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

            __instance.transform.Translate(translation);
            __instance.transform.Rotate(rotation);
        }
    }
}
