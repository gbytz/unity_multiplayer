using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class DisconnectManager : NetworkBehaviour {


    public void QuitGame(CustomNetworkManager manager)
    {
        if (!isServer)
        {
            if (NetworkClient.allClients.Count > 0)
            {
                NetworkClient.allClients[0].Disconnect();
                print("Called Client Disconnect");
            }
            
            print("Ran Client Disconnect Portion of the Function");
        }

        if (isServer)
        {
            Network.Disconnect(0);
            //connectionToServer.Disconnect();
            //manager.StopHost();
            //Network.Disconnect(10);
            //MasterServer.UnregisterHost();
            print("Server Disconnected");
        }

        SceneManager.LoadScene(0);

        print("Restarted");

        return;
         
    }
}
