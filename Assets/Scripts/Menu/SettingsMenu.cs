using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private float fadeDuration = 0.3f;


    public void HideSettings()
    {
        panel.DOFade(0, fadeDuration).OnComplete(() =>
        {
            panel.interactable = false;
            panel.blocksRaycasts = false;
            panel.gameObject.SetActive(false);
        });
    }
}
