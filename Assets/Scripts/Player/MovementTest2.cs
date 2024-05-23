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

    private Vector3 previousVelocity;
    private float decelerationThreshold = 12.5f;

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
    public float raycastDistanceToFloor = 1.35f;
    public float minDistForTrick = 4f;

    public LayerMask floorLayerMask;
    public LayerMask wallLayerMask;

    [Space]
    [Header("TRICKS SETTINGS")]
    private float nextActionTime = 0.0f;
    public float trickCooldown = 1.0f;
    public float trickBoostToAdd = 0.5f;
    public ParticleSystem trickParticleSystem;
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
    public float boostToDeplete = 0.25f;
    private float normalMaxSpeed;
    private float boostMaxSpeed;
    private float normalAcceleration;
    private float boostAcceleration;

    [Space]
    [Header("ADDITIONAL RAIL GRIND SETTINGS")]
    [SerializeField] PlayerGrind playerGrind;
    public bool isGrinding;
    public ParticleSystem trailSparksParticleSystem;
    private bool wasOnRail = false;

    [Header("CAMERA SETTINGS")]
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera virtualCamera2;
    public float minLensFOV;
    public float maxLensFOV;
    private int speedInteger;

    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        previousVelocity = rb.velocity;

        //Guarda os valores de velocidades e acelerações antes de iniciar o jogo.
        normalMaxSpeed = maxMoveSpeed;
        boostMaxSpeed = maxMoveSpeed * boostSpeedMultiplier;
        normalAcceleration = acceleration;
        boostAcceleration = acceleration * 2.25f;

        //Garante que emissão de partículas esteja desligada de início.
        for (int i = 0; i < driftParticleSystems.Length; i++)
        {
            var driftEmissionModule = driftParticleSystems[i].emission;
            driftEmissionModule.enabled = false;
        }
        var boostEmissionModule = boostParticleSystem.emission;
        boostEmissionModule.enabled = false;
        var trailSparksEmissionModule = trailSparksParticleSystem.emission;
        trailSparksEmissionModule.enabled = false;
    }

    void Update()
    {
        //Regula os parâmetros envolvendo o grind de trilhos.
        UpdateRailGrind();

        //Regula o valor da barra de boost.
        UpdateBoostValue();

        //Muda a cor do rastro dependendo da velocidade.
        UpdateTrail();

        //Gerencia os sons de loop do patins.
        UpdateSpeedSound();

        //Regula o campo de visão da câmera dependendo da velocidade.
        ChangeLensSize();

        DetectSuddenDeceleration();

        SwitchCameras();

        //Enquanto jogador segurar botão direito do mouse e estiver no chão, personagem terá boost de velocidade.
        if (Input.GetKey(KeyCode.Mouse1) && boostValue > 0 && isGrounded && !isGrinding)
        {
            Boost();
        }
        else
        {
            isBoosting = false;
            var emissionModule = boostParticleSystem.emission;
            emissionModule.enabled = false;
            maxMoveSpeed = normalMaxSpeed;
            acceleration = normalAcceleration;
        }

        //Faz personagem dar um pulo com barra de espaço.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping && !isGrinding)
        {
            Jump();
        }

        //Se personagem apertar ou segurar barra de espaço no ar, uma determinada distância do chão e com velocidade suficiente, fará um truque.
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, minDistForTrick, floorLayerMask);
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
            if (Input.GetKeyDown(KeyCode.A) && isGrounded && !isGrinding)
            {
                if (Time.time - lastATapTime <= doubleATapTimeThreshold)
                {
                    isDriftingA = true;
                    isDriftingD = false;
                }
                lastATapTime = Time.time;
            }
            if (Input.GetKeyDown(KeyCode.D) && isGrounded && !isGrinding)
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
            for (int i = 0; i < driftParticleSystems.Length; i++)
            {
                var emissionModule = driftParticleSystems[i].emission;
                emissionModule.enabled = true;
            }
        }
        else if (isDriftingD && isGrounded && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && rb.velocity.magnitude >= maxMoveSpeed / 5 * 3)
        {
            rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(0f, 2f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
            for (int i = 0; i < driftParticleSystems.Length; i++)
            {
                var emissionModule = driftParticleSystems[i].emission;
                emissionModule.enabled = true;
            }
        }
        else
        {
            isDriftingA = false;
            isDriftingD = false;
            for (int i = 0; i < driftParticleSystems.Length; i++)
            {
                var emissionModule = driftParticleSystems[i].emission;
                emissionModule.enabled = false;
            }

            //Caso personagem esteja no ar, rotação será reduzida.
            if (!isGrounded) rotation = Input.GetAxis("Horizontal") * rotSpeed / 3 * Mathf.Lerp(2.5f, 0.8f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;

            //Caso personagem esteja no chão, rotação será normal, depende do Input Horizontal, que varia de -1 a 1.
            else rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(2.5f, 0.8f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
        }

        //Se teclas A e D estiverem sendo seguradas ao mesmo tempo, personagem para de girar para os lados e desacelera.
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rotation = 0;
            if (isGrounded && !isBoosting && !isGrinding)
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

        bool isHit1 = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit1, raycastDistanceToFloor, floorLayerMask);
        bool isHit2 = Physics.Raycast(transform.position + Vector3.up + Vector3.forward, Vector3.down, out hit2, raycastDistanceToFloor, floorLayerMask);

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
                SFXManager.Instance.PlaySFXRandomPitch("impactoPatins");
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
        if (!isGrounded && !isGrinding)
        {
            rb.AddForce(Vector2.down * 15f);
        }

        Vector3 directionFront = new Vector3(0, 0, 1);
        directionFront = transform.TransformDirection(directionFront);

        Vector3 directionSides = new Vector3(1, 0, 0);
        directionSides = transform.TransformDirection(directionSides);

        //Enquanto personagem estiver abaixo da velocidade máxima e no chão, ele acelera.
        if (rb.velocity.magnitude < maxMoveSpeed && isGrounded && !isGrinding)
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
        if (isBraking && isGrounded && !isGrinding)
        {
            rb.AddForce(-directionFront * acceleration);
        }
    }

    public void UpdateRailGrind()
    {
        var emissionModule = trailSparksParticleSystem.emission;

        isGrinding = playerGrind.onRail;
        animator.SetBool("isGrinding", isGrinding);
        if (wasOnRail && !isGrinding)
        {
            Vector3 directionFront = new Vector3(0, 0, 1);
            directionFront = transform.TransformDirection(directionFront);

            Vector3 velocity = rb.velocity;
            velocity.x = 0f;
            velocity.y = 0f;
            velocity.z = 0f;
            rb.velocity = velocity;
            rb.AddForce(directionFront * acceleration * maxMoveSpeed * 1.25f);
            SFXManager.Instance.StopSFXLoop("trilhos");
        }

        if (!wasOnRail && isGrinding)
        {
            animator.SetTrigger("startedGrind");
            SFXManager.Instance.PlaySFXRandomPitch("impactoTrilhos");
        }

        wasOnRail = isGrinding;

        if (isGrinding)
        {
            Vector3 velocity = rb.velocity;
            velocity.x = 0f;
            velocity.y = 0f;
            velocity.z = 0f;
            rb.velocity = velocity;
            emissionModule.enabled = true;
            SFXManager.Instance.PlaySFXLoop("trilhos");
        }
        else
        {
            emissionModule.enabled = false;
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

    public void UpdateSpeedSound()
    {
        if (isBoosting)
        {
            SFXManager.Instance.PlaySFXLoop("velocidade1");
            SFXManager.Instance.PlaySFXLoop("velocidade2");
        }
        else if(isGrounded)
        {
            if (rb.velocity.magnitude >= maxMoveSpeed / 3 * 2)
            {
                SFXManager.Instance.PlaySFXLoop("velocidade1");
                SFXManager.Instance.PlaySFXLoop("velocidade2");
            }
            else if (rb.velocity.magnitude >= maxMoveSpeed / 3)
            {
                SFXManager.Instance.PlaySFXLoop("velocidade1");
                SFXManager.Instance.StopSFXLoop("velocidade2");
            }
            else
            {
                SFXManager.Instance.StopSFXLoop("velocidade1");
                SFXManager.Instance.StopSFXLoop("velocidade2");
            }
        }
        else
        {
            SFXManager.Instance.StopSFXLoop("velocidade1");
            SFXManager.Instance.StopSFXLoop("velocidade2");
        }
    }

    public void Jump()
    {
        isJumping = true;
        SFXManager.Instance.PlaySFXRandomPitch("pulo");
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
        trickNumber = Random.Range(0, 12);
        animator.SetInteger("trickNumber", trickNumber);
        animator.SetTrigger("tricked");
        trickParticleSystem.Play();
        boostValue += trickBoostToAdd;
        trickCombo += 1;
        ChangeLensSizeForTrick(trickCombo);
    }

    public void Boost()
    {
        var emissionModule = boostParticleSystem.emission;
        emissionModule.enabled = true;
        isBoosting = true;
        maxMoveSpeed = boostMaxSpeed;
        acceleration = boostAcceleration;
        boostValue -= boostToDeplete * Time.deltaTime;
    }

    public void DetectSuddenDeceleration()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocityChange = new Vector3(currentVelocity.x - previousVelocity.x, 0, currentVelocity.z - previousVelocity.z);

        if (horizontalVelocityChange.magnitude >= decelerationThreshold && isGrounded && !isGrinding && !isBoosting)
        {
            animator.SetTrigger("bumped");
        }

        previousVelocity = currentVelocity;
    }

    public void ChangeLensSize()
    {
        float normalizedSpeed = Mathf.Clamp01(rb.velocity.magnitude / boostMaxSpeed);

        float dampingFactor = 0.01f;
        float targetSize = Mathf.Lerp(minLensFOV, maxLensFOV, normalizedSpeed);
        float currentSize = virtualCamera.m_Lens.FieldOfView;
        float currentSize2 = virtualCamera2.m_Lens.FieldOfView;
        float newSize = Mathf.Lerp(currentSize, targetSize, dampingFactor);
        float newSize2 = Mathf.Lerp(currentSize2, targetSize, dampingFactor);
        virtualCamera.m_Lens.FieldOfView = newSize;
        virtualCamera2.m_Lens.FieldOfView = newSize2;
    }

    public void ChangeLensSizeForTrick(int combo)
    {
        float zoomIn = 2.5f;
        virtualCamera.m_Lens.FieldOfView -= (combo * zoomIn);
        virtualCamera2.m_Lens.FieldOfView -= (combo * zoomIn);
    }

    public void SwitchCameras()
    {
        Vector3 direction = new Vector3(1, 1, -1);
        RaycastHit hit;

        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y += 1;

        Debug.DrawRay(raycastOrigin, direction * 8f, Color.red);
        if (Physics.Raycast(raycastOrigin, direction, out hit, 8, wallLayerMask))
        {
            virtualCamera2.enabled = true;
        }
        else
        {
            virtualCamera2.enabled = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Vector3.down * raycastDistanceToFloor);
        Gizmos.DrawLine(transform.position + Vector3.forward, Vector3.down * raycastDistanceToFloor);
    }
}
