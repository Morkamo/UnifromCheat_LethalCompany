using HarmonyLib;

namespace UnifromEngine.Patches
{
    [HarmonyPatch(typeof(GrabbableObject), "Update")]
    public class BatteryPatch
    {
        static void Postfix(GrabbableObject __instance)
        {
            if (Engine.Instance.isInfiniteBatteryActive)
                __instance.insertedBattery.charge = 1f;
        }
    }
}