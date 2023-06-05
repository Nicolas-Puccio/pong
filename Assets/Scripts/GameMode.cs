using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameMode : NetworkBehaviour
{

  #region Singleton and Resources.Load

  public static GameMode singleton;


  void Start()
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


    singleton = this;
  }

  #endregion


  readonly string[] positions = new string[] { "top", "bot", "right", "left" };
  int nextPos = 0; //index de la posicion del proximo jugador


  static GameObject wallPrefab;
  static GameObject ballPrefab;
  static GameObject pickablePrefab;


  Transform wallLeft;
  Transform wallRight;


  void Update()
  {
    if (wallLeft)
    {
      wallLeft.position = new Vector2(-BackgroundSize.backgroundSize / 2, 0);
      wallLeft.transform.localScale = new Vector2(1, BackgroundSize.backgroundSize);
    }

    if (wallRight)
    {
      wallRight.position = new Vector2(BackgroundSize.backgroundSize / 2, 0);
      wallRight.transform.localScale = new Vector2(1, BackgroundSize.backgroundSize);
    }
  }


  public override void OnNetworkSpawn()
  {
    if (!IsHost)
    {
      Debug.Log("gamemode disable self on client");
      enabled = false;
    }
  }


  //used by playermovement    
  public string GetPos()
  {

    //when there are at least 2 players
    if (nextPos == 0)//-change to 1
    {
      CanvasBehaviour.singleton.EnableButton();
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
    GameState.singleton.GameStartClientRpc();
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
      wallLeft = wall.transform;
    }
    if (nextPos < 3)
    {
      GameObject wall = Instantiate(wallPrefab, new Vector2(BackgroundSize.backgroundSize / 2, 0), Quaternion.identity);
      wall.transform.localScale = new Vector2(1, BackgroundSize.backgroundSize);
      wall.GetComponent<NetworkObject>().Spawn();
      wall.name = "wallright";
      wallRight = wall.transform;
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
      Vector2 position = new(Random.Range(-BackgroundSize.backgroundSize / 3, BackgroundSize.backgroundSize / 3),
          Random.Range(-BackgroundSize.backgroundSize / 3, BackgroundSize.backgroundSize / 3));


      GameObject pickable = Instantiate(pickablePrefab, position, Quaternion.identity);
      pickable.GetComponent<Pickable>().enabled = true;
      pickable.GetComponent<NetworkObject>().Spawn();

      yield return new WaitForSeconds(2);
    }
  }


  public void ChangeCameraSize(float size)
  {
    GameState.singleton.CameraSizeClientRpc(size);
  }
}
