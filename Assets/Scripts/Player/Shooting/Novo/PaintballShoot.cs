using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaintballShoot : MonoBehaviour
{
    [Space]
    [Header("SHOOTING SETTINGS")]
    public Transform center;
    public Transform firePoint; // Ponto de origem do tiro
    public GameObject gun;
    public GameObject chamber;
    public GameObject chamberLeftHand;
    public GameObject bulletPrefab; // Prefab da bala
    public float bulletSpeed = 20f; // Velocidade da bala
    public float fireRate = 0.5f; // Taxa de disparo em tiros por segundo
    private float nextFireTime = 0f; // Tempo do próximo disparo, inicializado como 0 para permitir o primeiro tiro imediatamente
    private bool startedShooting = false;
    private float shootStartTime = 0f;
    public float maxAmmo = 15;
    public float currentAmmo;
    public float reloadTime = 3;
    private bool isReloading = false;
    private float lastShotTime = 0f;
    private float cursorDisplayDuration = 0.1f;
    public Image ammoReloadMeter;
    public TextMeshProUGUI ammoText;

    [Space]
    [Header("ANIMATION PROPERTIES")]
    [SerializeField] private Animator animator;
    private float targetLayerWeight = 0f; // The target weight for the shooting layer
    public float transitionSpeed = 2f; // Speed of the transition
    [SerializeField] private Animator gunPanelAnimator;

    [Space]
    [Header("LAYER MASK AND VISUAL FEEDBACK")]
    public LayerMask hitAimMask;
    public GameObject aimHitReference;
    [SerializeField] private Texture2D canShootCursor;
    [SerializeField] private Texture2D isShootingCursor;

    private MovementTest2 playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<MovementTest2>();
        currentAmmo = maxAmmo;
        chamber.SetActive(true);
        chamberLeftHand.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isReloading", isReloading);
        gunPanelAnimator.SetBool("isReloading", isReloading);

        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo;

        if (currentAmmo < maxAmmo / 4)
        {
            ammoText.color = new Vector4(1, 1, 0, 1);
        }
        else ammoText.color = new Vector4(1, 1, 1, 1);

        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Verifica se o botão esquerdo do mouse está pressionado e se já passou o tempo do próximo disparo. Tambem confere se jogo esta pausado ou foi finalizado
        if (Input.GetButton("Fire1") && (playerMovement.isGrounded || playerMovement.isGrinding) && playerMovement.canInput && currentAmmo > 0 && Time.timeScale != 0 && GameManager.Instance.state != GameState.Win && GameManager.Instance.state != GameState.Lose)
        {
            if (!startedShooting)
            {
                shootStartTime = Time.time;
                startedShooting = true;
            }

            Vector3 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitAimMask))
            {
                Vector3 direction = (hit.point - center.position);
                direction.y = 0f;


                // Get the character's forward direction
                Vector3 forward = transform.forward;
                forward.y = 0f;

                // Calculate the angle between the character's forward direction and the direction to the hit point
                float angle = Vector3.SignedAngle(forward, direction, Vector3.up);

                // Normalize the angle to be within [0, 360)
                angle = (angle + 360) % 360;

                // Map the angle to the range [0, 1] for the animator parameter
                float lookAngle = angle / 360f;

                // Adjust the lookAngle to match the desired orientation
                lookAngle = (lookAngle + 0.5f) % 1f;


                animator.SetFloat("lookAngle", lookAngle);

                targetLayerWeight = 1f;
                animator.SetLayerWeight(1, 1);

                ammoReloadMeter.gameObject.SetActive(true);
                gun.SetActive(true);
                Cursor.SetCursor(canShootCursor, new Vector2(canShootCursor.width / 2, canShootCursor.height / 2), CursorMode.Auto);
                
                if (Time.time < lastShotTime + cursorDisplayDuration)
                {
                    Cursor.SetCursor(isShootingCursor, new Vector2(isShootingCursor.width / 2, isShootingCursor.height / 2), CursorMode.Auto);
                }
                else
                {
                    Cursor.SetCursor(canShootCursor, new Vector2(canShootCursor.width / 2, canShootCursor.height / 2), CursorMode.Auto);
                }

                // Handle shooting
                if (Time.time >= shootStartTime + 0.025f && Time.time >= nextFireTime)
                {
                    currentAmmo--;
                    Shoot();

                    lastShotTime = Time.time;
                    // Define o tempo do próximo disparo adicionando o intervalo entre os tiros
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                targetLayerWeight = 0f;

                if (isReloading)
                {
                    if (playerMovement.isGrounded || playerMovement.isGrinding)
                    {
                        targetLayerWeight = 1f;
                        gun.SetActive(true);
                    }
                    else
                    {
                        gun.SetActive(false);
                    }
                    ammoReloadMeter.gameObject.SetActive(true);
                }
                else
                {
                    ammoReloadMeter.gameObject.SetActive(false);
                    gun.SetActive(false);
                }
            }
        }
        else
        {
            startedShooting = false;
            targetLayerWeight = 0f;
            if (isReloading)
            {
                if (playerMovement.isGrounded || playerMovement.isGrinding)
                {
                    targetLayerWeight = 1f;
                    gun.SetActive(true);
                }
                else
                {
                    gun.SetActive(false);
                }
                ammoReloadMeter.gameObject.SetActive(true);
            }
            else
            {
                ammoReloadMeter.gameObject.SetActive(false);
                gun.SetActive(false);
            }
            
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        // Smoothly transition the layer weight
        float currentLayerWeight = animator.GetLayerWeight(1);
        animator.SetLayerWeight(1, Mathf.MoveTowards(currentLayerWeight, targetLayerWeight, transitionSpeed * Time.deltaTime));
    }

    void LateUpdate()
    {
        // Get the parent's rotation
        Quaternion playerRotation = transform.rotation;

        // Convert the player's rotation to Euler angles
        Vector3 playerEulerAngles = playerRotation.eulerAngles;

        // Invert the x component of the player's Euler angles
        float invertedXRotation = -playerEulerAngles.x;

        // Get the current local rotation of the redArea as Euler angles
        Vector3 aimHitReferenceEulerAngles = aimHitReference.transform.localRotation.eulerAngles;

        // Set the x component to the inverted x rotation, keeping the y and z components the same
        aimHitReferenceEulerAngles.x = invertedXRotation;

        // Apply the modified rotation back to the redArea
        aimHitReference.transform.localRotation = Quaternion.Euler(aimHitReferenceEulerAngles);
    }

    void Shoot()
    {
        gunPanelAnimator.SetTrigger("shot");

        SFXManager.Instance.PlaySFXRandomPitch("tiro1");
        SFXManager.Instance.PlaySFXRandomPitch("tiro2");

        // Cria uma instância da bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Obtém a posição do mouse na tela
        Vector3 mousePosition = Input.mousePosition;

        // Converte a posição do mouse de tela para um raio no mundo 3D
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // Se o raio atingir algo no mundo
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitAimMask))
        //if (Physics.Raycast(ray, out hit))
        {
            // Calcula a direção do tiro baseado na posição do mouse e do personagem
            Vector3 direction = (hit.point - center.position).normalized;

            // Zera o componente Y da direção para garantir que o tiro permaneça na mesma altura
            direction.y = 0f;

            // Obtém o componente Rigidbody da bala
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            // Define a velocidade da bala na direção do mouse
            rb.velocity = direction * bulletSpeed;
        }
        else
        {
        }
    }

    IEnumerator Reload()
    {
        ammoReloadMeter.fillAmount = 0;
        isReloading = true;
        SFXManager.Instance.PlaySFX("recarga");

        for (int i = 0; i < maxAmmo; i++)
        {
            float startFill = ammoReloadMeter.fillAmount;
            float endFill = (i + 1) / maxAmmo;
            float elapsed = 0f;

            while (elapsed < reloadTime / maxAmmo)
            {
                ammoReloadMeter.fillAmount = Mathf.Lerp(startFill, endFill, elapsed / (reloadTime / maxAmmo));
                elapsed += Time.deltaTime;
                yield return null;
            }

            ammoReloadMeter.fillAmount = endFill;

            if (i < 12)
            {
                chamber.SetActive(false);
                if (playerMovement.isGrounded || playerMovement.isGrinding)
                {
                    chamberLeftHand.SetActive(true);
                }
            }
            else
            {
                chamber.SetActive(true);
                chamberLeftHand.SetActive(false);
            }
        }

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}