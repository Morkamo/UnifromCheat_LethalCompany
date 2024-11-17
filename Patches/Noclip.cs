using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnifromEngine.TextController;

namespace UnifromEngine.Patches
{
    public class Noclip : MonoBehaviour
    {
        public static bool isActive;
        public static bool isGodModeActive;
        
        public NoclipComponent noclipComponent;

        private TextMeshProUGUI NoclipStateInfo;

        private void Start()
        {
            NoclipStateInfo = CreateText($"Noclip speed: <b>{Engine.Instance.noclipSpeed}</b> \nPress <b>ALT</b> to switch mode",
                "NoclipStateInfo", -700, 450);
            
            Engine.Instance.UnifromHints.Add(NoclipStateInfo.gameObject);
            
            NoclipStateInfo.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (RoundManager.Instance == null)
                return;
            
            var player = RoundManager.Instance.playersManager.localPlayerController;
            
            if (player == null)
                return;
            
            Collider collider = player.GetComponent<CharacterController>();

            if (Keyboard.current.leftAltKey.wasPressedThisFrame && Engine.Instance.isNoclipOn && Engine.Instance.isMisscaleonsOn)
            {
                isActive = !isActive;
            }
            
            if (isActive && Engine.Instance.isNoclipOn && Engine.Instance.isMisscaleonsOn)
            {
                StopCoroutine(godMode());
                
                if (!Engine.Instance.DisableAllTextHints)
                    NoclipStateInfo.gameObject.SetActive(true);

                if (NoclipStateInfo)
                    NoclipStateInfo.text = $"Noclip speed: <b>{Engine.Instance.noclipSpeed}</b> \nPress <b>ALT</b> to switch mode";

                if (noclipComponent == null) 
                    noclipComponent = player.gameObject.AddComponent<NoclipComponent>();
                
                collider.enabled = false;
            }
            else if (noclipComponent != null)
            {
                StartCoroutine(godMode());

                NoclipStateInfo.gameObject.SetActive(false);
                
                collider.enabled = true;
                Destroy(noclipComponent);
                noclipComponent = null;
            }
        }

        public IEnumerator godMode()
        {
            isGodModeActive = true;
            yield return new WaitForSeconds(5f);

            if (isGodModeActive)
                isGodModeActive = false;
        }
    }
    
    public class NoclipComponent : MonoBehaviour
    {
        private void Update()
        {
            Vector3 input = new Vector3();

            if (Keyboard.current.wKey.isPressed) input += transform.forward;
            if (Keyboard.current.sKey.isPressed) input -= transform.forward;
            if (Keyboard.current.aKey.isPressed) input -= transform.right;
            if (Keyboard.current.dKey.isPressed) input += transform.right;
            if (Keyboard.current.spaceKey.isPressed) input += transform.up;
            if (Keyboard.current.ctrlKey.isPressed) input -= transform.up;
            
            transform.position += input * (Engine.Instance.noclipSpeed * Time.deltaTime * 10f);
        }
    }
}