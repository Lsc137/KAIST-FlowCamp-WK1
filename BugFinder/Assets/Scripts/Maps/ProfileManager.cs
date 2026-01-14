using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [Header("Profile Info")]
    public TextMeshProUGUI followerText;
    public Text followButtonText;
    private int followerCount = 255;
    private bool isFollowing = false;

    [Header("Message Panel")]
    public GameObject messagePanel;   // MessagePanel 오브젝트 연결
    public TMP_InputField chatInput;  // 채팅 입력창 연결
    public Transform chatContent;     // Scroll View -> Content 연결
    public GameObject messagePrefab;  // 방금 만든 MessageItem 프리팹 연결

    [Header("Photo Gallery Settings")]
    public GameObject photoPopupPanel; // 전체화면 팝업창 (처음엔 꺼둠)
    public Image popupDetailImage;     // 팝업창 안에 뜰 큰 이미지
    void Start() { followerText.text = followerCount.ToString(); }

    public void OnFollowButtonClick()
    {
        isFollowing = !isFollowing;
        followerCount += isFollowing ? 1 : -1;
        followButtonText.text = isFollowing ? "Unfollow" : "+Follow";
        followerText.text = followerCount.ToString();
    }

    // 메세지 패널 열기
    public void OpenMessagePanel() { messagePanel.SetActive(true); }
    
    // 메세지 패널 닫기 (뒤로가기용)
    public void CloseMessagePanel() { messagePanel.SetActive(false); }

    // 메세지 전송
    // 메세지 전송 함수 (수정됨)
    public void SendChatMessage()
    {
        // 1. 빈 내용이면 전송 안 함
        if (string.IsNullOrEmpty(chatInput.text)) return;

        // 2. 프리팹 생성
        GameObject newMessage = Instantiate(messagePrefab, chatContent);

        // 3. [위치/스케일 안전장치] Z축 0으로 평평하게, 크기 1배
        newMessage.transform.localScale = Vector3.one;
        Vector3 pos = newMessage.transform.localPosition;
        pos.z = 0;
        newMessage.transform.localPosition = pos;

        // 4. [텍스트 연동 수정]
        // 프리팹 구조가 (Image -> Text)로 바뀌었으므로, 자식들 중에서 TMP를 찾아야 함
        TextMeshProUGUI textComp = newMessage.GetComponentInChildren<TextMeshProUGUI>();

        if (textComp != null)
        {
            textComp.text = chatInput.text; // 입력한 내용 넣기
        }
        else
        {
            Debug.LogError("⛔ 프리팹 안에 TextMeshProUGUI가 없습니다! 구조를 확인하세요.");
        }

        // 5. [입력창 초기화]
        chatInput.text = ""; // 텍스트 지우기
        chatInput.ActivateInputField(); // 엔터 친 후에도 커서 유지 (연속 입력 편하게)
        
        // (선택사항) 스크롤 맨 아래로 내리기
        // LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
    }

    // [기능 1] 작은 사진을 눌렀을 때 (버튼에 연결)
    public void OnPhotoClick(Image clickedImage)
    {
        // 1. 팝업창을 켠다
        photoPopupPanel.SetActive(true);

        // 2. 클릭한 사진의 그림(Sprite)을 큰 이미지로 옮긴다
        if (clickedImage != null && popupDetailImage != null)
        {
            popupDetailImage.sprite = clickedImage.sprite;
            
            // (팁) 원본 비율 유지 (이미지가 찌그러지지 않게)
            popupDetailImage.preserveAspect = true; 
        }
    }

    // [기능 2] 팝업창을 닫을 때 (팝업창 배경 버튼에 연결)
    public void ClosePhotoPopup()
    {
        photoPopupPanel.SetActive(false);
    }
}