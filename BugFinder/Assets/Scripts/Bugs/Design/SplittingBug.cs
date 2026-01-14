using UnityEngine;

public class SplittingBug : BugBase
{
    [Header("Split Settings")]
    public GameObject miniBugPrefab; // 작은 버그 프리팹 (연결 필수!)
    public int splitCount = 2;       // 분열 개수
    public float spreadRange = 100f; // 퍼지는 범위 (너무 좁으면 겹쳐서 안보임, 100정도로 늘림)

    protected override void Die()
    {
        // 0. 프리팹 연결 확인 (가장 흔한 실수)
        if (miniBugPrefab == null)
        {
            Debug.LogError("⛔ [SplittingBug] Mini Bug Prefab이 연결되지 않았습니다!");
            base.Die();
            return;
        }

        // 1. [Backup] 부모의 유언장(클리어 신호)을 복사
        System.Action savedCallback = onDeathCallback;

        // 2. [Silence] 부모는 입을 다뭄 (지금 죽어도 클리어 처리 안 되게)
        onDeathCallback = null;

        Debug.Log($"🐛 분열 시작! {splitCount}마리 생성 시도...");

        // 3. [Counter] 생존자 카운터
        int remainingChildren = splitCount;

        // 부모의 부모(Canvas/Panel)를 찾음
        Transform targetParent = transform.parent;

        for (int i = 0; i < splitCount; i++)
        {
            // 4. 생성 (부모와 같은 레벨의 형제로 생성)
            GameObject mini = Instantiate(miniBugPrefab, targetParent);
            
            // --- [핵심 수정: 눈에 보이게 강제 교정] ---
            
            // A. 위치: 부모 위치 기준 + 랜덤 오프셋
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * spreadRange;
            mini.transform.position = transform.position; // 일단 부모 위치로 이동
            
            // RectTransform으로 좌표 미세 조정 (anchoredPosition 사용)
            RectTransform miniRect = mini.GetComponent<RectTransform>();
            if (miniRect != null)
            {
                miniRect.anchoredPosition += randomOffset;
            }

            // B. 스케일: 1,1,1로 초기화 (부모 스케일 영향 제거)
            mini.transform.localScale = Vector3.one;

            // C. Z축: 앞으로 확 당김 (배경 뒤로 숨는 것 방지)
            Vector3 localPos = mini.transform.localPosition;
            localPos.z = 0f; // UI 평면과 맞춤 (필요하면 -100f로 더 당김)
            mini.transform.localPosition = localPos;

            // ---------------------------------------

            // 5. [Delegate] 자식에게 임무 부여
            BugBase miniScript = mini.GetComponent<BugBase>();
            if (miniScript != null)
            {
                // 크기를 좀 작게 줄이고 싶다면? (선택사항)
                // mini.transform.localScale = Vector3.one * 0.7f;

                miniScript.onDeathCallback = () => 
                {
                    remainingChildren--; 
                    Debug.Log($"🔹 자식 버그 사망. 남은 수: {remainingChildren}");

                    if (remainingChildren <= 0)
                    {
                        Debug.Log("🎉 분열 버그 완전 박멸! 클리어 신호 전송.");
                        savedCallback?.Invoke();
                    }
                };
            }
            else
            {
                // 프리팹에 스크립트가 없다면 즉시 카운트 감소 (안전장치)
                Debug.LogError("⛔ Mini Bug Prefab에 BugBase 스크립트가 없습니다!");
                remainingChildren--;
            }
        }

        // 6. 부모 사망
        base.Die(); 
    }
}