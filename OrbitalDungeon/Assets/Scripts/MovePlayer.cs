using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MovePlayer : MonoBehaviour
{
    //Health
    public int maxHealth;
    public int health;

    public float rotationSpeed, jumpSpeed, gravity;
    Vector3 startDirection;
    float speedY;
    private float speed;

    private Animator anim;

    private bool right;

    Transform startPoint; // Punto inicial
    Transform endPoint;   // Punto final
    private float startTime;     // Tiempo inicial
    private float journeyLength; // Longitud del recorrido
    bool jump;
    public FollowPlayer cameraScript; // Referencia a la cámara

    //GunSystem
    private int gunId;
    public GameObject GunSystem;
    private GunSystem scriptGunSystem;

    //Barra de vida;
    public GameObject HealthBar;
    private PlayerHealthBar scriptHealthBar;

    private bool godMode = false;

    //Dodge
    private int originalLayer;
    private int dodgeLayer;
    public float dodgeTime;
    public float dodgeWaitTime;
    private bool isDodging = false;

    //Trap
    private bool onTrap = false;
    public int graveTrapDamage;
    public int graveTrapSpeed;

    private bool dead = false;

    public GameObject dieCanvas;

    private AudioSource audioSource;
    public AudioClip walkSound; // Asigna el AudioClip para caminar desde el Inspector
    public AudioClip dieSound;

    private AudioSource sonidoPadre;

    public GameObject floatingTextObject;
    private TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        dieCanvas.SetActive(false);
        sonidoPadre = transform.parent.GetComponent<AudioSource>();

        audioSource = GetComponent<AudioSource>();

        originalLayer = gameObject.layer;
        dodgeLayer = 8; //Dodge Layer

        scriptGunSystem = GunSystem.GetComponent<GunSystem>();
        gunId = 0;

        jump =false;
        anim = GetComponent<Animator>(); //componente de animacion
        anim.SetBool("Walk", false);

        startDirection = transform.position - transform.parent.position;
        startDirection.y = 0.0f;
        startDirection.Normalize();

        speedY = 0;
        speed = rotationSpeed;

        right = true;

        health = maxHealth;
        scriptHealthBar = HealthBar.GetComponent<PlayerHealthBar>();
        scriptHealthBar.setMaxHealth(maxHealth);

        textMesh = floatingTextObject.GetComponent<TextMeshProUGUI>();
        floatingTextObject.SetActive(false);
    }

    //RECIBIR DA�O Y MORIR
    public void TakeDamage(int damage)
    {
        if (!godMode)
        {
            health -= damage;
            scriptHealthBar.SetHealth(health);
        }
        //if (health <= 0) Die();
    }

    public bool FacingRight() {
        return right;
    }
    public void GainHealth(int healthAmount)
    {
        Debug.Log("GainHealth");
        StartCoroutine(ShowFloatingText("+" + healthAmount.ToString() + " Health"));
        health += healthAmount;
        if (health > maxHealth) health = maxHealth;
        scriptHealthBar.SetHealth(health);
    }

    public void Jump()
    {
        speedY = jumpSpeed;
    }

    void Update()
    {
        if (!jump) {
             if (Input.GetKeyDown(KeyCode.P))
            {
                if (gunId == 0)
                {
                    if (scriptGunSystem.selectGun(1, right)) gunId = 1;
                }
                else if (gunId == 1)
                {
                    if (scriptGunSystem.selectGun(0, right)) gunId = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (godMode) godMode = false;
                else godMode = true;
            }

            if (Input.GetKey(KeyCode.T))
            {
                if (!isDodging) StartCoroutine(DoDodge());
            }
            
            if (health < 0 && !dead) {
                Debug.Log("muertoooo");
                dead = true;
                jump=true;
                Die();
                //audioSource.Stop();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CharacterController charControl = GetComponent<CharacterController>();
        Vector3 position;

        if (!jump) {

            // Left-right movement
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (!audioSource.isPlaying) {
                    audioSource.clip = walkSound;
                    audioSource.Play();
                }
             
                anim.SetBool("Walk", true);
                float angle;
                Vector3 direction, target;

                position = transform.position;
                angle = speed * Time.deltaTime;
                direction = position - transform.parent.position;
                if (Input.GetKey(KeyCode.A))
                {
                    //if (right) floatingTextObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

                    right = false; 
                    target = transform.parent.position + Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    if (charControl.Move(target - position) != CollisionFlags.None)
                    {
                        transform.position = position;
                        Physics.SyncTransforms();
                    }
                }
                if (Input.GetKey(KeyCode.D))
                {
                    //if (!right) floatingTextObject.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

                    right = true;

                    target = transform.parent.position + Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    if (charControl.Move(target - position) != CollisionFlags.None)
                    {
                        transform.position = position;
                        Physics.SyncTransforms();
                    }
                }
            }

            else {
                audioSource.Stop();
                anim.SetBool("Walk", false);

            }

            // Correct orientation of player
            // Compute current direction
            Vector3 currentDirection = transform.position - transform.parent.position;
            currentDirection.y = 0.0f;
            currentDirection.Normalize();
            // Change orientation of player accordingly
            Quaternion orientation;
            if (!right)
            {
                if ((startDirection - currentDirection).magnitude < 1e-3)
                    orientation = Quaternion.AngleAxis(0.0f, Vector3.up);
                else if ((startDirection + currentDirection).magnitude < 1e-3)
                    orientation = Quaternion.AngleAxis(180.0f, Vector3.up);
                else
                    orientation = Quaternion.FromToRotation(startDirection, currentDirection);
            }
            else
            {
                if ((startDirection - currentDirection).magnitude < 1e-3)
                    orientation = Quaternion.AngleAxis(180.0f, Vector3.up);
                else if ((startDirection + currentDirection).magnitude < 1e-3)
                    orientation = Quaternion.AngleAxis(0.0f, Vector3.up);
                else
                    orientation = Quaternion.FromToRotation(startDirection * -1.0f, currentDirection);
            }
            transform.rotation = orientation;

            // Apply up-down movement
            position = transform.position;
            if (charControl.Move(speedY * Time.deltaTime * Vector3.up) != CollisionFlags.None)
            {
                transform.position = position;
                Physics.SyncTransforms();
            }
            if (charControl.isGrounded)
            {
                if (speedY < 0.0f)
                    speedY = 0.0f;
                if (Input.GetKey(KeyCode.W))
                    speedY = jumpSpeed;
            }
            else
                speedY -= gravity * Time.deltaTime;
        }
    }

    public void changePosition(float height,float z) {
            jump = true;
            startTime = Time.time;
            Vector3 va = transform.position;
            transform.position = new Vector3(va.x, va.y+height, va.z + z);
            jump = false;
    }
    
    public void UnlockGun(int id)
    {
        scriptGunSystem.UnlockGun(id);
        if (scriptGunSystem.selectGun(id, right)) gunId = id;

        StartCoroutine(ShowFloatingText("New weapon unlocked! Press P to change"));
    }

    public void addBullets(int id, int num)
    {
        scriptGunSystem.addBullets(id, num);

        if (id == 0) StartCoroutine(ShowFloatingText("+" + num.ToString() + " Short Range Bullets"));
        else if (id == 1) StartCoroutine(ShowFloatingText("+" + num.ToString() + " Long Range Bullets"));
    }

    private IEnumerator DoDodge()
    {
        isDodging = true;
        godMode = true;
        gameObject.layer = dodgeLayer;
        speed = rotationSpeed + 100;

        yield return new WaitForSeconds(dodgeTime);

        gameObject.layer = originalLayer;
        speed = rotationSpeed;

        yield return new WaitForSeconds(dodgeWaitTime);
        isDodging = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cross"))
        {
            //Debug.Log("GHOST: Bala detectada");
            // Acceder al componente BulletC de la bala que ha colisionado
            Cross cross = other.GetComponent<Cross>();

            // Verificar si el componente Bala existe en el objeto colisionado
            if (cross != null)
            {
                // Obtener el da�o de la bala y aplicarlo a la funci�n TakeDamage
                TakeDamage(cross.damage);
            }
        }

        if (other.CompareTag("Shovel"))
        {
            //Debug.Log("SHOVEL");
            // Acceder al componente BulletC de la bala que ha colisionado
            Shovel shovel = other.GetComponent<Shovel>();

            // Verificar si el componente Bala existe en el objeto colisionado
            if (shovel != null)
            {
                // Obtener el da�o de la bala y aplicarlo a la funci�n TakeDamage
                TakeDamage(shovel.damage);
            }
        }

        if (other.CompareTag("Grave"))
        {
            speed = graveTrapSpeed;
            StartCoroutine(DamageOnTrap());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grave"))
        {
            onTrap = false;
            speed = rotationSpeed;
        }
    }

    private IEnumerator DamageOnTrap()
    {
        onTrap = true;
        while (onTrap)
        {
            TakeDamage(graveTrapDamage);
            yield return new WaitForSeconds(1f);
        }
    }

        //SALTAR A LA SGUIENTE PLANTA
    public void jumpFloor(float height, float movespeed, float z, string type) {
        jump = true;
        startTime = Time.time;
        Vector3 va = transform.position;
        if (type == "normal" || type=="normal2") {
            //if (type == "normal2") descativar arma para q no sobre salga del ascensor
            anim.SetBool("Walk", false);
            Vector3 vb = new Vector3(va.x, va.y + height, va.z + z);
            journeyLength = Vector3.Distance(va, vb);
            StartCoroutine(MoveObject(va, vb, movespeed, type));
        }
        else if (type == "elevator") {
            Quaternion targetRotation = Quaternion.Euler(0, 72, 0);
            transform.rotation = targetRotation;
            anim.SetBool("Walk", true);

            Vector3 vb = new Vector3(-1.858f,6.704483f,-0.883f);

            journeyLength = Vector3.Distance(va, vb);
            StartCoroutine(MoveObject(va, vb, movespeed, type));
        }
        else if (type == "elevator2") {
            Quaternion targetRotation = Quaternion.Euler(0, -89, 0);
            transform.rotation = targetRotation;
            anim.SetBool("Walk", true);
            Vector3 vb = new Vector3(-6f,11.39545f,-3.131f);
            journeyLength = Vector3.Distance(va, vb);
            StartCoroutine(MoveObject(va, vb, movespeed, type));
        }
        else if (type == "elevator3") {
            Quaternion targetRotation = Quaternion.Euler(0, 54, 0);
            transform.rotation = targetRotation;
            anim.SetBool("Walk", true);

            Vector3 vb = new Vector3(-1.24f,va.y,-4.31f);

            journeyLength = Vector3.Distance(va, vb);
            StartCoroutine(MoveObject(va, vb, movespeed, type));
        }
    }

    IEnumerator MoveObject(Vector3 start, Vector3 end, float movespeed, string type) {
        while (Vector3.Distance(transform.position, end) > 0.01f) {
            float distCovered = (Time.time - startTime) * movespeed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(start, end, fracJourney);
            cameraScript.Update();
            yield return null;
        }
        //  Debug.Log("Objeto ha llegado al punto final.");
        if (type == "normal") {
            jump = false;
            speedY = 0f; 
            speedY -= gravity * Time.deltaTime;
        }

        else if (type == "elevator") {
            anim.SetBool("Walk", false);
            Quaternion targetRotation = Quaternion.Euler(0, 250, 0);
            transform.rotation = targetRotation;

        }

        else if (type == "elevator2") {
            anim.SetBool("Walk", false);
            jump = false;
        }

        else if (type == "elevator3") {
            anim.SetBool("Walk", false);
            jump = false;
        }
    }
    private void Die() {
        audioSource.clip = dieSound;
        audioSource.Play();
        anim.SetBool("Die", true);
        StartCoroutine(ShowDeathCanvasAfterAnimation());
    }   
    private IEnumerator ShowDeathCanvasAfterAnimation()
    {
        yield return new WaitForSeconds(3);

        if (dieCanvas != null)
        {
            sonidoPadre.Stop();
            Time.timeScale = 0;
            dieCanvas.SetActive(true);
            
        }
    }

    private IEnumerator ShowFloatingText(string text)
    {
        floatingTextObject.SetActive(true);

        textMesh.text = text;

        float elapsedTime = 0f;
        float duration = 1.5f;
        Vector3 startPos = floatingTextObject.transform.position;
        Vector3 endPos = new Vector3(startPos.x, startPos.y + 50f, startPos.z); // Cambia el valor vertical según tu preferencia

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            floatingTextObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            yield return null;
        }
        floatingTextObject.transform.position = startPos;
        floatingTextObject.SetActive(false);
    }
}
