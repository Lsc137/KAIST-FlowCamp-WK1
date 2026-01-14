using UnityEngine;

public class AppNavigation : MonoBehaviour
{
    public GameObject currentAppPanel; // 현재 앱 (꺼질 놈)
    public GameObject homeScreenPanel; // 홈 화면 (켜질 놈)

    public void GoHome()
    {
        if(currentAppPanel) currentAppPanel.SetActive(false);
        if(homeScreenPanel) homeScreenPanel.SetActive(true);
        
        // (선택) 홈으로 갈 때 효과음 같은거 넣으면 좋음
    }
}