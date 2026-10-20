using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void IniciaJogo()
    {
        AudioManager.Play("click");
        GameController.Init();
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void Menu()
    {
        AudioManager.Play("click");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Sair()
    {
        Application.Quit();
    }
}
