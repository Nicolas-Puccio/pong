using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CanvasBehaviour : MonoBehaviour
{

  #region Singleton

  public static CanvasBehaviour singleton;




  #endregion

  //only on server
  public GameObject startButton;

  void Start()
  {
    singleton = this;


    //sets position of score texts in client and server
    int offset = Screen.height / 2 - 25;
    foreach (RectTransform son in transform.GetComponentsInChildren<RectTransform>())
    {
      if (son.name.StartsWith("score"))
      {
        string pos = son.name[5..];
        son.anchoredPosition = pos == "top" ? new Vector2(0, offset) : pos == "right" ? new Vector2(offset, 0) : pos == "bot" ? new Vector2(0, -offset) : new Vector2(-offset, 0);
      }
    }
  }


  //only on server
  public void StartButton()
  {
    GameMode.singleton.StartGame();
    Destroy(startButton);
  }

  //only on server
  public void EnableButton()
  {
    startButton.SetActive(true);
  }




  //-debug only
  public void Update()
  {
    if (Input.GetKeyDown(KeyCode.H))
    {
      NetworkManager.Singleton.StartHost();
    }
    if (Input.GetKeyDown(KeyCode.C))
    {
      NetworkManager.Singleton.StartClient();
    }

    if (Input.GetKeyDown(KeyCode.Z))
    {
      GameMode.singleton.ChangeCameraSize(1);
    }

    if (Input.GetKeyDown(KeyCode.X))
    {
      GameMode.singleton.ChangeCameraSize(-1);
    }
  }
}
