using UnityEngine;

// BugBase를 상속받음 (MonoBehaviour 아님)
public class SplittingBug : BugBase
{
    [Header("Split Settings")]
    public GameObject miniBugPrefab; // 분열되어 나올 작은 버그 프리팹
    public int splitCount = 2;       // 몇 마리로 쪼개질지

    // BugBase의 Die 함수를 덮어씀 (Override)
    protected override void Die()
    {
        // 1. 작은 버그들 소환
        if (miniBugPrefab != null)
        {
            for (int i = 0; i < splitCount; i++)
            {
                // 현재 위치에 생성
                GameObject mini = Instantiate(miniBugPrefab, transform.parent); // 부모 캔버스 유지
                mini.GetComponent<RectTransform>().position = this.transform.position;
                
                // (선택) 갓 태어난 버그들이 서로 겹치지 않게 살짝 랜덤 위치 조정 가능
            }
        }

        // 2. 부모 클래스의 Die() 실행 (이펙트 터지고, 본체 삭제됨)
        base.Die(); 
    }
}