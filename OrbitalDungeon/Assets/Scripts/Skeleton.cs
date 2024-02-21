using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    //Vida enemigo
    public int maxHealth;
    private int health;

    public int maxShield;
    private int shield;

    public Transform pointA;
    public Transform pointB;
    public float jumpSpeed;
    public float attackRange;

    public float shootingWait;

    private bool isJumping = false;
    private Vector3 actualPosition;
    //private float initialY;
    private bool posA;

    public GameObject ShovelObject;
    public Transform startPointA;
    public Transform startPointB;

    //Barra de vida;
    public GameObject HealthBar;
    private HealthBar scriptHealthBar;

    public GameObject ShieldBar;
    private HealthBar scriptShieldBar;
    public AudioSource audioSource;
    public GameObject winCanvas;
    Animator skeletonAnimator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        winCanvas.SetActive(false);
        health = maxHealth;
        shield = maxShield;

        transform.position = pointA.position;
        actualPosition = transform.position;

        scriptHealthBar = HealthBar.GetComponent<HealthBar>();
        scriptHealthBar.setMaxHealth(maxHealth);
        HealthBar.SetActive(false);

        scriptShieldBar = ShieldBar.GetComponent<HealthBar>();
        scriptShieldBar.setMaxHealth(maxShield);

        skeletonAnimator = GetComponent<Animator>();
        skeletonAnimator.SetBool("Die", false);
        skeletonAnimator.SetBool("Left", false);
        skeletonAnimator.SetBool("Right", false);
        skeletonAnimator.SetBool("Both", false);
        skeletonAnimator.SetBool("Idle", true);

        StartCoroutine(LaunchProjectilesCoroutine());
    }

    void Update()
    {
        if (!isJumping)
        {
            CheckPlayerInRange();
        }
    }

    //RECIBIR DA�O Y MORIR
    public void TakeDamage(int damage)
    {
        shield -= damage;
        if (shield > 0)
        {
            Debug.Log("Shield");
            scriptShieldBar.SetHealth(shield);
        }
        else if (shield <= 0 && ShieldBar.activeSelf)
        {
            HealthBar.SetActive(true);
            ShieldBar.SetActive(false);
        }
        else if (shield <= 0)
        {
            Debug.Log("Health");
            health -= damage;
            scriptHealthBar.SetHealth(health);
            if (health <= 0) Die();
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        HealthBar.SetActive(false);


        //moving = false;
        skeletonAnimator.SetBool("Die", true);

        StartCoroutine(WaitForAnimationToEnd());
        //gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        // Espera hasta que la animaci�n de "Die" haya terminado
        winCanvas.SetActive(true);
        audioSource.Play();
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void CheckPlayerInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                if (!isJumping) JumpBetweenPoints();
            }
        }
    }

    void JumpBetweenPoints()
    {
        StopAllCoroutines();

        skeletonAnimator.SetBool("Left", false);
        skeletonAnimator.SetBool("Right", false);
        skeletonAnimator.SetBool("Both", false);
        skeletonAnimator.SetBool("Idle", true);

        isJumping = true;

        // Calcular la nueva posici�n en base al punto opuesto actual
        Vector3 targetPosition;
        if (actualPosition == pointA.position)
        {
            targetPosition = pointB.position;
            //posA = false;
        }
        else
        {
            targetPosition = pointA.position;
            //posA = true;
        }

        // Iniciar la corrutina para el salto
        StartCoroutine(JumpCoroutine(targetPosition));
    }

    IEnumerator LaunchProjectilesCoroutine()
    {
        int shovel;
        int numShovel;

        skeletonAnimator.SetBool("Idle", false);

        while (true)
        {
            shovel = 0;
            numShovel = Mathf.RoundToInt(Random.Range(4, 8));
            skeletonAnimator.SetBool("Both", false);
            skeletonAnimator.SetBool("Left", true);

            while (shovel < numShovel)
            {
                GameObject newShovelA = Instantiate(ShovelObject, startPointA.position, startPointA.rotation);
                Shovel scriptShovelA = newShovelA.GetComponent<Shovel>();

                if (scriptShovelA != null)
                {
                    //Debug.Log("scriptShovelA");
                    scriptShovelA.ThrowShovel(false);
                }

                ++shovel;
                yield return new WaitForSeconds(shootingWait);
            }

            shovel = 0;
            numShovel = Mathf.RoundToInt(Random.Range(4, 8));
            skeletonAnimator.SetBool("Left", false);
            skeletonAnimator.SetBool("Right", true);

            while (shovel < numShovel)
            {
                GameObject newShovelB = Instantiate(ShovelObject, startPointB.position, startPointB.rotation);
                Shovel scriptShovelB = newShovelB.GetComponent<Shovel>();

                if (scriptShovelB != null)
                {
                    scriptShovelB.ThrowShovel(true);
                }

                ++shovel;
                yield return new WaitForSeconds(shootingWait);
            }

            shovel = 0;
            numShovel = Mathf.RoundToInt(Random.Range(8, 12));
            skeletonAnimator.SetBool("Right", false);
            skeletonAnimator.SetBool("Both", true);

            while (shovel < numShovel)
            {
                GameObject newShovelA = Instantiate(ShovelObject, startPointA.position, startPointA.rotation);
                Shovel scriptShovelA = newShovelA.GetComponent<Shovel>();

                if (scriptShovelA != null)
                {
                    //Debug.Log("scriptShovelA");
                    scriptShovelA.ThrowShovel(false);
                }

                GameObject newShovelB = Instantiate(ShovelObject, startPointB.position, startPointB.rotation);
                Shovel scriptShovelB = newShovelB.GetComponent<Shovel>();

                if (scriptShovelB != null)
                {
                    scriptShovelB.ThrowShovel(true);
                }

                ++shovel;
                yield return new WaitForSeconds(shootingWait);
            }
        }
    }

    IEnumerator JumpCoroutine(Vector3 targetPosition)
    {
        Debug.Log("saltar");
        float startTime = Time.time;
        //Vector3 startPosition = transform.position;

        while (Time.time - startTime <= jumpSpeed)
        {
            float t = (Time.time - startTime) / jumpSpeed;
            transform.position = Vector3.Slerp(actualPosition, targetPosition, t);

            yield return null;
        }

        // Restablecer el estado y reiniciar la corrutina de lanzamiento de proyectiles
        transform.Rotate(0f, 180f, 0f);
        isJumping = false;
        actualPosition = targetPosition;
        StartCoroutine(LaunchProjectilesCoroutine());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            //Debug.Log("GHOST: Bala detectada");
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
