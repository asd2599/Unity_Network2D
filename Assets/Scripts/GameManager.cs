using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    static public GameManager Instance;

    [SerializeField]
    private GameObject _connectedUI;
    [SerializeField]
    private Button _startBtn;
    [SerializeField]
    private GameObject _errorPanel;

    private bool _isConnectLocked = false;

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
        //NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void OnDisconnected(ulong clientId)
    {
        print("Disconnected");

        if(NetworkManager.Singleton.LocalClientId == clientId)
        {
            _errorPanel.SetActive(true);
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        print("ApprovalCheck");

        if(_isConnectLocked)
        {
            print("새로운 클라이언트 접속 거부!");
            response.Approved = false;
            response.Reason = "Arreay GameStart!";
            _errorPanel.SetActive(true);
        }
        else
        {
            print("새로운 클라이언트 접속 허용!");
            response.Approved = true;
            response.CreatePlayerObject = true;
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
    }



    public void SetHp(int hp)
    {
        //_testText.text = hp.ToString();
    }
}
