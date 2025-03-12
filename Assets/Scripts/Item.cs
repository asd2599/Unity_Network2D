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
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (_collider.OverlapPoint(worldMousePos))
            {
                OnClickItemClientRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

<<<<<<< HEAD
    [ServerRpc(RequireOwnership = false)]
    private void OnClickItemServerRpc(ulong clientId)
    {
        OnClickItemClientRpc(clientId);

        _networkObject.Despawn();        
    }


    [ClientRpc(RequireOwnership = false)]
=======
    [ClientRpc]
>>>>>>> 6e64aa225dc0eb74d25c6268111ce3d6cadec7dd
    private void OnClickItemClientRpc(ulong clientId)
    {
        if(IsServer)
        {
            _networkObject.Despawn();
        }

        Cursor.OnClickItemCallback(clientId);
    }
}
