using UnityEngine;
using Unity.Netcode;

public class BallMovement : NetworkBehaviour
{
  IPickable pickable;
  bool destroyed;

  [SerializeField]
  int speed = 10;


  public GameObject lastPlayerHit;

  public Rigidbody2D rb;


  public void SetPickable(IPickable pickable = null)
  {
    this.pickable = pickable;
    //-here call a function that changes ball image?
  }


  public override void OnNetworkSpawn()
  {
    if (!IsHost)
      return;

    rb = GetComponent<Rigidbody2D>();

    Vector2 direction = RotationHelper.DegreesToVector2(Random.Range(0f, 360f));

    // Set the velocity with the constant speed and random direction
    rb.velocity = direction * speed;
  }



  void OnCollisionEnter2D(Collision2D col)
  {
    pickable?.OnCollisionEnter2D(col);

    if (destroyed)
      return;


    if (col.gameObject.CompareTag("Player"))
    {
      lastPlayerHit = col.gameObject;
      lastPlayerHit.GetComponent<PlayerMovement>().TryToTilt(col, transform.position); //let player script handle tilting if weak
    }




    //idk why balls lose velocity when colliding against each other despite both being bouncy, so i set a lower limit to magnitude
    if (rb.velocity.magnitude < speed)
    {
      // If the magnitude is below the minimum, clamp it to the minimum speed
      rb.velocity = rb.velocity.normalized * speed;
    }

    //limits ball speed, usually increases when multiple balls spawn together
    if (rb.velocity.magnitude > speed)
    {
      // If the magnitude is below the minimum, clamp it to the minimum speed
      rb.velocity = rb.velocity.normalized * speed;
    }
  }



  void FixedUpdate()
  {
    pickable?.FixedUpdate();

    if (destroyed)
      return;



    //-coudl check if it was close to hitting another wall and count towards both players
    if (transform.localPosition.x > BackgroundSize.backgroundSize / 2 && rb.velocity.x > 0)
    {
      DestroyAndShock("right");
    }
    else if (transform.localPosition.x < -BackgroundSize.backgroundSize / 2 && rb.velocity.x < 0)
    {
      DestroyAndShock("left");
    }
    else if (transform.localPosition.y > BackgroundSize.backgroundSize / 2 && rb.velocity.y > 0)
    {
      DestroyAndShock("top");
    }
    else if (transform.localPosition.y < -BackgroundSize.backgroundSize / 2 && rb.velocity.y < 0)
    {
      DestroyAndShock("bot");
    }
  }



  public void DestroyAndShock(string pos)
  {
    GameMode.singleton.Shock(pos);

    if (!destroyed)
      GetComponent<NetworkObject>().Despawn();

    destroyed = true;
  }
}



#region IPickable

public interface IPickable
{
  void OnCollisionEnter2D(Collision2D col) { }

  void FixedUpdate() { }
}



public class ShockPickable : IPickable
{
  readonly BallMovement ballMovement;

  public ShockPickable(BallMovement ballMovement)
  {
    this.ballMovement = ballMovement;
  }


  public void OnCollisionEnter2D(Collision2D col)
  {
    if (!col.gameObject.CompareTag("Player"))
      return;

    ballMovement.DestroyAndShock(col.gameObject.GetComponent<PlayerMovement>().position.Value.ToString());
    ballMovement.SetPickable();
  }
}



public class WackyPickable : IPickable
{
  readonly BallMovement ballMovement;


  public int wackChance;
  public bool canWack = true;

  public WackyPickable(BallMovement ballMovement, int wackChance)
  {
    this.ballMovement = ballMovement;
    this.wackChance = wackChance;
  }

  public void FixedUpdate()
  {
    if (!canWack)
      return;


    //si esta cerca de la pared derecha o izquierda
    if ((ballMovement.rb.velocity.x > 0 && ballMovement.transform.localPosition.x > BackgroundSize.backgroundSize / 4 && GameObject.Find("wallright") == null) ||
    (ballMovement.rb.velocity.x < 0 && ballMovement.transform.localPosition.x < -BackgroundSize.backgroundSize / 4 && GameObject.Find("wallleft") == null))
    {
      if (Random.Range(0, 100) <= wackChance)
      {
        ballMovement.rb.velocity = new Vector2(ballMovement.rb.velocity.x, -ballMovement.rb.velocity.y);
        canWack = false;
      }
    }


    //arriba o abajo
    if ((ballMovement.rb.velocity.y > 0 && ballMovement.transform.localPosition.y > BackgroundSize.backgroundSize / 4) ||
    (ballMovement.rb.velocity.y < 0 && ballMovement.transform.localPosition.y < -BackgroundSize.backgroundSize / 4))
    {
      if (Random.Range(0, 100) <= wackChance)
      {
        ballMovement.rb.velocity = new Vector2(-ballMovement.rb.velocity.x, ballMovement.rb.velocity.y);
        canWack = false;
      }
    }
  }

  public void OnCollisionEnter2D(Collision2D col)
  {
    if (!col.gameObject.CompareTag("Player"))
      return;

    if (ballMovement.rb.velocity.x > 0 && ballMovement.transform.localPosition.x < 0 || // esta yendo a la derecha y esta en la izquierda
     ballMovement.rb.velocity.x < 0 && ballMovement.transform.localPosition.x > 0 || // esta yendo izquierda y esta en la derecha
     ballMovement.rb.velocity.y > 0 && ballMovement.transform.localPosition.y < 0 || // esta yendo arriba y esta abajo
     ballMovement.rb.velocity.y < 0 && ballMovement.transform.localPosition.y > 0) // esta yendo abajo y esta arriba
    {
      canWack = true;
    }
  }
}



public class InvertInput : IPickable
{
  readonly BallMovement ballMovement;


  public InvertInput(BallMovement ballMovement)
  {
    this.ballMovement = ballMovement;
  }

  public void OnCollisionEnter2D(Collision2D col)
  {
    if (!col.gameObject.CompareTag("Player"))
      return;

    PlayerMovement playerMovement = col.gameObject.GetComponent<PlayerMovement>();
    playerMovement.inputDirection.Value = -playerMovement.inputDirection.Value;
    ballMovement.SetPickable();
  }
}



public class InvertGameRule : IPickable
{
  readonly BallMovement ballMovement;


  public InvertGameRule(BallMovement ballMovement)
  {
    this.ballMovement = ballMovement;
  }

  public void FixedUpdate()
  {
    if ((ballMovement.transform.localPosition.x > BackgroundSize.backgroundSize / 2 && ballMovement.rb.velocity.x > 0) ||
        (ballMovement.transform.localPosition.x < -BackgroundSize.backgroundSize / 2 && ballMovement.rb.velocity.x < 0) ||
        (ballMovement.transform.localPosition.y > BackgroundSize.backgroundSize / 2 && ballMovement.rb.velocity.y > 0) ||
        (ballMovement.transform.localPosition.y < -BackgroundSize.backgroundSize / 2 && ballMovement.rb.velocity.y < 0))
    {
      ballMovement.rb.velocity = new(-ballMovement.rb.velocity.x, -ballMovement.rb.velocity.y);
    }
  }

  public void OnCollisionEnter2D(Collision2D col)
  {
    if (!col.gameObject.CompareTag("Player"))
      return;

    ballMovement.DestroyAndShock(col.gameObject.GetComponent<PlayerMovement>().position.Value.ToString());
  }
}

#endregion