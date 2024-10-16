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
    private bool wasFast = true;
    private bool wasFalling = false;
    public float jumpForce;
    public bool isBraking = false;
    public TrailRenderer[] trailRenderers;
    public Gradient[] trailGradients;
    public ParticleSystem jumpParticleSystem;
    public bool canInput = true;

    private Vector3 previousVelocity;
    private float decelerationThreshold = 12.5f;

    [Space]
    [Header("DRIFT SETTINGS")]
    public bool isDriftingA = false;
    public bool isDriftingD = false;
    private bool isDrifting = false;
    private bool wasDrifting = false;
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
    public bool isOnWater = false;
    private bool wasOnWater = false;

    public LayerMask floorLayerMask;
    public LayerMask wallLayerMask;

    public ParticleSystem landNormalParticleSystem;
    public ParticleSystem[] landSuccessParticleSystems;
    public ParticleSystem[] landFailParticleSystems;
    public ParticleSystem[] waterLandParticleSystems;
    public ParticleSystem[] waterRunParticleSystems;

    [Space]
    [Header("TRICKS SETTINGS")]
    private float nextActionTime = 0.0f;
    public float trickCooldown = 1.0f;
    public float trickBoostToAdd = 0.5f;
    public ParticleSystem trickParticleSystem;
    private int trickCombo = 0;
    private int trickNumber;
    private bool isTricking = false;
    private bool failedTrick = false;

    [Space]
    [Header("BOOST SETTINGS")]
    public bool isBoosting = false;
    public ParticleSystem[] boostParticleSystems;
    public Animator boostBarAnimator;
    public Image whiteBoostMeter;
    public Image boostMeter;
    public Image boostMeterValueToGain;
    public Image boostMeterValueGaining;
    private float totalBoostValueToAdd = 0f;
    private float boostValueToAdd = 0f;
    public float maxBoostValue;
    public float boostValue;
    public float boostSpeedMultiplier;
    public float boostToDeplete = 0.25f;
    private float normalMaxSpeed;
    private float boostMaxSpeed;
    private float normalAcceleration;
    private float boostAcceleration;
    private bool wasBoosting = false;

    [Space]
    [Header("ADDITIONAL RAIL GRIND SETTINGS")]
    [SerializeField] PlayerGrind playerGrind;
    public bool isGrinding;
    public ParticleSystem trailSparksParticleSystem;
    private bool wasOnRail = false;

    [Space]
    [Header("CAMERA SETTINGS")]
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera virtualCamera2;
    public float minLensFOV;
    public float maxLensFOV;
    private int speedInteger;

    Rigidbody rb;

    private void Awake()
    {
        //Application.targetFrameRate = 60;
    }

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
        for (int i = 0; i < boostParticleSystems.Length; i++)
        {
            if (boostParticleSystems[i].main.loop)
            {
                var boostEmissionModule = boostParticleSystems[i].emission;
                boostEmissionModule.enabled = false;
            }
        }
        for (int i = 0; i < waterRunParticleSystems.Length; i++)
        {
            var waterRunEmissionModule = waterRunParticleSystems[i].emission;
            waterRunEmissionModule.enabled = false;
        }
        var trailSparksEmissionModule = trailSparksParticleSystem.emission;
        trailSparksEmissionModule.enabled = false;

        float boostSliderValueToGain = (boostValue + totalBoostValueToAdd) / maxBoostValue;
        boostMeterValueToGain.fillAmount = Mathf.Lerp(0.4f, 0.6f, boostSliderValueToGain);
    }

    void Update()
    {
        //Ativa o cheat
        if (Input.GetKeyDown(KeyCode.P))
        {
            maxBoostValue = 10000;
            boostValue = maxBoostValue;
        }

        //Regula o valor e a barra de boost.
        UpdateBoostValue();

        //Muda a cor do rastro dependendo da velocidade.
        UpdateTrail();

        //Gerencia os sons de loop do patins.
        UpdateSpeedSound();

        //Regula o campo de visão da câmera dependendo da velocidade.
        ChangeLensSize();

        //Detecta desacelerações bruscas para animação de impacto com parede.
        DetectSuddenDeceleration();

        //Muda entre duas câmeras durante o jogo.
        SwitchCameras();

        if (canInput)
        {
            //Enquanto jogador segurar botão direito do mouse e estiver no chão, personagem terá boost de velocidade.
            if (Input.GetKey(KeyCode.LeftShift) && boostValue > 0 && isGrounded && !isGrinding && !isBraking && !failedTrick)
            {
                Boost();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isGrinding && !isBraking && !failedTrick)
                {
                    boostBarAnimator.SetTrigger("valueEmpty");
                    SFXManager.Instance.PlaySFXRandomPitch("boostVazio1");
                    SFXManager.Instance.PlaySFXRandomPitch("boostVazio2");
                }

                isBoosting = false;
                for (int i = 0; i < boostParticleSystems.Length; i++)
                {
                    if (boostParticleSystems[i].main.loop)
                    {
                        var emissionModule = boostParticleSystems[i].emission;
                        emissionModule.enabled = false;
                    }
                }
                wasBoosting = false;
                SFXManager.Instance.StopSFXLoop("boost");
            }

            //Faz personagem dar um pulo com barra de espaço.
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping && !isGrinding && !failedTrick && Time.timeScale != 0)
            {
                Jump();
            }

            //Se personagem apertar ou segurar barra de espaço no ar, uma determinada distância do chão e com velocidade suficiente, fará um truque.
            RaycastHit hit;
            bool isHit = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, minDistForTrick, floorLayerMask);
            if (Input.GetKey(KeyCode.Space) && !isGrounded && !isHit && rb.velocity.magnitude >= maxMoveSpeed / 2 && Time.timeScale != 0)
            {
                if (Time.time > nextActionTime)
                {
                    nextActionTime = Time.time + trickCooldown;
                    Trick();
                }
            }

            //Se jogador possuir velocidade suficiente, pode fazer uma curva brusca clicando duas vezes rapidamente para uma direção.
            if (rb.velocity.magnitude >= maxMoveSpeed / 3 * 2)
            {
                //TESTE CLIQUE DUPLO

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

                //TESTE SHIFT
                /*
                if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.D) && isGrounded && !isGrinding)
                {
                    isDriftingA = true;
                    isDriftingD = false;
                }
                else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.A) && isGrounded && !isGrinding)
                {
                    isDriftingA = false;
                    isDriftingD = true;
                }
                */
            }

            animator.SetBool("isDriftingD", isDriftingD);
            animator.SetBool("isDriftingA", isDriftingA);

            //Variável local de rotação.
            float rotation;

            //Checa se personagem está realizando uma curva brusca. Se não estiver, personagem rotaciona normalmente para as direções A e D.
            if (isDriftingA && isGrounded && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && rb.velocity.magnitude >= maxMoveSpeed / 5 * 3 && !failedTrick)
            {
                isDrifting = true;
                rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(0f, 2f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
                for (int i = 0; i < driftParticleSystems.Length; i++)
                {
                    var emissionModule = driftParticleSystems[i].emission;
                    emissionModule.enabled = true;
                }
            }
            else if (isDriftingD && isGrounded && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && rb.velocity.magnitude >= maxMoveSpeed / 5 * 3 && !failedTrick)
            {
                isDrifting = true;
                rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(0f, 2f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
                for (int i = 0; i < driftParticleSystems.Length; i++)
                {
                    var emissionModule = driftParticleSystems[i].emission;
                    emissionModule.enabled = true;
                }
            }
            else
            {
                isDrifting = false;
                isDriftingA = false;
                isDriftingD = false;
                for (int i = 0; i < driftParticleSystems.Length; i++)
                {
                    var emissionModule = driftParticleSystems[i].emission;
                    emissionModule.enabled = false;
                }

                //Caso personagem esteja no ar, rotação será reduzida.
                if (!isGrounded) rotation = Input.GetAxis("Horizontal") * rotSpeed / 2 * Mathf.Lerp(3f, 0.75f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;

                //Caso personagem esteja no chão, rotação será normal, depende do Input Horizontal, que varia de -1 a 1.
                else rotation = Input.GetAxis("Horizontal") * rotSpeed * Mathf.Lerp(3f, 0.75f, rb.velocity.magnitude / maxMoveSpeed) * Time.deltaTime;
            }

            if (isDrifting && !wasDrifting)
            {
                wasDrifting = true;
                SFXManager.Instance.PlaySFXLoop("drift");
            }
            else if(!isDrifting && wasDrifting)
            {
                wasDrifting = false;
                SFXManager.Instance.StopSFXLoop("drift");
            }

            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            {
                rotation = 0;
                if (isGrounded && !isGrinding)
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
        }

        if(rb.velocity.magnitude >= maxMoveSpeed * 2 / 3 && wasFast)
        {
            SFXManager.Instance.PlaySFXLoop("vento");
            wasFast = false;
        }
        if (rb.velocity.magnitude < maxMoveSpeed * 2 / 3 && !wasFast)
        {
            SFXManager.Instance.StopSFXLoop("vento");
            wasFast = true;
        }
        if(rb.velocity.y < -2.5f && !isGrounded && !wasFalling)
        {
            SFXManager.Instance.PlaySFXLoop("vento2");
            wasFalling = true;
        }
        else if (isGrounded && wasFalling)
        {
            SFXManager.Instance.StopSFXLoop("vento2");
            wasFalling = false;
        }
    }

    //Para todas as linhas de código que envolvem a constante aplicação de forças direcionais ao Rigidbody do personagem, utilizei o FixedUpdate().
    void FixedUpdate()
    {
        if (isTricking && Time.time > (nextActionTime - trickCooldown / 4))
        {
            isTricking = false;
        }
        animator.SetBool("isTricking", isTricking);

        //Gera uma Raycast abaixo do personagem que detecta colisão com chão.
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        RaycastHit hit4;

        bool isHit1 = Physics.Raycast(transform.position + Vector3.up + (Vector3.forward / 4), Vector3.down, out hit1, raycastDistanceToFloor, floorLayerMask);
        bool isHit2 = Physics.Raycast(transform.position + Vector3.up + (Vector3.back / 4), Vector3.down, out hit2, raycastDistanceToFloor, floorLayerMask);
        bool isHit3 = Physics.Raycast(transform.position + Vector3.up + (Vector3.left / 4), Vector3.down, out hit3, raycastDistanceToFloor, floorLayerMask);
        bool isHit4 = Physics.Raycast(transform.position + Vector3.up + (Vector3.right / 4), Vector3.down, out hit4, raycastDistanceToFloor, floorLayerMask);

        if (isHit1 || isHit2 || isHit3 || isHit4)
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
            boostBarAnimator.SetBool("isGrounded", true);
        }
        else
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
            boostBarAnimator.SetBool("isGrounded", false);
            isAirborne = true;
        }

        //Regula os parâmetros envolvendo o grind de trilhos.
        UpdateRailGrind();

        RaycastHit hit5;

        bool isHit5 = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit5, 1.15f, floorLayerMask);

        if (isHit5)
        {
            //Evita a nulificação de velocidade ao entrar em contato com o chão.
            if (isAirborne)
            {
                Vector3 velocity = previousVelocity;
                velocity.y = 0f;
                rb.velocity = velocity;
                isAirborne = false;
                ChangeLensSizeForTrick(trickCombo);
                if (trickCombo == 0)
                {
                    SFXManager.Instance.PlaySFXRandomPitch("impactoPatins");
                    landNormalParticleSystem.Play();
                }
                else
                {
                    /*
                    if (Time.time >= (nextActionTime - trickCooldown/4))
                    {
                        boostBarAnimator.SetTrigger("trickSuccess");
                        SFXManager.Instance.PlaySFXRandomPitch("impactoPatins2");
                        boostValue += totalBoostValueToAdd;
                    }
                    */
                    if (!isTricking)
                    {
                        boostBarAnimator.SetTrigger("trickSuccess");
                        for (int i = 0; i < landSuccessParticleSystems.Length; i++)
                        {
                            landSuccessParticleSystems[i].Play();
                        }
                        SFXManager.Instance.PlaySFXRandomPitch("impactoPatins2");
                        SFXManager.Instance.PlaySFX("truqueSucesso");
                        boostValue += totalBoostValueToAdd;
                    }
                    else
                    {
                        boostBarAnimator.SetTrigger("trickFail");
                        for (int i = 0; i < landFailParticleSystems.Length; i++)
                        {
                            landFailParticleSystems[i].Play();
                        }
                        SFXManager.Instance.PlaySFX("truqueFalha");
                        SFXManager.Instance.PlaySFXRandomPitch("impactoChao");
                        rb.velocity = velocity/4;
                        StartCoroutine(ResetFailedTrickFlag());
                    }
                    //Debug.Log(Time.time - (nextActionTime - trickCooldown / 4));
                    //Debug.Log(animator.GetFloat("trickTimer"));
                    float trickTimer = Time.time - (nextActionTime - trickCooldown / 4);
                    animator.SetFloat("trickTimer", trickTimer);
                    animator.SetTrigger("landed");
                    Debug.Log(animator.GetFloat("trickTimer"));

                }
                trickCombo = 0;
                animator.SetInteger("trickCombo", 0);
                StartCoroutine(ResetLandedFlag());
                totalBoostValueToAdd = 0;
                boostValueToAdd = 0;
                boostMeterValueToGain.fillAmount = 0;
            }
        }

        bool isHit6 = Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit5, 1.15f, LayerMask.GetMask("Water"), QueryTriggerInteraction.Collide);
        if (isHit6)
        {
            if (!wasOnWater)
            {
                for (int i = 0; i < waterLandParticleSystems.Length; i++)
                {
                    waterLandParticleSystems[i].Play();
                }
                wasOnWater = true;
            }
            if(rb.velocity.magnitude > 10f)
            {
                for (int i = 0; i < waterRunParticleSystems.Length; i++)
                {
                    var waterRunEmissionModule = waterRunParticleSystems[i].emission;
                    waterRunEmissionModule.enabled = true;
                }
            }
            else
            {
                for (int i = 0; i < waterRunParticleSystems.Length; i++)
                {
                    var waterRunEmissionModule = waterRunParticleSystems[i].emission;
                    waterRunEmissionModule.enabled = false;
                }
            }
        }
        else
        {
            wasOnWater = false;
            for (int i = 0; i < waterRunParticleSystems.Length; i++)
            {
                var waterRunEmissionModule = waterRunParticleSystems[i].emission;
                waterRunEmissionModule.enabled = false;
            }
        }

        //Se personagem estiver não estiver no chão, força para baixo é aplicada.
        if (!isGrounded && !isGrinding)
        {
            rb.AddForce(Vector3.down * 20f);
        }

        Vector3 directionFront = new Vector3(0, 0, 1);
        directionFront = transform.TransformDirection(directionFront);

        Vector3 directionSides = new Vector3(1, 0, 0);
        directionSides = transform.TransformDirection(directionSides);

        //Enquanto personagem estiver abaixo da velocidade máxima e no chão, ele acelera.
        if (rb.velocity.magnitude <= maxMoveSpeed && isGrounded && !isGrinding)
        {
            rb.AddForce(directionFront * acceleration);
        }

        if(isGrounded && !isGrinding)
        {
            if (canInput)
            {
                //Enquanto personagem estiver fazendo a curva brusca, a velocidade dele reduzirá e será aplicada uma força lateral ao personagem que fará o personagem dar curvas mais acentuadas.
                if (isDriftingA || isDriftingD)
                {
                    rb.velocity *= 0.99f;
                    Vector3 driftForce = isDriftingA ? -directionSides : directionSides;
                    if(Mathf.Abs(rb.velocity.x) <= maxMoveSpeed)
                    {
                        rb.AddForce(driftForce * acceleration);
                    }
                }
                //Porém se a curva for normal, a velocidade dele reduzirá, a força lateral será menor e também proporcional à velocidade frontal do personagem.
                else
                {
                    if (!isBraking && Mathf.Abs(rb.velocity.x) <= maxMoveSpeed)
                    {
                        rb.AddForce(directionSides * Input.GetAxis("Horizontal") * ((acceleration / 2.5f) * (rb.velocity.magnitude / maxMoveSpeed)));
                        if (Input.GetAxis("Horizontal") != 0)
                        {
                            rb.AddForce(-directionFront * acceleration / 5);
                        }
                    }
                }
            }
        }
        

        //Se personagem brecar, é aplicada uma força oposta para desacelerar.
        if (isBraking && isGrounded && !isGrinding && canInput)
        {
            rb.AddForce(-directionFront * acceleration);
        }
    }

    public void UpdateRailGrind()
    {
        var emissionModule = trailSparksParticleSystem.emission;

        isGrinding = playerGrind.onRail;
        ObjectiveManager.Instance.isGrinding = isGrinding;

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

            if (!isGrounded)
            {
                rb.AddForce(directionFront * acceleration * maxMoveSpeed * 1.5f);
            }
            SFXManager.Instance.StopSFXLoop("trilhos");
        }

        if (!wasOnRail && isGrinding)
        {
            Vector3 velocity = rb.velocity;
            velocity.x = 0f;
            velocity.y = 0f;
            velocity.z = 0f;
            rb.velocity = velocity;

            animator.SetTrigger("startedGrind");
            animator.ResetTrigger("bumped");
            SFXManager.Instance.PlaySFXRandomPitch("impactoTrilhos");
        }

        wasOnRail = isGrinding;

        if (isGrinding && !isGrounded)
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
        if(trickCombo > 0 && Time.time > nextActionTime - trickCooldown)
        {
            if(trickCombo == 1)
            {
                boostMeterValueGaining.fillAmount = Mathf.Lerp(boostMeterValueToGain.fillAmount, boostMeter.fillAmount, nextActionTime - Time.time);
            }
            else
            {
                boostMeterValueGaining.fillAmount = Mathf.Lerp(boostMeterValueToGain.fillAmount, boostMeter.fillAmount + Mathf.Lerp(0f, 0.2f, boostValueToAdd / maxBoostValue), nextActionTime - Time.time);
            }
        }

        if (isBoosting)
        {
            maxMoveSpeed = boostMaxSpeed;
            acceleration = boostAcceleration;
            boostBarAnimator.SetBool("valueSpending", true);
        }
        else
        {
            maxMoveSpeed = normalMaxSpeed;
            acceleration = normalAcceleration;
            boostBarAnimator.SetBool("valueSpending", false);
        }

        float boostSliderValue = boostValue / maxBoostValue;
        boostMeter.fillAmount = Mathf.Lerp(0.4f, 0.6f, boostSliderValue);
        whiteBoostMeter.fillAmount = Mathf.Lerp(0.4f, 0.6f, boostSliderValue);

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
        jumpParticleSystem.Play();
        SFXManager.Instance.PlaySFXRandomPitch("pulo");
        rb.AddForce(Vector3.up * jumpForce * 100);
        animator.SetTrigger("jumped");
        animator.SetBool("isGrounded", false);
        boostBarAnimator.SetBool("isGrounded", false);
        StartCoroutine(ResetJumpFlag());
    }

    IEnumerator ResetJumpFlag()
    {
        yield return new WaitForSeconds(0.5f);
        isJumping = false;
        animator.ResetTrigger("jumped");
    }

    IEnumerator ResetLandedFlag()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("landed");
    }

    IEnumerator ResetFailedTrickFlag()
    {
        failedTrick = true;
        yield return new WaitForSeconds(0.75f);
        failedTrick = false;
    }

    public void Trick()
    {
        isTricking = true;

        Debug.Log("TRICK!!");
        ObjectiveManager.Instance.tricksNumber++;
        trickNumber = Random.Range(0, 12);
        animator.SetInteger("trickNumber", trickNumber);
        animator.SetTrigger("tricked");
        boostBarAnimator.SetTrigger("trick");
        trickParticleSystem.Play();

        switch (trickCombo)
        {
            case 0:
                SFXManager.Instance.PlaySFXRandomPitch("truque1");
                boostValueToAdd = trickBoostToAdd;
                break;
            case 1:
                SFXManager.Instance.PlaySFXRandomPitch("truque2");
                boostValueToAdd = trickBoostToAdd + trickBoostToAdd / 4;
                break;
            case 2:
                SFXManager.Instance.PlaySFXRandomPitch("truque3");
                boostValueToAdd = trickBoostToAdd + trickBoostToAdd / 2;
                break;
            default:
                SFXManager.Instance.PlaySFXRandomPitch("truque4");
                boostValueToAdd = trickBoostToAdd * 2;
                break;
        }
        totalBoostValueToAdd += trickBoostToAdd;
        float boostSliderValueToGain = (boostValue + totalBoostValueToAdd) / maxBoostValue;
        boostMeterValueToGain.fillAmount = Mathf.Lerp(0.4f, 0.6f, boostSliderValueToGain);
        int discoInteger = Random.Range(0, 4) + 1;
        SFXManager.Instance.PlaySFX("disco" + discoInteger);

        trickCombo += 1;
        animator.SetInteger("trickCombo", trickCombo);
        ChangeLensSizeForTrick(trickCombo);
    }

    public void Boost()
    {
        for (int i = 0; i < boostParticleSystems.Length; i++)
        {
            if (boostParticleSystems[i].main.loop)
            {
                var emissionModule = boostParticleSystems[i].emission;
                emissionModule.enabled = true;
            }
        }
        isBoosting = true;
        boostValue -= boostToDeplete * Time.deltaTime;


        if (!wasBoosting && !failedTrick)
        {
            for (int i = 0; i < boostParticleSystems.Length; i++)
            {
                if (!boostParticleSystems[i].main.loop)
                {
                    boostParticleSystems[i].Play();
                }
            }
            SFXManager.Instance.PlaySFXLoop("boost");
            SFXManager.Instance.PlaySFXRandomPitch("boost");
            wasBoosting = true;
        }
    }

    public void DetectSuddenDeceleration()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocityChange = new Vector3(currentVelocity.x - previousVelocity.x, 0, currentVelocity.z - previousVelocity.z);

        if (horizontalVelocityChange.magnitude >= decelerationThreshold && isGrounded && !isGrinding && !isBoosting && !isTricking && !failedTrick)
        {
            animator.SetTrigger("bumped");
            SFXManager.Instance.PlaySFXRandomPitch("impactoParede");
        }

        previousVelocity = currentVelocity;
    }

    public void ActivateInput(bool isActive)
    {
        canInput = isActive;
    }

    public void ChangeLensSize()
    {
        float normalizedSpeed = Mathf.Clamp01(rb.velocity.magnitude / boostMaxSpeed);

        float dampingFactor = 0.0075f;
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
        float zoomIn = 1.5f;
        virtualCamera.m_Lens.FieldOfView -= (combo * zoomIn);
        virtualCamera2.m_Lens.FieldOfView -= (combo * zoomIn);
    }

    public void SwitchCameras()
    {
        Vector3 direction = new Vector3(1, 1, -1);
        RaycastHit hit;

        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y += 2;

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

        // Draw the first raycast
        Vector3 rayStart1 = transform.position + Vector3.up + (Vector3.forward / 4);
        Vector3 rayEnd1 = rayStart1 + Vector3.down * raycastDistanceToFloor;
        Gizmos.DrawLine(rayStart1, rayEnd1);

        // Draw the second raycast
        Vector3 rayStart2 = transform.position + Vector3.up + (Vector3.back / 4);
        Vector3 rayEnd2 = rayStart2 + Vector3.down * raycastDistanceToFloor;
        Gizmos.DrawLine(rayStart2, rayEnd2);

        // Draw the third raycast
        Vector3 rayStart3 = transform.position + Vector3.up + (Vector3.left / 4);
        Vector3 rayEnd3 = rayStart3 + Vector3.down * raycastDistanceToFloor;
        Gizmos.DrawLine(rayStart3, rayEnd3);

        // Draw the second raycast
        Vector3 rayStart4 = transform.position + Vector3.up + (Vector3.right / 4);
        Vector3 rayEnd4 = rayStart4 + Vector3.down * raycastDistanceToFloor;
        Gizmos.DrawLine(rayStart4, rayEnd4);

        // Optionally, draw spheres at the end points for better visualization
        Gizmos.DrawSphere(rayEnd1, 0.1f);
        Gizmos.DrawSphere(rayEnd2, 0.1f);
        Gizmos.DrawSphere(rayEnd3, 0.1f);
        Gizmos.DrawSphere(rayEnd4, 0.1f);
    }
}
