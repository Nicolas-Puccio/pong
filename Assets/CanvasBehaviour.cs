using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBehaviour : MonoBehaviour
{

    #region Singleton

    private static CanvasBehaviour singleton;

        // Public property to access the instance
        public static CanvasBehaviour Singleton
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

    //only on server
    public GameObject startButton;

    void Start()
    {
        //sets position of score texts in client and server
        int offset = Screen.height/2-25;
        foreach (RectTransform son in transform.GetComponentsInChildren<Transform>())
        {
            string pos = son.name.Substring(5);
            son.anchoredPosition = pos == "top" ? new Vector2(0,offset) : pos == "right" ? new Vector2(offset,0) : pos == "bot" ? new Vector2(0,-offset) : new Vector2(-offset,0);
        }
    }


    //only on server
    public void StartButton(){
        GameMode.Singleton.StartGame();
    }

    //only on server
    public void EnableButton(){
        startButton.SetActive(true);    
    }
}
