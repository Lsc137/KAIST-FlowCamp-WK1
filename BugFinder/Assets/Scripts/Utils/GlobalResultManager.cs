using UnityEngine;

public class GlobalResultManager : MonoBehaviour
{
    [Header("System References")]
    public OSManager osManager;      // 홈 복귀용
    public GameObject resultPopup;   // 자기 자신 (System_Result_Popup)

    // 버그가 해결되면 호출 (UniversalBugTrigger -> OnBugFixed)
    public void ShowClearPopup()
    {
        resultPopup.SetActive(true);
        // 필요하다면 여기서 축하 효과음이나 파티클 재생
    }

    // [홈으로] 버튼 클릭 시 호출
    public void GoHome()
    {
        resultPopup.SetActive(false); // 팝업 닫고
        osManager.GoHome();           // 바탕화면으로 이동
    }
}