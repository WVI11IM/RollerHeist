using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest2 : MonoBehaviour
{
    public Animator animator;

    [Header("PLAYER PROPERTIES")]
    public float acceleration;
    public float maxMoveSpeed;
    public float rotSpeed;
    public float jumpForce;
    public bool isBraking = false;
    public TrailRenderer[] trailRenderers;
    public Gradient[] trailGradients;

    [Space]
    [Header("DRIFT SETTINGS")]
    public bool isDriftingA = false;
    public bool isDriftingD = false;
    public ParticleSystem[] driftParticleSystems;

    private float doubleATapTimeThreshold = 0.25f;
    private float lastATapTime;
    private float doubleDTapTimeThreshold = 0.25f;
    private float lastDTapTime;

    [Space]
    [Header("FLOOR DETECTION SETTINGS")]
    public bool isGrounded = false;
    private bool isAirborne = false;
    public float raycastDistanceToFloor = 1.25f;
    public LayerMask floorLayerMask;

    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Muda a cor do rastro dependendo da velocidade.
        if (rb.velocity.magnitude >= maxMoveSpeed / 3 * 2)
        {
            for (int i = 0; i < trailRenderers.Length; i++)
            {
                trailRenderers[i].colorGradient = trailGradients[2];
            }
            animator.SetInteger("speed", 2);
        }
        else if (rb.velocity.magnitude >= maxMoveSpeed / 3)
        {
            for (int i = 0; i < trailRenderers.Length; i++)
            {
                trailRenderers[i].colorGradient = trailGradients[1];
            }
            animator.SetInteger("speed", 1);
        }
        else
        {
            for (int i = 0; i < trailRenderers.Length; i++)
            {
                trailRenderers[i].colorGradient = trailGradients[0];
            }
            if (rb.velocity.magnitude <= 0.05f) animator.SetInteger("speed", 0);
            else animator.SetInteger("speed", 1);
        }

        if (!isGrounded)
        {
            rb.AddForce(-Vector2.up * 9.81f);
        }

        //Faz personagem dar um pulo com barra de espa�o.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce * 100);
            animator.SetBool("isGrounded", false);
        }

        //Se jogador possuir velocidade suficiente, pode fazer uma curva brusca clicando duas vezes rapidamente para uma dire��o.
        if (rb.velocity.magnitude >= maxMoveSpeed / 3 * 2)
        {
            if (Input.GetKeyDown(KeyCode.A) && isGrounded)
            {
                if (Time.time - lastATapTime <= doubleATapTimeThreshold)
                {
                    isDriftingA = true;
                    isDriftingD = false;
                }
                lastATapTime = Time.time;
            }
            if (Input.GetKeyDown(KeyCode.D) && isGrounded)
            {
                if (Time.time - lastDTapTime <= doubleDTapTimeThreshold)
                {
                    isDriftingD = true;
                    isDriftingA = false;
                }
                lastDTapTime = Time.time;
            }
        }

        animator.SetBool("isDriftingD", isDriftingD);
        animator.SetBool("isDriftingA", isDriftingA);

        //Vari�vel local de rota��o.
        float rotation;

        //Checa se personagem est� realizando uma curva brusca. Se n�o estiver, personagem rotaciona normalmente para as dire��es A e D.
        if (isDriftingA && isGrounded && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && rb.velocity.magnitude >= maxMoveSpeed / 5 * 3)
        {
            rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(0f, 2f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
            for (int i = 0; i < driftParticleSystems.Length; i++) driftParticleSystems[i].Play();
        }
        else if (isDriftingD && isGrounded && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && rb.velocity.magnitude >= maxMoveSpeed / 5 * 3)
        {
            rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(0f, 2f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
            for (int i = 0; i < driftParticleSystems.Length; i++) driftParticleSystems[i].Play();
        }
        else
        {
            isDriftingA = false;
            isDriftingD = false;
            for (int i = 0; i < driftParticleSystems.Length; i++) driftParticleSystems[i].Stop();

            //Caso personagem esteja no ar, rota��o ser� reduzida.
            if (!isGrounded) rotation = Input.GetAxis("Horizontal") * rotSpeed / 2 * Mathf.Lerp(2.5f, 0.75f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;

            //Caso personagem esteja no ch�o, rota��o ser� normal, depende do Input Horizontal, que varia de -1 a 1.
            else rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(2.5f, 0.75f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
        }

        //Se teclas A e D estiverem sendo seguradas ao mesmo tempo, personagem para de girar para os lados e desacelera.
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rotation = 0;
            if (isGrounded)
            {
                isBraking = true;
            }
        }
        else
        {
            isBraking = false;
        }

        //Rotaciona o personagem a partir da vari�vel "rotation".
        transform.eulerAngles += new Vector3(0, rotation, 0);

        //Gera uma Raycast abaixo do personagem que detecta colis�o com ch�o.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistanceToFloor, floorLayerMask))
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
            //Evita a nulifica��o de velocidade ao entrar em contato com o ch�o.
            if (isAirborne)
            {
                Vector3 velocity = rb.velocity;
                velocity.y = 0f;
                rb.velocity = velocity;
                isAirborne = false;
            }
        }
        else
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
            isAirborne = true;
        }
    }

    //Para todas as linhas de c�digo que envolvem a constante aplica��o de for�as direcionais ao Rigidbody do personagem, utilizei o FixedUpdate().
    void FixedUpdate()
    {
        Vector3 directionFront = new Vector3(0, 0, 1);
        directionFront = transform.TransformDirection(directionFront);

        Vector3 directionSides = new Vector3(1, 0, 0);
        directionSides = transform.TransformDirection(directionSides);

        //Enquanto personagem estiver abaixo da velocidade m�xima e no ch�o, ele acelera.
        if (rb.velocity.magnitude < maxMoveSpeed && isGrounded)
        {
            rb.AddForce(directionFront * acceleration);

            //Enquanto personagem estiver fazendo a curva brusca, a velocidade dele reduzir� e ser� aplicada uma for�a lateral ao personagem que far� o personagem dar curvas mais acentuadas.
            if (isDriftingA || isDriftingD)
            {
                rb.velocity /= 1.01f;
                if (isDriftingA)
                {
                    rb.AddForce(-directionSides * acceleration);
                }
                else
                {
                    rb.AddForce(directionSides * acceleration);
                }
            }
            //Por�m se a curva for normal, a velocidade dele reduzir�, a for�a lateral ser� menor e tamb�m proporcional � velocidade frontal do personagem.
            else
            {
                if (!isBraking)
                {
                    rb.AddForce(directionSides * Input.GetAxis("Horizontal") * ((acceleration / 3) * (rb.velocity.magnitude / maxMoveSpeed)));
                    if (Input.GetAxis("Horizontal") != 0)
                    {
                        rb.AddForce(-directionFront * acceleration / 10);
                    }
                }
            }
        }

        //Se personagem brecar, � aplicada uma for�a oposta para desacelerar.
        if (isBraking && isGrounded)
        {
            rb.AddForce(-directionFront * acceleration);
        }
    }
}
