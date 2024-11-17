using System;
using UnityEngine;

namespace UnifromEngine.Patches
{
    public class MagnetItems : MonoBehaviour
    {
        public static void MagnetizeItems()
        {
            var playerPos = RoundManager.Instance.playersManager.localPlayerController.transform.position;

            GrabbableObject[] grabbableObjects = FindObjectsOfType<GrabbableObject>();

            foreach (var grabbable in grabbableObjects)
            {
                if (!grabbable.isInShipRoom)
                {
                    var transform1 = grabbable.transform;

                    transform1.position = playerPos;
                    transform1.localPosition = playerPos;

                    grabbable.targetFloorPosition = playerPos;
                }
            }
        }
    }
}