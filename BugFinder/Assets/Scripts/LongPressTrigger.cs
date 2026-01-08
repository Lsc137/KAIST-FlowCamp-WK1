using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public float requiredHoldTime = 1.0f;
    
    [Header("Spawn Settings")]
    public GameObject bugPrefab; // 소환할 벌레 프리팹 연결용 변수
    public Transform canvasTransform; // 벌레가 생성될 부모 캔버스

    private bool isPressed = false;
    private bool hasTriggered = false;
    private float currentHoldTime = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        hasTriggered = false;
        currentHoldTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        currentHoldTime = 0f;
    }

    void Update()
    {
        if (isPressed && !hasTriggered)
        {
            currentHoldTime += Time.deltaTime;

            if (currentHoldTime >= requiredHoldTime)
            {
                ExecuteTrigger();
            }
        }
    }

    private void ExecuteTrigger()
    {
        hasTriggered = true;
        Handheld.Vibrate();
        Debug.Log("📳 진동 발생! 버그 출현!");

        // --- 수정된 부분: 벌레 소환 로직 ---
        if (bugPrefab != null && canvasTransform != null)
        {
            // 1. 벌레 생성
            GameObject newBug = Instantiate(bugPrefab, canvasTransform);
            
            // 2. 생성 위치 설정 (주홍색 박스의 위치와 동일하게)
            // RectTransform을 사용하여 UI 좌표계 위치를 맞춤
            newBug.GetComponent<RectTransform>().position = this.transform.position;
            
            // 3. (선택사항) 버그가 나왔으니 주홍색 영역은 숨기기?
            // 이펙트 후 사라지게 하려면 나중에 추가. 지금은 그대로 둠.
        }
        else
        {
            Debug.LogError("Bug Prefab 또는 Canvas Transform이 연결되지 않았습니다!");
        }
    }
}