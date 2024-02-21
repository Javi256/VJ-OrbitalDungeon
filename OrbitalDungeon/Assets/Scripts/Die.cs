using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Die : MonoBehaviour
{
    public GameObject ObjectPauseMenu;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Restart() {
        Debug.Log("vuelve a empezar");
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }

    public void Menu(string namemenu) {
        SceneManager.LoadScene(namemenu);
        Time.timeScale = 1;
    }

    public void Exit() {
        Application.Quit();
    }
}

