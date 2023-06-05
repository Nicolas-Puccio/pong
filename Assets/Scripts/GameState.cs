using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameState : NetworkBehaviour
{

  #region Singleton

  public float cameraLerpSpeed;
  float cameraSize;

  public static GameState singleton;

  private void Start()
  {
    singleton = this;

    if (cameraLerpSpeed == 0f)
      Debug.LogError("cameraLerpSpeed not set");

    cameraSize = Camera.main.orthographicSize;
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
    cameraSize = size;
  }

  void Update()
  {
    Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraSize, cameraLerpSpeed * Time.deltaTime);
  }


  //called by playermovement
  public void PlayerJoined(string position, bool isMe)
  {
    Debug.Log(position);
    GameObject.Find($"score{position}").GetComponent<Text>().enabled = true;
  }
}
