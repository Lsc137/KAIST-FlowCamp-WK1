using UnityEngine;

public class BugMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 500f; // 이동 속도

    private RectTransform rectTransform;
    private Vector2 movementDirection;
    private Vector2 screenBounds; // 화면 크기 저장

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // 1. 시작 시 완전 랜덤한 방향 설정 (대각선 움직임을 위해 X, Y 모두 랜덤)
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        
        // 0이 되지 않도록 보정 (멈춰있으면 안되므로)
        if (randomX == 0) randomX = 1f;
        if (randomY == 0) randomY = 1f;

        movementDirection = new Vector2(randomX, randomY).normalized;
    }

    void Update()
    {
        // 2. 이동 로직 (현재 방향으로 계속 이동)
        rectTransform.anchoredPosition += movementDirection * moveSpeed * Time.deltaTime;

        // [추가된 부분] 회전 로직: 이동하는 방향(movementDirection)을 바라보게 함
        // 버그 이미지의 '위쪽(Up)'이 진행 방향이 되도록 설정
        if (movementDirection != Vector2.zero)
        {
            transform.up = movementDirection;
        }
        
        // 3. 화면 경계 체크 및 튕기기 (DVD 로고 스타일)
        CheckBounds();
    }

    void CheckBounds()
    {
        // 현재 캔버스(화면)의 해상도 크기를 가져옴
        // 부모가 Canvas라고 가정할 때의 로컬 좌표계 경계입니다.
        float halfWidth = Screen.width / 2f; // Canvas Scaler 설정에 따라 다를 수 있으나, 일단 Screen 기준
        float halfHeight = Screen.height / 2f;

        // 현재 위치
        Vector2 pos = rectTransform.anchoredPosition;

        // 버그 이미지의 반지름 (크기의 절반) - 벽에 파묻히지 않게 하기 위함
        float objectHalfSize = rectTransform.rect.width / 2f;

        // 오른쪽 벽 충돌 -> 왼쪽으로 튕김
        if (pos.x + objectHalfSize > 1080 / 2f) // Canvas 해상도 너비(1080) 기준 절반
        {
            movementDirection.x = -Mathf.Abs(movementDirection.x);
        }
        // 왼쪽 벽 충돌 -> 오른쪽으로 튕김
        else if (pos.x - objectHalfSize < -1080 / 2f)
        {
            movementDirection.x = Mathf.Abs(movementDirection.x);
        }

        // 위쪽 벽 충돌 -> 아래로 튕김
        if (pos.y + objectHalfSize > 1920 / 2f) // Canvas 해상도 높이(1920) 기준 절반
        {
            movementDirection.y = -Mathf.Abs(movementDirection.y);
        }
        // 아래쪽 벽 충돌 -> 위로 튕김
        else if (pos.y - objectHalfSize < -1920 / 2f)
        {
            movementDirection.y = Mathf.Abs(movementDirection.y);
        }
    }
}