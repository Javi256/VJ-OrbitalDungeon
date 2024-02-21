using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire : MonoBehaviour
{
    //Vida enemigo
    public int maxHealth;
    private int health;

    public int maxShield;
    private int shield;

    public float speed; // Velocidad de la bala
    public GameObject center;
    private bool moving = false;
    private bool dying = false;

    public float shootingTime;
    private float time;

    public GameObject CrossObject;
    private Cross scriptCross;
    public Transform startPoint;

    Animator vampireAnimator;
    private bool holding = false;

    //Barra de vida;
    public GameObject HealthBar;
    private HealthBar scriptHealthBar;

    public GameObject ShieldBar;
    private HealthBar scriptShieldBar;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        shield = maxShield;

        scriptHealthBar = HealthBar.GetComponent<HealthBar>();
        scriptHealthBar.setMaxHealth(maxHealth);
        HealthBar.SetActive(false);

        scriptShieldBar = ShieldBar.GetComponent<HealthBar>();
        scriptShieldBar.setMaxHealth(maxShield);

        vampireAnimator = GetComponent<Animator>();

        vampireAnimator.SetBool("Die", false);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (!moving)
        {
            moving = true;
            StartCoroutine(MoveVampire());
        }
    }

    //RECIBIR DA�O Y MORIR
    public void TakeDamage(int damage)
    {
        shield -= damage;
        if (shield > 0)
        {
            //Debug.Log("Shield");
            scriptShieldBar.SetHealth(shield);
        }
        else if (shield <= 0 && ShieldBar.activeSelf)
        {
            HealthBar.SetActive(true);
            ShieldBar.SetActive(false);
        }
        else if (shield <= 0)
        {
            //Debug.Log("Health");
            health -= damage;
            scriptHealthBar.SetHealth(health);
        }
        if (health <= 0) Die();
    }

    private void Die()
    {
        dying = true;
        moving = false;
        if (scriptCross != null)
        {
            scriptCross.DisableCross();
        }

        HealthBar.SetActive(false);
        vampireAnimator.SetBool("Die", true);

        StartCoroutine(WaitForAnimationToEnd());
        //gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        // Espera hasta que la animaci�n de "Die" haya terminado
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    IEnumerator MoveVampire()
    {
        while (moving && !dying)
        {
            //Gira hacia la izquierda, para cambiar a la derecha poner la speed a negativo
            transform.RotateAround(center.transform.position, new(0, 1, 0), speed * Time.deltaTime);

            if (!holding)
            {
                //Debug.Log("CREAR CRUZ");
                holding = true;
                time = 0;
                // Instanciar una nueva bala en el punto de disparo
                GameObject newCross = Instantiate(CrossObject, startPoint.position, startPoint.rotation);

                // Obtener el componente Bullet de la nueva bala
                scriptCross = newCross.GetComponent<Cross>();
                scriptCross.setStartPoint(startPoint);
            }

            if (time >= shootingTime) ShootCross();

            yield return null;
        }

        //transform.position = startPoint.position;
    }

    void ShootCross()
    {
        //Debug.Log("DISPARAR CRUZ");
        time = 0;
        // Verificar si el componente existe antes de activar la bala
        if (scriptCross != null)
        {
            // Activar la bala
            scriptCross.ShootCross();
        }

        holding = false;
    }

    //TRIGGERS Y COLISIONES//
    void OnTriggerEnter(Collider other)
    {
        // Verificar colisi�n con otros objetos y realizar las acciones necesarias
        if (other.CompareTag("Object") || other.CompareTag("Player"))
        {
            speed *= -1;
            transform.Rotate(0f, 180f, 0f);
        }

        if (other.CompareTag("Bullet"))
        {
            Debug.Log("VAMPIRE: Bala detectada");
            // Acceder al componente BulletC de la bala que ha colisionado
            Bullet bullet = other.GetComponent<Bullet>();

            // Verificar si el componente Bala existe en el objeto colisionado
            if (bullet != null)
            {
                // Obtener el da�o de la bala y aplicarlo a la funci�n TakeDamage
                TakeDamage(bullet.damage);
            }
        }
    }
}
