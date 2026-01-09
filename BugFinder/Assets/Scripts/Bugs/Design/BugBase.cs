using UnityEngine;
using UnityEngine.EventSystems;
using System; // [필수] Action(콜백)을 사용하기 위해 필요

public class BugBase : MonoBehaviour, IPointerDownHandler
{
    [Header("Basic Stats")]
    public int hp = 1;              // 체력
    public float moveSpeed = 500f;  // 이동 속도

    [Header("Movement Settings")]
    public float changeDirInterval = 1.5f; // 방향 전환 주기
    
    [Header("Effects")]
    public GameObject deathEffectPrefab; // 죽을 때 터지는 이펙트

    // [핵심] 죽을 때 나를 소환한 쪽에 알리기 위한 콜백 (연락처)
    public Action onDeathCallback;

    // 내부 변수들
    protected RectTransform rectTransform;
    protected Vector2 moveDir;
    protected float changeDirTimer = 0f;

    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // 초기 방향 설정
        SetRandomDirection();
    }

    protected virtual void Update()
    {
        // 1. 이동
        rectTransform.anchoredPosition += moveDir * moveSpeed * Time.deltaTime;

        // 2. 방향 전환 타이머 체크
        changeDirTimer += Time.deltaTime;
        if (changeDirTimer >= changeDirInterval)
        {
            ChangeDirectionRandomly();
            changeDirTimer = 0f;
        }

        // 3. 회전 (머리가 진행 방향을 보게)
        if (moveDir != Vector2.zero)
        {
            transform.up = moveDir;
        }

        // 4. 벽 튕기기
        CheckBounds();
    }

    // --- 이동 관련 로직 ---

    void SetRandomDirection()
    {
        // [수정] System.Random과 충돌 방지를 위해 UnityEngine.Random이라고 명시
        moveDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        
        if (moveDir == Vector2.zero) moveDir = Vector2.right;
    }

    void ChangeDirectionRandomly()
    {
        // [수정] UnityEngine.Random 사용
        // -90+-30, 90+-30 범위 (즉, 60~120도 혹은 -120~-60도 회전)
        float angle = UnityEngine.Random.Range(60f, 120f);
        
        // 50% 확률로 왼쪽(-) 혹은 오른쪽(+) 결정
        if (UnityEngine.Random.value > 0.5f) angle *= -1f;

        // 현재 벡터를 angle만큼 회전시키는 쿼터니언 연산
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        moveDir = (rotation * moveDir).normalized;
    }

    void CheckBounds()
    {
        // 1080x1920 해상도 기준 벽 튕기기 (부모 캔버스 크기에 따라 조절 필요시 수정)
        float halfW = 1080f / 2f; 
        float halfH = 1920f / 2f;
        Vector2 pos = rectTransform.anchoredPosition;
        float radius = rectTransform.rect.width / 2f;

        if (pos.x + radius > halfW) moveDir.x = -Mathf.Abs(moveDir.x);
        else if (pos.x - radius < -halfW) moveDir.x = Mathf.Abs(moveDir.x);

        if (pos.y + radius > halfH) moveDir.y = -Mathf.Abs(moveDir.y);
        else if (pos.y - radius < -halfH) moveDir.y = Mathf.Abs(moveDir.y);
    }

    // --- 전투/상호작용 로직 ---

    public void OnPointerDown(PointerEventData eventData)
    {
        OnHit();
    }

    public virtual void OnHit()
    {
        hp--;
        
        Handheld.Vibrate(); // 타격 시 진동
        
        if (hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log("🐛 버그 사망!");
        
        // [핵심] 나 죽었다고 연락처(Callback)에 신호 보냄
        // CalculatorBugTrigger의 FixBug()가 여기서 실행됨
        onDeathCallback?.Invoke();

        // 이펙트 생성
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}