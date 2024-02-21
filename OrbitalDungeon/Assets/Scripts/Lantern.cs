using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    public int damage;
    public float movingTime;

    private bool moving = false;
    private Transform startPoint;

    public float explosionRange = 5f;
    public ParticleSystem explosionEffect;
    private bool blowing = false;

    private Vector3 target;

    void Update()
    {
        if (!moving && startPoint != null) transform.position = startPoint.position;
    }

    IEnumerator MoveLantern()
    {
        moving = true;

        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        while (Time.time - startTime <= movingTime) // Duración total del movimiento
        {
            float t = (Time.time - startTime) / movingTime; // Normaliza el tiempo entre 0 y 1
            transform.position = Vector3.Slerp(startPosition, target, t);

            yield return null; // Espera hasta el siguiente frame
        }

        yield return new WaitForSeconds(0.25f);
        if (!blowing) StartCoroutine(Blow());
    }

    public void setStartPoint(Transform puntoIni)
    {
        startPoint = puntoIni;
        gameObject.SetActive(true);
    }

    public void ShootLantern(Transform newTarget)
    {
        //Debug.Log("ShootLantern()");
        // Activar la bala
        //gameObject.SetActive(true);

        // Configura el objetivo
        target = newTarget.position;

        StartCoroutine(MoveLantern());
    }

    IEnumerator Blow()
    {
        blowing = true;

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Light>().enabled = false;
        explosionEffect.Play();

        yield return new WaitForSeconds(0.5f);
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
        // Desactiva la bala y restablece el estado de disparo
        gameObject.SetActive(false);
        // Destruye la bala al colisionar con cualquier objeto
        Destroy(gameObject);
        yield return null;
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar colisión con otros objetos y realizar las acciones necesarias
        if (other.CompareTag("Object") || other.CompareTag("Player"))
        {
            //Debug.Log("LANTERN: Colisión Obstaculo detectada");
            if (!blowing) StartCoroutine(Blow());
        }
    }

    public void DisableLantern()
    {
        // Desactivar la bala y restablecer el estado de disparo
        gameObject.SetActive(false);
        // Destruir la bala al colisionar con cualquier objeto
        Destroy(gameObject);
    }
}
