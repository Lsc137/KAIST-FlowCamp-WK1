using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic; // 리스트 사용을 위해 추가

public class TodoManager : MonoBehaviour
{
    [Header("Basic Settings")]
    public GameObject todoItemPrefab;
    public Transform listParent;
    public TextMeshProUGUI countText;
    public int maxCount = 12;

    [Header("UI References")]
    public Image deleteModeButtonImage;
    public Color normalColor = Color.white;
    public Color deleteModeColor = Color.red;

    [Header("🔥 Malfunctions (Bugs)")]
    public bool bug_DoubleSpawn = true;    // + 누르면 2개 생성
    public bool bug_ZombieClear = true;    // 전체 삭제 시 1초간 숨었다 부활
    public bool bug_BrokenCounter = true;  // 카운터 텍스트 깨짐

    // 내부 변수
    private int currentTodoCount = 0;
    private int taskIndex = 1;
    private bool isDeleteMode = false;
    
    // [중요] 초기 위치 기억용 변수
    private Vector2 initialContentPos;

    void Start()
    {
        // 1. 게임 시작 시점에 사용자가 설정해둔 Y값(200 등)을 기억합니다.
        if (listParent != null)
        {
            initialContentPos = listParent.GetComponent<RectTransform>().anchoredPosition;
        }

        UpdateUI();
        UpdateDeleteModeUI();
    }

    // --- 기능 1: 할 일 추가 (+) ---
    public void AddTodoItem()
    {
        int loopCount = bug_DoubleSpawn ? 2 : 1;

        for (int i = 0; i < loopCount; i++)
        {
            if (currentTodoCount >= maxCount) return;
            CreateItemProcess();
        }
        UpdateUI();
    }

    void CreateItemProcess()
    {
        GameObject newItem = Instantiate(todoItemPrefab, listParent);

        // 위치/스케일 교정
        newItem.transform.localScale = Vector3.one;
        Vector3 pos = newItem.transform.localPosition;
        pos.z = 0;
        newItem.transform.localPosition = pos;

        string content = $"Task {taskIndex}";
        if (bug_BrokenCounter) content = GetGibberish(); // (추가) 생성 시점에도 깨지게 하려면

        TodoItem itemScript = newItem.GetComponent<TodoItem>();
        if (itemScript != null)
        {
            itemScript.Setup(this, content);
        }

        currentTodoCount++;
        taskIndex++;
    }

    // --- 기능 2: 전체 삭제 (Clear All) ---
    public void ClearAllItems()
    {
        // [버그 분기점]
        if (bug_ZombieClear)
        {
            // 버그 ON: 지우지 않고 숨기기만 함
            StartCoroutine(ZombieHideAndSeek());
        }
        else
        {
            // 버그 OFF: 진짜로 삭제 (정상 기능)
            foreach (Transform child in listParent)
            {
                Destroy(child.gameObject);
            }
            currentTodoCount = 0;
            
            // [위치 초기화] 기억해둔 초기 위치(Y=200)로 복귀
            ResetScrollPosition();
            
            UpdateUI();
        }
    }

    // 좀비 버그 로직: 아이템을 잠깐 껐다가 다시 켬
    IEnumerator ZombieHideAndSeek()
    {
        Debug.Log("🧟 좀비 버그: 아이템들이 투명해집니다...");

        // 1. 현재 있는 아이템들을 리스트에 담음 (foreach 중 오류 방지)
        List<GameObject> currentItems = new List<GameObject>();
        foreach (Transform child in listParent)
        {
            currentItems.Add(child.gameObject);
        }

        // 2. 안 보이게 숨김 (SetActive false)
        foreach (var item in currentItems)
        {
            if(item) item.SetActive(false);
        }
        
        // *사용자는 삭제된 줄 알겠지?*
        // 여기서도 위치를 초기화해줘야 "삭제돼서 스크롤이 올라간 느낌"을 줌
        ResetScrollPosition();

        // 3. 1초 대기
        yield return new WaitForSeconds(1.0f);

        Debug.Log("🧟 좀비 부활!");

        // 4. 다시 보이게 켬 (SetActive true)
        foreach (var item in currentItems)
        {
            if(item) item.SetActive(true);
        }
        
        // 카운트는 줄어들지 않았으므로 UI 갱신 불필요 (그대로 유지)
    }

    // --- 기능 3: 개별 삭제 ---
    public void OnItemTouched(GameObject item, bool isCheckOn)
    {
        if (isDeleteMode) RequestDelete(item);
        else
        {
            TodoItem script = item.GetComponent<TodoItem>();
            if(script) script.UpdateVisual(isCheckOn);
        }
    }

    public void RequestDelete(GameObject item)
    {
        // (참고) 개별 삭제 시 청개구리 버그는 제거하셨나요? 
        // 만약 필요하면 여기에 다시 넣으시면 됩니다. 지금은 깔끔하게 삭제만 구현.
        Destroy(item);
        currentTodoCount--;
        UpdateUI();
    }

    // --- 유틸리티 ---
    
    // 스크롤 위치를 "처음 세팅한 그곳"으로 되돌리는 함수
    void ResetScrollPosition()
    {
        RectTransform rt = listParent.GetComponent<RectTransform>();
        if (rt != null)
        {
            // X값은 유지하고, Y값만 아까 기억한(initialContentPos.y) 값으로 변경
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, initialContentPos.y);
        }
    }

    public void ToggleDeleteMode()
    {
        isDeleteMode = !isDeleteMode;
        UpdateDeleteModeUI();
    }

    void UpdateUI()
    {
        if (countText == null) return;

        if (bug_BrokenCounter)
        {
            countText.text = $"To-do: {GetGibberish()}";
            countText.color = Color.red;
        }
        else
        {
            countText.text = $"To-Do: {currentTodoCount} / {maxCount}";
            countText.color = Color.black;
        }
    }

    void UpdateDeleteModeUI()
    {
        if (deleteModeButtonImage != null)
            deleteModeButtonImage.color = isDeleteMode ? deleteModeColor : normalColor;
    }

    string GetGibberish()
    {
        string[] words = { "$%#@!", "NUL", "Err", "???", "Fail" };
        return words[Random.Range(0, words.Length)];
    }

    // --- UniversalBugTrigger 연결용 ---
    public void Solve_DoubleSpawn() { bug_DoubleSpawn = false; Debug.Log("✨ 더하기 버그 해결"); }
    public void Solve_ZombieClear() { bug_ZombieClear = false; Debug.Log("✨ 전체삭제 버그 해결"); }
    public void Solve_BrokenCounter() { bug_BrokenCounter = false; UpdateUI(); Debug.Log("✨ 텍스트 버그 해결"); }
}