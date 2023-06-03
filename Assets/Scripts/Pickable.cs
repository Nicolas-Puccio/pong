using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class Pickable : MonoBehaviour
{
    //-deberia usar herencia
    [SerializeField]
    public MultiplyProperties multiplyProperties;
    [SerializeField]
    public ChangeBallProperties changeBallProperties;
    [SerializeField]
    public ShockSelfProperties shockSelfProperties;

    static GameObject  ballPrefab;


    void Awake()
    {
        if(!ballPrefab)
            ballPrefab = ballPrefab = Resources.Load<GameObject>("Ball");


        var random = UnityEngine.Random.Range(0, 1);

        switch (random)
        {
            case 0:
                // Handle Multiply powerup
                multiplyProperties = new MultiplyProperties(UnityEngine.Random.Range(1, 5), UnityEngine.Random.Range(15, 30));
                Debug.Log(multiplyProperties.amount);
                Debug.Log(multiplyProperties.spreadDegrees);
                break;
            case 1:
                // Handle ChangeBall powerup
                break;
            default:
                // Handle unknown powerup type
                Debug.Log("Unknown powerup type.");
                break;
        }
    }

    //-for the enable checkbox to appear
    void Start(){}


    void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.gameObject.CompareTag("Ball"))
            return;

        //-disable self to avoid infinite loop do i really need this?
        GetComponent<CircleCollider2D>().enabled = false;


        if(multiplyProperties!=null){

        for (int i = 0; i < multiplyProperties.amount; i++)
        {
    
            Debug.Log("SPAWN BALL");
            /*
            Vector2 direction = (transform.position - col.transform.position).normalized;

            int randomAngle = UnityEngine.Random.Range(-multiplyProperties.spreadDegrees, multiplyProperties.spreadDegrees);
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);
            Vector2 randomDirection = randomRotation * direction;
            */

            GameObject ball = Instantiate(ballPrefab, transform);
            ball.GetComponent<BallMovement>().enabled = true;
            ball.GetComponent<BallMovement>().InitBallVelocity();
            ball.GetComponent<NetworkObject>().Spawn();

        }

            //Destroy self
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}

public class MultiplyProperties {
    //new amount? new velocity? new scale?
    public int amount;
    public int spreadDegrees;
    
    public MultiplyProperties(int amount, int spreadDegrees) {
        this.amount = amount;
        this.spreadDegrees = spreadDegrees;
    }
}

public class ChangeBallProperties {
    //new scale? new mass? new texture?
    //public int amount;
    public ChangeBallProperties(int amount, int spreadDegrees) {
       
    }
}


public class ShockSelfProperties {
    //new scale? new mass? new texture?
    //public int amount;
    public ShockSelfProperties(int amount, int spreadDegrees) {
        
    }
}
