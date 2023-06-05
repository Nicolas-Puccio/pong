using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Pickable : NetworkBehaviour
{
  public NetworkVariable<FixedString32Bytes> type = new();



  //-deberia usar herencia
  public MultiplyProperties multiplyProperties;
  public ShockProperties shockProperties;
  public WackyBallProperties wackyBallProperties;

  public InvertInputProperties invertInputProperties;

  static GameObject ballPrefab;


  public override void OnNetworkSpawn()
  {
    if (IsHost)
      Init();

    SetImage();
  }


  void Init()
  {
    if (!ballPrefab)
      ballPrefab = ballPrefab = Resources.Load<GameObject>("Ball");

    var random = Random.Range(0, 4);

    switch (random)
    {
      case 0:
        multiplyProperties = new MultiplyProperties(Random.Range(1, 5));
        type.Value = "Multiply";
        break;
      case 1:
        shockProperties = new ShockProperties(true,false,true);//-
        type.Value = "Shock";
        break;
      case 2:
        wackyBallProperties = new WackyBallProperties(100);
        type.Value = "Wacky";
        break;
      case 3:
        //invert self 25% chance
        if (Random.value > .75f)
        {
          invertInputProperties = new InvertInputProperties(true, false, false);
          type.Value = "InvertSelf";
        }
        //invert next player 25% chance
        else if (Random.value > .5f)
        {
          invertInputProperties = new InvertInputProperties(false, true, false);
          type.Value = "InvertNext";
        }
        //invert only enemies 25% chance
        else if (Random.value > .25f)
        {
          invertInputProperties = new InvertInputProperties(false, false, true);
          type.Value = "InvertEnemies";
        }
        //invert everyone 25% chance
        else
        {
          invertInputProperties = new InvertInputProperties(true, false, true);
          type.Value = "InvertAll";
        }
        break;
      default:
        Debug.LogError("Unknown powerup type.");
        break;
    }
  }



  void SetImage()
  {
    switch (type.Value.ToString())
    {
      case "Multiply":
        GetComponent<SpriteRenderer>().color = new Color(0.5f, .5f, 0.5f, 1f);
        break;
      case "Shock":
        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);
        break;
      case "Wacky":
        GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 1f);
        break;
      case "InvertSelf":
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f, 1f);
        break;
      case "InvertNext":
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f, 1f);
        break;
      case "InvertEnemies":
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f, 1f);
        break;
      case "InvertAll":
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f, 1f);
        break;
      default:
        Debug.LogError("Unknown powerup type2.");
        break;
    }
  }



  void OnTriggerEnter2D(Collider2D col)
  {
    if (!IsHost)
      return;

    if (!col.gameObject.CompareTag("Ball"))
      return;

    BallMovement ballMovement = col.GetComponent<BallMovement>();

    //-disable self to avoid infinite loop... do i really need this?
    GetComponent<CircleCollider2D>().enabled = false;



    if (multiplyProperties != null)
    {
      for (int i = 0; i < multiplyProperties.amount; i++)
      {
        GameObject ball = Instantiate(ballPrefab, transform);
        ball.GetComponent<BallMovement>().enabled = true;
        ball.GetComponent<NetworkObject>().Spawn();
      }

    }



    if (shockProperties != null)
    {
      //-get pos of player
      string pos = "";



      GameMode.singleton.Shock(pos);
    }



    if (wackyBallProperties != null)
    {
      ballMovement.wackyChance = wackyBallProperties.steerChance;
    }



    if (invertInputProperties != null)
    {
      ballMovement.invertNext = invertInputProperties.invertNext;

      if (invertInputProperties.invertSelf)
      {
        ballMovement.lastPlayerHit.GetComponent<PlayerMovement>().inputDirection.Value = -ballMovement.lastPlayerHit.GetComponent<PlayerMovement>().inputDirection.Value;
      }

      if (invertInputProperties.invertEnemies)
      {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
          if (player != ballMovement.lastPlayerHit)
            player.GetComponent<PlayerMovement>().inputDirection.Value = -player.GetComponent<PlayerMovement>().inputDirection.Value;
        }
      }
    }


    Debug.Log("destroy pickable");

    //Destroy self
    GetComponent<NetworkObject>().Despawn();
  }
}

public class MultiplyProperties
{
  public int amount; // amount of new balls spawned

  public MultiplyProperties(int amount)
  {
    this.amount = amount;
  }
}


public class ShockProperties
{
  public bool shockSelf; //shock player who touch it
  public bool shockNext; //shock next player that touches it
  public bool shockEnemies; //shock enemies

  //si Self y Enemies son ambas verdaderas entonces shockea a todos

  public ShockProperties(bool shockSelf, bool shockNext, bool shockEnemies)
  {
    this.shockSelf = shockSelf;
    this.shockNext = shockNext;
    this.shockEnemies = shockEnemies;
  }
}


public class WackyBallProperties
{
  public int steerChance; //0-100 chance the ball stears

  public WackyBallProperties(int steerChance)
  {
    this.steerChance = steerChance;
  }
}

public class InvertInputProperties
{
  public bool invertSelf; //invert input of player who touch it
  public bool invertNext; //invert input of next player that touches it
  public bool invertEnemies; //invert enemies input

  //si Self y Enemies son ambas verdaderas entonces invierte a todos

  public InvertInputProperties(bool invertSelf, bool invertNext, bool invertEnemies)
  {
    this.invertSelf = invertSelf;
    this.invertNext = invertNext;
    this.invertEnemies = invertEnemies;
  }
}

public class MapSizeProperties
{
  public int amount; //size to add to camera

  public MapSizeProperties(int amount)
  {
    this.amount = amount;
  }
}
