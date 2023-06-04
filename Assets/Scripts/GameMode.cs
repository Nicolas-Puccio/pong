using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameMode : NetworkBehaviour
{

  #region Singleton and Resources.Load

  private static GameMode singleton;

  // Public property to access the instance
  public static GameMode Singleton
  {
    get { return singleton; }
  }

  private void Awake()
  {
    //load Wall Resource
    if (!wallPrefab)
      wallPrefab = Resources.Load<GameObject>("Wall");

    //load Ball Resource
    if (!ballPrefab)
      ballPrefab = Resources.Load<GameObject>("Ball");

    //load Pickable Resource
    if (!pickablePrefab)
      pickablePrefab = Resources.Load<GameObject>("Pickable");


    // Ensure only one instance of the Singleton exists
    if (singleton != null && singleton != this)
    {
      Destroy(gameObject);
      return;
    }

    singleton = this;
  }

  #endregion


  string[] positions = new string[] { "top", "bot", "right", "left" };
  int nextPos = 0; //index de la posicion del proximo jugador


  static GameObject wallPrefab;
  static GameObject ballPrefab;
  static GameObject pickablePrefab;



  public override void OnNetworkSpawn()
  {
    if (!IsHost)
    {
      Debug.Log("gamemode disable self on client");
      enabled = false;
    }
  }


  //used by playermovement    
  public string getPos()
  {

    //when there are at least 2 players
    if (nextPos == 0)//-change to 1
    {
      CanvasBehaviour.Singleton.EnableButton();
    }

    return positions[nextPos++];
  }


  //called by ui
  public void StartGame()
  {

    GameObject ball = Instantiate(Resources.Load<GameObject>("Ball"));
    ball.GetComponent<BallMovement>().enabled = true;
    ball.GetComponent<NetworkObject>().Spawn();

    SpawnWalls();
    GameState.Singleton.GameStartClientRpc();
    StartCoroutine(SpawnPickable());
  }



  void SpawnWalls()
  {
    if (nextPos < 4)
    {
      GameObject wall = Instantiate(wallPrefab, new Vector2(-BackgroundSize.backgroundSize / 2, 0), Quaternion.identity);
      wall.transform.localScale = new Vector2(1, BackgroundSize.backgroundSize);
      wall.GetComponent<NetworkObject>().Spawn();
      wall.name = "wallleft";
    }
    if (nextPos < 3)
    {
      GameObject wall = Instantiate(wallPrefab, new Vector2(BackgroundSize.backgroundSize / 2, 0), Quaternion.identity);
      wall.transform.localScale = new Vector2(1, BackgroundSize.backgroundSize);
      wall.GetComponent<NetworkObject>().Spawn();
      wall.name = "wallright";
    }
  }



  //if pos == "" should shock all
  public void Shock(string pos)
  {
    //-handle pos == ""
    if (pos == "")
    {
      //call itself for all players?
      return;
    }




    Text scoreText = GameObject.Find($"score{pos}").GetComponent<Text>();
    scoreText.text = (int.Parse(scoreText.text) + 1).ToString();

    GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
    if (balls.Length == 1)
    {
      GameObject ball = Instantiate(ballPrefab);
      ball.GetComponent<BallMovement>().enabled = true;
      ball.GetComponent<NetworkObject>().Spawn();
    }
  }



  IEnumerator SpawnPickable()
  {
    while (true)
    {
      Vector2 position = new Vector2(Random.Range(-BackgroundSize.backgroundSize / 3, BackgroundSize.backgroundSize / 3),
          Random.Range(-BackgroundSize.backgroundSize / 3, BackgroundSize.backgroundSize / 3));


      GameObject pickable = Instantiate(pickablePrefab, position, Quaternion.identity);
      pickable.GetComponent<Pickable>().enabled = true;
      pickable.GetComponent<NetworkObject>().Spawn();

      yield return new WaitForSeconds(2);
    }
  }


}
