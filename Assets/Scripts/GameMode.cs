using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameMode :  NetworkBehaviour 
{
    
    #region Singleton and Resources.Load

    private static GameMode singleton;

    // Public property to access the instance
    public static GameMode Singleton
    {
        get { return singleton; }
    }

    private void Awake()
        {
            //load Wall Resource
            if(!wallPrefab)
                wallPrefab = Resources.Load<GameObject>("Wall");

            //load Ball Resource
            if(!ballPrefab)
                ballPrefab = Resources.Load<GameObject>("Ball");


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


    static GameObject wallPrefab;
    static GameObject ballPrefab;



    public override void OnNetworkSpawn()
    {
        if (!IsHost) 
        {
            Debug.Log("gamemode disable self on client");
            enabled = false;
        }
    }
    

    //used by playermovement    
    public string getPos(){

        //when there are at least 2 players
        if(nextPos == 0)//-change to 1
        {
            CanvasBehaviour.Singleton.EnableButton();
        }

        return positions[nextPos++];
    }


    //called by ui
    public void StartGame(){
 
        GameObject ball = Instantiate(Resources.Load<GameObject>("Ball"));
        ball.GetComponent<BallMovement>().enabled = true;
        ball.GetComponent<BallMovement>().InitBallVelocity();
        ball.GetComponent<NetworkObject>().Spawn();

        SpawnWalls();
        GameState.Singleton.GameStartClientRpc();
    }



    void SpawnWalls(){
        if(nextPos <4)
        {
            GameObject wall = Instantiate(wallPrefab,new Vector2(-BackgroundSize.backgroundSize/2,0),Quaternion.identity);
            wall.transform.localScale = new Vector2(1,BackgroundSize.backgroundSize);
            wall.GetComponent<NetworkObject>().Spawn();
        }
        if(nextPos <3)
        {
            GameObject wall = Instantiate(wallPrefab,new Vector2(BackgroundSize.backgroundSize/2,0),Quaternion.identity);
            wall.transform.localScale = new Vector2(1,BackgroundSize.backgroundSize);
            wall.GetComponent<NetworkObject>().Spawn();
        }
    }

    public void Shock(string pos){
        
        Text scoreText = GameObject.Find($"score{pos}").GetComponent<Text>();        
        scoreText.text = (int.Parse(scoreText.text)+1).ToString();

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if(balls.Length == 1)
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.GetComponent<BallMovement>().enabled = true;
            ball.GetComponent<BallMovement>().InitBallVelocity();
            ball.GetComponent<NetworkObject>().Spawn();
        }
    }
}
