using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace UnifromEngine.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerControllerBPatch
    {
        private static bool isFullBrightOn;
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void texturePatch(ref Light ___nightVision)
        {
            if (Engine.Instance.isFullBrightOn)
            {
                ___nightVision.type = LightType.Point;
                ___nightVision.color = new Color(0.5f, 0.55f, 0.6f);
                ___nightVision.intensity = Engine.Instance.BrightIntensity;
                ___nightVision.range = 99999f;
                ___nightVision.shadowStrength = 0f;
                ___nightVision.shadows = LightShadows.None;
                ___nightVision.bounceIntensity = 1000f;
                ___nightVision.innerSpotAngle = 45f;
                ___nightVision.spotAngle = 60f;
            }
        }
 
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void updateLightState(ref Light ___nightVision)
        {
            if (PlayerControllerBPatch.isFullBrightOn)
            {
                var settings = Engine.Instance;
                
                ___nightVision.gameObject.SetActive(true);
                ___nightVision.intensity = settings.BrightIntensity;
                ___nightVision.color = new Color(settings.Fb_R, settings.Fb_G, settings.Fb_B);
            }
            else
            {
                ___nightVision.gameObject.SetActive(false);
            }
        }

        public static void SetFullBright(bool enabled)
        {
            isFullBrightOn = enabled;
        }
    }
}