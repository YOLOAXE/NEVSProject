using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameKeyModifier : MonoBehaviour
{
    [Header("Runtime Setting")]
    [SerializeField] private bool onChangeKey = false;
    [SerializeField] private int indicekey = 0;
    [Header("UI Setting")]
    [SerializeField] private TextMeshProUGUI[] textKey = null;
    [Space,Header("Liste Touche")]
    [SerializeField] private string[] keyMaps = new string[13]
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
    [SerializeField] private KeyCode[] defaults = new KeyCode[13]
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

    void Start()
    {
        GetKeyPP();
    }

    void GetKeyPP()
    {
        for(int i = 0;i < defaults.Length;i++)
        {
            string key = PlayerPrefs.GetString(keyMaps[i], "");
            if(key != "")
            {
                textKey[i].text = key;
                GameInputManager.SetKeyMap(keyMaps[i], (KeyCode)System.Enum.Parse(typeof(KeyCode), key));
            }
            else
            {
                textKey[i].text = defaults[i].ToString();
            }            
        }
    }

    public void ChangeKey(int key)
    {
        if (!onChangeKey)
        {
            this.indicekey = key;
            this.textKey[this.indicekey].text = "...";
            onChangeKey = true;
        }
    }

    void Save(int i)
    {
        PlayerPrefs.SetString(keyMaps[i], textKey[i].text);
        GameInputManager.SetKeyMap(keyMaps[i], (KeyCode)System.Enum.Parse(typeof(KeyCode), textKey[i].text));
    }

    void OnGUI()
    {
        Event e = Event.current;
        if(Input.GetKey(KeyCode.LeftShift))
        {
            AplyKey(KeyCode.LeftShift);
        }
        else if (e.isKey)
        {
            AplyKey(e.keyCode);
        }
        else if(e.isMouse)
        {
            AplyKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse" + e.button.ToString())); 
        }
    }

    void AplyKey(KeyCode key)
    {
        if (onChangeKey)
        {
            onChangeKey = false;
            this.textKey[this.indicekey].text = key.ToString();
            Save(this.indicekey);
        }
    }
}
