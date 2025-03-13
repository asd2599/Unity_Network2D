using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public delegate void OnNetworkAtion(ulong clientId);

public class GameManager : NetworkBehaviour
{ 
    static public GameManager Instance;

    [SerializeField]
    private GameObject _connectedUI;
    [SerializeField]
    private Button _startBtn;
    [SerializeField]
    private GameObject _errorPanel;
    [SerializeField]
    private Transform _scorePanel;
    [SerializeField]
    private GameObject _endPanel;

    private bool _isConnectLocked = false;
    private int _playerNum = 0;
    private bool _isGameOver = false;
    public bool IsGameOver
    {
        get { return _isGameOver; }
    }

    private Dictionary<ulong, int> _playerScores = new Dictionary<ulong, int>();
    private List<TextMeshProUGUI> _playerScoreTexts = new List<TextMeshProUGUI>();

    private void Awake()
    {
        Instance = this;
        _startBtn.onClick.AddListener(OnStartGame);
        _startBtn.gameObject.SetActive(false);
        _errorPanel.SetActive(false);        
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
    }

    private void OnDisconnected(ulong clientId)
    {
        print("Disconnected");

        if(NetworkManager.Singleton.LocalClientId == clientId)
        {
            _errorPanel.SetActive(true);
        }
    }

    private void OnConnected(ulong clientId)
    {
        if (_isConnectLocked)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }

        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        _connectedUI.SetActive(false);
        _startBtn.gameObject.SetActive(true);

        if (IsServer)
            _startBtn.interactable = true;
        else
            _startBtn.interactable = false;
    }

    private void OnStartGame()
    {
        _isConnectLocked = true;

        OnStartClientRpc();        
    }

    [ClientRpc]
    private void OnStartClientRpc()
    {
        _startBtn.gameObject.SetActive(false);

        EnemyManager.Instance.StartGame();

        CreateScore();
    }

    private void CreateScore()
    {
        _playerNum = NetworkManager.Singleton.ConnectedClientsList.Count;        

        GameObject prefab = Resources.Load<GameObject>("Prefabs/Score");

        for(int i = 0; i < _playerNum; i++)
        {
            GameObject score = Instantiate(prefab, _scorePanel);
            TextMeshProUGUI text = score.GetComponent<TextMeshProUGUI>();
            _playerScoreTexts.Add(text);

            NetworkClient client = NetworkManager.Singleton.ConnectedClientsList[i];
            _playerScores.Add(client.ClientId, 0);
            //text.text = "Player" + i + " : " + _playerScores[i];
        }

        UpdateScore();
    }

    public void UpdateScore()
    {
        int count = 0;
        foreach(KeyValuePair<ulong, int> playerScore in _playerScores)
        {
            TextMeshProUGUI text = _playerScoreTexts[count];
            text.text = "Player" + count + " : " + playerScore.Value;
            count++;
        }
    }

    public void IncreaseScore(ulong clientId)
    {
        _playerScores[clientId]++;

        UpdateScore();

        if (IsServer && _playerScores[clientId] == 5)
        {
            OnEndClientRpc(clientId);
        }
    }

    [ClientRpc]
    private void OnEndClientRpc(ulong clientId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            player.SetActive(false);
        }

        _endPanel.SetActive(true);

        TextMeshProUGUI text = _endPanel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Player" + clientId + " Win!";

        _isGameOver = true;
    }
}
