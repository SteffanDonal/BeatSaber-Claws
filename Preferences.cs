using IPA.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        const string PrefsSection = "Claws";
        const string TranslationKey = "Translation";
        const string RotationKey = "Rotation";

        public const float Length = 0.3f;

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
        /* Removed this in case anyone wanted to inject their own preferences. 
         * I'd prefer that folks Claws experiences be the same throughout.
        public static void Invalidate_old()
        {
            Plugin.Log.Debug("Refreshing user preferences...");

            LeftTranslation = Vector3.zero;
            LeftRotation = Vector3.zero;


            String userTranslationString = ModPrefs.GetString(PrefsSection, TranslationKey);
            String userRotationString = ModPrefs.GetString(PrefsSection, RotationKey);

            // When any user preference exists, ignore all defaults.
            if (!string.IsNullOrWhiteSpace(userTranslationString) || !string.IsNullOrWhiteSpace(userRotationString))
            {
                Plugin.Log.Debug("Applying user offsets...");

                if (!string.IsNullOrWhiteSpace(userTranslationString))
                    LeftTranslation = ParseVector3(userTranslationString);

                if (!string.IsNullOrWhiteSpace(userRotationString))
                    LeftRotation = ParseVector3(userRotationString);
            }
            else
            {
                Invalidate();
            }

            RightTranslation = MirrorTranslation(LeftTranslation);
            RightRotation = MirrorRotation(LeftRotation);
        }*/

        static VRControllerType GetActiveControllersType()
        {
            var nodeStates = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodeStates);

            var controllers = nodeStates
                .Where(node => node.nodeType == XRNode.LeftHand || node.nodeType == XRNode.RightHand)
                .Select(node => InputTracking.GetNodeName(node.uniqueID))
                .Where(controller => !string.IsNullOrWhiteSpace(controller));

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
                if (controller.IndexOf(@"Oculus Touch", StringComparison.InvariantCultureIgnoreCase) >=0)
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

<<<<<<< Updated upstream
                Plugin.Log("Discovering controller: " + controller.ToString() + "failed! please open an issue with this log statement");
=======
                Plugin.Log.Error("Discovering controller: " + controller.ToString() + "failed! please open an issue with this log statement"
                    );
>>>>>>> Stashed changes
            }

            return VRControllerType.Unknown;
        }

        static Vector3 MirrorTranslation(Vector3 translation) => new Vector3(-translation.x, translation.y, translation.z);
        static Vector3 MirrorRotation(Vector3 rotation) => new Vector3(rotation.x, -rotation.y, -rotation.z);

        static Vector3 ParseVector3(string serialized)
        {
            var components = serialized.Trim().Split(',');
            var parsedVector = Vector3.zero;

            if (components.Length != 3) return parsedVector;

            TryParseInvariantFloat(components[0], out parsedVector.x);
            TryParseInvariantFloat(components[1], out parsedVector.y);
            TryParseInvariantFloat(components[2], out parsedVector.z);

            return parsedVector;
        }

        /// <summary>
        /// Tries to parse a float using invariant culture.
        /// </summary>
        /// <param name="number">The string containing the float to parse.</param>
        /// <param name="result">The parsed float, if successful.</param>
        /// <returns>True on success, false on failure.</returns>
        static bool TryParseInvariantFloat(string number, out float result)
        {
            return float.TryParse(
                number,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out result
            );
        }
    }
}
