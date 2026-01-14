using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageClearEffect : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform shineBar;   // 기울어진 흰색 막대
    public GameObject clearButton;   // 클리어 버튼
    public CanvasGroup clearButtonGroup; // (선택) 버튼 페이드인을 위해

    [Header("Settings")]
    public float shineDuration = 0.8f; // 빛이 지나가는 시간
    public float startX = -1500f;      // 시작 위치 (왼쪽 화면 밖)
    public float endX = 1500f;         // 끝 위치 (오른쪽 화면 밖)

    void Start()
    {
        // 시작할 땐 버튼도 없고 빛도 초기 위치
        if(clearButton) clearButton.SetActive(false);
        ResetShine();
    }

    // 외부에서 "게임 클리어!"가 되면 이 함수를 호출하세요.
    public void PlayClearSequence()
    {
        gameObject.SetActive(true); // 패널 켜기
        StartCoroutine(ShineRoutine());
    }

    IEnumerator ShineRoutine()
    {
        // 1. 빛 지나가기 (쓱-)
        float time = 0;
        while (time < shineDuration)
        {
            time += Time.deltaTime;
            float t = time / shineDuration;
            
            // 선형 보간(Lerp)으로 위치 이동
            Vector2 pos = shineBar.anchoredPosition;
            pos.x = Mathf.Lerp(startX, endX, t); // 부드럽게 쓰윽
            shineBar.anchoredPosition = pos;
            
            yield return null;
        }

        // 2. 빛이 다 지나가면 버튼 등장
        if (clearButton != null)
        {
            clearButton.SetActive(true);
            
            // (선택) 버튼이 뿅! 하고 나타나는 연출 (Scale Up)
            clearButton.transform.localScale = Vector3.zero;
            float popTime = 0;
            while(popTime < 0.3f)
            {
                popTime += Time.deltaTime;
                clearButton.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, popTime / 0.3f);
                yield return null;
            }
        }
    }

    void ResetShine()
    {
        if (shineBar)
        {
            Vector2 pos = shineBar.anchoredPosition;
            pos.x = startX;
            shineBar.anchoredPosition = pos;
        }
    }
}