using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class PlayerMovement : NetworkBehaviour
{
  public float speed = 10f; // set by editor
  public float playerOffsetFromEdge; // set by editor

  public NetworkVariable<int> inputDirection;//1 = normal input, -1 = inverted input

  public NetworkVariable<FixedString32Bytes> position = new(); // set by OnNetworkSpawn

  public override void OnNetworkSpawn()
  {
    inputDirection.Value = 1;//-can i set this above?

    if (IsHost)
    {
      //distance from center
      float playerSpawnDistance = BackgroundSize.backgroundSize / 2 - playerOffsetFromEdge;

      position.Value = GameMode.singleton.GetPos();
      transform.SetLocalPositionAndRotation(
        position.Value == "top" ? new Vector2(0, playerSpawnDistance) : position.Value == "right" ? new Vector2(playerSpawnDistance, 0) : position.Value == "bot" ? new Vector2(0, -playerSpawnDistance) : new Vector2(-playerSpawnDistance, 0),
        position.Value == "top" || position.Value == "bot" ? Quaternion.Euler(0f, 0f, 90f) : Quaternion.identity);
    }

    if (!IsOwner)
    {
      enabled = false;
      Debug.Log("player disabled self");
    }

    GameState.singleton.PlayerJoined(position.Value.ToString(), IsOwner);
  }




  // Update is called once per frame
  void Update()
  {
    // Get the vertical input axis (up and down arrow keys)
    float input = Input.GetAxis(position.Value == "top" || position.Value == "bot" ? "Horizontal" : "Vertical") * speed * Time.deltaTime * inputDirection.Value;

    // Calculate the new position based on the vertical input
    Vector3 newPosition = transform.position + new Vector3(position.Value == "top" || position.Value == "bot" ? input : 0f, position.Value == "left" || position.Value == "right" ? input : 0f, 0f);

    float playerSpawnDistance = BackgroundSize.backgroundSize / 2 - playerOffsetFromEdge;

    newPosition = new Vector2(position.Value == "top" || position.Value == "bot" ? newPosition.x : position.Value == "right" ? playerSpawnDistance : -playerSpawnDistance,
    position.Value == "left" || position.Value == "right" ? newPosition.y : position.Value == "top" ? playerSpawnDistance : -playerSpawnDistance);


    transform.position = newPosition;
  }
}
