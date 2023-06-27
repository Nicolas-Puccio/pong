using UnityEngine;
using Unity.Netcode;

public class HippoPlayerController : NetworkBehaviour
{
  NetworkVariable<bool> isMyTurn;
  public bool IsMyTurn
  {
    get { return isMyTurn.Value; }
    set { isMyTurn.Value = value; }
  }



  void Start() { }//in order to disable script



  public override void OnNetworkSpawn()
  {
    if (IsHost)
    {
      HippoGameMode.singleton.PlayerJoined(this);
    }

    if (IsOwner)
    {
      enabled = true;
      Debug.Log("player enabled self");
    }
  }



  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      //-make ray only detect teeth? layer? tag?
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

      if (hit.collider != null)
      {
        Debug.Log("CLICKED " + hit.collider.name);

        if (IsHost)
          HippoGameMode.singleton.Touch(this, hit.transform);//hostcalls touch
        else
          TouchServerRpc(hit.transform.GetComponent<NetworkObject>().NetworkObjectId);//clients call touch through a RPC
      }
    }
  }



  [ServerRpc]
  void TouchServerRpc(ulong networkObjectId)
  {
    //finds tooth by networkObjectID
    NetworkObject tooth = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
    HippoGameMode.singleton.Touch(this, tooth.transform);
  }
}
