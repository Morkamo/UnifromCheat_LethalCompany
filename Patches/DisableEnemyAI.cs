using HarmonyLib;

namespace UnifromEngine.Patches
{
    public class DisableEnemyAI
    {
        [HarmonyPatch(typeof(EnemyAI), "Update")]
        public class DisableEnemyAIPatch
        {
            static bool Prefix()
            {
                if (Engine.Instance.isEnemyAIDisabled)
                {
                    return false;
                }

                return true;
            }
        }
    }
}