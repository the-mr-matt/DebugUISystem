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

        bool demoToggle;
        string demoField;
        string demoArea;
        Vector2 verticalScroll;
        Vector2 horizontalScroll;
        float horizontalSlider;
        float verticalSlider;

        private void Start()
        {
            for (int i = 0; i < actions.Length; i++)
            {
                DebugUISystem.RegisterAction(actions[i], () => Debug.Log("Action Called"));
            }

            for (int i = 0; i < windows.Length; i++)
            {
                DebugUISystem.RegisterWindow(windows[i], new Vector2(250, 400), () =>
                {
                    GUILayout.Label("Line");
                    GUILayout.Label("Line");
                    GUILayout.Button("Button");
                    demoToggle = GUILayout.Toggle(demoToggle, "Toggle");
                    demoField = GUILayout.TextField(demoField);
                    demoArea = GUILayout.TextArea(demoArea);
                    GUILayout.Box("Box");

                    horizontalScroll = GUILayout.BeginScrollView(horizontalScroll, true, true, GUILayout.Height(100));
                    GUILayout.Label("A long string that wraps.");
                    GUILayout.Label("Line");
                    GUILayout.Label("Line");
                    GUILayout.Label("Line");
                    GUILayout.Label("Line");
                    GUILayout.Button("Button");
                    GUILayout.EndScrollView();

                    GUILayout.BeginHorizontal();
                    GUILayout.Button("Left");
                    GUILayout.Button("Right");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical();
                    GUILayout.Button("Top");
                    GUILayout.Button("Bottom");
                    GUILayout.EndVertical();

                    horizontalSlider = GUILayout.HorizontalSlider(horizontalSlider, 0f, 1f);
                    verticalSlider = GUILayout.VerticalSlider(verticalSlider, 0f, 1f);
                });
            }
        }
    }
}