using GameNetcodeStuff;
using HarmonyLib;

namespace UnifromEngine.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class InfiniteStamina
    {
        static void Postfix(PlayerControllerB __instance)
        {
            if (Engine.Instance.isMovementBoost)
            {
                __instance.movementSpeed = Engine.Instance.movementBoostValue;
            }
            else
                __instance.movementSpeed = 4.6f;

            if (Engine.Instance.isInfiniteStaminaActive)
            {
                __instance.sprintMeter = float.MaxValue;
            }
        }
    }
}