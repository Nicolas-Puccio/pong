using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PongGameState : NetworkBehaviour
{
  public static PongGameState singleton;
  public float cameraLerpSpeed;//set in unity editor dragndrop

  private void Start()
  {
    singleton = this;

    if (cameraLerpSpeed == 0f)
      Debug.LogError("cameraLerpSpeed not set");

    cameraSize = Camera.main.orthographicSize;
  }





  float cameraSize;
  public float CameraSize
  {
    get { return cameraSize; }
    set { cameraSize = value; }
  }




  //called by gamemode
  [ClientRpc]
  public void GameStartClientRpc()
  {
    Debug.Log("GAME START");
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
