using UnityEngine;
using UnityEngine.UI; // 이미지 제어를 위해 필요

public class InvisibleBug : BugBase
{
    [Header("Invisible Settings")]
    public float visibleTime = 1.0f;   // 보이는 시간
    public float invisibleTime = 2.0f; // 안 보이는 시간
    public float alphaWhenInvisible = 0.1f; // 투명할 때의 투명도 (0이면 완전 안보임, 0.1은 흐릿하게)

    private Image bugImage;
    private float toggleTimer = 0f;
    private bool isInvisible = false;

    protected override void Start()
    {
        base.Start();
        
        // 1. 이미지 컴포넌트 가져오기
        bugImage = GetComponent<Image>();
        
        // 2. 스펙 강제 조정 (느리고 둔하게)
        // (프리팹에서 설정해도 되지만, 코드로 강제하면 실수를 줄임)
        moveSpeed = 300f;          // 느림
        changeDirInterval = 3.0f;  // 방향 잘 안 바꿈
    }

    protected override void Update()
    {
        base.Update(); // BugBase의 기본 이동/벽 튕기기 유지

        // 투명화 사이클 로직
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        if (bugImage == null) return;

        toggleTimer += Time.deltaTime;

        // 상태 전환 체크
        float targetTime = isInvisible ? invisibleTime : visibleTime;

        if (toggleTimer >= targetTime)
        {
            toggleTimer = 0f;
            isInvisible = !isInvisible; // 상태 뒤집기
            
            // 색상 적용
            Color c = bugImage.color;
            c.a = isInvisible ? alphaWhenInvisible : 1.0f;
            bugImage.color = c;
        }
    }
}