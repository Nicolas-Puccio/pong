using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameMode :  NetworkBehaviour 
{
    public int clientsRequired;

    string[] positions = new string[] { "top", "right", "bot", "left" };
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
    
    void Start()
    {/*
        float cameraSize = Camera.main.orthographicSize;
        float backgroundSize = cameraSize*2 * mapRatio / 100;
        GameObject.Find("Background").transform.localScale = new Vector2(backgroundSize,backgroundSize);
    
        //distance from center
        float playerSpawnDistance = backgroundSize/2-playerOffsetFromEdge;
        
        // Load the prefab from the "Resources" folder
        
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
         if (playerPrefab == null)
        {
            Debug.LogError("Prefabs/Player prefab missing");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            string stringPosition = i == 0 ? "top" : i == 1 ? "right" : i == 2 ? "bot" : "left";
            Vector2 transformPosition = i == 0 ? new Vector2(0,playerSpawnDistance) : i == 1 ? new Vector2(playerSpawnDistance,0) : i == 2 ? new Vector2(0,-playerSpawnDistance) : new Vector2(-playerSpawnDistance,0);
            Quaternion transformRotation = i==0||i==2?Quaternion.Euler(0f, 0f, 90f):Quaternion.identity;
            
            GameObject player = Instantiate(playerPrefab, transformPosition,transformRotation );
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("YourScriptOnInstantiatedObject component not found on the instantiated object.");
                return;
            }
            
                playerMovement.position = stringPosition;
            
            
        }*/




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
