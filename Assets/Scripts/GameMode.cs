using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameMode :  NetworkBehaviour 
{
    public int clientsRequired;

    string[] positions = new string[] { "top",  "bot","right", "left" };
    int nextPos =  0; //index de la posicion del proximo jugador

    public override void OnNetworkSpawn()
    {
        if (!IsHost) 
        {
            enabled = false;
            Debug.Log("gamemode disabled self");
        }
    }

    public string getPos(){
        if(nextPos == clientsRequired-1)GameObject.FindWithTag("Ball").GetComponent<BallMovement>().enabled = true;

        return positions[nextPos++];
    }
    
    
    private static GameMode singleton;

    // Public property to access the instance
    public static GameMode Singleton
    {
        get { return singleton; }
    }

    private void Awake()
    {
        // Ensure only one instance of the Singleton exists
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;
    }
}
