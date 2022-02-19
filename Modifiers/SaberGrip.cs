using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace Claws.Modifiers
{
    internal static class SaberGrip
    {
        internal static bool IsLeftVisible { get; set; }
        internal static bool IsRightVisible { get; set; }

        /// <summary>
        /// Clears base game offset whenever the sabers are visible.
        /// </summary>
        internal static void ClearBaseGameOffsets(XRNode node, ref Vector3 position, ref Vector3 rotation)
        {
            if (!Plugin.IsEnabled) return;

            switch (node)
            {
                case XRNode.LeftHand:
                    if (!IsLeftVisible) return;
                    break;

                case XRNode.RightHand:
                    if (!IsRightVisible) return;
                    break;
            }

            position = Vector3.zero;
            rotation = Vector3.zero;
        }

        /// <summary>
        /// Applies our offsets whenever the sabers are visible.
        /// </summary>
        internal static void AdjustControllerTransform(XRNode node, Transform transform)
        {
            if (!Plugin.IsEnabled) return;

            switch (node)
            {
                case XRNode.LeftHand:
                    if (!IsLeftVisible) return;
                    transform.Translate(Preferences.LeftTranslation);
                    transform.Rotate(Preferences.LeftRotation);
                    break;

                case XRNode.RightHand:
                    if (!IsRightVisible) return;
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
        ) => SaberGrip.ClearBaseGameOffsets(node, ref position, ref rotation);

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
        ) => SaberGrip.ClearBaseGameOffsets(node, ref position, ref rotation);

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
        ) => SaberGrip.ClearBaseGameOffsets(node, ref position, ref rotation);

        static void Postfix(
            OpenVRHelper __instance,
            XRNode node, Transform transform,
            Vector3 position, Vector3 rotation
        ) => SaberGrip.AdjustControllerTransform(node, transform);
    }
}
