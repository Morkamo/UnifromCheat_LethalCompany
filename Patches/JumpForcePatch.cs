using GameNetcodeStuff;
using HarmonyLib;

namespace UnifromEngine.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class JumpForcePatch
    {
        static void Postfix(PlayerControllerB __instance)
        {
            if (Engine.Instance.isEnhancedJumpOn)
            {
                __instance.jumpForce = Engine.Instance.jumpForce;
            }
            else
                __instance.jumpForce = 13f;
        }
    }
}