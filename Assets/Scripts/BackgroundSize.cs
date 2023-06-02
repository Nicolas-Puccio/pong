using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSize : MonoBehaviour
{
    public int mapRatio;
    public static float backgroundSize;

    void Start()
    {
        float cameraSize = Camera.main.orthographicSize;
        backgroundSize = cameraSize * 2 * mapRatio / 100;
        Debug.Log(backgroundSize);
        GameObject.Find("Background").transform.localScale = new Vector2(backgroundSize,backgroundSize);
    }
}
