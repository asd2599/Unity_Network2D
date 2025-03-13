using UnityEngine;
using Unity.Netcode;
using System.Collections;
using NUnit.Framework;

public class Cursor : NetworkBehaviour
{   
    static public OnNetworkAtion OnClickItemCallback;

    private bool _isStop = false;
    private ulong _clientId;

    private void Start()
    {
        OnClickItemCallback += OnStopMove;

        _clientId = NetworkManager.Singleton.LocalClientId;
    }

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;
        if (_isStop) return;

        Vector3 screenPos = Input.mousePosition;

        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }
        
    public void OnStopMove(ulong clientId, bool isStop)
    {
        if (clientId == _clientId)
        {
            if (isStop)
                StartCoroutine(StopAction());
            return;
        }
        else
        {
            if(!isStop)
                StartCoroutine(StopAction());
        }
    }

    private IEnumerator StopAction()
    {
        _isStop = true;

        yield return new WaitForSeconds(3.0f);

        _isStop = false;
    }
}
