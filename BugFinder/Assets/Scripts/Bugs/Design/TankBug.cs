using UnityEngine;
using System.Collections;

public class TankBug : BugBase
{
    [Header("Shield Settings")]
    public float shieldDuration = 2.0f;     // 방어막 켜지는 시간 (무적)
    public float vulnerableDuration = 1.0f; // 방어막 꺼지는 시간 (타격 가능)
    public GameObject shieldVisual;         // 방어막 이펙트 (방울 모양 이미지 등)

    private bool isShielded = false;

    protected override void Start()
    {
        base.Start();
        
        // 1. 초기 스펙 설정 (HP는 적당히, 속도는 느리게)
        hp = 3; 
        moveSpeed = 300f;

        // 2. 방어막 패턴 시작
        StartCoroutine(ShieldRoutine());
    }

    // BugBase의 OnHit(피격) 함수를 가로채서(Override) 무적 판정을 넣음
    public override void OnHit()
    {
        if (isShielded)
        {
            // 방어막 상태면 데미지 무시
            Debug.Log("🛡️ 팅! (방어막 작동 중)");
            
            // (선택) 팅겨내는 효과음이나 진동을 짧게 줄 수도 있음
            // Handheld.Vibrate(); 
            return;
        }

        // 방어막이 없으면 부모의 기본 로직(체력 감소, 사망 등) 실행
        base.OnHit();
    }

    IEnumerator ShieldRoutine()
    {
        while (true)
        {
            // --- Phase 1: 방어막 ON (2초) ---
            isShielded = true;
            if (shieldVisual != null) shieldVisual.SetActive(true);
            
            yield return new WaitForSeconds(shieldDuration);

            // --- Phase 2: 방어막 OFF (1초) ---
            isShielded = false;
            if (shieldVisual != null) shieldVisual.SetActive(false);
            
            yield return new WaitForSeconds(vulnerableDuration);
        }
    }
}