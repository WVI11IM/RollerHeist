using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest2 : MonoBehaviour
{
    [Header("PLAYER PROPERTIES")]
    public float acceleration;
    public float maxMoveSpeed;
    public float rotSpeed;
    public bool isGrounded = false;
    public bool isBraking = false;
    public TrailRenderer trailRenderer;
    public Gradient[] gradients;

    [Space]
    [Header("DRIFT SETTINGS")]
    public bool isDriftingA = false;
    public bool isDriftingD = false;
    public ParticleSystem driftSparks;

    private float doubleATapTimeThreshold = 0.25f;
    private float lastATapTime;
    private float doubleDTapTimeThreshold = 0.25f;
    private float lastDTapTime;

    [Space]
    [Header("FLOOR DETECTION SETTINGS")]
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
        if (rb.velocity.magnitude >= maxMoveSpeed / 3 * 2) trailRenderer.colorGradient = gradients[2];
        else if (rb.velocity.magnitude >= maxMoveSpeed / 3) trailRenderer.colorGradient = gradients[1];
        else trailRenderer.colorGradient = gradients[0];

        //Se jogador possuir velocidade suficiente, pode fazer uma curva brusca clicando duas vezes rapidamente para uma dire��o
        if(rb.velocity.magnitude >= maxMoveSpeed / 3 * 2)
        {
            if (Input.GetKeyDown(KeyCode.A) && isGrounded)
            {
                if (Time.time - lastATapTime <= doubleATapTimeThreshold)
                {
                    isDriftingA = true;
                    isDriftingD = false;
                    rb.velocity /= 1.25f;
                }
                lastATapTime = Time.time;
            }
            if (Input.GetKeyDown(KeyCode.D) && isGrounded)
            {
                if (Time.time - lastDTapTime <= doubleDTapTimeThreshold)
                {
                    isDriftingD = true;
                    isDriftingA = false;
                    rb.velocity /= 1.25f;
                }
                lastDTapTime = Time.time;
            }
        }

        //Vari�vel local de rota��o.
        float rotation;

        //Checa se personagem est� realizando uma curva brusca. Se n�o estiver, personagem rotaciona normalmente para as dire��es A e D.
        if (isDriftingA && isGrounded && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && rb.velocity.magnitude > maxMoveSpeed / 3 * 2)
        {
            rotation = Input.GetAxis("Horizontal") * rotSpeed * 3f * Time.deltaTime;
            driftSparks.Play();
        }
        else if (isDriftingD && isGrounded && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && rb.velocity.magnitude > maxMoveSpeed / 3 * 2)
        {
            rotation = Input.GetAxis("Horizontal") * rotSpeed * 3f * Time.deltaTime;
            driftSparks.Play();
        }
        else
        {
            isDriftingA = false;
            isDriftingD = false;
            driftSparks.Stop();
            rotation = Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        }

        //Se teclas A e D estiverem sendo seguradas ao mesmo tempo, personagem para de girar para os lados e d� r�.
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rotation = 0;
            if (isGrounded) isBraking = true;
        }
        else isBraking = false;

        transform.eulerAngles += new Vector3(0, rotation, 0);

        //Gera uma Raycast abaixo do personagem que detecta colis�o com ch�o.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistanceToFloor, floorLayerMask))
        {
            isGrounded = true;
        }
        else isGrounded = false;
    }

    void FixedUpdate()
    {
        Vector3 direction = new Vector3(0, 0, 1);
        direction = transform.TransformDirection(direction);

        //Enquanto personagem estiver abaixo da velocidade m�xima e no ch�o, ele acelera.
        if (rb.velocity.magnitude < maxMoveSpeed && isGrounded && !isBraking)
        {
            rb.AddForce(direction * acceleration);
            Debug.Log("Applying force");
        }

        if (isBraking) rb.AddForce(-direction * acceleration);
    }
}
