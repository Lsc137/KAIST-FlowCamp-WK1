using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 터치 이벤트 감지용

public class BuggyPhoto : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public float rotateSpeed = 500f; // 속도 더 올림
    public bool isBugged = true;     // 현재 버그 상태인지

    private bool isHolding = false;  // 누르고 있는지 확인
    private ProfileManager profileManager;

    void Start()
    {
        profileManager = FindObjectOfType<ProfileManager>();
    }

    void Update()
    {
        // 버그 상태이고 + 누르고 있지 않을 때만 -> 회전
        if (isBugged && !isHolding)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
    }

    // 1. 눌렀을 때 (터치 시작)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBugged)
        {
            isHolding = true; // 회전 멈춤
        }
        else
        {
            // 버그가 해결된 상태라면 -> 팝업 열기
            if (profileManager != null) 
                profileManager.OnPhotoClick(GetComponent<Image>());
        }
    }

    // 2. 뗐을 때 (터치 끝)
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isBugged)
        {
            isHolding = false; // 다시 회전 시작
        }
    }

    // 3. 외부(UniversalTrigger)에서 호출해서 버그를 고치는 함수
    public void FixBug()
    {
        isBugged = false;
        isHolding = false;
        transform.rotation = Quaternion.identity; // 각도 0으로 원상복구
        Debug.Log("✨ 사진 버그 해결됨!");
    }
}