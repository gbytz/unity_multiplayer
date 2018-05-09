using System;
using System.ComponentModel;

#if ENABLE_UNET

namespace UnityEngine.Networking
{
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class NetworkManagerHUD : MonoBehaviour
    {
        public NetworkManager manager;
		public float scale;
        [SerializeField] public bool showGUI = true;
        [SerializeField] public int offsetX;
        [SerializeField] public int offsetY;

        // Runtime variable
        bool m_ShowServer;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void Update()
        {
            if (!showGUI)
                return;

            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
            {
                if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        manager.StartServer();
                    }
                    if (Input.GetKeyDown(KeyCode.H))
                    {
                        manager.StartHost();
                    }
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    manager.StartClient();
                }
            }
            if (NetworkServer.active && manager.IsClientConnected())
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    manager.StopHost();
                }
            }
        }

        void OnGUI()
        {
            if (!showGUI)
                return;

            int xpos = 10 + offsetX;
            int ypos = 40 + offsetY;
			const int spacing = 24;

			GUI.skin.label.fontSize = 12 * (int)scale;
			GUI.skin.button.fontSize = 12 * (int)scale;
			GUI.skin.textField.fontSize = 12 * (int)scale;

            bool noConnection = (manager.client == null || manager.client.connection == null ||
                                 manager.client.connection.connectionId == -1);

            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
            {
                if (noConnection)
                {
                    if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                    {
                        if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "LAN Host(H)"))
                        {
                            manager.StartHost();
                        }
                        ypos += spacing;
                    }

                    if (GUI.Button(ScaleRect(xpos, ypos, 105, 20), "LAN Client(C)"))
                    {
                        manager.StartClient();
                    }

                    manager.networkAddress = GUI.TextField(ScaleRect(xpos + 100, ypos, 95, 20), manager.networkAddress);
                    ypos += spacing;

                    if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // cant be a server in webgl build
                        GUI.Box(ScaleRect(xpos, ypos, 200, 25), "(  WebGL cannot be server  )");
                        ypos += spacing;
                    }
                    else
                    {
                        if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "LAN Server Only(S)"))
                        {
                            manager.StartServer();
                        }
                        ypos += spacing;
                    }
                }
                else
                {
                    GUI.Label(ScaleRect(xpos, ypos, 200, 20), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..");
                    ypos += spacing;


                    if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Cancel Connection Attempt"))
                    {
                        manager.StopClient();
                    }
                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    string serverMsg = "Server: port=" + manager.networkPort;
                    if (manager.useWebSockets)
                    {
                        serverMsg += " (Using WebSockets)";
                    }
                    GUI.Label(ScaleRect(xpos, ypos, 300, 20), serverMsg);
                    ypos += spacing;
                }
                if (manager.IsClientConnected())
                {
                    GUI.Label(ScaleRect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                    ypos += spacing;
                }
            }

            if (manager.IsClientConnected() && !ClientScene.ready)
            {
                if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Client Ready"))
                {
                    ClientScene.Ready(manager.client.connection);

                    if (ClientScene.localPlayers.Count == 0)
                    {
                        ClientScene.AddPlayer(0);
                    }
                }
                ypos += spacing;
            }

            if (NetworkServer.active || manager.IsClientConnected())
            {
                if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Stop (X)"))
                {
                    manager.StopHost();
                }
                ypos += spacing;
            }

            if (!NetworkServer.active && !manager.IsClientConnected() && noConnection)
            {
                ypos += 10;

                if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    GUI.Box(ScaleRect(xpos - 5, ypos, 220, 25), "(WebGL cannot use Match Maker)");
                    return;
                }

                if (manager.matchMaker == null)
                {
                    if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Enable Match Maker (M)"))
                    {
                        manager.StartMatchMaker();
                    }
                    ypos += spacing;
                }
                else
                {
                    if (manager.matchInfo == null)
                    {
                        if (manager.matches == null)
                        {
                            if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Create Internet Match"))
                            {
                                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
                            }
                            ypos += spacing;

                            GUI.Label(ScaleRect(xpos, ypos, 100, 20), "Room Name:");
                            manager.matchName = GUI.TextField(ScaleRect(xpos + 100, ypos, 100, 20), manager.matchName);
                            ypos += spacing;

                            ypos += 10;

                            if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Find Internet Match"))
                            {
                                manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
                            }
                            ypos += spacing;
                        }
                        else
                        {
                            for (int i = 0; i < manager.matches.Count; i++)
                            {
                                var match = manager.matches[i];
                                if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Join Match:" + match.name))
                                {
                                    manager.matchName = match.name;
                                    manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
                                }
                                ypos += spacing;
                            }

                            if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Back to Match Menu"))
                            {
                                manager.matches = null;
                            }
                            ypos += spacing;
                        }
                    }

                    if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Change MM server"))
                    {
                        m_ShowServer = !m_ShowServer;
                    }
                    if (m_ShowServer)
                    {
                        ypos += spacing;
                        if (GUI.Button(ScaleRect(xpos, ypos, 100, 20), "Local"))
                        {
                            manager.SetMatchHost("localhost", 1337, false);
                            m_ShowServer = false;
                        }
                        ypos += spacing;
                        if (GUI.Button(ScaleRect(xpos, ypos, 100, 20), "Internet"))
                        {
                            manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
                            m_ShowServer = false;
                        }
                        ypos += spacing;
                        if (GUI.Button(ScaleRect(xpos, ypos, 100, 20), "Staging"))
                        {
                            manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
                            m_ShowServer = false;
                        }
                    }

                    ypos += spacing;

                    GUI.Label(ScaleRect(xpos, ypos, 300, 20), "MM Uri: " + manager.matchMaker.baseUri);
                    ypos += spacing;

                    if (GUI.Button(ScaleRect(xpos, ypos, 200, 20), "Disable Match Maker"))
                    {
                        manager.StopMatchMaker();
                    }
                    ypos += spacing;
                }
            }
        }

		private Rect ScaleRect(float x, float y, float width, float height){
			return new Rect (x * scale, y * scale, width * scale, height * scale);
		}

		public void ToggleGUI(){
			showGUI = !showGUI;
		}

		public void ScaleUp(){
			scale = scale + 0.2f;
		}

		public void ScaleDown(){
			scale = scale - 0.2f;
		}
    }
}
#endif //ENABLE_UNET
