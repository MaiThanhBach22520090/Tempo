using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health and Shield Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int maxShield = 5;
    [SerializeField] private int shieldRegenRate = 1;
    [SerializeField] private int shieldRegenNeeded = 20;
    [SerializeField] private Transform healthVisual;
    [SerializeField] private Transform shieldVisual;
    [SerializeField] private bool allowInvincibility = false;
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private float flickerInterval = 0.1f; // Interval between flickers
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int currentHealth;
    private int currentShield;
    private float regenTimer;
    private int regenCharge;
    private CapsuleCollider collider;
    private bool isInvincible = false;

    // Delegate and event for player death
    public delegate void PlayerDieHandler();
    public event PlayerDieHandler OnPlayerDeath;

    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int CurrentShield { get => currentShield; set => currentShield = value; }
    public int RegenCharge { get => regenCharge; set => regenCharge = value; }

    private void Start()
    {
        currentHealth = maxHealth;
        currentShield = 0;
        collider = GetComponent<CapsuleCollider>();

        UpdateHealthVisual();
        UpdateShieldVisual();
    }

    private void Update()
    {
        PassiveRegenShield();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        SetInvincibility(true);

        if (currentShield > 0)
        {
            currentShield -= damage;

            if (currentShield < 0)
            {
                currentHealth += currentShield; // This subtracts the remaining negative shield value from health
                currentShield = 0;
                UpdateHealthVisual();
            }

            UpdateShieldVisual();
        }
        else
        {
            currentHealth -= damage;
            UpdateHealthVisual();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (gameObject.GetComponent<Player>() != null)
        {
            OnPlayerDeath?.Invoke();
            gameObject.SetActive(false);
            return;
        }
        Destroy(gameObject);
    }

    public void AddShield(int shield)
    {
        currentShield += shield;
        if (currentShield > maxShield)
        {
            currentShield = maxShield;
        }
        UpdateShieldVisual();
    }

    public void AddHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthVisual();
    }

    private void PassiveRegenShield()
    {
        if (currentShield < maxShield)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= shieldRegenRate)
            {
                AddShield(1);
                regenTimer = 0;
            }
        }
    }

    public void IncreaseRegenCharge()
    {
        regenCharge++;
        if (regenCharge >= shieldRegenNeeded)
        {
            regenCharge = 0;
            AddShield(1);
        }
    }

    public void SetInvincibility(bool value)
    {
        if (allowInvincibility)
        {
            StartCoroutine(InvincibilityRoutine(value));
        }
    }

    private IEnumerator InvincibilityRoutine(bool value)
    {
        isInvincible = value;
        if (collider != null)
        {
            collider.enabled = !value;
        }

        if (value)
        {
            float elapsedTime = 0f;

            while (elapsedTime < invincibilityTime)
            {
                // Toggle sprite renderer visibility to create flicker effect
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }

                yield return new WaitForSeconds(flickerInterval);

                elapsedTime += flickerInterval;
            }

            // Ensure sprite renderer is enabled after invincibility period
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            isInvincible = false;

            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    private void UpdateHealthVisual()
    {
        if (healthVisual != null)
        {
            healthVisual.localScale = new Vector3(1, (float)currentHealth / maxHealth, 1);
        }
    }

    private void UpdateShieldVisual()
    {
        if (shieldVisual != null)
        {
            shieldVisual.localScale = new Vector3(1, (float)currentShield / maxShield, 1);
        }
    }
}
