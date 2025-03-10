using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemyManager : NetworkBehaviour
{
    static public EnemyManager Instance;

    private GameObject _enemyPrefab;

    private void Awake()
    {
        Instance = this;

        _enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
    }

    public void StartGame()
    {
        if(IsServer)
        {
            StartCoroutine(SpawnLoop());
        }        
    }
    
    private IEnumerator SpawnLoop()
    {
        while(true)
        {
            Vector3 pos = GetRandomPositionInView();

            GameObject obj = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            obj.GetComponent<NetworkObject>().Spawn();

            yield return new WaitForSeconds(1.0f);
        }
    }

    public static Vector3 GetRandomPositionInView()
    {
        Camera cam = Camera.main;

        // 화면의 왼쪽 아래(0,0) ~ 오른쪽 위(1,1) 사이의 랜덤한 뷰포트 좌표 생성
        Vector3 randomViewportPos = new Vector3(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), cam.nearClipPlane);

        // 뷰포트 좌표를 월드 좌표로 변환
        Vector3 worldPos = cam.ViewportToWorldPoint(randomViewportPos);
        worldPos.z = 0; // 2D 게임이면 Z 값은 0으로 설정

        return worldPos;
    }
}
