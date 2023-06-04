using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSize : MonoBehaviour
{
  //1-100% how much of the screen shoudl the background take?
  public int mapRatio;
  //static reference of the backgroundSize other gameobjects can use to position themselves
  public static float backgroundSize;


  void Start()
  {
    if (mapRatio == 0)
      Debug.LogError("mapRatio not set");
  }


  void Update()
  {
    backgroundSize = Camera.main.orthographicSize * 2 * mapRatio / 100;
    transform.localScale = new Vector2(backgroundSize, backgroundSize);
  }
}
