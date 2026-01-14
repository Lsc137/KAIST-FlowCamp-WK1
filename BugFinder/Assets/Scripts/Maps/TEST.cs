using UnityEngine;

public class TestButtonSpawner : MonoBehaviour
{
    [Header("테스트 설정")]
    public GameObject bugPrefab;   // 소환할 버그 프리팹
    public Transform spawnRoot;    // 버그가 생성될 부모 (보통 MainMenu 패널)

    // 버튼 클릭 시 연결할 함수
    public void OnClickSpawn()
    {
        if (bugPrefab == null || spawnRoot == null) 
        {
            Debug.LogError("프리팹이나 부모 연결이 안 됐습니다!");
            return;
        }

        // 1. 생성
        GameObject newBug = Instantiate(bugPrefab, spawnRoot);

        // 2. 위치 및 스케일 안전 초기화 (UI 버그 방지)
        newBug.transform.localScale = Vector3.one;
        
        RectTransform rt = newBug.GetComponent<RectTransform>();
        // 버튼 근처나 중앙에서 소환 (랜덤 위치 원하면 아래 주석 해제)
        rt.anchoredPosition = Vector2.zero; 
        
        // rt.anchoredPosition = new Vector2(Random.Range(-300, 300), Random.Range(-500, 500));

        Vector3 pos = rt.localPosition;
        pos.z = 0; // Z축 0 고정
        rt.localPosition = pos;

        Debug.Log("🧪 테스트 버그 소환 완료!");
    }
}