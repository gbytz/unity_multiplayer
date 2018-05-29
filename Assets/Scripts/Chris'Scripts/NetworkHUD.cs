using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class NetworkHUD : MonoBehaviour {

    private NetworkManager _networkManager;

    public Button JoinGameBTN;
    public Button HostGameBTN;

    public Button CancelJoinBTN;
    public Button ConfirmJoinBTN;

    public Text JoiningText;

    public Dropdown ServerSelectDropdown;

	// Use this for initialization
	void Start () {

        _networkManager = FindObjectOfType<NetworkManager>();

        //Assign Network Commands Here
        JoinGameBTN.onClick.AddListener(FindServers);
        HostGameBTN.onClick.AddListener(HostGame);
        CancelJoinBTN.onClick.AddListener(CancelJoin);
        ConfirmJoinBTN.onClick.AddListener(JoinGame);
	}
	
	// Update is called once per frame
	void Update () {


	}

    void FindServers ()
    {
        if (_networkManager.matchMaker == null)
            _networkManager.StartMatchMaker();
        
        _networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, ListMatchesCallback);
        JoiningText.text = "Searching...";

        JoinGameBTN.gameObject.SetActive(false);
        HostGameBTN.gameObject.SetActive(false);
        CancelJoinBTN.gameObject.SetActive(true);
        JoiningText.gameObject.SetActive(true);
        ServerSelectDropdown.gameObject.SetActive(true);
        ConfirmJoinBTN.gameObject.SetActive(true);
    }

    void JoinGame()
    {

        //if (!_networkManager.IsClientConnected() && !NetworkServer.active && _networkManager.matchMaker == null)
        //    _networkManager.StartClient();



        /*for (int i = 0; i < _networkManager.matches.Count; i++)
        {
            var match = _networkManager.matches[i];
            _networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, _networkManager.OnMatchJoined);
        }*/

        StartCoroutine(StartGameRoutine());
    }

    private void ListMatchesCallback(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if(success && matches.Count > 0){

            ServerSelectDropdown.options.Clear();

            List<Dropdown.OptionData> _newMatches = new List<Dropdown.OptionData>();

            for (int i = 0; i < matches.Count; i++)
            {
                _newMatches.Add(new Dropdown.OptionData(matches[i].name, null));
            }

            ServerSelectDropdown.AddOptions(_newMatches);
        }
    }

    void HostGame ()
    {
        if (_networkManager.matchMaker == null)
            _networkManager.StartMatchMaker();

        _networkManager.matchMaker.CreateMatch(SystemInfo.deviceName, _networkManager.matchSize, true, "", "", "", 0, 0, _networkManager.OnMatchCreate);

        JoiningText.text = "Starting Game... \n" + "Server Name - " + SystemInfo.deviceName;

        CancelJoinBTN.gameObject.SetActive(false);
        JoinGameBTN.gameObject.SetActive(false);
        JoiningText.gameObject.SetActive(true);
        HostGameBTN.gameObject.SetActive(false);
        ServerSelectDropdown.gameObject.SetActive(false);
        ConfirmJoinBTN.gameObject.SetActive(false);

        StartCoroutine(StartGameRoutine());
    }

    void CancelJoin ()
    {
        _networkManager.StopMatchMaker();

        CancelJoinBTN.gameObject.SetActive(false);
        JoinGameBTN.gameObject.SetActive(true);
        JoinGameBTN.interactable = true;
        HostGameBTN.gameObject.SetActive(true);
        JoiningText.gameObject.SetActive(false);
        ServerSelectDropdown.gameObject.SetActive(false);
        ConfirmJoinBTN.gameObject.SetActive(false);
    }

    IEnumerator StartGameRoutine()
    {
        _networkManager.StopMatchMaker();

        yield return new WaitForSeconds(2.0f);

        FindObjectOfType<GameManager>().StartGame();
        gameObject.SetActive(false);
    }
}
