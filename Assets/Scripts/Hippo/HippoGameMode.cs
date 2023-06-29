using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class HippoGameMode : NetworkBehaviour
{
  public static HippoGameMode singleton;
  public GameObject startButton;//set in unity editor dragndrop
  GameObject toothPrefab;
  GameObject hintPrefab;



  void Start()
  {
    hintPrefab = Resources.Load<GameObject>("Hippo/Hint");
    toothPrefab = Resources.Load<GameObject>("Hippo/Tooth");
    players = new();


    if (!startButton)
      Debug.LogError("startButton not set");

    singleton = this;
  }





  List<HippoPlayerController> players;//uses the order of the list to manage turns, currently order of join, could be shuffled on start
  bool AllowInput = true;//to compensate with delay if the user touches a tooth at the same time it becomes their turn...
                         //user intends to hint a tooth, but as it is now his turn, he accidentally touches it, making it fall or causing a shock, this bool prevents it




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



  public void StartGame()//called by startButton and Restart corouting
  {
    startButton.SetActive(false);//hide start button

    SpawnTeeth();
    //-could shuffle players list here to make the order random
    players[0].IsMyTurn = true;
  }



  void SpawnTeeth()//spawns teeth and sets one's name to "BadTooth"
  {
    int badTooth = Random.Range(0, 16);

    Vector3 pos = new(-3.5f, 1.5f, 0f);

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
    pos.x -= 1;


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



  void Shock(HippoPlayerController player)//-
  {
    Debug.Log("shock" + player.name);//-networkid?
  }



  //called by HippoPlayerController onNetworkSpawn
  public void PlayerJoined(HippoPlayerController player)
  {
    players.Add(player);
  }



  //called by HippoPlayerController
  public void Touch(HippoPlayerController touchPlayer, Transform tooth)
  {
    //ignore input if cooldown
    if (!AllowInput)
      return;



    if (touchPlayer.IsMyTurn)
    {
      AllowInput = false;

      if (tooth.name == "BadTooth")//si tocaste el diente malo te shockea(y reinicia el juego)
      {
        Shock(touchPlayer);
        //restart coroutine
        StartCoroutine(Restart());
      }
      else
      {
        tooth.GetComponent<NetworkObject>().Despawn();
        //-animation? if an animation should play (tooth falling) we should not destroy when despawning


        //no longer this player's turn
        touchPlayer.IsMyTurn = false;


        //gets index of current player
        int currentIndex = players.FindIndex(player => player == touchPlayer);

        //adds 1 and loops around if needed
        int nextIndex = (currentIndex + 1) % players.Count;

        //sets next player's turn
        players[nextIndex].IsMyTurn = true;


        //enable input coroutine
        StartCoroutine(EnableInput());
      }
    }
    else
    {
      //if it is not the player's turn, it spawns a hint at the tooth cliked
      SpawnHintClientRpc(tooth.position);
    }
  }



  //despawns all teeth, waits X seconds, starts game again
  IEnumerator Restart()
  {
    //despanw all teeth
    GameObject[] teeth = GameObject.FindGameObjectsWithTag("Tooth");
    foreach (var tooth in teeth)
    {
      tooth.GetComponent<NetworkObject>().Despawn();
    }

    yield return new WaitForSeconds(1f);

    StartGame();
  }





  //server calls this and is executed on all clients spawning the hint
  //the hint is not a network object, i guess it was not really necesary to make it one
  //method gets called regardless of if the script is enabled or not
  [ClientRpc]
  void SpawnHintClientRpc(Vector3 position)
  {
    //si Y es positivo la flecha apunta arriba, sino, abajo
    Quaternion rotation = position.y > 0 ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);

    //posicione el hint entre el centro de la pantalla y el diente
    position += new Vector3(0, position.y > 0 ? -1 : 1, 0);


    Instantiate(hintPrefab, position, rotation);
  }





  IEnumerator EnableInput()
  {
    yield return new WaitForSeconds(.5f);
    AllowInput = true;
  }
}
