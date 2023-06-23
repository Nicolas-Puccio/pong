using UnityEngine;
using Unity.Netcode;

public class HippoPlayerController : NetworkBehaviour
{
  public NetworkVariable<bool> isMyTurn;
  public void SetIsMyTurn(bool isMyTurn)
  {
    this.isMyTurn.Value = isMyTurn;
  }



  public override void OnNetworkSpawn()
  {
    if (IsHost)
    {
      HippoGameMode.singleton.PlayerJoined(this);
    }

    if (IsOwner)
    {
      enabled = true;
      Debug.Log("player disabled self");
    }
  }

  void Start() { }

  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

      if (hit.collider != null)
      {
        Debug.Log("CLICKED " + hit.collider.name);

        if (IsHost)
          HippoGameMode.singleton.Touch(this, hit.transform);
        else
          TouchServerRpc(hit.transform.GetComponent<NetworkObject>().NetworkObjectId);

      }
    }
  }


  [ServerRpc]
  void TouchServerRpc(ulong networkObjectId)
  {
    NetworkObject tooth = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
    HippoGameMode.singleton.Touch(this, tooth.transform);
  }
}
