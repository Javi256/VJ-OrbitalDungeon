using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    public int maxCapacity;
    private int numBullets;

    public GameObject Bullet;
    public Transform startPoint;
    private bool direction;

    public GameObject floatingTextObject;
    private TextMeshProUGUI textMesh;

    public float bulletSpeed;
    public float bulletLifeTime;
    public int bulletDamage;

    public bool available = false;

    public string trailColor;

    private AudioSource audioSource;
    public AudioClip shoot;

    private MovePlayer movePlayer; // Referencia al script MovePlayer

    public void Start()
    {
        movePlayer = FindObjectOfType<MovePlayer>();
        direction = movePlayer.FacingRight();
        numBullets = maxCapacity;
        audioSource = GetComponent<AudioSource>();


        textMesh = floatingTextObject.GetComponent<TextMeshProUGUI>();
        textMesh.text = numBullets.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            direction = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction = false;
        }
        // Verificar si se presiona la tecla "G"
        if (Input.GetKeyDown(KeyCode.K))
        {
            audioSource.clip = shoot;
            audioSource.Play();
            //Debug.Log("G apretada");
            if (numBullets > 0)
            {
                // Instanciar una nueva bala en el punto de disparo
                GameObject newBullet = Instantiate(Bullet, startPoint.position, startPoint.rotation);

                // Obtener el componente Bullet de la nueva bala
                Bullet scriptBullet = newBullet.GetComponent<Bullet>();

                // Verificar si el componente existe antes de activar la bala
                if (scriptBullet != null)
                {
                    // Activar la bala
                    scriptBullet.SetDamage(bulletDamage);
                    scriptBullet.SetSpeed(bulletSpeed);
                    scriptBullet.SetLifeTime(bulletLifeTime);
                    scriptBullet.SetTrail(trailColor);

                    scriptBullet.ShootBullet(direction);
                }
                --numBullets;
                textMesh.text = numBullets.ToString();
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            numBullets = maxCapacity;
            textMesh.text = numBullets.ToString();
        }
    }

    public void addBullets(int num)
    {
        numBullets += num;
        if (numBullets > maxCapacity) numBullets = maxCapacity;
        textMesh.text = numBullets.ToString();
    }

    public void setStartDirection(bool d)
    {
        direction = d;
    }

    public void setAvailable()
    {
        available = true;
        //GunDisplayObject.SetActive(true);
    }
}
