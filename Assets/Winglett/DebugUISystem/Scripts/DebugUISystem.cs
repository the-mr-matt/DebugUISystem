// =================================
//      (C) Winglett 2021
// =================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Winglett.DebugSystem
{
    public class DebugUISystem : MonoBehaviour
    {
        private static DebugUISystem Instance;

        #region ----ENUMS----
        public enum MenuType { Menu, SubMenu, Window, Action }
        public enum PathType { Window, Action }
        #endregion

        #region ----CONFIG----
        public GUISkin skin;

        private const int MENU_BUTTON_HEIGHT = 20;
        private const int MENU_BUTTON_CHARACTER_WIDTH = 7;
        private const int MENU_BUTTON_PADDING = 15;
        private const int MENU_BUTTON_SPACING = 2;
        private const int MENU_SPACING = 2;
        private const int MENU_PADDING = 5;
        private const int MENU_RIGHT_PADDING = 40;
        #endregion

        #region ----DATA----
        private List<MenuItem> menus = new List<MenuItem>();
        private List<MenuItem> windows = new List<MenuItem>();

        private int activeMenu = -1;
        #endregion

        private void Awake() => Instance = this;

        private void OnGUI()
        {
            GUI.skin = skin;

            Toolbar();
            SubMenus();
            Windows();
        }


        private void Toolbar()
        {
            GUIExtensions.Toolbar(new Vector2(0f, 0f), menus.Select(x => x.name).ToArray(), (index, rect) =>
            {
                int prevActive = activeMenu;
                if (menus[index].menuType == MenuType.Action)
                {
                    menus[index].action?.Invoke();

                    activeMenu = -1;
                }
                else if (menus[index].menuType == MenuType.Window)
                {
                    menus[index].enabled = !menus[index].enabled;

                    activeMenu = -1;
                }
                else
                {
                    // Toggle the menu
                    menus[index].enabled = !menus[index].enabled;

                    // Select the active menu
                    if (!menus[index].enabled) activeMenu = -1;
                    else activeMenu = index;

                    // Set the alignment for top level sub menus
                    menus[index].alignment = rect.x;
                }

                // Disable any current menus in this menu
                if (prevActive >= 0)
                {
                    ActionOnRecursiveMenuItems(new List<MenuItem>() { menus[prevActive] }, x =>
                    {
                        // Don't disable the current menu - toggle it below
                        // Don't change the value of windows
                        if (x != menus[prevActive] && x.menuItems.Count > 0) x.enabled = false;
                    });
                }
            });
        }

        private void SubMenus()
        {
            if (activeMenu >= 0)
            {
                ActionOnRecursiveMenuItems(new List<MenuItem>() { menus[activeMenu] }, menu =>
                {
                    if (menu.enabled && menu.menuItems.Count > 0)
                    {
                        Rect rect = new Rect();
                        Vector2 origin = new Vector2(menu.parent == null ? menu.alignment : menu.parent.rect.x + menu.parent.rect.width + MENU_SPACING, menu.parent == null ? (MENU_BUTTON_HEIGHT + MENU_SPACING) : (menu.parent.rect.y + menu.startHeight));
                        rect = GUIExtensions.Dropdown(menu.index, origin, menu.menuItems.Select(x => x.name).ToArray(), menu.menuItems.Select(x => x.menuItems.Count > 0).ToArray(), index =>
                        {
                            if (menu.menuItems[index].menuType == MenuType.Action)
                            {
                                menu.menuItems[index].action?.Invoke();

                                // Close any sub menus
                                ActionOnRecursiveMenuItems(menus, x =>
                                {
                                    // Don't disable the current menu - toggle it below
                                    if (x != menu.menuItems[index] && x.menuItems.Count > 0) x.enabled = false;
                                });
                            }
                            else if (menu.menuItems[index].menuType == MenuType.Window)
                            {
                                menu.menuItems[index].enabled = !menu.menuItems[index].enabled;

                                // Close any sub menus
                                ActionOnRecursiveMenuItems(menus, x =>
                                {
                                    // Don't disable the current menu - toggle it below
                                    if (x != menu.menuItems[index] && x.menuItems.Count > 0) x.enabled = false;
                                });
                            }
                            else
                            {
                                menu.menuItems[index].enabled = !menu.menuItems[index].enabled;
                                menu.menuItems[index].startHeight = (MENU_BUTTON_HEIGHT + MENU_BUTTON_SPACING) * index;
                            }

                            // Disable any other opened sub menus
                            ActionOnRecursiveMenuItems(menu.menuItems, x =>
                            {
                                // Don't disable the current menu - toggle it below
                                if (x != menu.menuItems[index]) x.enabled = false;
                            });

                            menu.rect = rect;
                        });
                    }
                });
            }
        }

        private void Windows()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                int index = i;
                if (windows[i].enabled) windows[i].rect = GUIExtensions.Window(windows[i].index, windows[i].name, windows[i].rect, () => windows[index].action?.Invoke(), () => windows[index].enabled = false);
            }
        }

        public static void RegisterWindow(string path, Vector2 defaultSize, Action onDrawWindow)
        {
            RegisterPath(path, PathType.Window, onDrawWindow);
            ActionOnRecursiveMenuItems(Instance.menus, menu =>
            {
                if (menu.path == path) menu.rect = new Rect(10f, 50f, defaultSize.x, defaultSize.y);
            });
        }

        public static void RegisterAction(string path, Action action) => RegisterPath(path, PathType.Action, action);

        private static void RegisterPath(string path, PathType pathType, Action action)
        {
            // Check for duplicates
            bool duplicateFound = false;
            ActionOnRecursiveMenuItems(Instance.menus, x =>
            {
                if (x.path == path)
                {
                    duplicateFound = true;
                    Debug.LogError($"The {pathType} \"{path}\" already exists.");
                    return;
                }
            });

            if (duplicateFound) return;

            // Get all items in the path
            var split = path.Split('/');
            string menu = split[0];
            string item = split[split.Length - 1];

            // Determine path type
            MenuType menuType = pathType == PathType.Action ? MenuType.Action : MenuType.Window;

            if (split.Length == 1)
            {
                if (!Instance.menus.Any(x => x.name == menu)) Instance.menus.Add(new MenuItem(menu, menuType, action));

                var endMenu = Instance.menus.First(x => x.name == menu);
                if (endMenu.menuType == MenuType.Window) Instance.windows.Add(endMenu);
            }
            else
            {
                // Find the menu
                if (!Instance.menus.Any(x => x.name == menu)) Instance.menus.Add(new MenuItem(menu, MenuType.Menu));
                var rootMenu = Instance.menus.First(x => x.name == menu);

                // Start at the root and work down adding sub menus
                MenuItem parent = rootMenu;
                for (int i = 1; i < split.Length; i++)
                {
                    // If the submenu doesn't exist, add it
                    if (!parent.menuItems.Any(x => x.name == split[i]))
                    {
                        if (i == split.Length - 1)
                        {
                            parent.menuItems.Add(new MenuItem(split[i], menuType, action, parent));

                            var endMenu = parent.menuItems.First(x => x.name == split[i]);
                            if (endMenu.menuType == MenuType.Window) Instance.windows.Add(endMenu);
                        }
                        else parent.menuItems.Add(new MenuItem(split[i], MenuType.SubMenu, parent));
                    }

                    // Set the new parent
                    parent = parent.menuItems.First(x => x.name == split[i]);
                }
            }
        }

        private static void ActionOnRecursiveMenuItems(List<MenuItem> menus, Action<MenuItem> action)
        {
            for (int i = 0; i < menus.Count; i++)
            {
                action?.Invoke(menus[i]);
                ActionOnRecursiveMenuItems(menus[i].menuItems, action);
            }
        }
    }
}