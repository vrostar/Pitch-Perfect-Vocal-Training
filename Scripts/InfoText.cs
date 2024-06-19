using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoText : MonoBehaviour
{
    public int fontSize = 20;

    public int w = 300;
    public int h = 200;

    public Color color = Color.white;
    public string text;

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = color;

        int x = 30;
        int y = 100;

        GUI.color = color;
        GUI.Label(new Rect(x, y, w, h), text, style);
    }
}
