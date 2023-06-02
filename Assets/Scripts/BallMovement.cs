using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class BallMovement : NetworkBehaviour
{
    public float speed;
    
    private static GameObject ballPrefab;

    void Awake(){
        if(!ballPrefab)
        ballPrefab = Resources.Load<GameObject>("Ball");
    }

    void Start()
    {
        float randomAngle = Random.Range(0f, 360f);

        // Convert the angle to a direction vector
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));

        // Set the velocity with the constant speed and random direction
        GetComponent<Rigidbody2D>().velocity = direction * speed;
        
    }

    void FixedUpdate()
    {
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
        Debug.Log(pos);

        Text scoreText = GameObject.Find($"score{pos}").GetComponent<Text>();

        scoreText.text = (int.Parse(scoreText.text)+1).ToString();

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if(balls.Length == 1)
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.GetComponent<BallMovement>().enabled = true;
            ball.GetComponent<NetworkObject>().Spawn();
        }

GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
