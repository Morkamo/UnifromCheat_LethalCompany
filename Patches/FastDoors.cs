using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace UnifromEngine.Patches
{
    [HarmonyPatch(typeof(DoorLock), "OnHoldInteract")]
    public class FastDoors
    {
        public static float firstTimeToHold = 0;

        static bool Prefix(DoorLock __instance)
        {
            if (Engine.Instance.isAdvancedDoorsOn && Engine.Instance.isFastDoorsOn)
            {
                Debug.Log("[DoorBypass] - Patch complete!");

                var doorTriggerField =
                    typeof(DoorLock).GetField("doorTrigger", BindingFlags.NonPublic | BindingFlags.Instance);

                if (doorTriggerField != null)
                {
                    var doorTrigger = doorTriggerField.GetValue(__instance) as InteractTrigger;
                    if (doorTrigger != null)
                    {
                        doorTrigger.timeToHold = 0;
                        doorTrigger.interactable = true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}