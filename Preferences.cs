using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace Claws
{
    internal enum VRControllerType
    {
        Unknown,
        Vive,
        Touch,
        WMR,
        Knuckles,
        OculusStoreTouch,
    }

    internal static class Preferences
    {
        const string IsEnabledPreference = @"Claws.Plugin.IsEnabled";
        const string LastCustomSaberPreference = @"Claws.Plugin.LastCustomSaber";

        public const float Length = 0.3f;

        public static bool IsEnabled { get; internal set; }

        public static string LastCustomSaber { get; internal set; }

        public static Vector3 LeftTranslation { get; private set; }
        public static Vector3 LeftRotation { get; private set; }

        public static Vector3 RightTranslation { get; private set; }
        public static Vector3 RightRotation { get; private set; }

        static readonly Dictionary<VRControllerType, Vector3> DefaultTranslation = new Dictionary<VRControllerType, Vector3>
        {
            { VRControllerType.Unknown,      Vector3.zero },
            { VRControllerType.Vive,      new Vector3(-0.04f, -0.0125f, -0.06f) },
            { VRControllerType.Touch,    new Vector3(-0.03f, -0.0225f, -0.095f ) },
            { VRControllerType.Knuckles, new Vector3(-0.04f, -0.0225f, -0.11f) },
            { VRControllerType.OculusStoreTouch, new Vector3(-0.1f, -0.0225f, -0.06f) }
        };
        static readonly Dictionary<VRControllerType, Vector3> DefaultRotation = new Dictionary<VRControllerType, Vector3>
        {
            { VRControllerType.Unknown,      Vector3.zero },
            { VRControllerType.Vive,      new Vector3(75f, 0f, 90f) },
            { VRControllerType.Touch,    new Vector3(75f, 0f, 90f ) },
            { VRControllerType.Knuckles, new Vector3(75f, 0f, 90f) },
            { VRControllerType.OculusStoreTouch, new Vector3(25f, 0f, 90f) }
        };

        public static void Store()
        {
            Plugin.Log.Info("Storing plugin preferences...");

            PlayerPrefs.SetInt(IsEnabledPreference, IsEnabled ? 1 : 0);
            PlayerPrefs.SetString(LastCustomSaberPreference, LastCustomSaber);
            PlayerPrefs.Save();

            Plugin.Log.Info("Stored!");
        }

        public static void Restore()
        {
            Plugin.Log.Info("Loading plugin preferences...");

            IsEnabled = PlayerPrefs.GetInt(IsEnabledPreference, 0) != 0;
            LastCustomSaber = PlayerPrefs.GetString(LastCustomSaberPreference, null);

            var pluginState = Plugin.IsEnabled ? "enabled" : "disabled";
            Plugin.Log.Info($"Loaded! Plugin is {pluginState}.");
        }

        public static void Invalidate()
        {
            LeftTranslation = Vector3.zero;
            LeftRotation = Vector3.zero;
            var controllerType = GetActiveControllersType();

            Plugin.Log.Debug($"Applying default offsets for {controllerType} controllers!");

            if (DefaultTranslation.ContainsKey(controllerType))
                LeftTranslation = DefaultTranslation[controllerType];

            if (DefaultRotation.ContainsKey(controllerType))
                LeftRotation = DefaultRotation[controllerType];

            RightTranslation = MirrorTranslation(LeftTranslation);
            RightRotation = MirrorRotation(LeftRotation);
        }

        static VRControllerType GetActiveControllersType()
        {
            var nodeStates = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodeStates);

            var controllers = nodeStates
                .Where(node => node.nodeType == XRNode.LeftHand || node.nodeType == XRNode.RightHand)
                .Select(node => InputTracking.GetNodeName(node.uniqueID))
                .Where(controller => !String.IsNullOrWhiteSpace(controller));

            foreach (var controller in controllers)
            {
                /*
                 * Known Vive controller names:
                 *   Vive. Controller MV
                 *   Vive. Controller DVT
                 */
                if (controller.IndexOf(@"Vive", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VRControllerType.Vive;

                /*
                 * Known Oculus controller names:
                 *   Oculus Rift CV1
                 */
                if (controller.IndexOf(@"Oculus Rift", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VRControllerType.Touch;
                /*
                 * Known Oculus Store controller names:
                 *   Oculus Touch Controller
                 */
                if (controller.IndexOf(@"Oculus Touch", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VRControllerType.OculusStoreTouch;
                /*
                 * Known WMR controller names:
                 *   WindowsMR: 0x045e/0x065b/0/2
                 */
                if (controller.IndexOf(@"WindowsMR", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VRControllerType.WMR;

                /*
                 * Known Knuckles controller names:
                 *   Knuckles EV1.3
                 *   Knuckles EV3.0
                 */
                if (controller.IndexOf(@"Knuckles", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VRControllerType.Knuckles;

                Plugin.Log.Error("Discovering controller: " + controller + " failed! Please open an issue with this log statement.");
            }

            return VRControllerType.Unknown;
        }

        static Vector3 MirrorTranslation(Vector3 translation) => new Vector3(-translation.x, translation.y, translation.z);
        static Vector3 MirrorRotation(Vector3 rotation) => new Vector3(rotation.x, -rotation.y, -rotation.z);
    }
}
