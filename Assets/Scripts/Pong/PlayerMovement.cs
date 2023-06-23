using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class PlayerMovement : NetworkBehaviour
{
  public float speed = 10f; // set by editor
  public float playerOffsetFromEdge; // set by editor

  public NetworkVariable<FixedString32Bytes> position = new(); // set by OnNetworkSpawn


  #region pickable properties

  public NetworkVariable<int> inputDirection;//1 = normal input, -1 = inverted input

  public bool weak;//-false

  #endregion

  BoxCollider2D boxCollider;


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

      boxCollider = GetComponent<BoxCollider2D>();
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


  public void TryToTilt(Collision2D col, Vector3 ballPosition)
  {
    return;//-not working properly

    if (!weak)
      return;

    float strength = GetTiltStrength(col.GetContact(0).point);
    Debug.Log(strength);

    if (strength == 0f)
      return;

    if (position.Value == "top")
    {
      if (ballPosition.x > transform.position.x)
        transform.Rotate(new(0f, 0f, 5f));
      else
        transform.Rotate(new(0f, 0f, -5f));
      Debug.Log("DONE");
    }

    Debug.Log("tilt mf"); //use GetContacts instead?
  }


  //players only tilt when hit comes from the center of the map, not the sides
  //returns 0 if should not tilt
  float GetTiltStrength(Vector2 contact)
  {
    // Get the bounds of the box collider
    Bounds bounds = boxCollider.bounds;

    // Calculate the center and extents of the box collider
    Vector2 center = bounds.center;
    Vector2 extents = bounds.extents;

    // Calculate the distances from the contact position to the box collider's center
    float distanceX = contact.x - center.x;
    float distanceY = contact.y - center.y;

    // Calculate the normalized distances
    float normalizedDistanceX = distanceX / extents.x;
    float normalizedDistanceY = distanceY / extents.y;



    // Check the absolute values of the normalized distances to determine the side of collision
    if (Mathf.Abs(normalizedDistanceX) > Mathf.Abs(normalizedDistanceY))
    {
      if (normalizedDistanceX > 0)
        return position.Value == "left" ? 1 : 0;//hit comes from right, so only affects if player is left
      else
        return position.Value == "right" ? 1 : 0;
    }
    else
    {
      if (normalizedDistanceY > 0)
        return position.Value == "bot" ? 1 : 0;
      else
        return position.Value == "top" ? 1 : 0;
    }
  }
}
