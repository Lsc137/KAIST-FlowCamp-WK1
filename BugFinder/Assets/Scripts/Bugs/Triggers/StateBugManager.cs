using UnityEngine;
using System.Collections.Generic;

public class StageBugManager : MonoBehaviour
{
    [Header("Settings")]
    public GlobalResultManager globalResultManager;

    private int totalBugs = 0;
    private int currentFixedBugs = 0;

    void OnEnable()
    {
        // 0.5초 뒤에 초기화 (혹시 버그들이 늦게 뜰까봐 안전장치)
        Invoke("InitializeStage", 0.1f);
    }

    void InitializeStage()
    {
        currentFixedBugs = 0;
        
        // 내 자식들 중에서 버그 찾기
        UniversalBugTrigger[] bugs = GetComponentsInChildren<UniversalBugTrigger>(true);
        totalBugs = bugs.Length;

        Debug.Log($"🧐 [심판관] {gameObject.name} 스캔 완료! 발견된 버그 수: {totalBugs}개");

        if (totalBugs == 0)
        {
            Debug.LogWarning("⚠️ [심판관] 버그가 하나도 없습니다! UniversalBugTrigger가 자식으로 있는지 확인하세요.");
        }

        foreach (var bug in bugs)
        {
            // 이벤트 연결 (중복 방지)
            bug.OnBugFixed.RemoveListener(OnOneBugFixed);
            bug.OnBugFixed.AddListener(OnOneBugFixed);
        }
    }

    void OnOneBugFixed()
    {
        currentFixedBugs++;
        Debug.Log($"🔨 [심판관] 버그 1마리 검거! ({currentFixedBugs} / {totalBugs})");

        if (currentFixedBugs >= totalBugs)
        {
            Debug.Log("🎉 [심판관] 모든 버그 해결! 팝업 요청 보냄.");
            if (globalResultManager != null)
            {
                globalResultManager.ShowClearPopup();
            }
            else
            {
                Debug.LogError("⛔ [심판관] GlobalResultManager가 연결되지 않았습니다!");
            }
        }
    }
}