using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class HippoGameMode : NetworkBehaviour
{
  public static HippoGameMode singleton;
  public GameObject toothPrefab;
  public GameObject hintPrefab;
  public GameObject startButton;//set in unity editor dragndrop

  void Start()
  {
    hintPrefab = Resources.Load<GameObject>("Hippo/Hint");
    toothPrefab = Resources.Load<GameObject>("Hippo/Tooth");


    if (!startButton)
      Debug.LogError("startButton not set");

    singleton = this;
  }





  List<HippoPlayerController> players;//uses the order of the list to manage turns, currently order of join, could be shuffled
  bool AllowInput = true;//to compensate with delay or lag if the user touches a tooth juuuust beore it was just turn in order to hint it, and ends up toching it



  public override void OnNetworkSpawn()//called when host or client is created
  {
    if (IsHost)
    {
      startButton.SetActive(true);//enables start button for host
    }
    else
    {
      Debug.Log("gamemode disable self on client");//clients don't need this script
      enabled = false;
    }
  }



  public void StartGame()//called by startButton
  {
    SpawnTeeth();
    //-could shuffle here to make the order random
    players[0].IsMyTurn = true;
  }



  void SpawnTeeth()//spawns teeth and sets one's name to "BadTooth"
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



  //called by HippoPlayerController onNetworkSpawn
  public void PlayerJoined(HippoPlayerController player)
  {
    players.Add(player);
  }



  //called by HippoPlayerController
  public void Touch(HippoPlayerController touchPlayer, Transform tooth)//should receive gameobject instead of ray?
  {
    //ignore input if cooldown
    if (!AllowInput)
      return;



    if (touchPlayer.IsMyTurn)
    {
      AllowInput = false;

      if (tooth.name == "BadTooth")
      {
        Shock(touchPlayer);
      }
      else
      {
        tooth.GetComponent<NetworkObject>().Despawn();
        //-animation?


        //no longer this player's turn
        touchPlayer.IsMyTurn = false;


        //sets next player's turn
        int currentIndex = players.FindIndex(player => player == touchPlayer);
        int nextIndex = (currentIndex + 1) % players.Count;
        players[nextIndex].IsMyTurn = true;


        //enable input coroutine
        StartCoroutine(EnableInput());
      }
    }
    else
    {
      //if it is not the player's turn, it spawns a hitn at the tooth cliked
      SpawnHintClientRpc(tooth.position);
    }
  }





  //server calls this and is executed on all clients spawning the hint
  //the hint is not a network object, i guess it was not really necesary to make it one
  //method gets called regardless of if the script is enabled or not
  [ClientRpc]
  void SpawnHintClientRpc(Vector3 position)
  {
    Quaternion rotation = position.y > 0 ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
    position += new Vector3(0, position.y > 0 ? -1 : 1, 0);


    Instantiate(hintPrefab, position, rotation);
  }





  IEnumerator EnableInput()
  {
    yield return new WaitForSeconds(1);
    AllowInput = true;
  }
}
