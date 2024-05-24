using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballShoot : MonoBehaviour
{
    public LayerMask hitAimMask;
    public Transform firePoint; // Ponto de origem do tiro
    public GameObject bulletPrefab; // Prefab da bala
    public float bulletSpeed = 20f; // Velocidade da bala
    public float maxAngle = 180f; // �ngulo m�ximo de tiro em rela��o � dire��o do personagem
    public float fireRate = 0.5f; // Taxa de disparo em tiros por segundo

    private float nextFireTime = 0f; // Tempo do pr�ximo disparo, inicializado como 0 para permitir o primeiro tiro imediatamente

    [SerializeField] private Animator animator;

    // Update is called once per frame
    void Update()
    {
        // Verifica se o bot�o esquerdo do mouse est� pressionado e se j� passou o tempo do pr�ximo disparo
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
                        // Define o tempo do pr�ximo disparo adicionando o intervalo entre os tiros
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

            // Calcula o �ngulo entre a dire��o do tiro e a dire��o para onde o personagem est� olhando
            float angle = Vector3.Angle(direction, transform.forward);

            // Verifica se o �ngulo est� dentro do intervalo permitido
            if (angle <= maxAngle)
            {
                // Obt�m o componente Rigidbody da bala
                Rigidbody rb = bullet.GetComponent<Rigidbody>();

                // Define a velocidade da bala na dire��o do mouse
                rb.velocity = direction * bulletSpeed;
            }
            else
            {
                Debug.Log("Tiro bloqueado! O �ngulo de tiro est� al�m do limite permitido.");
                Destroy(bullet); // Destruir a bala porque est� fora do limite
            }
        }
        else
        {
            Debug.Log("Mouse n�o est� apontando para um objeto no cen�rio.");
        }
    }
}