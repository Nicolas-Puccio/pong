using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BallMovement : NetworkBehaviour
{
  [SerializeField]
  int speed = 10;


  GameObject lastPlayerHit;

  Rigidbody2D rb;


  #region powerups data

  public int wackyChance = 0;
  public bool canWack = true;//set to false when it does, set to true oncollisionenter

  #endregion



  public void Start()
  {
    rb = GetComponent<Rigidbody2D>();

    Vector2 direction = RotationHelper.DegreesToVector2(Random.Range(0f, 360f));

    // Set the velocity with the constant speed and random direction
    rb.velocity = direction * speed;
  }



  void OnCollisionEnter2D(Collision2D col)
  {
    if (col.gameObject.CompareTag("Player"))
    {
      lastPlayerHit = col.gameObject;
      canWack = true;
    }


    //idk why balls lose velocity when colliding against each other despite both being bouncy, so i set a lower limit to magnitude
    if (rb.velocity.magnitude < speed)
    {
      // If the magnitude is below the minimum, clamp it to the minimum speed
      rb.velocity = rb.velocity.normalized * speed;
    }
  }



  void FixedUpdate()
  {
    if (wackyChance != 0 && canWack)
    {
      //si esta cerca de la pared derecha o izquierda
      if ((rb.velocity.x > 0 && transform.localPosition.x > BackgroundSize.backgroundSize / 3 && GameObject.Find("wallright") == null) ||
      (rb.velocity.x < 0 && transform.localPosition.x < -BackgroundSize.backgroundSize / 3 && GameObject.Find("wallleft") == null))
      {
        if (Random.Range(0, 100) <= wackyChance)
        {
          rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
          canWack = false;
        }
      }


      //arriba o abajo
      if ((rb.velocity.y > 0 && transform.localPosition.y > BackgroundSize.backgroundSize / 3) ||
      (rb.velocity.y < 0 && transform.localPosition.y < -BackgroundSize.backgroundSize / 3))
      {
        if (Random.Range(0, 100) <= wackyChance)
        {
          rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
          canWack = false;
        }
      }
    }



    //-coudl check if it was close to hitting another wall and count towards both players
    if (transform.localPosition.x > BackgroundSize.backgroundSize / 2)
    {
      OffLimit("right");
    }
    else if (transform.localPosition.x < -BackgroundSize.backgroundSize / 2)
    {
      OffLimit("left");
    }
    else if (transform.localPosition.y > BackgroundSize.backgroundSize / 2)
    {
      OffLimit("top");
    }
    else if (transform.localPosition.y < -BackgroundSize.backgroundSize / 2)
    {
      OffLimit("bot");
    }
  }



  void OffLimit(string pos)
  {
    GameMode.Singleton.Shock(pos);

    GetComponent<NetworkObject>().Despawn();
    DestroyImmediate(gameObject);
  }
}
