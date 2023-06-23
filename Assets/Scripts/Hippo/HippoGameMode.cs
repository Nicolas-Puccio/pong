using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class HippoGameMode : NetworkBehaviour
{

  public static HippoGameMode singleton;

  public GameObject startButton;

  public GameObject toothPrefab;
  public GameObject hintPrefab;

  public List<HippoPlayerController> players;//uses the order of the list to manage turns, currently order of join, could be shuffled

  public bool AllowInput = true;//to compensate with delay or lag if the user touches a tooth juuuust beore it was just turn in order to hint it, and ends up toching it


  void Start()
  {
    toothPrefab = Resources.Load<GameObject>("Hippo/Tooth");
    hintPrefab = Resources.Load<GameObject>("Hippo/Hint");

    singleton = this;
  }
  

  public override void OnNetworkSpawn()
  {

    if (IsHost)
    {
      startButton.SetActive(true);
    }
    else
    {
      Debug.Log("gamemode disable self on client");
      enabled = false;
    }
  }


  //called by startButton
  public void StartGame()
  {
    SpawnTeeth();
    //-could shuffle here to make the order random
    players[0].SetIsMyTurn(true);
  }



  void SpawnTeeth()
  {
    int badTooth = Random.Range(0, 16);

    Vector3 pos = new(-4.5f, 1.5f, 0f);

    for (int i = 0; i < 8; i++)
    {
      GameObject tooth = Instantiate(toothPrefab, pos, Quaternion.identity);
      tooth.GetComponent<NetworkObject>().Spawn();

      pos.x += 1;
      if (badTooth == i)
      {
        tooth.name = "BadTooth";
      }
    }

    pos.y = -pos.y;

    for (int i = 0; i < 8; i++)
    {
      GameObject tooth = Instantiate(toothPrefab, pos, Quaternion.identity);
      tooth.GetComponent<NetworkObject>().Spawn();

      pos.x -= 1;

      if (badTooth == i + 8)
      {
        tooth.name = "BadTooth";
      }
    }
  }



  public void Shock(HippoPlayerController player)//-
  {
    Debug.Log("shock" + player.name);
  }



  public void PlayerJoined(HippoPlayerController player)
  {
    players.Add(player);
  }



  public void Touch(HippoPlayerController touchPlayer, Transform tooth)//should receive gameobject instead of ray?
  {
    //ignore input if cooldown
    if (!AllowInput)
      return;


    if (touchPlayer.isMyTurn.Value)
    {
      if (tooth.name == "BadTooth")
      {
        Shock(touchPlayer);
      }
      else
      {
        tooth.GetComponent<NetworkObject>().Despawn();
        //-animation?



        //sets next player's turn
        touchPlayer.SetIsMyTurn(false);
        int currentIndex = players.FindIndex(player => player == touchPlayer);
        int nextIndex = (currentIndex + 1) % players.Count;
        players[nextIndex].SetIsMyTurn(true);
      }
    }
    else
    {
      SpawnHintClientRpc(tooth.position);
    }
  }



  [ClientRpc]
  void SpawnHintClientRpc(Vector3 position)
  {
    Instantiate(hintPrefab, position, Quaternion.identity);
  }
}
