using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using TMPro;
using UnifromEngine.Patches;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnifromEngine.TextController;

namespace UnifromEngine
{
    public class Engine : MonoBehaviour
    {
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
        public float MC_G;
        public float MC_B = 1;
        public float MC_A = 1;
        public bool DisableAllTextHints;

        // player
        public bool isGodModeActive;
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

            Debug.LogWarning($"\n--------------------------\n     [CHEAT-INJECTED]\n Welcome to Unifrom 2.0.0\n--------------------------\n");

            CreateText("Unifrom 2.0.0 - by Morkamo", "UnifromBadge", 1000f, 470f, 14);
            UnifromHints.Add(GameObject.Find("UnifromBadge"));
        }

        private void Update()
        {
            player = FindObjectOfType<PlayerControllerB>();

            if (player == null) 
                return;

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

                RectMenu = GUILayout.Window(MenuID, RectMenu, GUIMenu, "Unifrom 2.0.0");

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
                        if (hint.name != "NoclipStateInfo")
                            hint.SetActive(true);
                        else if (Noclip.isActive && isNoclipOn && isMisscaleonsOn)
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
                
                GUILayout.EndVertical();
            }
            
            GUILayout.EndVertical();
            
            isGodModeActive = GUILayout.Toggle(isGodModeActive, " God mode");
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
                    noclipSpeed = GUILayout.HorizontalSlider(noclipSpeed, 4.6f, 30f);
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