using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 터치 감지용

public class BugWalker : MonoBehaviour, IPointerDownHandler
{
    [Header("Movement")]
    public float moveSpeed = 100f;
    public float changeTargetInterval = 2.0f; // 방향 바꾸는 주기

    [Header("Death")]
    public GameObject explosionEffect; // 터지는 이펙트 프리팹 (파티클 등)

    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Vector2 targetPosition;
    private float timer;
    
    // 버그가 죽었다고 매니저에게 알리기 위한 대리자(Delegate)
    public System.Action OnBugDeath; 

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
        SetRandomTarget();
    }

    void Update()
    {
        // 1. 이동
        rectTransform.anchoredPosition = Vector2.MoveTowards(
            rectTransform.anchoredPosition, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );

        // 2. 회전 (머리를 진행 방향으로)
        Vector2 dir = targetPosition - rectTransform.anchoredPosition;
        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90); // 스프라이트 방향에 따라 -90 조절
        }

        // 3. 목적지 도착했거나 시간이 되면 새 목적지 설정
        timer += Time.deltaTime;
        if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 10f || timer > changeTargetInterval)
        {
            SetRandomTarget();
            timer = 0;
        }
    }

    void SetRandomTarget()
    {
        if (parentRect == null) return;
        
        // 부모 크기 안에서 랜덤 좌표 생성
        float w = parentRect.rect.width * 0.45f; // 테두리 여백 감안해서 0.45
        float h = parentRect.rect.height * 0.45f;
        targetPosition = new Vector2(Random.Range(-w, w), Random.Range(-h, h));
    }

    // 터치되었을 때 (사망)
    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. 이펙트 생성 (있다면)
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.parent);
            effect.transform.position = transform.position;
            Destroy(effect, 1.0f); // 1초 뒤 이펙트 삭제
        }

        // 2. 매니저에게 알림 (나 죽어요~)
        OnBugDeath?.Invoke();

        // 3. 자폭
        Destroy(gameObject);
    }
}