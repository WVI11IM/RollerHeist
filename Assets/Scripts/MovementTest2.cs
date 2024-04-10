using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest2 : MonoBehaviour
{
    [Header("PLAYER PROPERTIES")]
    public float acceleration;
    public float maxMoveSpeed;
    public float rotSpeed;
    public float jumpForce;
    public bool isBraking = false;
    public TrailRenderer trailRenderer;
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
        if (rb.velocity.magnitude >= maxMoveSpeed / 3 * 2) trailRenderer.colorGradient = trailGradients[2];
        else if (rb.velocity.magnitude >= maxMoveSpeed / 3) trailRenderer.colorGradient = trailGradients[1];
        else trailRenderer.colorGradient = trailGradients[0];

        //Faz personagem dar um pulo com barra de espaço
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce * 100);
        }

        //Se jogador possuir velocidade suficiente, pode fazer uma curva brusca clicando duas vezes rapidamente para uma direção
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

        //Variável local de rotação.
        float rotation;

        //Checa se personagem está realizando uma curva brusca. Se não estiver, personagem rotaciona normalmente para as direções A e D.
        if (isDriftingA && isGrounded && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && rb.velocity.magnitude >= maxMoveSpeed / 3)
        {
            rotation = Input.GetAxis("Horizontal") * rotSpeed * 1.75f * Time.deltaTime;
            for (int i = 0; i < driftParticleSystems.Length; i++) driftParticleSystems[i].Play();
        }
        else if (isDriftingD && isGrounded && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && rb.velocity.magnitude >= maxMoveSpeed / 3)
        {
            rotation = Input.GetAxis("Horizontal") * rotSpeed * 1.75f * Time.deltaTime;
            for (int i = 0; i < driftParticleSystems.Length; i++) driftParticleSystems[i].Play();
        }
        else
        {
            isDriftingA = false;
            isDriftingD = false;
            for (int i = 0; i < driftParticleSystems.Length; i++) driftParticleSystems[i].Stop();
            if(!isGrounded) rotation = Input.GetAxis("Horizontal") * rotSpeed / 2 * Time.deltaTime;
            else rotation = Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
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

        //Rotaciona o personagem a partir da variável "rotation".
        transform.eulerAngles += new Vector3(0, rotation, 0);

        //Gera uma Raycast abaixo do personagem que detecta colisão com chão.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistanceToFloor, floorLayerMask))
        {
            isGrounded = true;
            //Evita a nulificação de velocidade ao entrar em contato com o chão
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
            isAirborne = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        direction = transform.TransformDirection(direction);

        //Enquanto personagem estiver abaixo da velocidade máxima e no chão, ele acelera.
        if (rb.velocity.magnitude < maxMoveSpeed && isGrounded)
        {
            rb.AddForce(direction * acceleration);
            if (isDriftingA || isDriftingD) rb.velocity /= 1.015f;
        }

        //Se personagem brecar, é aplicada uma força oposta para desacelerar.
        if (isBraking)
        {
            rb.AddForce(-direction * acceleration);
        }
    }
}
