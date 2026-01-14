using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 어디서든 접근 가능한 싱글톤

    [Header("App Clear Flags (자동 저장됨)")]
    public bool isTodoClear = false;
    public bool isSNSClear = false;
    public bool isCalcClear = false;

    void Awake()
    {
        // 싱글톤 설정 (중복 생성 방지)
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 씬이 바뀌어도 파괴되지 않게 하려면 아래 주석 해제
        // DontDestroyOnLoad(gameObject); 
    }

    // 각 앱에서 버그를 다 잡으면 이 함수들을 호출합니다.
    public void CompleteTodo() 
    { 
        isTodoClear = true; 
        Debug.Log("✅ Todo 앱 클리어 저장됨!"); 
    }

    public void CompleteSNS() 
    { 
        isSNSClear = true; 
        Debug.Log("✅ SNS 앱 클리어 저장됨!"); 
    }

    public void CompleteCalc() 
    { 
        isCalcClear = true; 
        Debug.Log("✅ 계산기 앱 클리어 저장됨!"); 
    }
}