using HarmonyLib;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace UnifromEngine.Patches
{
    /*[HarmonyPatch(typeof(DoorLock), "OnHoldInteract")]
    public class BypassForDoors
    {
        static void Postfix()
        {
            if (Engine.Instance.isBypassModeOn)
            {
                var camera = RoundManager.Instance.playersManager.activeCamera;

                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 5))
                {
                    if (hit.collider.gameObject.GetComponent<DoorLock>())
                    {
                        Debug.LogWarning(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<DoorLock>().UnlockDoor();
                        hit.collider.gameObject.GetComponent<DoorLock>();
                    }
                }
            }
        }
    }*/
}