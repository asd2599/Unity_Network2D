using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Enemy : NetworkBehaviour
{
    [SerializeField]
    Vector3 _hpBarOffset;

    private BoxCollider2D _collider;
    private Rigidbody2D _rigidbody;
    private NetworkObject _networkObject;

    private Image _hpBarImage;
    private GameObject _hpBar;
    private Transform _hpBarPanel;    

    private float _moveTime = 0.0f;
    private int _curHp = 5;
    private int _maxHp = 5;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _networkObject = GetComponent<NetworkObject>();
    }

    private void Start()
    {
        _hpBarPanel = GameObject.Find("HpBarPanel").transform;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/HpBar");
        _hpBar = Instantiate(prefab, _hpBarPanel);
        _hpBarImage = _hpBar.transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (_collider.OverlapPoint(worldMousePos))
            {
                //_networkObject.Despawn();
                //gameObject.SetActive(false);
                PushServerRpc(_networkObject.OwnerClientId);                
            }
        }

        CheckMoveTime();
        _hpBar.transform.localPosition = 
            Camera.main.WorldToScreenPoint(transform.position + _hpBarOffset);
    }

    private void CheckMoveTime()
    {
        if (!IsServer) return;

        if (_moveTime > 0.0f)
        {
            _moveTime -= Time.deltaTime;

            if (_moveTime < 0.0f)
            {
                _rigidbody.linearVelocity = Vector2.zero;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PushServerRpc(ulong clientId)
    {
        //_networkObject.Despawn();

        Vector2 dir;
        dir.x = Random.Range(-1.0f, 1.0f);
        dir.y = Random.Range(-1.0f, 1.0f);
        
        float power = Random.Range(10.0f, 100.0f);
        
        _rigidbody.AddForce(dir.normalized * power);
        _moveTime = Random.Range(0.5f, 1.0f);        
        
        OnDamageClientRpc(clientId);
    }

    [ClientRpc]
    private void OnDamageClientRpc(ulong clientId)
    {
        _curHp--;        

        _hpBarImage.fillAmount = (float)_curHp / _maxHp;

        if(_curHp <= 0)
        {
            Destroy(_hpBar);
            GameManager.Instance.IncreaseScore(clientId);

            if (IsServer)
            {
                _networkObject.Despawn();                
            }            
        }
    }
}
