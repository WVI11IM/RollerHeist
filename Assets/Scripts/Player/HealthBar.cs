using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar Properties")]
    public Animator healthBarAnimator;
    public Image healthMeter;
    public Image whiteHealthMeter;
    public Image meterValueLost;
    public float maxHealth = 100f;
    public float health;
    public float invincibilityDuration = 2f; // Duração da invencibilidade em segundos

    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isInvincible = false;
    private float invincibilityTimer = 0f;

    [Space]
    public GameManager gameManager;
    MovementTest2 playerMovement;
    PaintballShoot playerShoot;
    public Animator animator;

    [Space]
    [Header("Player Materials")]
    public SkinnedMeshRenderer sMR;
    public Material hurtMaterial;
    public Material invincibleMaterial;
    Material currentSkin;

    [Space]
    public Volume damageVolume;

    private CinemachineImpulseSource impulseSource;


    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        
        damageVolume.weight = 0f;
        health = maxHealth;
        playerMovement = GetComponent<MovementTest2>();
        playerShoot = GetComponent<PaintballShoot>();
        StartCoroutine(LateStart(0.5f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentSkin = sMR.material;
    }

    void Update()
    {
        //Ativa o cheat com a tecla P
        if (Input.GetKeyDown(KeyCode.P))
        {
            maxHealth = 10000;
            health = maxHealth;
        }

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }

        float healthSliderValue = health / maxHealth;
        healthMeter.fillAmount = Mathf.Lerp(0.4f, 0.6f, healthSliderValue);
        whiteHealthMeter.fillAmount = Mathf.Lerp(0.4f, 0.6f, healthSliderValue);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            animator.SetBool("isFainted", true);
            animator.SetTrigger("fainted");
            playerMovement.enabled = false;
            playerShoot.enabled = false;
            animator.SetLayerWeight(1, 0);
            gameManager.UpdateGameState(GameState.Lose);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible && !isDead)
        {
            impulseSource.GenerateImpulse();

            ObjectiveManager.Instance.hasTakenDamage = true;
            animator.SetTrigger("flinched");
            healthBarAnimator.SetTrigger("valueLost");
            SFXManager.Instance.PlaySFX("dano");
            float healthSliderValue = health / maxHealth;
            meterValueLost.fillAmount = Mathf.Lerp(0.4f, 0.6f, healthSliderValue);
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);
            if (health > 0)
            {
                StartCoroutine(HurtSkinChange());
            }
            StartCoroutine(HurtDamageVolume());
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    IEnumerator HurtSkinChange()
    {
        sMR.material = hurtMaterial;
        yield return new WaitForSeconds(invincibilityDuration / 8);
        sMR.material = invincibleMaterial;
        yield return new WaitForSeconds(invincibilityDuration / 8);

        float firstPhaseDuration = invincibilityDuration / 2;
        float secondPhaseDuration = invincibilityDuration / 2;

        for (float t = 0; t < firstPhaseDuration; t += invincibilityDuration / 8)
        {
            sMR.material = (sMR.material == currentSkin) ? invincibleMaterial : currentSkin;
            yield return new WaitForSeconds(invincibilityDuration / 8);
        }

        for (float t = 0; t < secondPhaseDuration; t += invincibilityDuration / 16)
        {
            sMR.material = (sMR.material == currentSkin) ? invincibleMaterial : currentSkin;
            yield return new WaitForSeconds(invincibilityDuration / 16);
        }

        sMR.material = currentSkin;
    }

    IEnumerator HurtDamageVolume()
    {
        for (int t = 0; t < 5; t++)
        {
            damageVolume.weight = t / 5f;
            yield return new WaitForSeconds(0.015f);
        }
        for (int t = 0; t < 10; t++)
        {
            damageVolume.weight = 1f - (t / 10f);
            yield return new WaitForSeconds(0.015f);
        }
        damageVolume.weight = 0;
    }
}
