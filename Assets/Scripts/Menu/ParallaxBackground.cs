using UnityEngine;
using DG.Tweening;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private RectTransform background;
    [SerializeField] private float maxOffset = 40f;
    [SerializeField] private float smoothTime = 0.3f;

    private Vector2 startPos;
    private Tween moveTween;

    private void Start()
    {
        startPos = background.anchoredPosition;
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Vector2 offset = (mousePos - screenCenter) / screenCenter;
        offset = Vector2.ClampMagnitude(offset, 1f);

        Vector2 targetPos = startPos + offset * maxOffset;

        moveTween?.Kill();
        moveTween = background.DOAnchorPos(targetPos, smoothTime).SetEase(Ease.OutQuad);
    }
}
