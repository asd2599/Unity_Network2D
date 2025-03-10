using UnityEngine;
using Unity.Netcode;

public class Cursor : NetworkBehaviour
{
    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        Vector3 screenPos = Input.mousePosition;

        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }
}
