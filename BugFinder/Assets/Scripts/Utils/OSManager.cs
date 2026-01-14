using UnityEngine;

public class OSManager : MonoBehaviour
{
    [Header("1. Main Menu Group (타이틀 화면)")]
    public GameObject mainMenuPanel; // 게임 시작 전 타이틀 화면
    public GameObject phoneOSGroup;  // 스마트폰 화면 전체 부모 (바탕화면 + 앱들)

    [Header("2. Phone Screens (앱 연결)")]
    public GameObject homeScreen;    // 바탕화면 (스테이지 선택창)
    public GameObject calculatorApp; // 계산기 스테이지
    public GameObject galleryApp;    // 갤러리 스테이지 (없으면 비워도 됨)
    public GameObject testRoomApp;   // 테스트룸 (없으면 비워도 됨)

    [Header("3. System UI (공통 UI)")]
    public GameObject backButton;    // 뒤로가기 버튼 (홈 화면에선 숨김)

    // 내부 변수: 현재 켜져 있는 앱을 기억함
    private GameObject currentOpenApp;

    void Start()
    {
        // 게임 시작 시 타이틀 화면 보여주기
        ShowMainMenu();
    }

    // --- [1] 게임 시작 흐름 ---

    // 타이틀 화면 상태 (초기 상태)
    public void ShowMainMenu()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (phoneOSGroup) phoneOSGroup.SetActive(false); // 폰 꺼둠
        if (backButton) backButton.SetActive(false);     // 뒤로가기 버튼 숨김
    }

    // [Start] 버튼을 누르면 실행됨
    public void GameStart()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false); // 타이틀 숨김
        if (phoneOSGroup) phoneOSGroup.SetActive(true);    // 폰 켜기
        
        GoHome(); // 폰이 켜지면 바탕화면으로 진입
    }

    // --- [2] 앱 열기 기능 (아이콘 버튼 연결용) ---

    public void OpenCalculator()
    {
        OpenApp(calculatorApp);
    }

    public void OpenGallery()
    {
        if (galleryApp == null)
        {
            Debug.Log("🚧 갤러리 앱이 아직 연결되지 않았습니다.");
            return;
        }
        OpenApp(galleryApp);
    }

    public void OpenTestRoom()
    {
        if (testRoomApp == null)
        {
            Debug.Log("🚧 테스트룸이 아직 연결되지 않았습니다.");
            return;
        }
        OpenApp(testRoomApp);
    }

    // 내부 공통 로직: 홈을 끄고 특정 앱을 켬
    public void OpenApp(GameObject app) 
    {
        if (app == null) return;

        if (homeScreen) homeScreen.SetActive(false);
        
        currentOpenApp = app;
        currentOpenApp.SetActive(true);
        
        // 뒤로가기 버튼 로직은 삭제했으므로 패스
    }

    // --- [3] 뒤로 가기 (홈으로 복귀) ---
    
    // System_UI의 [뒤로가기] 버튼 & GameResultManager에서 호출
    public void GoHome()
    {
        // 1. 현재 열려있는 앱이 있다면 닫기
        if (currentOpenApp != null)
        {
            currentOpenApp.SetActive(false);
            currentOpenApp = null;
        }

        // 2. 바탕화면(스테이지 선택) 켜기
        if (homeScreen) homeScreen.SetActive(true);
        
        // 3. 바탕화면에서는 뒤로가기 버튼 필요 없음
        if (backButton) backButton.SetActive(false);
    }
}