using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int maxHealth;
    private int health;

    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(Shake());
        if (health <= 0) Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private IEnumerator Shake()
    {
        Vector3 posicionInicial = transform.position;
        float tiempoPasado = 0f;

        while (tiempoPasado < shakeDuration)
        {
            float factor = Mathf.Sin(tiempoPasado / shakeDuration * Mathf.PI);
            transform.position = posicionInicial + Random.insideUnitSphere * shakeIntensity * factor;

            tiempoPasado += Time.deltaTime;
            yield return null;
        }

        // Restaurar la posición original
        transform.position = posicionInicial;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            // Acceder al componente Bullet de la bala que ha colisionado
            Bullet bullet = other.GetComponent<Bullet>();

            // Verificar si el componente Bala existe en el objeto colisionado
            if (bullet != null)
            {
                // Obtener el daño de la bala y aplicarlo a la función TakeDamage
                TakeDamage(bullet.damage);
            }
        }
    }
}
