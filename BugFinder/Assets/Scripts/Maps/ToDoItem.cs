using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TodoItem : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI contentText; // 할 일 내용 텍스트
    public Toggle checkToggle;          // 체크박스

    private TodoManager manager;
    private GameObject myObj;

    // 생성될 때 매니저가 설정해주는 함수
    public void Setup(TodoManager mgr, string text)
    {
        manager = mgr;
        myObj = this.gameObject;

        // 텍스트 설정
        if (contentText) contentText.text = text;

        // 토글 이벤트 연결
        if (checkToggle)
        {
            checkToggle.onValueChanged.RemoveAllListeners();
            checkToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    // 체크박스가 눌렸을 때
    void OnToggleChanged(bool isOn)
    {
        if (manager == null) return;

        // 매니저에게 "나 터치됐어, 현재 상태는 isOn이야" 라고 보고
        // (삭제 모드인지, 그냥 체크인지 판단은 매니저가 함)
        manager.OnItemTouched(myObj, isOn);
    }

    // 시각적 업데이트 (취소선 긋기) - 매니저가 호출함
    public void UpdateVisual(bool isChecked)
    {
        if (contentText)
        {
            contentText.fontStyle = isChecked ? FontStyles.Strikethrough : FontStyles.Normal;
            contentText.color = isChecked ? Color.gray : Color.black;
        }
        
        // 토글 UI 상태도 코드로 강제 동기화 (필요시)
        if (checkToggle && checkToggle.isOn != isChecked)
        {
            checkToggle.SetIsOnWithoutNotify(isChecked);
        }
    }
}