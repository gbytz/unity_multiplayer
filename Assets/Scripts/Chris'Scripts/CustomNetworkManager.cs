using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager {

    public NetworkHUD _networkHUD;
    public GameManager _gameManager;

    private void Update()
    {
        if (!_networkHUD)
        {
            _networkHUD = FindObjectOfType<NetworkHUD>();
        }
        if (!_gameManager)
        {
            _gameManager = FindObjectOfType<GameManager>();
            _gameManager.QuitGameBTN.onClick.AddListener(RestartGame);
        }
    }

    public void RestartGame (){

        if (Network.isClient)
            client.Disconnect();

        if(Network.isServer)
            StopHost();

        SceneManager.LoadScene(0);

        return;

        /*Not going to use this manual route. Requires manual management of GameManager's "IsStarted" and Player Objects....reloading the scene seem to be enough and work properly
        _gameManager.InGameUI.SetActive(false);
        _networkHUD.Container.SetActive(true);
        */
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        RestartGame();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        //LostConnection(); Currently even if someone else disconnects this will trip so I am leaving it out for now
    }

    public void LostConnection()
    {
        if (Network.isClient)
            client.Disconnect();

        if (Network.isServer)
            StopHost();

        SceneManager.LoadScene(0);
    }
}
