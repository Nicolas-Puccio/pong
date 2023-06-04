using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class PlayerMovement : NetworkBehaviour
{
  public float speed = 10f; // set by editor
  public float playerOffsetFromEdge; // set by editor

  public NetworkVariable<FixedString32Bytes> position = new NetworkVariable<FixedString32Bytes>(); // set by OnNetworkSpawn

  public override void OnNetworkSpawn()
  {
    if (IsHost)
    {
      //distance from center
      float playerSpawnDistance = BackgroundSize.backgroundSize / 2 - playerOffsetFromEdge;

      position.Value = GameMode.Singleton.getPos();
      transform.localPosition = position.Value == "top" ? new Vector2(0, playerSpawnDistance) : position.Value == "right" ? new Vector2(playerSpawnDistance, 0) : position.Value == "bot" ? new Vector2(0, -playerSpawnDistance) : new Vector2(-playerSpawnDistance, 0);
      transform.localRotation = position.Value == "top" || position.Value == "bot" ? Quaternion.Euler(0f, 0f, 90f) : Quaternion.identity;
    }

    if (!IsOwner)
    {
      enabled = false;
      Debug.Log("player disabled self");
    }

    GameState.Singleton.PlayerJoined(position.Value.ToString(), IsOwner);
  }




  // Update is called once per frame
  void Update()
  {
    // Get the vertical input axis (up and down arrow keys)
    float input = Input.GetAxis(position.Value == "top" || position.Value == "bot" ? "Horizontal" : "Vertical") * speed * Time.deltaTime;

    // Calculate the new position based on the vertical input
    Vector3 newPosition = transform.position + new Vector3(position.Value == "top" || position.Value == "bot" ? input : 0f, position.Value == "left" || position.Value == "right" ? input : 0f, 0f);

    // Move the object to the new position
    transform.position = newPosition;
  }
}
