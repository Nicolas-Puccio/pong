using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CanvasBehaviour : NetworkBehaviour
{
  public static CanvasBehaviour singleton;




  public NetworkVariable<int> scoreleft;
  public NetworkVariable<int> scoreright;
  public NetworkVariable<int> scoretop;
  public NetworkVariable<int> scorebot;


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

  public void FixedUpdate()
  {
    foreach (RectTransform son in transform.GetComponentsInChildren<RectTransform>())
    {
      if (son.name == "scoretop")
        son.GetComponent<Text>().text = scoretop.Value.ToString();
      if (son.name == "scorebot")
        son.GetComponent<Text>().text = scorebot.Value.ToString();
      if (son.name == "scoreleft")
        son.GetComponent<Text>().text = scoreleft.Value.ToString();
      if (son.name == "scoreright")
        son.GetComponent<Text>().text = scoreright.Value.ToString();
    }
  }



  public void IncreaseScore(string pos)
  {
    if (pos == "top")
      scoretop.Value++;
    if (pos == "bot")
      scorebot.Value++;
    if (pos == "left")
      scoreleft.Value++;
    if (pos == "right")
      scoreright.Value++;
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
