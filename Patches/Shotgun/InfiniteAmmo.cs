using HarmonyLib;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace UnifromEngine.Patches.Shotgun
{
    [HarmonyPatch(typeof(ShotgunItem), "ItemActivate")]
    public class InfiniteAmmo
    {
        static void Prefix(ShotgunItem __instance)
        {
            if (Engine.Instance.isInfiniteAmmo && Engine.Instance.isAdvancedShotgun)
            {
                var transform = RoundManager.Instance.playersManager.activeCamera.transform;

                __instance.ShootGun(transform.position,
                    transform.forward);
            }
        }
    }
}