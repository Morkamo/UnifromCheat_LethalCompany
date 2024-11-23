using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.Mono;
using GameNetcodeStuff;
using HarmonyLib;
using TMPro;
using UnifromEngine.Patches;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnifromEngine.TextController;
using Rect = UnityEngine.Rect;

namespace UnifromEngine
{
    public class Engine : MonoBehaviour
    {
        public string cheatVersion = "2.1.0"; 
        
        public static Engine Instance;
        private Harmony harmony;
        
        public Noclip Noclip;
        public NoclipComponent noclipComponent;
        public WallHack Wallhack;

        public List<GameObject> UnifromHints = new List<GameObject>();
        public TextController TextController;
        public PlayerControllerB player;

        #region Menu parameters
        // Menu
        private Rect RectMenu;
        private static float dpiScaling = Screen.dpi / 100f;
        private Vector2 scrollPosition = Vector2.zero;
        
        public byte MenuID = 1;
        public bool MenuState = true;
        public bool AdvancedMenuSettings;
        public float MC_R = 1;
        public float MC_G = 0.8f;
        public float MC_B = 0.8f;
        public float MC_A = 1;
        public bool DisableAllTextHints;
        public bool islocalizationRu = true;
        public bool isLocSwitched;

        // player
        public bool isGodModeEnabled;
        public bool isGodModeActive;
        public string GodModeState;
        private TextMeshProUGUI GodModeStateInfo;

        public bool PlayerHealthController;
        public bool isHealthControllerSwitched;
        public float HealValue = 20;
        public TextMeshProUGUI CurrentHealthUIText;
        
        public bool isInfiniteStaminaActive; 
        public bool isInfiniteBatteryActive;
        
        // world
        public bool isEnemyAIDisabled;

        // doors
        public bool isAdvancedDoorsOn;
        public bool isFastDoorsOn;

        // wallhack
        public bool isWallHackOn = true;
        
        public bool isItemWallHackOn;
        
        public float IC_R = 0.8f;
        public float IC_G = 0.4f;
        public float IC_B = 0.4f;
        public float IC_A = 1;
        
        public bool hideInShip;
        public bool showItemName;
        public bool showItemPrice;
        public bool hideBigItems;

        public bool showExits;
        public bool showFireExit;
        
        public bool sortByPrice;
        public float sortPrice;
        public float sortSize = 14;
        
        public bool isEnemyWallHackOn;
        public bool isPlayerWallHackOn;

        // visuals
        public bool isVisualsOn = true;
        public bool isNoFogOn;
        
        public bool isFullBrightOn = true;
        public float BrightIntensity = 600;
        public float Fb_R = 0.5f;
        public float Fb_G = 0.55f; 
        public float Fb_B = 0.6f;

        // misc
        public bool isMisscaleonsOn;
        
        public bool isNoclipOn;
        public float noclipSpeed = 10f;
        public float scrollSpeed = 0.5f;
        public TextMeshProUGUI noclipSpeedText;

        public bool isAntiGhostGirlOn;
        public bool isMagnetItemsOn;
        public bool isTeleportMenuOn;

        // player
        public bool isMovementBoost;
        public float movementBoostValue = 4.6f;

        public bool isEnhancedJumpOn;
        public float jumpForce = 13f;
        
        #endregion

        private void Start()
        {
            Instance = this;

            Wallhack = gameObject.AddComponent<WallHack>();
            Noclip = gameObject.AddComponent<Noclip>();
            noclipComponent = gameObject.AddComponent<NoclipComponent>();
            TextController = gameObject.AddComponent<TextController>();

            harmony = new Harmony("ru.morkamo.unifromPatches");
            harmony.PatchAll();
            
            // Render menu settings
            float width = 300f * dpiScaling;
            float height = 700f * dpiScaling;

            RectMenu = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);

            
            CreateText($"Unifrom {cheatVersion} - by Morkamo", "UnifromBadge", 1000f, 470f, 14);
            UnifromHints.Add(GameObject.Find("UnifromBadge"));
            
            GodModeStateInfo = CreateText($"God mode: <b>{GodModeState}</b>",
                "GodModeStateInfo", 110, -505);
            
            UnifromHints.Add(GameObject.Find("GodModeStateInfo"));
            GodModeStateInfo.gameObject.SetActive(false);
            
            
            CurrentHealthUIText = CreateText($"HP: 0", 
                "CurrentHealthUIText", -670, -500, 24);
            
            UnifromHints.Add(GameObject.Find("CurrentHealthUIText"));

            Debug.LogWarning($"\n--------------------------\n     [CHEAT-INJECTED]\n Welcome to Unifrom {cheatVersion}\n--------------------------\n");
        }

        private void Update()
        {
            player = FindObjectOfType<PlayerControllerB>();
            
            if (!DisableAllTextHints)
                if (player == null)
                    CurrentHealthUIText.text = $"HP: 0";
                else
                    CurrentHealthUIText.text = $"HP: {GameNetworkManager.Instance.localPlayerController.health}";

            if (player == null)
                return;

            if (Keyboard.current.zKey.wasPressedThisFrame && isGodModeEnabled)
            {
                isGodModeActive = !isGodModeActive;
            }

            Wallhack.RenderItems();
            Wallhack.RenderEnemies();
            Wallhack.RenderPlayers();
            Wallhack.RenderDoors();

            // Noclip scroll speed value
            if (isMisscaleonsOn && isNoclipOn)
            {
                float scrollDelta = Mouse.current.scroll.ReadValue().y;
                
                if (scrollDelta > 0)
                    noclipSpeed += scrollSpeed;
                else if (scrollDelta < 0)
                    noclipSpeed -= scrollSpeed;

                noclipSpeed = Mathf.Clamp(noclipSpeed, 0.5f, 20);
            }
            
            if (isMisscaleonsOn && isMagnetItemsOn)
                MagnetItems.MagnetizeItems();
            
            if (isGodModeEnabled)
            {
                GodModeStateInfo.gameObject.SetActive(true);
                
                if (!isGodModeActive)
                {
                    GodModeState = "<color=#930000>OFF</color>";
                    GodModeStateInfo.text = $"God mode: <b>{GodModeState}</b>";
                }
                else
                {
                    GodModeState = "<color=#00930d>ON</color>";
                    GodModeStateInfo.text = $"God mode: <b>{GodModeState}</b>";
                }
            }
            else
            {
                GodModeStateInfo.gameObject.SetActive(false);
                isGodModeActive = false;
            }
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Insert || Event.current.keyCode == KeyCode.End || Event.current.keyCode == KeyCode.Home)
                {
                    MenuState = !MenuState;

                    if (player != null)
                    {
                        /*if (MenuState)
                            RoundManager.Instance.playersManager.localPlayerController.enabled = false;
                        else
                            RoundManager.Instance.playersManager.localPlayerController.enabled = true;*/
                        
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                    
                    return;
                }
            }
            
            if (MenuState)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                Matrix4x4 originalMatrix = GUI.matrix;

                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(dpiScaling, dpiScaling, 1f));
                
                RectMenu.x = Mathf.Clamp(RectMenu.x, 0, Screen.width / dpiScaling - RectMenu.width);
                RectMenu.y = Mathf.Clamp(RectMenu.y, 0, Screen.height / dpiScaling - RectMenu.height);
                
                // Menu color
                GUI.backgroundColor = new Color(MC_R, MC_G, MC_B, MC_A);
                GUI.color = new Color(MC_R, MC_G, MC_B, MC_A);

                RectMenu = GUILayout.Window(MenuID, RectMenu, GUIMenu, $"Unifrom {cheatVersion}");

                GUI.matrix = originalMatrix;
            }
        }

        public void GUIMenu(int id)
        {
            float maxWidth = 300f * dpiScaling;
            float maxHeight = 700f * dpiScaling;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(maxWidth - 20), GUILayout.Height(maxHeight - 20));
            
            GUILayout.BeginVertical("box");

            if (GUILayout.Button("MENU SETTINGS"))
            {
                AdvancedMenuSettings = !AdvancedMenuSettings;
            }
            
            if (AdvancedMenuSettings)
            {
                DisableAllTextHints = GUILayout.Toggle(DisableAllTextHints, " Disable all menu hints");

                if (DisableAllTextHints)
                {
                    foreach (var hint in UnifromHints)
                    {
                        hint.SetActive(false);
                    }
                }
                else
                {
                    foreach (var hint in UnifromHints)
                    {
                        if (hint.name != "NoclipStateInfo" && hint.name != "GodModeStateInfo")
                            hint.SetActive(true);
                        else if ((Noclip.isActive && isNoclipOn && isMisscaleonsOn) || isGodModeEnabled)
                            hint.SetActive(true);
                    }
                }
                
                GUILayout.Label("Menu color:");
                GUILayout.BeginVertical("box");
                    
                // R
                GUILayout.Label("R: " + MC_R.ToString("F1"));
                MC_R = GUILayout.HorizontalSlider(MC_R, 0, 1, GUILayout.Width(100));
                    
                // G
                GUILayout.Label("G: " + MC_G.ToString("F1"));
                MC_G = GUILayout.HorizontalSlider(MC_G, 0, 1, GUILayout.Width(100));
                    
                // B
                GUILayout.Label("B: " + MC_B.ToString("F1"));
                MC_B = GUILayout.HorizontalSlider(MC_B, 0, 1, GUILayout.Width(100));
                
                // A
                GUILayout.Label("B: " + MC_A.ToString("F1"));
                MC_A = GUILayout.HorizontalSlider(MC_A, 0, 1, GUILayout.Width(100));

                GUILayout.BeginVertical("box");
                
                if (GUILayout.Button("Switch localization (EU/RU)"))
                {
                    islocalizationRu = !islocalizationRu;

                    if (!islocalizationRu)
                    {
                        GrabbableObject[] grabbableObjects = Wallhack.grabbableObjects;

                        foreach (var grabbable in grabbableObjects)
                        {
                            foreach (var key in Localizations.EU)
                            {
                                if (grabbable.itemProperties.name == key.Key)
                                    grabbable.itemProperties.name = key.Value;
                            }
                        }
                    }
                    else
                    {
                        if (islocalizationRu)
                        {
                            GrabbableObject[] grabbableObjects = Wallhack.grabbableObjects;

                            foreach (var grabbable in grabbableObjects)
                            {
                                foreach (var key in Localizations.RU)
                                {
                                    if (grabbable.itemProperties.name == key.Key)
                                        grabbable.itemProperties.name = key.Value;
                                }
                            }
                        }
                    }
                }
                
                if (!islocalizationRu)
                    GUILayout.Label("              Current loc: [EU]");
                else
                    GUILayout.Label("              Current loc: [RU]");
                
                GUILayout.EndVertical();
                
                GUILayout.EndVertical();
            }
            
            GUILayout.EndVertical();
            
            
            if (!isGodModeEnabled)
                isGodModeEnabled = GUILayout.Toggle(isGodModeEnabled, " God mode");
            else
            {
                GUILayout.BeginVertical("box");
                
                isGodModeEnabled = GUILayout.Toggle(isGodModeEnabled, " God mode");
                GUILayout.Label("Bind: [z]"); 
                
                GUILayout.EndVertical();
            }

            if (!PlayerHealthController)
            {
                PlayerHealthController = GUILayout.Toggle(PlayerHealthController, " Health control");
                CurrentHealthUIText.gameObject.SetActive(false);
            }
            else
            {
                CurrentHealthUIText.gameObject.SetActive(true);
                GUILayout.BeginVertical("box");
                
                PlayerHealthController = GUILayout.Toggle(PlayerHealthController, " Health control");
                
                GUILayout.Label("Need HP amount: " + HealValue.ToString("F0"));
                HealValue = GUILayout.HorizontalSlider(HealValue, 1, 100);

                if (GUILayout.Button("ADD"))
                {
                    GameNetworkManager.Instance.localPlayerController.health += (int)HealValue;
                    
                    if (GameNetworkManager.Instance.localPlayerController.health > 100)
                        GameNetworkManager.Instance.localPlayerController.health = 100;

                }
                
                if (GUILayout.Button("SET"))
                {
                    GameNetworkManager.Instance.localPlayerController.health = (int)HealValue;
                }

                GUILayout.EndVertical();
            }

            if (PlayerHealthController)
            {
                GUILayout.BeginVertical("box");
                
                GUILayout.EndVertical();
            }

            isInfiniteStaminaActive = GUILayout.Toggle(isInfiniteStaminaActive, " Infinity stamina");
            isInfiniteBatteryActive = GUILayout.Toggle(isInfiniteBatteryActive, " Infinity battery");
            isAdvancedDoorsOn = GUILayout.Toggle(isAdvancedDoorsOn, " Advanced doors");

            if (isAdvancedDoorsOn)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("Settings:");
                
                isFastDoorsOn = GUILayout.Toggle(isFastDoorsOn, " Fast doors");

                if (GUILayout.Button("Unlock doors", GUILayout.Width(100)))
                {
                    DoorLock[] doors = FindObjectsOfType<DoorLock>();
                    
                    foreach (var door in doors)
                    {
                        door.UnlockDoor();
                        door.UnlockDoorServerRpc();
                    }
                }
                
                if (GUILayout.Button("Destroy doors\n(only for you)", GUILayout.Width(100)))
                {
                    DoorLock[] doors = FindObjectsOfType<DoorLock>();
                    
                    foreach (var door in doors)
                    {
                        Destroy(door.transform.parent.parent.gameObject);
                    }
                }
                
                GUILayout.Label(" ");
                
                GUILayout.EndVertical();
            }
            
            isEnemyAIDisabled = GUILayout.Toggle(isEnemyAIDisabled, " Disable enemy AI (only for you)");

            isWallHackOn = GUILayout.Toggle(isWallHackOn, " WallHack");

            if (isWallHackOn)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("General:");
                
                isItemWallHackOn = GUILayout.Toggle(isItemWallHackOn, " Show items");
                
                if (isItemWallHackOn)
                {
                    GUILayout.BeginVertical("box");
                    
                    GUILayout.BeginVertical("box");
                    GUILayout.Label("Text color:");
                    
                    GUILayout.Label("R: " + IC_R.ToString("F1"));
                    IC_R = GUILayout.HorizontalSlider(IC_R, 0, 1, GUILayout.Width(100));
                    
                    GUILayout.Label("G: " + IC_G.ToString("F1"));
                    IC_G = GUILayout.HorizontalSlider(IC_G, 0, 1, GUILayout.Width(100));
                    
                    GUILayout.Label("B: " + IC_B.ToString("F1"));
                    IC_B = GUILayout.HorizontalSlider(IC_B, 0, 1, GUILayout.Width(100));
                    
                    GUILayout.Label("A: " + IC_A.ToString("F1"));
                    IC_A = GUILayout.HorizontalSlider(IC_A, 0, 1, GUILayout.Width(100));
                    
                    GUILayout.EndVertical();

                    hideInShip = GUILayout.Toggle(hideInShip, " Hide in ship");
                    showItemName = GUILayout.Toggle(showItemName, " Show name");
                    showItemPrice = GUILayout.Toggle(showItemPrice, " Show price");
                    hideBigItems = GUILayout.Toggle(hideBigItems, " Hide heavy items");

                    sortByPrice = GUILayout.Toggle(sortByPrice, " Sort by min price");

                    if (sortByPrice)
                    {
                        GUILayout.Label("Sort price: " + sortPrice.ToString("F1"));
                        sortPrice = GUILayout.HorizontalSlider(sortPrice, 0, 200);
                    }

                    if (showItemPrice)
                    {
                        GUILayout.Label("Size: " + sortSize.ToString("F0"));
                        sortSize = GUILayout.HorizontalSlider(sortSize, 14, 200);
                    }
                    
                    GUILayout.EndVertical();
                }
                isEnemyWallHackOn = GUILayout.Toggle(isEnemyWallHackOn, " Show enemies");
                isPlayerWallHackOn = GUILayout.Toggle(isPlayerWallHackOn, " Show teammates");
                
                GUILayout.Label("Doors:");
                
                showExits = GUILayout.Toggle(showExits, " Show exits");

                GUILayout.Label(" ");
                
                GUILayout.EndVertical();
            }
            
            isVisualsOn = GUILayout.Toggle(isVisualsOn, " Visuals");

            if (isVisualsOn)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("Settings:");
                
                isNoFogOn = GUILayout.Toggle(isNoFogOn, " No fog");
                isFullBrightOn = GUILayout.Toggle(isFullBrightOn, " Full bright");
                
                if (isFullBrightOn)
                {
                    GUILayout.Label("Brightness: " + BrightIntensity.ToString("F0"));
                    BrightIntensity = GUILayout.HorizontalSlider(BrightIntensity, 0, 30000);
                    
                    // R
                    GUILayout.Label("R: " + Fb_R.ToString("F3"));
                    Fb_R = GUILayout.HorizontalSlider(Fb_R, 0, 1, GUILayout.Width(100));
                    
                    // G
                    GUILayout.Label("G: " + Fb_G.ToString("F3"));
                    Fb_G = GUILayout.HorizontalSlider(Fb_G, 0, 1, GUILayout.Width(100));
                    
                    // B
                    GUILayout.Label("B: " + Fb_B.ToString("F3"));
                    Fb_B = GUILayout.HorizontalSlider(Fb_B, 0, 1, GUILayout.Width(100));
                }
                

                GUILayout.Label(" ");
                GUILayout.EndVertical();

                if (player != null)
                {
                    if (isNoFogOn)
                        HUDManager.Instance.playerGraphicsVolume.enabled = false;
                    else
                        HUDManager.Instance.playerGraphicsVolume.enabled = true;
                    
                    PlayerControllerBPatch.SetFullBright(isFullBrightOn);
                }
            }
            
            isMisscaleonsOn = GUILayout.Toggle(isMisscaleonsOn, " Misc");

            if (isMisscaleonsOn)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("Settings:");

                isNoclipOn = GUILayout.Toggle(isNoclipOn, " Noclip");

                if (isNoclipOn)
                {
                    GUILayout.Label("Bind: [alt]");
                    GUILayout.Label("Speed: " + noclipSpeed.ToString("F1"));
                    noclipSpeed = GUILayout.HorizontalSlider(noclipSpeed, 0.5f, 30f);
                }

                isAntiGhostGirlOn = GUILayout.Toggle(isAntiGhostGirlOn, " Anti-ghost girl");

                isMagnetItemsOn = GUILayout.Toggle(isMagnetItemsOn, " Magnetize all items \n (not auto add in ship)");
                
                isTeleportMenuOn = GUILayout.Toggle(isTeleportMenuOn, " Teleport menu");

                if (isTeleportMenuOn)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label("Teleport to:");

                    if (GUILayout.Button("Ship", GUILayout.Width(100)))
                    {
                        GameObject coil = GameObject.Find("ChargeInductiveCoil");

                        if (coil != null)
                        {
                            RoundManager.Instance.playersManager.localPlayerController.TeleportPlayer(coil.transform.position + new Vector3(0, -1, 2));
                        }
                    }

                    if (GUILayout.Button("Main entrance", GUILayout.Width(100)))
                    {
                        EntranceTeleport[] teleportTargets = FindObjectsOfType<EntranceTeleport>();
                        
                        foreach (var target in teleportTargets)
                        {
                            if (target.gameObject.name == "EntranceTeleportA")
                            {
                                RoundManager.Instance.playersManager.localPlayerController.TeleportPlayer(target.transform.position);
                            }
                        }
                    }
                    
                    if (GUILayout.Button("Fire exit", GUILayout.Width(100)))
                    {
                        EntranceTeleport[] teleportTargets = FindObjectsOfType<EntranceTeleport>();
                        
                        foreach (var target in teleportTargets)
                        {
                            if (target.gameObject.name == "EntranceTeleportB")
                            {
                                RoundManager.Instance.playersManager.localPlayerController.TeleportPlayer(target.transform.position);
                            }
                        }
                    }
                    
                    GUILayout.EndVertical();
                }

                GUILayout.Label(" ");
                GUILayout.EndVertical();
            }

            isEnhancedJumpOn = GUILayout.Toggle(isEnhancedJumpOn, " Enhanced jump");

            if (isEnhancedJumpOn)
            {
                GUILayout.Label("Jump force: " + jumpForce.ToString("F1"));
                jumpForce = GUILayout.HorizontalSlider(jumpForce, 13f, 50f);
            } 
            
            isMovementBoost = GUILayout.Toggle(isMovementBoost, " Movement speed"); 
            
            if (isMovementBoost)
            {
                GUILayout.Label("Value: " + movementBoostValue.ToString("F1"));
                movementBoostValue = GUILayout.HorizontalSlider(movementBoostValue, 4.6f, 20f);
            }
            
            GUILayout.EndScrollView();
            
            GUI.DragWindow();
        }
    }
}