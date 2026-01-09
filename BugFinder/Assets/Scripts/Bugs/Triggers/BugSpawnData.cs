using UnityEngine;

[System.Serializable]
public class BugSpawnData
{
    public string name;           // 버그 이름
    public GameObject prefab;     // 버그 프리팹
    [Range(1, 100)] public int weight; // 확률 가중치
}