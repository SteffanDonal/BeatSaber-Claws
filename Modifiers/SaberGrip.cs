using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace Claws.Modifiers
{
    internal static class SaberGrip
    {
        internal static bool IsInGame { get; set; }

        internal static void AdjustControllerTransform(
            IVRPlatformHelper platformHelper,
            XRNode node, Transform transform
        )
        {
            if (!Plugin.IsEnabled) return;
            if (!IsInGame) return;

            switch (node)
            {
                case XRNode.LeftHand:
                    transform.Translate(Preferences.LeftTranslation);
                    transform.Rotate(Preferences.LeftRotation);
                    break;

                case XRNode.RightHand:
                    transform.Translate(Preferences.RightTranslation);
                    transform.Rotate(Preferences.RightRotation);
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(DevicelessVRHelper))]
    [HarmonyPatch(nameof(IVRPlatformHelper.AdjustControllerTransform))]
    class SaberGripDevicelessControllerPatch
    {
        static void Postfix(
            DevicelessVRHelper __instance,
            XRNode node, Transform transform,
            Vector3 position, Vector3 rotation
        ) => SaberGrip.AdjustControllerTransform(__instance, node, transform);
    }

    [HarmonyPatch(typeof(OculusVRHelper))]
    [HarmonyPatch(nameof(IVRPlatformHelper.AdjustControllerTransform))]
    class SaberGripOculusControllerPatch
    {
        static void Postfix(
            OculusVRHelper __instance,
            XRNode node, Transform transform,
            Vector3 position, Vector3 rotation
        ) => SaberGrip.AdjustControllerTransform(__instance, node, transform);
    }

    [HarmonyPatch(typeof(OpenVRHelper))]
    [HarmonyPatch(nameof(IVRPlatformHelper.AdjustControllerTransform))]
    class SaberGripOpenVRControllerPatch
    {
        static void Postfix(
            OpenVRHelper __instance,
            XRNode node, Transform transform,
            Vector3 position, Vector3 rotation
        ) => SaberGrip.AdjustControllerTransform(__instance, node, transform);
    }
}
