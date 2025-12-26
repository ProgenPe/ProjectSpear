using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CameraZoomOut2D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera targetCamera;       // камера для отдаления
    [SerializeField] private float targetOrthoSize = 10f; // конечный размер orthographic
    [SerializeField] private float zoomDuration = 1f;
    [SerializeField] private string targetScene = "MainMenu";
    [SerializeField] private Ease zoomEase = Ease.InOutCubic;

    private bool triggered = false;
    private float startOrthoSize;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        startOrthoSize = targetCamera.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        // проверка тега игрока
        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Плавное увеличение orthographicSize = «отдаление»
            DOTween.To(() => targetCamera.orthographicSize,
                       x => targetCamera.orthographicSize = x,
                       targetOrthoSize,
                       zoomDuration)
                   .SetEase(zoomEase)
                   .OnComplete(() =>
                   {
                       SceneManager.LoadScene(targetScene);
                   });
        }
    }
}
