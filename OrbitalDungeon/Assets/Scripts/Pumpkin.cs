using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    public int damage;
    public float explosionRange;
    public ParticleSystem explosionEffect;

    public float explosionTime;
    private float time;

    public void Plant()
    {
        gameObject.SetActive(true);
        StartCoroutine(PlantPumpinkg());
    }

    IEnumerator PlantPumpinkg()
    {
        time = 0;
        while (time < explosionTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Blow());
    }

    IEnumerator Blow()
    {
        //Debug.Log("PlayEffect");
        // Encuentra los objetos en un radio alrededor del lugar de la explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);

        // Aplica daño a los objetos encontrados
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<MovePlayer>().TakeDamage(damage);
            }
        }

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        explosionEffect.Play();
        yield return new WaitForSeconds(0.5f);
        // Desactiva la bala y restablece el estado de disparo
        gameObject.SetActive(false);
        // Destruye la bala al colisionar con cualquier objeto
        Destroy(gameObject);
        yield return null;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colision: " + other.tag);
        // Verificar colisión con otros objetos y realizar las acciones necesarias
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Blow());
        }
    }

}

