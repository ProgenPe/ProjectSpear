using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPreviewZoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform zoomContainer; // родитель всех UI элементов
    [SerializeField] private RectTransform levelButton;   // кнопка-превью
    [SerializeField] private string sceneName;

    [Header("Animation")]
    [SerializeField] private float zoomDuration = 0.8f;
    [SerializeField] private Ease zoomEase = Ease.InOutCubic;

    private Vector3 startScale;
    private Vector3 startLocalPos;

    private void Awake()
    {
        startScale = zoomContainer.localScale;
        startLocalPos = zoomContainer.localPosition;
    }

    public void ZoomToLevel()
    {
        var btn = GetComponent<Button>();
        if (btn != null) btn.interactable = false;

        // получаем позицию кнопки относительно контейнера
        Vector3 buttonLocalPos = zoomContainer.InverseTransformPoint(levelButton.position);

        // целевая позиция контейнера так, чтобы кнопка оказалась в центре экрана
        Vector3 targetLocalPos = startLocalPos - buttonLocalPos;

        // масштаб контейнера, чтобы кнопка полностью заполнила экран
        float scaleX = ((RectTransform)zoomContainer.parent).rect.width / levelButton.rect.width;
        float scaleY = ((RectTransform)zoomContainer.parent).rect.height / levelButton.rect.height;
        float targetScale = Mathf.Max(scaleX, scaleY);

        // анимация
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(zoomContainer.DOScale(targetScale, zoomDuration).SetEase(zoomEase));
        seq.Join(zoomContainer.DOLocalMove(targetLocalPos, zoomDuration).SetEase(zoomEase));
        seq.OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
}
