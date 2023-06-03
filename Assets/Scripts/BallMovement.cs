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

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitBallVelocity()
    {
            float randomAngle = Random.Range(0f, 360f);

            // Convert the angle to a direction vector
            Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));

            // Set the velocity with the constant speed and random direction
            rb.velocity = direction * speed;
        
    }

    public void InitBallVelocity(Vector2 direction)
    {
        rb.velocity = direction * speed;
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            lastPlayerHit = col.gameObject;

        //idk why balls lose velocity when colliding against each other despite both being bouncy, so i set a lower limit to magnitude
        if (rb.velocity.magnitude < speed )
        {
            // If the magnitude is below the minimum, clamp it to the minimum speed
            rb.velocity = rb.velocity.normalized * speed;
        }
    }



    void FixedUpdate()
    {
        //-coudl check if it was close to hitting another wall and count towards both players
        if(transform.localPosition.x > BackgroundSize.backgroundSize / 2)
        {
            OffLimit("right");
        }
        else if(transform.localPosition.x < -BackgroundSize.backgroundSize / 2)
        {
            OffLimit("left");
        }
        else if(transform.localPosition.y > BackgroundSize.backgroundSize / 2)
        {
            OffLimit("top");
        }
        else if(transform.localPosition.y < -BackgroundSize.backgroundSize / 2)
        {
            OffLimit("bot");
        }
    }
    


    void OffLimit(string pos){
        GameMode.Singleton.Shock(pos);

        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
