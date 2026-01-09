using UnityEngine;
using System.Collections.Generic;

// [CreateAssetMenu] : 프로젝트 창에서 우클릭 -> Create 메뉴에 이 항목을 추가함
[CreateAssetMenu(fileName = "NewBugDatabase", menuName = "Game/Bug Database")]
public class BugDatabase : ScriptableObject
{
    [Header("버그 소환 목록")]
    public List<BugSpawnData> bugList; // 여기에 버그들을 등록합니다.

    // 가중치 랜덤 뽑기 기능도 여기로 옮겨서 재사용성을 높입니다.
    public GameObject GetRandomBugPrefab()
    {
        if (bugList == null || bugList.Count == 0) return null;

        int totalWeight = 0;
        foreach (var data in bugList) totalWeight += data.weight;

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var data in bugList)
        {
            currentSum += data.weight;
            if (randomValue < currentSum) return data.prefab;
        }
        return bugList[0].prefab;
    }
}