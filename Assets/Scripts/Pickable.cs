using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pickable : NetworkBehaviour
{
  //-deberia usar herencia
  public MultiplyProperties multiplyProperties;
  public ShockProperties shockProperties;
  public WackyBallProperties wackyBallProperties;

  static GameObject ballPrefab;

  void Start()
  {
    if (!ballPrefab)
    {
      ballPrefab = ballPrefab = Resources.Load<GameObject>("Ball");
    }


    var random = UnityEngine.Random.Range(0, 3);

    switch (random)
    {
      case 0:
        // Handle Multiply powerup
        multiplyProperties = new MultiplyProperties(UnityEngine.Random.Range(1, 5));
        GetComponent<SpriteRenderer>().color = new Color(0.5f, .5f, 0.5f, 1f);
        break;
      case 1:
        // Handle Shock powerup
        shockProperties = new ShockProperties(UnityEngine.Random.value > 0.8f);
        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);
        break;
      case 2:
        // Handle Wacky powerup
        wackyBallProperties = new WackyBallProperties(100);
        GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 1f);
        break;
      default:
        // Handle unknown powerup type
        Debug.Log("Unknown powerup type.");
        break;
    }
  }



  void OnTriggerEnter2D(Collider2D col)
  {
    if (!IsHost)
      return;

    if (!col.gameObject.CompareTag("Ball"))
      return;



    //-disable self to avoid infinite loop do i really need this?
    GetComponent<CircleCollider2D>().enabled = false;



    if (multiplyProperties != null)
    {
      for (int i = 0; i < multiplyProperties.amount; i++)
      {
        Debug.Log("SPAWN BALL");
        GameObject ball = Instantiate(ballPrefab, transform);
        ball.GetComponent<BallMovement>().enabled = true;
        ball.GetComponent<NetworkObject>().Spawn();
      }

      //Destroy self
      GetComponent<NetworkObject>().Despawn();

    }



    if (shockProperties != null)
    {
      //-get pos of player
      string pos = "";



      GameMode.Singleton.Shock(pos);
    }


    if (wackyBallProperties != null)
    {
      col.GetComponent<BallMovement>().wackyChance = wackyBallProperties.steerChance;
    }



    Debug.Log("destroy pickable");
    Destroy(gameObject);
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
  public bool shockAll; //true shocks all players, false only player that hit it


  public ShockProperties(bool shockAll)
  {
    this.shockAll = shockAll;
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
