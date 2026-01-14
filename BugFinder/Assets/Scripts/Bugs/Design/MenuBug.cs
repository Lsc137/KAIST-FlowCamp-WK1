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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (explosionEffect != null)
        {
            // 1. 내가 속한 캔버스(Canvas)를 찾음 (제일 안전한 부모)
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            Transform targetParent = (rootCanvas != null) ? rootCanvas.transform : transform.root;

            // 2. 이펙트 생성 (일단 위치는 내 현재 위치로)
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity, targetParent);
            
            // 3. [핵심] 스케일 1,1,1로 초기화 (찌그러짐 방지)
            effect.transform.localScale = Vector3.one; 
            
            // 4. [핵심] Z축을 앞으로 확 당김 (화면 뒤로 숨는 거 방지)
            Vector3 pos = effect.transform.localPosition;
            pos.z = -500f; // UI보다 확실히 앞에 오도록 -500
            effect.transform.localPosition = pos;

            Destroy(effect, 1.0f);
        }

        OnBugDeath?.Invoke();
        Destroy(gameObject);
    }
}