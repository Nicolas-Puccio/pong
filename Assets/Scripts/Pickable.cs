using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Pickable : NetworkBehaviour
{
  bool destroyed;

  public NetworkVariable<FixedString32Bytes> type = new(); // to sync the type to the clients

  public IPickableProperties pickableProperties;



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

    var random = Random.Range(0, 6);

    switch (random)
    {
      case 0:
        pickableProperties = new MultiplyProperties(Random.Range(1, 5));
        type.Value = "Multiply";
        break;
      case 1:
        //shock self 25% chance
        if (Random.value > .75f)
        {
          pickableProperties = new ShockProperties(true, false, false);
          type.Value = "ShockSelf";
        }
        //shock next player 25% chance
        else if (Random.value > .5f)
        {
          pickableProperties = new ShockProperties(false, true, false);
          type.Value = "ShockNext";
        }
        //shock only enemies 25% chance
        else if (Random.value > .25f)
        {
          pickableProperties = new ShockProperties(false, false, true);
          type.Value = "ShockEnemies";
        }
        //shock everyone 25% chance
        else
        {
          pickableProperties = new ShockProperties(true, false, true);
          type.Value = "ShockAll";
        }
        break;
      case 2:
        pickableProperties = new WackyPickableProperties();
        type.Value = "Wacky";
        break;
      case 3:
        //invert self 25% chance
        if (Random.value > .75f)
        {
          pickableProperties = new InvertInputProperties(true, false, false);
          type.Value = "InvertSelf";
        }
        //invert next player 25% chance
        else if (Random.value > .5f)
        {
          pickableProperties = new InvertInputProperties(false, true, false);
          type.Value = "InvertNext";
        }
        //invert only enemies 25% chance
        else if (Random.value > .25f)
        {
          pickableProperties = new InvertInputProperties(false, false, true);
          type.Value = "InvertEnemies";
        }
        //invert everyone 25% chance
        else
        {
          pickableProperties = new InvertInputProperties(true, false, true);
          type.Value = "InvertAll";
        }
        break;
      case 4:
        if (Random.value > .5f)
        {
          pickableProperties = new MapSizeProperties(1);
          type.Value = "MapSize+";
        }
        else
        {
          pickableProperties = new MapSizeProperties(-1);
          type.Value = "MapSize-";
        }
        break;
      case 5:
        pickableProperties = new InvertGameRuleProperties();
        type.Value = "InvertGameRule";
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

      case "ShockSelf":
        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);
        break;
      case "ShockNext":
        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);
        break;
      case "ShockEnemies":
        GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);
        break;
      case "ShockAll":
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

      case "MapSize+":
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f);
        break;
      case "MapSize-":
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f);
        break;

      case "InvertGameRule":
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
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

    if (destroyed)
      return;

    Debug.Log(pickableProperties);
    Debug.Log(type.Value.ToString());

    //Destroy self
    destroyed = true;
    GetComponent<NetworkObject>().Despawn();

    BallMovement ballMovement = col.GetComponent<BallMovement>();










    if (pickableProperties is MultiplyProperties multiplyProperties)
    {
      for (int i = 0; i < multiplyProperties.amount; i++)
      {
        GameObject ball = Instantiate(ballPrefab, transform);
        ball.GetComponent<BallMovement>().enabled = true;
        ball.GetComponent<NetworkObject>().Spawn();
      }
    }



    if (pickableProperties is InvertGameRuleProperties)
    {
      ballMovement.SetPickable(new InvertGameRule(ballMovement));
    }



    if (pickableProperties is InvertInputProperties invertInputProperties)
    {
      if (invertInputProperties.invertNext)
        ballMovement.SetPickable(new InvertGameRule(ballMovement));


      if (invertInputProperties.invertSelf)
      {
        if (ballMovement.lastPlayerHit != null)
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



    if (pickableProperties is ShockProperties shockProperties)
    {
      if (shockProperties.shockNext)
        ballMovement.SetPickable(new ShockPickable(ballMovement));


      if (shockProperties.shockSelf)
      {
        if (ballMovement.lastPlayerHit != null)
          GameMode.singleton.Shock(ballMovement.lastPlayerHit.GetComponent<PlayerMovement>().position.Value.ToString());
      }


      if (shockProperties.shockEnemies)
      {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
          if (player != ballMovement.lastPlayerHit)
            GameMode.singleton.Shock(player.GetComponent<PlayerMovement>().position.Value.ToString());
        }
      }
    }



    if (pickableProperties is WackyPickableProperties)
    {
      ballMovement.SetPickable(new WackyPickable(ballMovement, 100));
    }



    if (pickableProperties is MapSizeProperties mapSizeProperties)
    {
      GameMode.singleton.ChangeCameraSize(mapSizeProperties.amount);
    }
  }
}










public interface IPickableProperties
{ }





public class MapSizeProperties : IPickableProperties
{
  public int amount; //size to add to camera (can be negative so it subtracts)

  public MapSizeProperties(int amount)
  {
    this.amount = amount;
  }
}



public class MultiplyProperties : IPickableProperties
{
  public int amount; //new balls spawned

  public MultiplyProperties(int amount)
  {
    this.amount = amount;
  }
}



public class WackyPickableProperties : IPickableProperties //-add chance?
{
  //public int wackChance = 100; //Random.Range(1, 101); //1-100 chance the ball stears
}



public class InvertGameRuleProperties : IPickableProperties
{
}



public class InvertInputProperties : IPickableProperties
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



public class ShockProperties : IPickableProperties
{
  public bool shockSelf; //shock player who touch it
  public bool shockNext; //shock next player that touches it
  public bool shockEnemies; //shock enemies

  //si Self y Enemies son ambas verdaderas entonces invierte a todos

  public ShockProperties(bool shockSelf, bool shockNext, bool shockEnemies)
  {
    this.shockSelf = shockSelf;
    this.shockNext = shockNext;
    this.shockEnemies = shockEnemies;
  }
}
