using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    public float speed; // Velocidad de la bala
    private Transform startPoint; // Punto de inicio de la bala
    public GameObject center;

    private float time = 2f;
    public float lifeTime;
    public int damage;

    private bool moving = false;

    void Start()
    {
        time = lifeTime;
    }

    // Método que se llama cuando se presiona la tecla "G"
    public void ThrowShovel(bool direction)
    {
        Debug.Log("ThrowShovel");
        // Activar la bala
        gameObject.SetActive(true);

        moving = true;

        // Iniciar la corutina para mover la bala
        if (direction) StartCoroutine(MoveShovelRight());
        else StartCoroutine(MoveShovelLeft());
    }

    // Corutina para mover la bala
    IEnumerator MoveShovelRight()
    {
        while (moving)
        {
            //Gira hacia la izquierda, para cambiar a la derecha poner la speed a negativo
            transform.RotateAround(center.transform.position, new(0, 1, 0), -speed * Time.deltaTime);

            if (time <= 0) DisableShovel();

            time -= Time.deltaTime;

            yield return null;
        }

        transform.position = startPoint.position;
    }

    // Corutina para mover la bala
    IEnumerator MoveShovelLeft()
    {
        while (moving)
        {
            //Gira hacia la izquierda, para cambiar a la derecha poner la speed a negativo
            transform.RotateAround(center.transform.position, new(0, 1, 0), speed * Time.deltaTime);

            if (time <= 0) DisableShovel();

            time -= Time.deltaTime;

            yield return null;
        }

        transform.position = startPoint.position;
    }


    // Método llamado cuando la bala entra en un trigger
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Tipo objeto: " + other.tag);
        //Debug.Log("BalaColisionaObjeto");

        // Verificar colisión con otros objetos y realizar las acciones necesarias
        if (other.CompareTag("Object") || other.CompareTag("Player"))
        {
            // Desactivar la bala al colisionar con el objeto destino
            DisableShovel();
        }
    }

    // Método para desactivar la bala
    void DisableShovel()
    {
        // Desactivar la bala y restablecer el estado de disparo
        moving = false;
        gameObject.SetActive(false);

        // Destruir la bala al colisionar con cualquier objeto
        Destroy(gameObject);
    }
}
