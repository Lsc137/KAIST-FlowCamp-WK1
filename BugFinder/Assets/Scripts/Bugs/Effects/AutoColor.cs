using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ParticleSystem))]
public class AutoParticleColor : MonoBehaviour
{
    [Header("Settings")]
    public bool syncEveryFrame = false; // 투명 버그처럼 실시간 색변화가 필요하면 체크
    [Range(0f, 1f)] public float alphaMultiplier = 0.6f; // 부모보다 얼마나 더 투명하게 할지 (먼지는 보통 연하니까)

    private ParticleSystem ps;
    private Image parentImage;
    private SpriteRenderer parentSprite;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        
        // 부모 찾기 (Image 우선, 없으면 SpriteRenderer)
        parentImage = GetComponentInParent<Image>();
        if (parentImage == null) 
            parentSprite = GetComponentInParent<SpriteRenderer>();

        UpdateColor();
    }

    void Update()
    {
        // 투명 버그 등 실시간 변화가 필요할 때만 매 프레임 실행
        if (syncEveryFrame)
        {
            UpdateColor();
        }
    }

    void UpdateColor()
    {
        if (ps == null) return;

        Color targetColor = Color.white;

        // 부모 색상 가져오기
        if (parentImage != null) targetColor = parentImage.color;
        else if (parentSprite != null) targetColor = parentSprite.color;

        // 먼지 느낌을 위해 투명도(Alpha)를 좀 더 낮춤
        targetColor.a *= alphaMultiplier;

        // 파티클 시스템의 Main 모듈에 접근해서 색상 변경
        var main = ps.main;
        main.startColor = targetColor;
    }
}