using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameState : NetworkBehaviour
{

  #region Singleton

  public static GameState singleton;

  private void Start()
  {
    singleton = this;
  }

  #endregion



  //called by gamemode
  [ClientRpc]
  public void GameStartClientRpc()
  {
    Debug.Log("GAME START");
  }



  //called by gamemode
  [ClientRpc]
  public void CameraSizeClientRpc(float size)
  {
    Camera.main.orthographicSize = size;
  }



  //called by playermovement
  public void PlayerJoined(string position, bool isMe)
  {
    Debug.Log(position);
    GameObject.Find($"score{position}").GetComponent<Text>().enabled = true;
  }
}
