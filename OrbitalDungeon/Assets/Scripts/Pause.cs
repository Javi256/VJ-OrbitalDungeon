using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject ObjectPauseMenu;
    public bool pause = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!pause) {
                ObjectPauseMenu.SetActive(true);
                pause = true;
                Time.timeScale = 0;
            }

        else {
                Resumir();
            }
        }
    }

    public void Resumir() {
        Debug.Log("has pulsado");
        ObjectPauseMenu.SetActive(false);
        pause = false;
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
