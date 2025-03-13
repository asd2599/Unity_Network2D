using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class Item : NetworkBehaviour
{
    [SerializeField]
    private List<Sprite> _itemSprites;

    private BoxCollider2D _collider;
    private NetworkObject _networkObject;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _networkObject = GetComponent<NetworkObject>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (_collider.OverlapPoint(worldMousePos))
            {
                OnClickItemServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnClickItemServerRpc(ulong clientId)
    {
        OnClickItemClientRpc(clientId, Random.Range(0, 2) == 0);

        _networkObject.Despawn();        
    }


    [ClientRpc]
    private void OnClickItemClientRpc(ulong clientId, bool isStop)
    {
        Cursor.OnClickItemCallback(clientId, isStop);
    }
}
