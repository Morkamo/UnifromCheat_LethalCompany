using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace UnifromEngine.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    public class GodMode
    {
        static bool Prefix(int damageNumber, CauseOfDeath causeOfDeath)
        {
            if (Engine.Instance.isGodModeActive || Noclip.isActive)
            {
                Debug.Log("CALLED -> GodModePatch");
                Debug.Log($"DamageNum: {damageNumber}, DeathCause: {causeOfDeath} ");
                Debug.Log("Damage eventargs has been denied");
                
                return false;
            }
            
            if (Noclip.isGodModeActive)
            {
                Debug.Log("CALLED -> Noclip-GodModePatch");
                Debug.Log($"DamageNum: {damageNumber}, DeathCause: {causeOfDeath} ");
                Debug.Log("Damage eventargs has been denied");
                
                Noclip.isGodModeActive = false;
                return false;
            }

            return true;
        }
    }
    
    [HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
    public class DisableOneShotPlayer
    {
        static bool Prefix()
        {
            if (Engine.Instance.isGodModeActive || Noclip.isActive)
            {
                Debug.Log("CALLED -> OneShotPlayerPatch");
                Debug.Log("Death eventargs has been denied");
                
                return false;
            }
            
            if (Noclip.isGodModeActive)
            {
                Debug.Log("CALLED -> Noclip-OneShotPlayerPatch");
                Debug.Log("Death eventargs has been denied");

                Noclip.isGodModeActive = false;
                return false;
            }

            return true;
        }
    }
}