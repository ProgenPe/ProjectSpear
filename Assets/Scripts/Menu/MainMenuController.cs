using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private CanvasGroup levelSelectPanel;
    [SerializeField] private CanvasGroup settingsPanel;

    private CanvasGroup currentPanel;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.4f;

    [SerializeField] private bool isInMenu = true;
    private void Start()
    {
        if (isInMenu) ShowMainPanelInstant();
    }

    // ===== ÊÍÎÏÊÈ =====

    public void OnPlayClicked()
    {
        SwitchPanel(mainPanel, levelSelectPanel);
        currentPanel = levelSelectPanel;
    }

    public void OnContinueClicked() 
    { 
        GameManager.Instance.TogglePauseMenu();
    }

    public void OnSettingsClicked()
    {
        SwitchPanel(mainPanel, settingsPanel);
        currentPanel = settingsPanel;
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnBackClicked()
    {
        SwitchPanel(currentPanel, mainPanel);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ===== ÂÑÏÎÌÎÃÀÒÅËÜÍÎÅ =====

    private void SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        from.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            from.interactable = false;
            from.blocksRaycasts = false;

            to.gameObject.SetActive(true);
            to.alpha = 0;
            to.interactable = true;
            to.blocksRaycasts = true;
            to.DOFade(1, fadeDuration).SetUpdate(true);
        });
    }

    private void ShowMainPanelInstant()
    {
        mainPanel.gameObject.SetActive(true);
        levelSelectPanel.gameObject.SetActive(false);

        mainPanel.alpha = 1;
        mainPanel.interactable = true;
        mainPanel.blocksRaycasts = true;
    }
}
