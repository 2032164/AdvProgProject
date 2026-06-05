using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string SceneName;

    public void Play()
    {
        SceneManager.LoadScene(SceneName);
    } 
    public void Exit()
    {
        Application.Quit();
    }
}
