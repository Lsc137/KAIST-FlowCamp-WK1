using UnityEngine;
using System.Collections;

public class GreenBug : BugBase
{
    [Header("Behavior Pattern")]
    public float idleDuration = 1.0f; // 멈춰있는 시간
    public float dashDuration = 0.5f; // 달리는 시간
    public float dashSpeed = 1500f;   // 달릴 때 속도
    
    [Header("Shake Effect")]
    public float shakeIntensity = 5f; // 떨림 강도

    private bool isDashing = false;
    private Vector2 fixedPosBeforeShake;
    
    // [추가] 파티클 제어용 변수
    private ParticleSystem trailParticle;

    protected override void Start()
    {
        base.Start();
        
        // [추가] 자식으로 있는 파티클(Trail) 찾아두기
        trailParticle = GetComponentInChildren<ParticleSystem>();

        // 패턴 시작
        StartCoroutine(BehaviorRoutine());
    }

    protected override void Update()
    {
        if (isDashing)
        {
            base.Update(); 
        }
        else
        {
            changeDirTimer += Time.deltaTime;
            
            // 떨림 효과
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
            moveSpeed = 0f;
            fixedPosBeforeShake = rectTransform.anchoredPosition;

            // [추가] 파티클 끄기 (연기 안 나옴)
            SetTrailEmission(false);
            
            yield return new WaitForSeconds(idleDuration);

            // --- Phase 2: 발사 (Dash) ---
            isDashing = true;
            moveSpeed = dashSpeed;
            
            // 방향 전환
            moveDir = UnityEngine.Random.insideUnitCircle.normalized; 
            
            // [추가] 파티클 켜기 (연기 뿜뿜)
            SetTrailEmission(true);

            yield return new WaitForSeconds(dashDuration);
        }
    }

    // 파티클의 Emission(발사) 모듈을 켰다 껐다 하는 함수
    void SetTrailEmission(bool isEnabled)
    {
        if (trailParticle != null)
        {
            var emission = trailParticle.emission;
            emission.enabled = isEnabled;
        }
    }
}