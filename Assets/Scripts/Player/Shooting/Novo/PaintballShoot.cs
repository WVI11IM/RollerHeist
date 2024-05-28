using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballShoot : MonoBehaviour
{
    [Space]
    [Header("SHOOTING SETTINGS")]
    public Transform firePoint; // Ponto de origem do tiro
    public GameObject bulletPrefab; // Prefab da bala
    public float bulletSpeed = 20f; // Velocidade da bala
    public float maxAngle = 180f; // �ngulo m�ximo de tiro em rela��o � dire��o do personagem
    public float fireRate = 0.5f; // Taxa de disparo em tiros por segundo
    private float nextFireTime = 0f; // Tempo do pr�ximo disparo, inicializado como 0 para permitir o primeiro tiro imediatamente
    public float maxAmmo = 5;
    public float currentAmmo;
    public float reloadTime = 2;

    [Space]
    [Header("ANIMATION PROPERTIES")]
    [SerializeField] private Animator animator;
    private float targetLayerWeight = 0f; // The target weight for the shooting layer
    public float transitionSpeed = 2f; // Speed of the transition

    [Space]
    [Header("LAYER MASK AND VISUAL FEEDBACK")]
    public LayerMask hitAimMask;
    public GameObject aimHitReference;
    [SerializeField] private Texture2D canShootCursor;

    private MovementTest2 playerMovement;

    private void Start()
    {
        playerMovement = GetComponent<MovementTest2>();
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        // Verifica se o bot�o esquerdo do mouse est� pressionado e se j� passou o tempo do pr�ximo disparo
        if (Input.GetButton("Fire1") && (playerMovement.isGrounded || playerMovement.isGrinding) && playerMovement.canInput && currentAmmo > 0)
        {
            Vector3 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitAimMask))
            {
                Vector3 direction = (hit.point - firePoint.position);
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
                Cursor.SetCursor(canShootCursor, Vector2.zero, CursorMode.Auto);
                if (Time.time >= nextFireTime)
                {
                    currentAmmo--;

                    Shoot();
                    // Define o tempo do pr�ximo disparo adicionando o intervalo entre os tiros
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                targetLayerWeight = 0f;
            }
        }
        else
        {
            targetLayerWeight = 0f;
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
        SFXManager.Instance.PlaySFXRandomPitch("tiro1");
        SFXManager.Instance.PlaySFXRandomPitch("tiro2");

        // Cria uma inst�ncia da bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Obt�m a posi��o do mouse na tela
        Vector3 mousePosition = Input.mousePosition;

        // Converte a posi��o do mouse de tela para um raio no mundo 3D
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // Se o raio atingir algo no mundo
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitAimMask))
        //if (Physics.Raycast(ray, out hit))
        {
            // Calcula a dire��o do tiro baseado na posi��o do mouse e do personagem
            Vector3 direction = (hit.point - firePoint.position).normalized;

            // Zera o componente Y da dire��o para garantir que o tiro permane�a na mesma altura
            direction.y = 0f;

            // Obt�m o componente Rigidbody da bala
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            // Define a velocidade da bala na dire��o do mouse
            rb.velocity = direction * bulletSpeed;
        }
        else
        {
            Debug.Log("Mouse n�o est� apontando para um objeto no cen�rio.");
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
    }
}