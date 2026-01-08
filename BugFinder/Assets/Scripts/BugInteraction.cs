using UnityEngine;
using UnityEngine.EventSystems;

public class BugInteraction : MonoBehaviour, IPointerDownHandler
{
    [Header("Effects")]
    public GameObject deathEffectPrefab; // 폭발 이펙트 프리팹 연결용

    public void OnPointerDown(PointerEventData eventData)
    {
        CatchBug();
    }

    private void CatchBug()
    {
        Debug.Log("🐛 버그 체포 완료!");

        // [추가된 부분] 사망 이펙트 생성
        if (deathEffectPrefab != null)
        {
            // 현재 버그의 위치(transform.position)와 회전값(transform.rotation)에 이펙트 생성
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        }

        Handheld.Vibrate();
        Destroy(gameObject);
    }
}