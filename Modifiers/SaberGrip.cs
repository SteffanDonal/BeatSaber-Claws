using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace Claws.Modifiers
{
    internal static class SaberGrip
    {
        internal static bool IsInGame { get; set; }
        internal static bool IsLeftVisible { get; set; }
        internal static bool IsRightVisible { get; set; }

        /// <summary>
        /// When the plugin is enabled, clears player offset preferences when ingame.
        /// </summary>
        internal static void ClearBaseGameOffsets(ref Vector3 position, ref Vector3 rotation)
        {
            if (!Plugin.IsEnabled) return;
            if (!IsInGame) return;

            position = Vector3.zero;
            rotation = Vector3.zero;
        }

        /// <summary>
        /// When the plugin is enabled, applies current controller offsets when ingame.
        /// </summary>
        internal static void AdjustControllerTransform(XRNode node, Transform transform)
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
    internal class SaberGripDevicelessControllerPatch
    {
        static void Prefix(
            DevicelessVRHelper __instance,
            XRNode node, Transform transform,
            ref Vector3 position, ref Vector3 rotation
        ) => SaberGrip.ClearBaseGameOffsets(ref position, ref rotation);

        static void Postfix(
            DevicelessVRHelper __instance,
            XRNode node, Transform transform,
            Vector3 position, Vector3 rotation
        ) => SaberGrip.AdjustControllerTransform(node, transform);
    }

    [HarmonyPatch(typeof(OculusVRHelper))]
    [HarmonyPatch(nameof(IVRPlatformHelper.AdjustControllerTransform))]
    internal class SaberGripOculusControllerPatch
    {
        static void Prefix(
            OculusVRHelper __instance,
            XRNode node, Transform transform,
            ref Vector3 position, ref Vector3 rotation
        ) => SaberGrip.ClearBaseGameOffsets(ref position, ref rotation);

        static void Postfix(
            OculusVRHelper __instance,
            XRNode node, Transform transform,
            Vector3 position, Vector3 rotation
        ) => SaberGrip.AdjustControllerTransform(node, transform);
    }

    [HarmonyPatch(typeof(OpenVRHelper))]
    [HarmonyPatch(nameof(IVRPlatformHelper.AdjustControllerTransform))]
    internal class SaberGripOpenVRControllerPatch
    {
        static void Prefix(
            OpenVRHelper __instance,
            XRNode node, Transform transform,
            ref Vector3 position, ref Vector3 rotation
        ) => SaberGrip.ClearBaseGameOffsets(ref position, ref rotation);

        static void Postfix(
            OpenVRHelper __instance,
            XRNode node, Transform transform,
            Vector3 position, Vector3 rotation
        ) => SaberGrip.AdjustControllerTransform(node, transform);
    }
}
