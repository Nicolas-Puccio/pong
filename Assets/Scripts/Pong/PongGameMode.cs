using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PongGameMode : NetworkBehaviour
{
  public static PongGameMode singleton;
  static GameObject wallPrefab;
  static GameObject ballPrefab;
  static GameObject pickablePrefab;
  public GameObject startButton;//set in unity editor dragndrop

  void Start()
  {
    wallPrefab = Resources.Load<GameObject>("Pong/Wall");
    ballPrefab = Resources.Load<GameObject>("Pong/Ball");
    pickablePrefab = Resources.Load<GameObject>("Pong/Pickable");

    if (!startButton)
      Debug.LogError("startButton not set");

    singleton = this;
  }





  string[] positions = new string[] { "top", "bot", "right", "left" }; //order in which to spawn
  int nextPos = 0; //index de la posicion del proximo jugador

  Transform wallLeft, wallRight;



  public override void OnNetworkSpawn()
  {
    if (!IsHost)
    {
      Debug.Log("gamemode disable self on client");
      enabled = false;
    }
  }



  //called by PlayerMovement to get the position it should go in  
  public string GetPos()
  {

    //min players required to start -1
    if (nextPos == 0)//-change to 1
    {
      startButton.SetActive(true);
    }

    return positions[nextPos++];
  }



  //map size is dynamic so wall position and scale must be dynamic too
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




  //called by startButton ui
  public void StartGame()
  {
    Destroy(startButton);

    //spawns initial ball
    GameObject ball = Instantiate(ballPrefab);
    ball.GetComponent<BallMovement>().enabled = true;
    ball.GetComponent<NetworkObject>().Spawn();


    SpawnWalls();
    PongGameState.singleton.GameStartClientRpc();//notify clients that the game started
    StartCoroutine(SpawnPickable());//start pickable spawn coroutine
  }



  void SpawnWalls()
  {
    if (nextPos < 4)//spawn left wall if under 4 players
    {
      GameObject wall = Instantiate(wallPrefab, new Vector2(-BackgroundSize.backgroundSize / 2, 0), Quaternion.identity);
      wall.transform.localScale = new Vector2(1, BackgroundSize.backgroundSize);
      wall.GetComponent<NetworkObject>().Spawn();
      wall.name = "wallleft";
      wallLeft = wall.transform;//stores reference to dynamically adjust in Update
    }
    if (nextPos < 3)//spawn right wall if under 3 players
    {
      GameObject wall = Instantiate(wallPrefab, new Vector2(BackgroundSize.backgroundSize / 2, 0), Quaternion.identity);
      wall.transform.localScale = new Vector2(1, BackgroundSize.backgroundSize);
      wall.GetComponent<NetworkObject>().Spawn();
      wall.name = "wallright";
      wallRight = wall.transform;//stores reference to dynamically adjust in Update
    }
  }



  //if pos == "" should shock none
  //if pos == "all" should shock all
  public void Shock(string pos)
  {
    if (pos == "")
    {
      //workaround si la pelota entra en una posicion invalida llama a este metodo, no pude llamar al coroutine desde afuera del script asi q probe esto y anduvo, idk
    }
    else if (pos == "all")
    {
      //-call itself multiple times?
    }
    else
    {
      CanvasBehaviour.singleton.IncreaseScore(pos);
      //-
    }

    StartCoroutine(TryRespawnBall());
  }



  IEnumerator TryRespawnBall()
  {
    yield return new WaitForSeconds(.1f);//wait for the last ball to get destroyed (can't DestroyImmediate because of network)

    GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
    if (balls.Length == 0)//there are no balls spawned
    {
      GameObject ball = Instantiate(ballPrefab);
      ball.GetComponent<BallMovement>().enabled = true;
      ball.GetComponent<NetworkObject>().Spawn();
    }
  }



  IEnumerator SpawnPickable()
  {
    while (true)//-should this stop?
    {
      //random Vector2 position of pickabled
      Vector2 position = new(Random.Range(-BackgroundSize.backgroundSize / 3, BackgroundSize.backgroundSize / 3),
          Random.Range(-BackgroundSize.backgroundSize / 3, BackgroundSize.backgroundSize / 3));


      GameObject pickable = Instantiate(pickablePrefab, position, Quaternion.identity);
      pickable.GetComponent<Pickable>().enabled = true;
      pickable.GetComponent<NetworkObject>().Spawn();

      yield return new WaitForSeconds(10);
    }
  }





  //called by pickable and canvasbehaviour debug
  [ClientRpc]
  public void CameraSizeClientRpc(float size)
  {
    PongGameState.singleton.CameraSize += size;
  }
}
