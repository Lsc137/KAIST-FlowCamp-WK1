using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UniversalBugTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("📂 Data Source")]
    public BugDatabase bugDatabase; 

    [Header("⚙️ Settings")]
    // [변경 3] 테스트룸을 위한 재소환 허용 옵션 (기본값 false)
    public bool allowRespawn = false; 

    [Header("📡 Events")]
    public UnityEvent OnBugStart; // 앱 고장내기 (초기화 및 리스폰 시 호출)
    public UnityEvent OnBugFixed; // 앱 고치기 (버그 잡았을 때 호출)

    [Header("Visual Hint")]
    public float holdTime = 1.0f;
    public bool shakeButton = true;
    public float shakeIntensity = 5f;

    // 내부 상태 변수
    private bool isPressed = false;
    private float timer = 0f;
    private GameObject spawnedBug; // 현재 소환된 벌레
    private bool isCleared = false; // [핵심] 버그를 잡아서 해결된 상태인지?
    private Vector3 originalPos;

    void Start()
    {
        // [변경 1] 조건 없이 무조건 시작하자마자 고장 냄
        TriggerBreakApp();
    }

    void Update()
    {
        // 1. 이미 벌레가 나와있으면 조작 금지
        if (spawnedBug != null) return;

        // 2. 이미 해결된 상태(isCleared)인데, 재소환(allowRespawn)이 꺼져있다면 조작 금지
        // -> 즉, 일반 게임에서는 한 번 잡으면 더 이상 눌러도 반응 없음
        if (isCleared && !allowRespawn) return;

        if (isPressed)
        {
            timer += Time.deltaTime;
            
            if (shakeButton)
            {
                // 부들부들 떨기
                Vector2 shakeOffset = UnityEngine.Random.insideUnitCircle * shakeIntensity;
                transform.localPosition = originalPos + new Vector3(shakeOffset.x, shakeOffset.y, 0);
            }

            if (timer >= holdTime)
            {
                SpawnBug();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (spawnedBug != null) return;
        if (isCleared && !allowRespawn) return;

        isPressed = true;
        timer = 0f;
        originalPos = transform.localPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (spawnedBug != null) return;
        
        // 눌렀다 떼면 떨림 멈추고 복귀
        isPressed = false;
        timer = 0f;
        transform.localPosition = originalPos; 
    }

    // 앱을 고장내는 함수 (시작 시, 혹은 재소환 시 호출)
    void TriggerBreakApp()
    {
        isCleared = false; // 해결 안 된 상태로 변경
        OnBugStart.Invoke(); // 이벤트 발송: "기능아 고장나라!"
    }

    void SpawnBug()
    {
        if (bugDatabase == null)
        {
            Debug.LogError("⛔ 버그 데이터베이스 연결 안됨!");
            return;
        }

        // [중요] 만약 재소환(테스트룸) 상황이라면, 앱이 고쳐져 있을 테니 다시 고장 냄
        if (isCleared)
        {
            TriggerBreakApp();
        }

        GameObject selectedPrefab = bugDatabase.GetRandomBugPrefab();

        if (selectedPrefab != null)
        {
            // 버튼 상태 초기화
            isPressed = false;
            transform.localPosition = originalPos; 
            Handheld.Vibrate();

            // 캔버스 찾아 소환
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            Transform targetParent = (rootCanvas != null) ? rootCanvas.transform : transform.parent;

            spawnedBug = Instantiate(selectedPrefab, targetParent);
            spawnedBug.transform.position = transform.position;
            spawnedBug.transform.localScale = Vector3.one;

            // 콜백 연결
            BugBase bugScript = spawnedBug.GetComponent<BugBase>();
            if (bugScript != null)
            {
                // [변경 2] 버그가 죽으면 FixBug 실행
                bugScript.onDeathCallback = () => { FixBug(); };
            }
            
            Debug.Log($"🐛 버그 소환: {selectedPrefab.name}");
        }
    }

    // 버그를 잡았을 때 호출되는 함수
    void FixBug()
    {
        isCleared = true; // 해결됨 표시
        
        // [변경 2] 기능 복구 이벤트 실행
        OnBugFixed.Invoke(); 
        
        // 버튼 위치 확실하게 복구
        transform.localPosition = originalPos;
        
        Debug.Log("✨ 앱 기능 정상화!");
    }
}