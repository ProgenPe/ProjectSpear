using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    [SerializeField] private GameObject panel;
    private bool isPauseMenuOpen = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu() 
    { 
        isPauseMenuOpen=!isPauseMenuOpen;
        if (isPauseMenuOpen)
        {
            panel.SetActive(false);
            Time.timeScale = 1;
        }
        else 
        { 
            panel.SetActive(true);
            Time.timeScale = 0;
        }
    }


}
