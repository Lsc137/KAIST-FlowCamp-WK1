using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ProfileManager : MonoBehaviour
{
    [Header("Profile Info")]
    public TextMeshProUGUI nameText; 
    public TextMeshProUGUI followerText;
    public Text followButtonText;
    private int followerCount = 255;
    private bool isFollowing = false;

    [Header("Message Panel")]
    public GameObject messagePanel;
    public TMP_InputField chatInput;
    public Transform chatContent;
    public GameObject messagePrefab;

    [Header("Photo Gallery")]
    public GameObject photoPopupPanel;
    public Image popupImage;
    
    [Header("Navigation")]
    public GameObject backButton; // [추가] 뒤로가기 버튼

    [Header("🔥 Malfunctions")]
    public bool bug_AutoUnfollow = true; 
    public bool bug_TrollChat = true;    
    public bool bug_NameGlitch = true;   

    void Start()
    {
        UpdateFollowUI();
    }

    // 앱 켜질 때 상태 확인
    void OnEnable()
    {
        // 1. 이미 깬 상태라면?
        if (GameManager.Instance != null && GameManager.Instance.isSNSClear)
        {
            SetNormalMode();
        }
        else
        {
            // 2. 아직이면 버그 모드 가동
            bug_AutoUnfollow = true;
            bug_TrollChat = true;
            bug_NameGlitch = true;
            
            // 이름 글리치 시작
            StopCoroutine("NameGlitchRoutine");
            StartCoroutine("NameGlitchRoutine");

            if (backButton) backButton.SetActive(false);
        }
    }

    public void SetNormalMode()
    {
        bug_AutoUnfollow = false;
        bug_TrollChat = false;
        bug_NameGlitch = false;

        // UI 정상화
        if(nameText) { nameText.text = "MadCamp"; nameText.color = Color.black; }
        if(followButtonText) followButtonText.text = isFollowing ? "Unfollow" : "+Follow";

        StopCoroutine("NameGlitchRoutine"); // 글리치 강제 종료

        if (backButton) backButton.SetActive(true); // 탈출구
        Debug.Log("🛡️ SNS: 정상 모드 가동");
    }

    public void CheckAllBugsFixed()
    {
        if (!bug_AutoUnfollow && !bug_TrollChat && !bug_NameGlitch)
        {
            if (GameManager.Instance) GameManager.Instance.CompleteSNS();
            if (backButton) backButton.SetActive(true);
            Debug.Log("🎉 SNS 앱 완전 정복!");
        }
    }

    // --- 기능 로직 (기존 유지) ---

    public void OnFollowButtonClick()
    {
        isFollowing = !isFollowing;
        followerCount += isFollowing ? 1 : -1;
        UpdateFollowUI();

        if (isFollowing && bug_AutoUnfollow)
        {
            StopCoroutine("AutoUnfollowProcess");
            StartCoroutine("AutoUnfollowProcess");
        }
    }

    IEnumerator AutoUnfollowProcess()
    {
        yield return new WaitForSeconds(0.3f);
        isFollowing = false;
        followerCount--; 
        UpdateFollowUI();
        if(followButtonText) followButtonText.text = "Failed";
    }

    void UpdateFollowUI()
    {
        if (followButtonText) followButtonText.text = isFollowing ? "Unfollow" : "+Follow";
        if (followerText) followerText.text = followerCount.ToString();
    }

    public void OpenMessagePanel() { messagePanel.SetActive(true); }
    public void CloseMessagePanel() { messagePanel.SetActive(false); }

    public void SendChatMessage()
    {
        if (string.IsNullOrEmpty(chatInput.text)) return;

        string finalMessage = chatInput.text;
        if (bug_TrollChat)
        {
            string[] trollMsgs = { "Error_404", "Connection_Lost", "System_Fault", "$%#@!" };
            finalMessage = trollMsgs[Random.Range(0, trollMsgs.Length)];
        }

        GameObject newMessage = Instantiate(messagePrefab, chatContent);
        newMessage.transform.localScale = Vector3.one;
        Vector3 pos = newMessage.transform.localPosition;
        pos.z = 0;
        newMessage.transform.localPosition = pos;

        TextMeshProUGUI textComp = newMessage.GetComponentInChildren<TextMeshProUGUI>();
        if (textComp != null) textComp.text = finalMessage;

        chatInput.text = "";
        chatInput.ActivateInputField();
    }

    IEnumerator NameGlitchRoutine()
    {
        if(nameText) nameText.text = "MadCamp";
        while (bug_NameGlitch)
        {
            float waitNormal = Random.Range(2.0f, 3.0f);
            if(nameText) { nameText.text = "MadCamp"; nameText.color = Color.black; }
            yield return new WaitForSeconds(waitNormal);

            if (bug_NameGlitch) 
            {
                if(nameText) { nameText.text = "BugCamp"; nameText.color = Color.red; }
                yield return new WaitForSeconds(0.2f);
            }
        }
        if(nameText) { nameText.text = "MadCamp"; nameText.color = Color.black; }
    }

    public void OnPhotoClick(Image clickedImage)
    {
        photoPopupPanel.SetActive(true);
        popupImage.sprite = clickedImage.sprite;
        popupImage.preserveAspect = true; 
    }
    public void ClosePhotoPopup() { photoPopupPanel.SetActive(false); }

    // --- 버그 해결 트리거 (CheckAllBugsFixed 추가됨) ---
    public void Solve_AutoUnfollow() 
    { 
        bug_AutoUnfollow = false; 
        Debug.Log("✨ 팔로우 버그 해결");
        CheckAllBugsFixed();
    }
    public void Solve_TrollChat() 
    { 
        bug_TrollChat = false; 
        Debug.Log("✨ 채팅 버그 해결");
        CheckAllBugsFixed();
    }
    public void Solve_NameGlitch() 
    { 
        bug_NameGlitch = false; 
        if(nameText) { nameText.text = "MadCamp"; nameText.color = Color.black; }
        Debug.Log("✨ 이름 버그 해결");
        CheckAllBugsFixed();
    }
}