using UnityEngine;
using System.Collections;

public class HomeBugSpawner : MonoBehaviour
{
    public GameObject bugPrefab;      // 버그 프리팹
    public Transform spawnArea;       // 버그가 돌아다닐 영역 (Home_Screen)
    public int maxBugs = 2;           // 최대 마릿수
    public float respawnDelay = 1.5f; // 리스폰 대기 시간

    private int currentBugCount = 0;

    void Start()
    {
        // 시작할 때 2마리 생성
        SpawnBug();
        SpawnBug();
    }

    void SpawnBug()
    {
        if (currentBugCount >= maxBugs) return;

        // 1. 버그 생성
        GameObject newBug = Instantiate(bugPrefab, spawnArea);
        
        // 2. [수정됨] 랜덤 위치 계산
        // spawnArea의 크기를 가져와서 그 안에서 랜덤 좌표를 뽑습니다.
        RectTransform areaRect = spawnArea.GetComponent<RectTransform>();
        float w = areaRect.rect.width * 0.45f;  // 화면 꽉 차게 하면 잘리니까 45% 정도만
        float h = areaRect.rect.height * 0.45f;

        float randomX = Random.Range(-w, w);
        float randomY = Random.Range(-h, h);

        // 3. 위치 적용
        RectTransform rt = newBug.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(randomX, randomY);
        rt.localScale = Vector3.one; 
        
        // Z축 0으로 평평하게 (혹시 모를 3D 회전 방지)
        Vector3 pos = rt.localPosition;
        pos.z = 0;
        rt.localPosition = pos;

        // 4. 버그 스크립트 연결 (죽으면 알림 받기)
        BugWalker walker = newBug.GetComponent<BugWalker>();
        if (walker != null)
        {
            walker.OnBugDeath = () => StartCoroutine(RespawnRoutine());
        }

        currentBugCount++;
    }

    IEnumerator RespawnRoutine()
    {
        currentBugCount--; // 카운트 감소
        yield return new WaitForSeconds(respawnDelay); // 대기
        SpawnBug(); // 재생성 (이때도 랜덤 위치로 나옴)
    }
}