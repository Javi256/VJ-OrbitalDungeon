using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextFloor : MonoBehaviour
{

    public GameObject tape; // Asigna el objeto en el inspector de Unity
    public float height;
    public float movespeed;
    public float z;
    public string type;
    public GameObject floatingText;
    private AudioSource audioSource;
    public AudioClip nextfloor;

    public GameObject[] enemies;
    public GameObject[] enemiesNext;
    public bool enemiesDefeated = false;
    public GameObject nextplatform;

    // Start is called before the first frame update
    void Start()
    {
        floatingText.SetActive(false);
        audioSource = GetComponent<AudioSource>();

        gameObject.GetComponent<MeshRenderer>().enabled = false;

        for (int i = 0; i < enemiesNext.Length; i++)
        {
            enemiesNext[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool alive = true;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null) alive = false;
        }
        enemiesDefeated = alive;

        if (enemiesDefeated && !gameObject.GetComponent<MeshRenderer>().enabled)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("Player"))
        {
            if (enemiesDefeated) floatingText.SetActive(true);
        }
    }

    void OnTriggerStay(Collider obj) {       
        if (Input.GetKey(KeyCode.J) && enemiesDefeated) {
            StartCoroutine(JumpAndActivate(obj.GetComponent<MovePlayer>()));
            audioSource.clip = nextfloor;
            audioSource.Play();
            if (nextplatform != null) nextplatform.SetActive(true);
        }


    }


    IEnumerator JumpAndActivate(MovePlayer movePlayer)
    {
        tape.SetActive(false);
        if (type == "normal") {
            movePlayer.changePosition(height, z);
            tape.SetActive(true);
        }
        else if (type == "elevator") {
            //esperamos primer mov (hacia dentro ascensor)
            movePlayer.jumpFloor(height, movespeed, z, type);
            yield return new WaitForSeconds(3);
            //pasamos a mov normal (hacia arriba)
            type = "normal2";
            movespeed = 1f;
            movePlayer.jumpFloor(height, movespeed, z, type);
            yield return new WaitForSeconds(5);
            //mov hacia fuera
            type = "elevator2";

            movespeed = 3f;

            movePlayer.jumpFloor(height, movespeed, z, type);
            // yield return new WaitForSeconds(2);
        }

        else if (type == "elevator3") {
            Debug.Log("Objeto ha llegado al punto final.");

            movePlayer.jumpFloor(height, movespeed, z, type);
            yield return new WaitForSeconds(3);
        }
    
    }
        
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floatingText.SetActive(false);
        }
    }

}
