![License MIT](https://img.shields.io/badge/license-MIT-green.svg)

# Debug UI System

![screen](https://i.imgur.com/n9ZBpC3.png)

Uses IMGUI to draw a menu system. Add actions and windows to this menu system with a little as one line of code.

## Installation

Download the latest Unity package here: https://github.com/the-mr-matt/DebugUISystem/releases

<em>This is not included by default in case you use a different input system.</em>

## Usage

See example folder for a demo.

1. Add the DebugUISystem prefab into your scene.
2. Call `DebugUISystem.RegisterAction` or `DebugUISystem.RegisterWindow`.

To hide the menus, simple disable the `DebugUISystem` component.

## Path

When registering an `action` or `window`, you need to pass in a `path`. Paths are of the following format:

`Menu/Sub Menu/Sub Menu 2/Action` or `Menu/Sub Menu/Sub Menu 2/Window Name`

You can add as many windows or actions to a menu or sub menu as you like.

<em>e.g.</em> `Diagnostics/Stats/FPS` will generate a menu button called Diagnostics, with a sub menu called Stats, with an FPS button.

## Actions

Actions are a single button press. Use a lambda or a function as the action. This is called when the button is pressed.

```csharp
private void Start()
{
    string path = "File/Save";
    DebugUISystem.RegisterAction(path: path, action: () => Debug.Log("Action Called"));
}
```

## Windows

Windows can contain customizable content. Content is drawn using IMGUI -- you are free to draw whatever you want inside the window. `GUILayout` is recommended for ease of use.

<em>Windows are not resizable.</em>

```csharp
private void Start()
{
    string path = "File/Save";
    DebugUISystem.RegisterWindow(path: path, defaultSize: new Vector2(250, 400), onDrawWindow: () =>
    {
        GUILayout.BeginHorizontal();
        GUILayout.Button("Left");
        GUILayout.Button("Right");
        GUILayout.EndHorizontal();
    });
}
```