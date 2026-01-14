using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// 어떤 앱에 속한 트리거인지 구별하기 위한 라벨
public enum AppType
{
    None,
    Calculator,
    Todo,
    SNS
}

public class UniversalBugTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("📂 Data Source")]
    public BugDatabase bugDatabase; 

    [Header("⚙️ Settings")]
    public AppType targetApp = AppType.None; // [중요] 인스펙터에서 꼭 설정하세요!
    public bool allowRespawn = false; 

    [Header("📡 Events")]
    public UnityEvent OnBugStart; // 고장내기 (버그 변수 true)
    public UnityEvent OnBugFixed; // 고치기 (버그 변수 false)

    [Header("Visual Hint")]
    public float holdTime = 1.0f;
    public bool shakeButton = true;
    public float shakeIntensity = 5f;

    // 내부 변수
    private bool isPressed = false;
    private float timer = 0f;
    private GameObject spawnedBug;
    private bool isCleared = false; 
    private Vector3 originalPos;

    void OnEnable() 
    {
        // 1. 현재 이 앱이 이미 클리어된 상태인지 확인 (눈치 챙기기)
        bool isAlreadyClear = CheckIfAppCleared();

        if (isAlreadyClear)
        {
            // 이미 깬 상태면? -> 얌전히 있는다.
            isCleared = true;
            isPressed = false;
            // TriggerBreakApp()을 호출하지 않음! -> AppManager의 SetNormalMode가 유지됨
        }
        else
        {
            // 아직 못 깼거나 처음이면? -> 고장 낸다.
            isCleared = false;
            TriggerBreakApp(); 
        }

        // 위치 초기화 (흔들림 보정)
        originalPos = transform.localPosition; 
    }

    // GameManager에게 물어보는 함수
    bool CheckIfAppCleared()
    {
        if (GameManager.Instance == null) return false;

        switch (targetApp)
        {
            case AppType.Calculator: return GameManager.Instance.isCalcClear;
            case AppType.Todo: return GameManager.Instance.isTodoClear;
            case AppType.SNS: return GameManager.Instance.isSNSClear;
            default: return false; // 설정 안 했으면 기본적으로 안 깬 걸로 간주
        }
    }

    void Update()
    {
        // 이미 깼으면 작동 안 함 (테스트 모드 제외)
        if (isCleared && !allowRespawn) return;
        
        if (spawnedBug != null) return;

        if (isPressed)
        {
            timer += Time.deltaTime;
            
            if (shakeButton)
            {
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
        // 클리어했으면 눌러도 반응 없게
        if (isCleared && !allowRespawn) return;
        if (spawnedBug != null) return;

        isPressed = true;
        timer = 0f;
        originalPos = transform.localPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (spawnedBug != null) return;
        
        isPressed = false;
        timer = 0f;
        transform.localPosition = originalPos; 
    }

    void TriggerBreakApp()
    {
        isCleared = false;
        OnBugStart.Invoke(); // 여기서 버그 변수들을 true로 만듦
    }

    void SpawnBug()
    {
        if (bugDatabase == null) return;

        // 재소환 시 다시 고장내기
        if (isCleared) TriggerBreakApp();

        GameObject selectedPrefab = bugDatabase.GetRandomBugPrefab();

        if (selectedPrefab != null)
        {
            isPressed = false;
            transform.localPosition = originalPos; 
            Handheld.Vibrate();

            Canvas rootCanvas = GetComponentInParent<Canvas>();
            Transform targetParent = (rootCanvas != null) ? rootCanvas.transform : transform.parent;

            spawnedBug = Instantiate(selectedPrefab, targetParent);
            spawnedBug.transform.position = transform.position; // 클릭 위치에서 소환
            spawnedBug.transform.localScale = Vector3.one;

            BugBase bugScript = spawnedBug.GetComponent<BugBase>();
            if (bugScript != null)
            {
                bugScript.onDeathCallback = () => { FixBug(); };
            }
        }
    }

    void FixBug()
    {
        isCleared = true;
        OnBugFixed.Invoke(); // 버그 변수 false로 + CheckAllBugsFixed 호출
        transform.localPosition = originalPos;
        Debug.Log("✨ 앱 기능 정상화!");
    }
}