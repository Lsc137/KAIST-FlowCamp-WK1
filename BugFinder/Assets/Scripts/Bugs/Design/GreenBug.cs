using UnityEngine;
using System.Collections; // 코루틴 사용

public class GreenBug : BugBase
{
    [Header("Behavior Pattern")]
    public float idleDuration = 1.0f; // 멈춰있는 시간
    public float dashDuration = 0.5f; // 달리는 시간
    public float dashSpeed = 1500f;   // 달릴 때 속도 (엄청 빠름)
    
    [Header("Shake Effect")]
    public float shakeIntensity = 5f; // 떨림 강도

    private bool isDashing = false;
    private Vector2 fixedPosBeforeShake; // 떨리기 전 기준 위치

    protected override void Start()
    {
        base.Start();
        // 패턴 시작
        StartCoroutine(BehaviorRoutine());
    }

    // BugBase의 Update를 덮어씌워서(Override) 이동 로직을 직접 통제함
    protected override void Update()
    {
        // 1. 대시 중일 때만 부모의 이동 로직(벽 튕기기 등)을 사용
        if (isDashing)
        {
            base.Update(); 
        }
        // 2. 멈춰있을 때는 직접 떨림 효과만 줌 (이동 X)
        else
        {
            // 부모의 changeDirTimer 등은 계속 돌려줌 (혹시 모르니)
            changeDirTimer += Time.deltaTime;
            
            // 떨림 효과: 기준 위치 + 랜덤 오프셋
            Vector2 shake = UnityEngine.Random.insideUnitCircle * shakeIntensity;
            rectTransform.anchoredPosition = fixedPosBeforeShake + shake;
        }
    }

    IEnumerator BehaviorRoutine()
    {
        while (true)
        {
            // --- Phase 1: 멈춰서 떨기 (Idle) ---
            isDashing = false;
            moveSpeed = 0f; // 속도 0
            fixedPosBeforeShake = rectTransform.anchoredPosition; // 현재 위치 고정
            
            yield return new WaitForSeconds(idleDuration);

            // --- Phase 2: 발사 (Dash) ---
            isDashing = true;
            moveSpeed = dashSpeed; // 속도 급상승
            
            // 발사할 때 방향을 랜덤으로 한 번 꺾어줌 (예측 불가하게)
            // BugBase에 있는 moveDir 변수를 직접 변경
            moveDir = UnityEngine.Random.insideUnitCircle.normalized; 
            
            yield return new WaitForSeconds(dashDuration);
        }
    }
}