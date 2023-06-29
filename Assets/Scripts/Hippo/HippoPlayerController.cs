using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class HippoPlayerController : NetworkBehaviour
{
  public GameObject yourTurnText;
  NetworkVariable<bool> isMyTurn = new();
  public bool IsMyTurn
  {
    get { return isMyTurn.Value; }
    set { isMyTurn.Value = value; }
  }



  void Start()
  {

  }//need a Start() in order to disable script on editor



  public override void OnNetworkSpawn()
  {
    if (IsHost)
    {
      HippoGameMode.singleton.PlayerJoined(this);//notify gamemode we joined...//-isn't there an event the gamemode can hook to?
    }

    if (IsOwner)
    {
      enabled = true;//if this is my player controller, we enable it
      Debug.Log("player enabled self");


      yourTurnText = GameObject.Find("YourTurnText");
      if (!yourTurnText)
        Debug.LogError("yourTurnText not set");


      isMyTurn.OnValueChanged += (bool oldValue, bool newValue) =>
      {
        yourTurnText.GetComponent<Text>().enabled = newValue;
      };
    }
  }



  void Update()
  {
    if (Input.GetMouseButtonDown(0))//-make it work with touch
    {
      //-make ray only detect teeth? layer? tag? currently gets anyting
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

      if (hit.collider != null)
      {
        Debug.Log("CLICKED " + hit.collider.name);

        if (!hit.transform.CompareTag("Tooth"))//don't care if it isn't a teeth
          return;

        if (IsHost)
          HippoGameMode.singleton.Touch(this, hit.transform);//host calls touch
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
