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
            //_gameManager.QuitGameBTN.onClick.AddListener(RestartGame);
        }
    }

    public void QuitGame (){

        StopHost();
        Network.Disconnect(2);
        MasterServer.UnregisterHost();
        SceneManager.LoadScene(0);

        return;
        if (!Network.isServer)
        {
            if (NetworkClient.allClients.Count > 0)
            {
                NetworkClient.allClients[0].Disconnect();
                print("Called Client Disconnect");
            }

            print("Ran Client Disconnect Portion of the Function");
        }

        if (Network.isServer)
        {
            //connectionToServer.Disconnect();
            //StopHost();
            //Network.Disconnect(10);
            //MasterServer.UnregisterHost();
            print("Server Disconnected");
        }

        SceneManager.LoadScene(0);

        print("Restarted");

        return;
    }

    public void RestartGame (){

        if (Network.isClient)
        {
            client.Disconnect();
        }

        if (Network.isServer)
        {
            StopHost();
            Network.Disconnect(0);
            MasterServer.UnregisterHost();
        }
        
        SceneManager.LoadScene(0);

        print("Restarted");

        return;

        /*Not going to use this manual route. Requires manual management of GameManager's "IsStarted" and Player Objects....reloading the scene seem to be enough and work properly
        _gameManager.InGameUI.SetActive(false);
        _networkHUD.Container.SetActive(true);
        */
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        print("Lobby Server Disconnected");
        QuitGame();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        print("Lobby Client Disconnected");
        //LostConnection(); Currently even if someone else disconnects this will trip so I am leaving it out for now
    }

    public void LostConnection()
    {
        if (Network.isClient)
            client.Disconnect();

        if (Network.isServer)
            StopHost();

        Network.Disconnect(1);

        SceneManager.LoadScene(0);
    }
}
