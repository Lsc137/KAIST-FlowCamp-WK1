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
    public void SendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            GameObject newMessage = Instantiate(messagePrefab, chatContent);
            newMessage.GetComponent<TextMeshProUGUI>().text = chatInput.text;
            chatInput.text = "";
            chatInput.ActivateInputField();
        }
    }
}