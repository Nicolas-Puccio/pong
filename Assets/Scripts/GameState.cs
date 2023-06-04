using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameState : NetworkBehaviour
{

  #region Singleton

  private static GameState singleton;

  // Public property to access the instance
  public static GameState Singleton
  {
    get { return singleton; }
  }

  private void Start()
  {
    // Ensure only one instance of the Singleton exists
    if (singleton != null && singleton != this)
    {
      Destroy(gameObject);
      return;
    }

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
