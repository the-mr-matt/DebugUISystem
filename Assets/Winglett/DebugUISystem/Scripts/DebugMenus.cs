// =================================
//      (C) Winglett 2021
// =================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Winglett.DebugSystem
{
    [System.Serializable]
    public class MenuItem
    {
        #region ----CONFIG----
        public string name;
        public string path;
        public int index;
        public int depth;
        public DebugUISystem.MenuType menuType;

        public List<MenuItem> menuItems = new List<MenuItem>();

        public Action action;
        #endregion

        #region ----DATA----
        private static int count;

        public bool enabled;

        public MenuItem parent;
        public List<MenuItem> Siblings
        {
            get
            {
                if (parent == null) return new List<MenuItem>();

                var siblings = parent.menuItems;
                if (siblings.Contains(this)) siblings.Remove(this);

                return siblings;
            }
        }

        public Rect rect;
        public float startHeight;
        public float alignment;
        #endregion

        #region ----CONSTRUCTORS----
        public MenuItem(string name, DebugUISystem.MenuType menuType) => Initialize(name, menuType, null, null);
        public MenuItem(string name, DebugUISystem.MenuType menuType, Action action = null) => Initialize(name, menuType, action, null);
        public MenuItem(string name, DebugUISystem.MenuType menuType, MenuItem parent = null) => Initialize(name, menuType, null, parent);
        public MenuItem(string name, DebugUISystem.MenuType menuType, Action action = null, MenuItem parent = null) => Initialize(name, menuType, action, parent);

        private void Initialize(string name, DebugUISystem.MenuType menuType, Action action = null, MenuItem parent = null)
        {
            this.name = name;
            this.index = count;
            this.menuType = menuType;
            this.parent = parent;
            this.action = action;

            path = parent == null ? name : $"{parent.path}/{name}";
            depth = parent == null ? 0 : parent.depth + 1;

            count++;
        }
        #endregion

        /// <summary>
        /// Recursively disables all sub menus of this menu including itself.
        /// </summary>
        public void DisableAllChildren() => DisableChildrenRecursive(this);

        private void DisableChildrenRecursive(MenuItem menu)
        {
            menu.enabled = false;
            for (int i = 0; i < menu.menuItems.Count; i++)
            {
                DisableChildrenRecursive(menu.menuItems[i]);
            }
        }
    }







    // NOT NEEDED
    [System.Serializable]
    public class DebugWindow
    {
        #region ----CONFIG----
        public string name;
        public string path;
        public MenuItem parent;

        public Vector2 size;
        #endregion

        #region ----DATA----
        public bool enabled;

        public delegate void DrawWindowHandler();
        public event DrawWindowHandler OnDrawWindow;
        #endregion

        public DebugWindow(string name, MenuItem parent, Vector2 size, DrawWindowHandler onDrawWindow)
        {
            this.name = name;
            this.parent = parent;
            this.size = size;
            this.OnDrawWindow = onDrawWindow;

            path = $"{parent.path}/{name}";
        }

        public void DrawWindow(int windowID)
        {
            if (GUI.Button(new Rect(4, 4, 11, 11), "x", GUI.skin.GetStyle("Close Button"))) enabled = false;

            OnDrawWindow?.Invoke();
            GUI.DragWindow();
        }
    }
}