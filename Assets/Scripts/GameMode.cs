using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameMode :  NetworkBehaviour 
{
    
    #region Singleton

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
    
    #endregion


    string[] positions = new string[] { "top",  "bot","right", "left" };
    int nextPos =  0; //index de la posicion del proximo jugador


    public override void OnNetworkSpawn()
    {
        if (!IsHost) 
        {
            Debug.Log("gamemode destroy self on client");
            Destroy(gameObject)
        }
    }
    

    //used by playermovement    
    public string getPos(){

        //when there are at least 2 players
        if(nextPos == 1)
        {
            CanvasBehaviour.Singleton.EnableButton();
        }

        return positions[nextPos++];
    }


    //called by ui
    public void StartGame(){
        GameObject.FindWithTag("Ball").GetComponent<BallMovement>().enabled = true;
    }
}
