using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePosition : MonoBehaviour
{
    void Start()
    {
        int offset = Screen.height/2-30;
        foreach (RectTransform son in transform.GetComponentsInChildren<Transform>())
        {
            string pos = son.name.Substring(5);
            son.anchoredPosition = pos == "top" ? new Vector2(0,offset) : pos == "right" ? new Vector2(offset,0) : pos == "bot" ? new Vector2(0,-offset) : new Vector2(-offset,0);
            
            
        }
/*
        // Set the position relative to the top of the screen
        Vector2 position = new Vector2(Screen.width / 2f, Screen.height - 50f); // Adjust the Y offset as needed
        textRectTransform.anchoredPosition = position;
        RectTransform textRectTransform = GetComponent<RectTransform>();*/
    }
}
