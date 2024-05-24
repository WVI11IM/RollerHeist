using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballShoot : MonoBehaviour
{
    public LayerMask hitAimMask;
    public Transform firePoint; // Ponto de origem do tiro
    public GameObject bulletPrefab; // Prefab da bala
    public float bulletSpeed = 20f; // Velocidade da bala
    public float maxAngle = 180f; // Ângulo máximo de tiro em relação à direção do personagem
    public float fireRate = 0.5f; // Taxa de disparo em tiros por segundo

    private float nextFireTime = 0f; // Tempo do próximo disparo, inicializado como 0 para permitir o primeiro tiro imediatamente

    [SerializeField] private Animator animator;

    // Update is called once per frame
    void Update()
    {
        // Verifica se o botão esquerdo do mouse está pressionado e se já passou o tempo do próximo disparo
        if (Input.GetButton("Fire1"))
        {
            Vector3 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitAimMask))
            //if (Physics.Raycast(ray, out hit))
            {
                Vector3 direction = (hit.point - firePoint.position);
                direction.y = 0f;
                float angle1 = Vector3.Angle(direction, transform.right);
                float angle2 = Vector3.Angle(direction, transform.forward);
                if (angle1 < 180 && angle1 > 0 && angle2 < 90)
                {
                    animator.SetLayerWeight(1, 1);
                    animator.SetFloat("lookAngle", Mathf.Lerp(0, 1, angle1/180));
                    if (Time.time >= nextFireTime)
                    {
                        Shoot();
                        // Define o tempo do próximo disparo adicionando o intervalo entre os tiros
                        nextFireTime = Time.time + 1f / fireRate;
                    }
                }
                else
                {
                    animator.SetLayerWeight(1, 0);
                }
            }
            else
            {
                animator.SetLayerWeight(1, 0);
            }
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }
    }

    void Shoot()
    {
        SFXManager.Instance.PlaySFXRandomPitch("tiro");

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
            Vector3 direction = (hit.point - firePoint.position).normalized;

            // Zera o componente Y da direção para garantir que o tiro permaneça na mesma altura
            direction.y = 0f;

            // Calcula o ângulo entre a direção do tiro e a direção para onde o personagem está olhando
            float angle = Vector3.Angle(direction, transform.forward);

            // Verifica se o ângulo está dentro do intervalo permitido
            if (angle <= maxAngle)
            {
                // Obtém o componente Rigidbody da bala
                Rigidbody rb = bullet.GetComponent<Rigidbody>();

                // Define a velocidade da bala na direção do mouse
                rb.velocity = direction * bulletSpeed;
            }
            else
            {
                Debug.Log("Tiro bloqueado! O ângulo de tiro está além do limite permitido.");
                Destroy(bullet); // Destruir a bala porque está fora do limite
            }
        }
        else
        {
            Debug.Log("Mouse não está apontando para um objeto no cenário.");
        }
    }
}