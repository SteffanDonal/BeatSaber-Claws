using IllusionPlugin;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Claws
{
    internal enum VRPlatform
    {
        OpenVR = 0,
        Oculus = 1,
        Unknown = 2
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

        public static VRPlatform ActiveVRPlatform { get; internal set; }

        static readonly Dictionary<VRPlatform, Vector3> DefaultTranslation = new Dictionary<VRPlatform, Vector3>
        {
            { VRPlatform.OpenVR, Vector3.zero },
            { VRPlatform.Oculus, Vector3.zero },
            { VRPlatform.Unknown, Vector3.zero }
        };

        static readonly Dictionary<VRPlatform, Vector3> DefaultRotation = new Dictionary<VRPlatform, Vector3>
        {
            { VRPlatform.OpenVR, Vector3.zero },
            { VRPlatform.Oculus, Vector3.zero },
            { VRPlatform.Unknown, Vector3.zero }
        };

        public static void Invalidate()
        {
            if (ModPrefs.HasKey(PrefsSection, TranslationKey) || ModPrefs.HasKey(PrefsSection, RotationKey))
            {
                LeftTranslation = ParseVector3(ModPrefs.GetString(PrefsSection, TranslationKey, "0,0,0"));
                LeftRotation = ParseVector3(ModPrefs.GetString(PrefsSection, RotationKey, "0,0,0"));
            }
            else
            {
                LeftTranslation = DefaultTranslation[ActiveVRPlatform];
                LeftRotation = DefaultRotation[ActiveVRPlatform];
            }

            RightTranslation = MirrorTranslation(LeftTranslation);
            RightRotation = MirrorRotation(LeftRotation);
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
