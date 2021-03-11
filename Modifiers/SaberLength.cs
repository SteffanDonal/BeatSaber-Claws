using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Claws.Modifiers
{
    internal static class SaberLength
    {
        static List<WeakReference<SaberModelContainer>> ActiveSabers => new List<WeakReference<SaberModelContainer>>();

        static float CurrentLength => Plugin.IsEnabled ? Preferences.Length : 1.0f;
        static bool IsVisible => !Plugin.IsEnabled;

        internal static void TrackSaber(SaberModelContainer saberModelContainer)
        {
            ActiveSabers.Add(new WeakReference<SaberModelContainer>(saberModelContainer));
            ApplyToSaber(saberModelContainer);
        }
        internal static void ApplyToSabers()
        {
            for (var i = ActiveSabers.Count - 1; i >= 0; i--)
            {
                var weakSaber = ActiveSabers[i];
                if (!weakSaber.TryGetTarget(out var saberModelContainer))
                {
                    ActiveSabers.RemoveAt(i);
                    continue;
                }

                ApplyToSaber(saberModelContainer);
            }
        }

        static void ApplyToSaber(SaberModelContainer saberModelContainer)
        {
            var saber = saberModelContainer.GetPrivateField<Saber>("_saber");
            var saberModelController = saberModelContainer.GetComponent<SaberModelController>();

            var saberTop = saber.GetPrivateField<Transform>("_saberBladeTopTransform");
            var saberBottom = saber.GetPrivateField<Transform>("_saberBladeBottomTransform");

            saberTop.localPosition = new Vector3(saberTop.localPosition.x, saberTop.localPosition.y, saberBottom.localPosition.z + CurrentLength);
        }
    }

    [HarmonyPatch(typeof(SaberModelContainer))]
    [HarmonyPatch(nameof(SaberModelContainer.Start))]
    internal class SaberLengthSaberInitPatch
    {
        static void Postfix(SaberModelContainer __instance)
        {
            var saber = __instance.GetPrivateField<Saber>("_saber");

            if (!IsValidSaber(saber))
                return;

            SaberLength.TrackSaber(__instance);
        }

        static bool IsValidSaber(Saber saber)
        {
            var currentObject = saber.gameObject;

            var isMultiplayerSaber = false;
            var isLocalSaber = false;

            while (currentObject.transform.parent != null)
            {
                currentObject = currentObject.transform.parent.gameObject;

                if (currentObject.name.Contains("LocalPlayerGameCore"))
                    isLocalSaber = true;
                else if (currentObject.name.Contains("Multiplayer"))
                    isMultiplayerSaber = true;
            }

            return !isMultiplayerSaber || isLocalSaber;
        }
    }
}
