using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance = null;

    [SerializeField] private GameObject roomCanvas;
    [SerializeField] private GameObject archiveSystem;

    public void Awake()
    {
        if(Instance == null)
        Instance = this;   
    }

    public void OpenMenu()
    {
        roomCanvas.SetActive(false);
        archiveSystem.SetActive(false);
    }

    public void BackToGame()
    {
        roomCanvas.SetActive(true);
        archiveSystem.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
