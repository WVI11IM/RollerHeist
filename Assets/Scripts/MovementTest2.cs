using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class MovementTest2 : MonoBehaviour
{
    public Animator animator;

    [Header("PLAYER PROPERTIES")]
    public float acceleration;
    public float maxMoveSpeed;
    public float rotSpeed;
    private bool hasJumped;
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
    private bool isJumping = false;
    public float raycastDistanceToFloor = 1.25f;
    public float minDistForTrick = 2.5f;

    public LayerMask floorLayerMask;

    [Space]
    [Header("TRICKS SETTINGS")]
    private float nextActionTime = 0.0f;
    public float trickCooldown = 1.0f;
    public float trickBoostToAdd = 0.5f;
    private int trickCombo = 0;
    private int trickNumber;

    [Space]
    [Header("BOOST SETTINGS")]
    public bool isBoosting = false;
    public ParticleSystem boostParticleSystem;
    public Slider boostSlider;
    public float maxBoostValue;
    public float boostValue;
    public float boostSpeedMultiplier;
    private float normalMaxSpeed;
    private float boostMaxSpeed;
    private float normalAcceleration;
    private float boostAcceleration;

    [Header("CAMERA SETTINGS")]
    public CinemachineVirtualCamera virtualCamera;
    public float minLensOrthoSize;
    public float maxLensOrthoSize;
    private int speedInteger;

    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //Guarda os valores de velocidades e acelerações antes de iniciar o jogo.
        normalMaxSpeed = maxMoveSpeed;
        boostMaxSpeed = maxMoveSpeed * boostSpeedMultiplier;
        normalAcceleration = acceleration;
        boostAcceleration = acceleration * 2.25f;
    }

    void Update()
    {
        //Regula o valor da barra de boost.
        UpdateBoostValue();

        //Muda a cor do rastro dependendo da velocidade.
        UpdateTrail();

        //Regula o campo de visão da câmera dependendo da velocidade.
        ChangeLensSize();

        //Enquanto jogador segurar botão direito do mouse e estiver no chão, personagem terá boost de velocidade.
        if (Input.GetKey(KeyCode.Mouse1) && boostValue > 0 && isGrounded)
        {
            Boost();
        }
        else
        {
            isBoosting = false;
            boostParticleSystem.Stop();
            maxMoveSpeed = normalMaxSpeed;
            acceleration = normalAcceleration;
        }

        //Faz personagem dar um pulo com barra de espaço.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            Jump();
        }

        //Se personagem apertar ou segurar barra de espaço no ar, uma determinada distância do chão e com velocidade suficiente, fará um truque.
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, Vector3.down, out hit, minDistForTrick, floorLayerMask);
        if (Input.GetKey(KeyCode.Space) && !isGrounded && !isHit && rb.velocity.magnitude >= maxMoveSpeed / 3 * 2)
        {if (Time.time > nextActionTime)
            {
                nextActionTime = Time.time + trickCooldown;
                Trick();
            }
        }
        else
        {
        }

        //Se jogador possuir velocidade suficiente, pode fazer uma curva brusca clicando duas vezes rapidamente para uma direção.
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

        //Variável local de rotação.
        float rotation;

        //Checa se personagem está realizando uma curva brusca. Se não estiver, personagem rotaciona normalmente para as direções A e D.
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

            //Caso personagem esteja no ar, rotação será reduzida.
            if (!isGrounded) rotation = Input.GetAxis("Horizontal") * rotSpeed / 3 * Mathf.Lerp(2.5f, 0.8f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;

            //Caso personagem esteja no chão, rotação será normal, depende do Input Horizontal, que varia de -1 a 1.
            else rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(2.5f, 0.8f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
        }

        //Se teclas A e D estiverem sendo seguradas ao mesmo tempo, personagem para de girar para os lados e desacelera.
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rotation = 0;
            if (isGrounded && !isBoosting)
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
        RaycastHit hit1;
        RaycastHit hit2;

        bool isHit1 = Physics.Raycast(transform.position, Vector3.down, out hit1, raycastDistanceToFloor, floorLayerMask);
        bool isHit2 = Physics.Raycast(transform.position + Vector3.forward, Vector3.down, out hit2, raycastDistanceToFloor, floorLayerMask);

        if (isHit1 || isHit2)
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
            trickCombo = 0;
            ChangeLensSizeForTrick(trickCombo);

            //Evita a nulificação de velocidade ao entrar em contato com o chão.
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

    //Para todas as linhas de código que envolvem a constante aplicação de forças direcionais ao Rigidbody do personagem, utilizei o FixedUpdate().
    void FixedUpdate()
    {
        //Se personagem estiver não estiver no chão, força para baixo é aplicada.
        if (!isGrounded)
        {
            rb.AddForce(Vector2.down * 10f);
        }

        Vector3 directionFront = new Vector3(0, 0, 1);
        directionFront = transform.TransformDirection(directionFront);

        Vector3 directionSides = new Vector3(1, 0, 0);
        directionSides = transform.TransformDirection(directionSides);

        //Enquanto personagem estiver abaixo da velocidade máxima e no chão, ele acelera.
        if (rb.velocity.magnitude < maxMoveSpeed && isGrounded)
        {
            rb.AddForce(directionFront * acceleration);

            //Enquanto personagem estiver fazendo a curva brusca, a velocidade dele reduzirá e será aplicada uma força lateral ao personagem que fará o personagem dar curvas mais acentuadas.
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
            //Porém se a curva for normal, a velocidade dele reduzirá, a força lateral será menor e também proporcional à velocidade frontal do personagem.
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

        //Se personagem brecar, é aplicada uma força oposta para desacelerar.
        if (isBraking && isGrounded)
        {
            rb.AddForce(-directionFront * acceleration);
        }
    }

    public void UpdateBoostValue()
    {
        boostSlider.value = boostValue / maxBoostValue * 100;
        if (boostValue > maxBoostValue) boostValue = maxBoostValue;
        if (boostValue < 0) boostValue = 0;
    }

    public void UpdateTrail()
    {
        if (isBoosting)
        {
            for (int i = 0; i < trailRenderers.Length; i++)
            {
                trailRenderers[i].colorGradient = trailGradients[3];
            }
            speedInteger = 3;
        }
        else
        {
            if (rb.velocity.magnitude >= maxMoveSpeed / 3 * 2)
            {
                for (int i = 0; i < trailRenderers.Length; i++)
                {
                    trailRenderers[i].colorGradient = trailGradients[2];
                }
                speedInteger = 2;
            }
            else if (rb.velocity.magnitude >= maxMoveSpeed / 3)
            {
                for (int i = 0; i < trailRenderers.Length; i++)
                {
                    trailRenderers[i].colorGradient = trailGradients[1];
                }
                speedInteger = 1;
            }
            else
            {
                for (int i = 0; i < trailRenderers.Length; i++)
                {
                    trailRenderers[i].colorGradient = trailGradients[0];
                }
                if (rb.velocity.magnitude <= 0.05f) speedInteger = 0;
                else speedInteger = 1;
            }
        }
        animator.SetInteger("speed", speedInteger);
    }

    public void Jump()
    {
        isJumping = true;
        rb.AddForce(Vector3.up * jumpForce * 100);
        animator.SetBool("isGrounded", false);
        StartCoroutine(ResetJumpFlag());
    }

    IEnumerator ResetJumpFlag()
    {
        yield return new WaitForSeconds(0.25f);
        isJumping = false;
    }

    public void Trick()
    {
        Debug.Log("TRICK!!");
        trickNumber = Random.Range(0, 8);
        animator.SetInteger("trickNumber", trickNumber);
        animator.SetTrigger("tricked");
        boostValue += trickBoostToAdd;
        trickCombo += 1;
        ChangeLensSizeForTrick(trickCombo);
    }

    public void Boost()
    {
        Debug.Log("BOOST!!");
        boostParticleSystem.Play();
        isBoosting = true;
        maxMoveSpeed = boostMaxSpeed;
        acceleration = boostAcceleration;
        boostValue -= 0.005f;
    }

    public void ChangeLensSize()
    {
        float normalizedSpeed = Mathf.Clamp01(rb.velocity.magnitude / boostMaxSpeed);

        float dampingFactor = 0.01f;
        float targetSize = Mathf.Lerp(minLensOrthoSize, maxLensOrthoSize, normalizedSpeed);
        float currentSize = virtualCamera.m_Lens.OrthographicSize;
        float newSize = Mathf.Lerp(currentSize, targetSize, dampingFactor);
        virtualCamera.m_Lens.OrthographicSize = newSize;
    }

    public void ChangeLensSizeForTrick(int combo)
    {
        float zoomIn = 0.5f;
        virtualCamera.m_Lens.OrthographicSize -= (combo * zoomIn);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Vector3.down * raycastDistanceToFloor);
        Gizmos.DrawLine(transform.position + Vector3.forward, Vector3.down * raycastDistanceToFloor);
    }
}
