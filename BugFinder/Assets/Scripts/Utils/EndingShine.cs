using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingShine : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform shineBar;  // 빛 이미지 (ShineBar)
    public float duration = 1.0f;   // 지나가는 데 걸리는 시간
    public float startDelay = 0.5f; // 엔딩 화면 뜨고 몇 초 뒤에 반짝일지

    [Header("Position")]
    public float startX = -1500f;   // 시작 위치 (왼쪽 화면 밖)
    public float endX = 1500f;      // 끝 위치 (오른쪽 화면 밖)

    // (선택) 반복해서 반짝이게 하고 싶으면 체크
    public bool loop = false; 

    void OnEnable()
    {
        // 이 패널이 켜지자마자 효과 시작
        if (shineBar != null)
        {
            // 위치 초기화
            Vector2 pos = shineBar.anchoredPosition;
            pos.x = startX;
            shineBar.anchoredPosition = pos;

            StartCoroutine(ShineProcess());
        }
    }

    IEnumerator ShineProcess()
    {
        // 딜레이 대기
        yield return new WaitForSeconds(startDelay);

        do // loop가 꺼져있으면 1번만 실행, 켜져있으면 무한 반복
        {
            float time = 0f;
            
            // 쓱 지나가는 애니메이션
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                
                // 위치 이동 (Lerp)
                Vector2 pos = shineBar.anchoredPosition;
                pos.x = Mathf.Lerp(startX, endX, t);
                shineBar.anchoredPosition = pos;

                yield return null;
            }

            // (반복용) 잠시 쉬었다가 다시 처음으로
            if (loop)
            {
                Vector2 resetPos = shineBar.anchoredPosition;
                resetPos.x = startX;
                shineBar.anchoredPosition = resetPos;
                yield return new WaitForSeconds(2.0f); // 2초마다 반짝임
            }

        } while (loop);
    }
}