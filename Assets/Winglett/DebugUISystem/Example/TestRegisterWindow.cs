// =================================
//      (C) Winglett 2021
// =================================

using UnityEngine;

namespace Winglett.DebugSystem
{
    public class TestRegisterWindow : MonoBehaviour
    {
        public string[] actions;
        public string[] windows;

        private void Start()
        {
            for (int i = 0; i < actions.Length; i++)
            {
                DebugUISystem.RegisterAction(actions[i], () => Debug.Log("Action Called"));
            }

            for (int i = 0; i < windows.Length; i++)
            {
                DebugUISystem.RegisterWindow(windows[i], new Vector2(200, 300), () =>
                {
                    GUI.Label(new Rect(5, 20, 160, 20), "Hello World");
                });
            }
        }
    }
}