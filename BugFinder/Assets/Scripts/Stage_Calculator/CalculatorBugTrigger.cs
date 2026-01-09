using UnityEngine;
using UnityEngine.EventSystems;

public class CalculatorBugTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum BugType
    {
        ReversePlus,
        TripleInput,
        CopyDelete,
        CorruptResult
    }

    [Header("Settings")]
    public BugType bugType;
    public Calculator calculator;
    
    // [변경 1] 단일 프리팹 대신 여러 개를 담을 수 있는 배열로 변경
    public GameObject[] bugPrefabs; 
    
    public float holdTime = 1.0f;

    [Header("Visual Hint")]
    public bool shakeButton = true;
    public float shakeIntensity = 5f;

    // 내부 변수
    private bool isPressed = false;
    private float timer = 0f;
    private bool isFixed = false;
    private GameObject spawnedBug;
    private bool bugSpawned = false;

    private Vector3 originalPos;

    void Start()
    {
        // 자동 연결 로직 (기존 유지)
        if (calculator == null) calculator = GetComponentInParent<Calculator>();
        if (calculator == null) calculator = transform.root.GetComponentInChildren<Calculator>(true);
        if (calculator == null) calculator = FindObjectOfType<Calculator>();

        SetBugStatus(true);
    }

    void Update()
    {
        if (isFixed) return;

        if (isPressed && !bugSpawned)
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
        if (isFixed) return;
        isPressed = true;
        timer = 0f;
        originalPos = transform.localPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isFixed) return;
        isPressed = false;
        timer = 0f;
        transform.localPosition = originalPos;
    }

    void SpawnBug()
    {
        // [변경] 리스트가 비었는지 체크하는 로직 삭제 (Calculator가 알아서 함)
        // Calculator가 없으면 중단
        if (calculator == null) return;

        bugSpawned = true;
        isPressed = false;
        transform.localPosition = originalPos;
        Handheld.Vibrate();

        // [핵심 변경] Calculator에게 "랜덤 버그 하나 주세요" 요청
        GameObject selectedBug = calculator.GetWeightedRandomBug();

        if (selectedBug != null)
        {
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            Transform targetParent = (rootCanvas != null) ? rootCanvas.transform : transform.parent;

            spawnedBug = Instantiate(selectedBug, targetParent);
            spawnedBug.transform.position = transform.position;
            spawnedBug.transform.localScale = Vector3.one;

            BugBase bugScript = spawnedBug.GetComponent<BugBase>();
            if (bugScript != null)
            {
                bugScript.onDeathCallback = () => { FixBug(); };
            }
        }
        else
        {
            Debug.LogError("⛔ Calculator에 등록된 버그가 없습니다!");
        }
        
        Debug.Log("🐛 버그 등장!");
    }

    void FixBug()
    {
        isFixed = true;
        SetBugStatus(false);
        transform.localPosition = originalPos;
        Debug.Log("✨ 버그 해결 완료!");
    }

    void SetBugStatus(bool isActive)
    {
        if (calculator == null) return;

        switch (bugType)
        {
            case BugType.ReversePlus: calculator.bug_ReversePlus = isActive; break;
            case BugType.TripleInput: calculator.bug_TripleThree = isActive; break;
            case BugType.CopyDelete: calculator.bug_CopyDelete = isActive; break;
            case BugType.CorruptResult: 
                calculator.bug_CorruptResult = isActive;
                if (!isActive) calculator.RefreshDisplay();
                break;
        }
    }
}