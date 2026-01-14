using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AppIconGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct AppInfo
    {
        public string appName;
        public Sprite iconImage;
        public GameObject appObject;
    }

    [Header("Settings")]
    public GameObject iconPrefab;
    public Transform gridParent;
    public OSManager osManager;

    [Header("App List")]
    public List<AppInfo> appList;

    void Start()
    {
        if (osManager == null) Debug.LogError("⛔ [Generator] OSManager가 연결되지 않았습니다!");
        if (iconPrefab == null) Debug.LogError("⛔ [Generator] Icon Prefab이 없습니다!");
        if (gridParent == null) Debug.LogError("⛔ [Generator] Grid Parent가 없습니다!");

        GenerateIcons();
    }

    void GenerateIcons()
    {
        // 기존 아이콘 청소
        foreach (Transform child in gridParent) Destroy(child.gameObject);

        foreach (var app in appList)
        {
            // [중요] C# 반복문 클로저 문제 해결을 위해 로컬 변수에 복사
            // 이걸 안 하면 모든 버튼이 마지막 앱만 켭니다!
            AppInfo localApp = app; 

            // 1. 생성
            GameObject newIcon = Instantiate(iconPrefab, gridParent);
            newIcon.name = $"Icon_{localApp.appName}";

            // 2. 이미지/텍스트 설정
            Image img = newIcon.GetComponent<Image>();
            if (img && localApp.iconImage) img.sprite = localApp.iconImage;

            TextMeshProUGUI txt = newIcon.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = localApp.appName;

            // 3. 버튼 연결
            Button btn = newIcon.GetComponent<Button>();
            if (btn != null)
            {
                // 이전 리스너 제거 (혹시 몰라서)
                btn.onClick.RemoveAllListeners();

                btn.onClick.AddListener(() => 
                {
                    Debug.Log($"🖱️ 아이콘 클릭됨: {localApp.appName}"); // 로그 1

                    if (osManager != null)
                    {
                        if (localApp.appObject != null)
                        {
                            Debug.Log($"✅ OSManager에게 요청: {localApp.appObject.name} 켜줘!"); // 로그 2
                            osManager.OpenApp(localApp.appObject);
                        }
                        else
                        {
                            Debug.LogError($"❌ [Generator] {localApp.appName}의 App Object가 비어있습니다 (None)!");
                        }
                    }
                    else
                    {
                        Debug.LogError("⛔ [Generator] 클릭은 됐는데 OSManager가 없습니다!");
                    }
                });
            }
            else
            {
                Debug.LogError($"⛔ 프리팹({newIcon.name})에 Button 컴포넌트가 없습니다!");
            }
        }
    }
}