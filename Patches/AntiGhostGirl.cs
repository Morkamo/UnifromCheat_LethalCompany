using HarmonyLib;

namespace UnifromEngine.Patches
{
    [HarmonyPatch(typeof(DressGirlAI), "Update")]
    public class AntiGhostGirl
    {
        static bool Prefix(DressGirlAI __instance)
        {
            if (Engine.Instance.isAntiGhostGirlOn && Engine.Instance.isMisscaleonsOn)
            {
                __instance.hauntingPlayer = null;
                return false;
            }

            return true;
        }
    }
}