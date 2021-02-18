// =================================
//      (C) Winglett 2021
// =================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Winglett
{
    public static class GUIExtensions
    {
        public static void Toolbar(Vector2 rectXY, string[] texts, Action<int, Rect> onSelect)
        {
            // Config
            const float BUTTON_HEIGHT = 20.0f;
            const float TEXT_PADDING = 15.0f;
            const float CHARACTER_WIDTH = 7.0f;
            const float SPACING = 2.0f;

            float alignment = 0f;

            for (int i = 0; i < texts.Length; i++)
            {
                float width = TEXT_PADDING + texts[i].Length * CHARACTER_WIDTH;
                Rect rect = new Rect(x: rectXY.x + alignment, y: rectXY.y, width: width, height: BUTTON_HEIGHT);
                if (GUI.Button(rect, texts[i]))
                {
                    onSelect?.Invoke(i, rect);
                }

                alignment += width + SPACING;
            }
        }

        public static Rect Dropdown(int id, Vector2 rectXY, string[] texts, bool[] subArrows, Action<int> onSelect)
        {
            // Config
            const float BUTTON_HEIGHT = 20.0f;
            const float SPACING = 2.0f;
            const float CHARACTER_WIDTH = 7.0f;
            const float TEXT_RIGHT_PADDING = 30.0f;

            // x: Top, y: Right, z: Bottom, w: Left
            Vector4 padding = new Vector4(5.0f, 5.0f, 5.0f, 5.0f);

            // Find the width of the longest sub menu name
            // Window, and all other sub menu buttons will be the same width
            float maxLabelWidth = 0f;
            for (int i = 0; i < texts.Length; i++)
            {
                float width = texts[i].Length * CHARACTER_WIDTH + padding.y;
                if (width > maxLabelWidth) maxLabelWidth = width;
            }

            // Draw dropdown
            Rect rect = new Rect(x: rectXY.x, y: rectXY.y, width: maxLabelWidth + padding.y + padding.w + TEXT_RIGHT_PADDING, height: BUTTON_HEIGHT * texts.Length + SPACING * (texts.Length - 1) + padding.x + padding.z);
            GUI.Window(id, rect, windowID =>
            {
                // Draw button for each text item
                for (int i = 0; i < texts.Length; i++)
                {
                    // Draw button
                    Rect buttonRect = new Rect(x: padding.w, y: (BUTTON_HEIGHT + SPACING) * i + padding.x, width: maxLabelWidth + TEXT_RIGHT_PADDING, height: BUTTON_HEIGHT);
                    if (GUI.Button(buttonRect, texts[i], GUI.skin.GetStyle("Menu Button")))
                    {
                        onSelect?.Invoke(i);
                    }

                    // If there's another sub menu, draw an arrow
                    if (subArrows[i])
                    {
                        Rect labelRect = new Rect(x: maxLabelWidth - 4 + TEXT_RIGHT_PADDING, y: buttonRect.y, width: 20, height: 20);
                        GUI.Label(labelRect, ">");
                    }
                }
            }, "", GUI.skin.GetStyle("Menu Panel"));

            return rect;
        }

        public static Rect Window(int id, string windowName, Rect rect, Action onDrawWindowContent, Action onCloseWindow)
        {
            // Draw window
            return GUI.Window(id, rect, windowID =>
            {
                // Close Button
                if (GUI.Button(new Rect(4, 4, 11, 11), "x", GUI.skin.GetStyle("Close Button"))) onCloseWindow?.Invoke();

                onDrawWindowContent?.Invoke();

                GUI.DragWindow();
            }, windowName);
        }
    }
}