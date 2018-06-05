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
    private List<MatchInfoSnapshot> matchesList = new List<MatchInfoSnapshot>(); 

	// Use this for initialization
	void Start () {

        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkManager.matchMaker == null)
            _networkManager.StartMatchMaker();

        //Assign Network Commands Here
        JoinGameBTN.onClick.AddListener(FindServerHook);
        HostGameBTN.onClick.AddListener(HostGame);
        CancelJoinBTN.onClick.AddListener(CancelJoin);
        ConfirmJoinBTN.onClick.AddListener(JoinGame);
	}

    void FindServerHook (){

        StartCoroutine(FindServers());

    }

    IEnumerator FindServers ()
    {
        if (_networkManager.matchMaker == null)
        {
            _networkManager.StartMatchMaker();
        }
        
        _networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, ListMatchesCallback);

        JoiningText.text = "Searching...";

        JoinGameBTN.gameObject.SetActive(false);
        HostGameBTN.gameObject.SetActive(false);
        CancelJoinBTN.gameObject.SetActive(true);
        JoiningText.gameObject.SetActive(true);
        ServerSelectDropdown.gameObject.SetActive(true);
        ConfirmJoinBTN.gameObject.SetActive(true);

        yield return null;

    }

    void JoinGame()
    {
        if(ServerSelectDropdown.options.Count <= 0){
            JoiningText.text = "No Games Available";
            return;
        }

        string _targetServerName = ServerSelectDropdown.options[ServerSelectDropdown.value].text;
        print("Server name = " + _targetServerName);

        if (matchesList.Count > 0)
        {
            for (int i = 0; i < matchesList.Count; i++)
            {
                var match = matchesList[i];

                if (match.name == _targetServerName)
                    _networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, _networkManager.OnMatchJoined);

                StartCoroutine(StartGameRoutine());
            }
        }else{

            JoiningText.text = "Failed To Join Game...Select A Different Game or Try Again";
            return;
        }
    }

    private void ListMatchesCallback(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if(success && matches.Count > 0)
        {
            ServerSelectDropdown.options.Clear();

            List<Dropdown.OptionData> _newMatches = new List<Dropdown.OptionData>();

            for (int i = 0; i < matches.Count; i++)
            {
                _newMatches.Add(new Dropdown.OptionData(matches[i].name, null));
                matchesList.Add(matches[i]);
            }

            ServerSelectDropdown.AddOptions(_newMatches);

            JoiningText.text = "Found " + matches.Count + "Game(s)";

        }else{
            JoiningText.text = "No Games Found...Try Again";
        }

        //StartCoroutine(FindServers());
    }

    void HostGame ()
    {
        if (_networkManager.matchMaker == null)
            _networkManager.StartMatchMaker();
        
        _networkManager.matchMaker.CreateMatch(SystemInfo.deviceName, _networkManager.matchSize, true, "", "", "", 0, 0, OnMatchCreateCallBack);

    }

    void OnMatchCreateCallBack (bool success, string extendedInfo, MatchInfo matchInfo){

        _networkManager.OnMatchCreate(success, extendedInfo, matchInfo);

        if(success){
            JoiningText.text = "Starting Game... \n" + "Server Name - " + SystemInfo.deviceName;

            CancelJoinBTN.gameObject.SetActive(false);
            JoinGameBTN.gameObject.SetActive(false);
            JoiningText.gameObject.SetActive(true);
            HostGameBTN.gameObject.SetActive(false);
            ServerSelectDropdown.gameObject.SetActive(false);
            ConfirmJoinBTN.gameObject.SetActive(false);

            //StopCoroutine(FindServers());
            StartCoroutine(StartGameRoutine());
        }else{

            JoiningText.text = "Failed To Start Game...Try Hosting Again";
        }
    }

    IEnumerator StartGameRoutine()
    {
        //_networkManager.StopMatchMaker();

        yield return new WaitForSeconds(0.0f);

        FindObjectOfType<GameManager>().StartGame();
        gameObject.SetActive(false);
    }

    void CancelJoin ()
    {
        _networkManager.StopMatchMaker();

        CancelJoinBTN.gameObject.SetActive(false);
        JoinGameBTN.gameObject.SetActive(true);
        JoinGameBTN.interactable = true;
        HostGameBTN.gameObject.SetActive(true);

        JoiningText.text = "Select Join or Host To Begin";
        //JoiningText.gameObject.SetActive(false);
        //ServerSelectDropdown.gameObject.SetActive(false);
        ConfirmJoinBTN.gameObject.SetActive(false);

        StopCoroutine(FindServers());
    }
}
