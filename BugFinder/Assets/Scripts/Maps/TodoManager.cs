using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TodoManager : MonoBehaviour
{
    [Header("Basic Settings")]
    public GameObject todoItemPrefab;
    public Transform listParent;
    public TextMeshProUGUI countText;
    public int maxCount = 12;

    [Header("Navigation")]
    public GameObject backButton; // [추가] 뒤로가기 버튼

    [Header("UI References")]
    public Image deleteModeButtonImage;
    public Color normalColor = Color.white;
    public Color deleteModeColor = Color.red;

    [Header("🔥 Malfunctions (Bugs)")]
    public bool bug_DoubleSpawn = true;
    public bool bug_ZombieClear = true;
    public bool bug_BrokenCounter = true;

    private int currentTodoCount = 0;
    private int taskIndex = 1;
    private bool isDeleteMode = false;
    private Vector2 initialContentPos;

    void Start()
    {
        if (listParent != null)
            initialContentPos = listParent.GetComponent<RectTransform>().anchoredPosition;

        UpdateUI();
        UpdateDeleteModeUI();
    }

    // 앱 켜질 때 상태 확인
    void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.isTodoClear)
        {
            SetNormalMode();
        }
        else
        {
            // 못 깼으면 버그 ON, 버튼 숨김
            bug_DoubleSpawn = true;
            bug_ZombieClear = true;
            bug_BrokenCounter = true;
            if (backButton) backButton.SetActive(false);
        }
    }

    public void SetNormalMode()
    {
        bug_DoubleSpawn = false;
        bug_ZombieClear = false;
        bug_BrokenCounter = false;

        if (countText) countText.color = Color.black;
        UpdateUI();

        if (backButton) backButton.SetActive(true); // 탈출구
        Debug.Log("🛡️ Todo: 정상 모드 가동");
    }

    public void CheckAllBugsFixed()
    {
        if (!bug_DoubleSpawn && !bug_ZombieClear && !bug_BrokenCounter)
        {
            if (GameManager.Instance) GameManager.Instance.CompleteTodo();
            if (backButton) backButton.SetActive(true);
            Debug.Log("🎉 Todo 앱 완전 정복!");
        }
    }

    // --- 기능 로직 (기존 유지) ---

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
        newItem.transform.localScale = Vector3.one;
        Vector3 pos = newItem.transform.localPosition;
        pos.z = 0;
        newItem.transform.localPosition = pos;

        string content = $"Task {taskIndex}";
        if (bug_BrokenCounter) content = GetGibberish();

        TodoItem itemScript = newItem.GetComponent<TodoItem>();
        if (itemScript != null) itemScript.Setup(this, content);

        currentTodoCount++;
        taskIndex++;
    }

    public void ClearAllItems()
    {
        if (bug_ZombieClear) StartCoroutine(ZombieHideAndSeek());
        else
        {
            foreach (Transform child in listParent) Destroy(child.gameObject);
            currentTodoCount = 0;
            ResetScrollPosition();
            UpdateUI();
        }
    }

    IEnumerator ZombieHideAndSeek()
    {
        List<GameObject> currentItems = new List<GameObject>();
        foreach (Transform child in listParent) currentItems.Add(child.gameObject);
        foreach (var item in currentItems) if(item) item.SetActive(false);
        
        ResetScrollPosition();
        yield return new WaitForSeconds(1.0f);

        foreach (var item in currentItems) if(item) item.SetActive(true);
    }

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
        Destroy(item);
        currentTodoCount--;
        UpdateUI();
    }

    void ResetScrollPosition()
    {
        RectTransform rt = listParent.GetComponent<RectTransform>();
        if (rt != null) rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, initialContentPos.y);
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

    // --- 버그 해결 트리거 (여기에 체크 로직 추가됨) ---
    public void Solve_DoubleSpawn() 
    { 
        bug_DoubleSpawn = false; 
        Debug.Log("✨ 더하기 버그 해결"); 
        CheckAllBugsFixed(); 
    }
    public void Solve_ZombieClear() 
    { 
        bug_ZombieClear = false; 
        Debug.Log("✨ 전체삭제 버그 해결"); 
        CheckAllBugsFixed(); 
    }
    public void Solve_BrokenCounter() 
    { 
        bug_BrokenCounter = false; 
        UpdateUI(); 
        Debug.Log("✨ 텍스트 버그 해결"); 
        CheckAllBugsFixed(); 
    }
}