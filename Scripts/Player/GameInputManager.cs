using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameInputManager
{
    static Dictionary<string, KeyCode> keyMapping;
    static string[] keyMaps = new string[13]
    {
        "Tire",
        "Viser",
        "Avancer",
        "Reculer",
        "strafeGauche",
        "strafeDroite",
        "ArmeSuivante",
        "ArmePrecedente",
        "Recharger",
        "Acroupie",
        "Sprint",
        "Sauter",
        "Interagire"
    };
    static KeyCode[] defaults = new KeyCode[13]
    {
        KeyCode.Mouse0,
        KeyCode.Mouse1,
        KeyCode.Z,
        KeyCode.S,
        KeyCode.Q,
        KeyCode.D,
        KeyCode.F,
        KeyCode.C,
        KeyCode.R,
        KeyCode.LeftControl,
        KeyCode.LeftShift,
        KeyCode.Space,
        KeyCode.E
    };

    static GameInputManager()
    {
        InitializeDictionary();
    }

    private static void InitializeDictionary()
    {
        keyMapping = new Dictionary<string, KeyCode>();
        for (int i = 0; i < keyMaps.Length; ++i)
        {
            keyMapping.Add(keyMaps[i], defaults[i]);
        }
    }

    public static void SetKeyMap(string keyMap, KeyCode key)
    {
        if (!keyMapping.ContainsKey(keyMap))
            throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
        keyMapping[keyMap] = key;
    }

    public static bool GetKeyDown(string keyMap)
    {
        return Input.GetKeyDown(keyMapping[keyMap]);
    }

    public static bool GetKey(string keyMap)
    {
        return Input.GetKey(keyMapping[keyMap]);
    }

    public static bool GetKeyUp(string keyMap)
    {
        return Input.GetKeyUp(keyMapping[keyMap]);
    }
}
