
using System;
using System.Collections.Generic;
using Items.Scripts;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class LivingEntity : MonoBehaviour
{
    public static Func<GameObject, GameObject, float, float, Color> DoEntityCollision;

    [Header("Living Entity Settings")]
    [SerializeField] protected InventoryItem equippedItem;
    [SerializeField] protected Rigidbody2D rigidBody;
    [SerializeField] protected ParticleSystem deathParticles;
    public Color deathParticlesColor;
    [SerializeField] protected HealthBar healthBar;

    [SerializeField] protected float maxHealth;
    protected float CurrentHealth;
    
    [Tooltip("The time, in seconds, that the entity will remain in combat for after combat has ended")]
    public float combatCooldown;
    public float healthRegenRate;

    private Dictionary<ItemUseData, object> _itemUseData;
    public Dictionary<ItemUseData, object> ItemUseData => _itemUseData ??= new Dictionary<ItemUseData, object>();
    
    private bool _isDeathParticlesNull;
    private bool _hasHealthBar;
    protected float CombatTimer;
    protected float Immunity;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        _isDeathParticlesNull = deathParticles == null;
        _hasHealthBar = healthBar != null;
        if(_hasHealthBar)
            healthBar.SetMaxHealth(maxHealth);
    }

    private void OnEnable()
    {
        DoEntityCollision += GameObjectEntityCollision;
    }

    private void OnDestroy()
    {
        DoEntityCollision -= GameObjectEntityCollision;
    }

    public void SetItemUseData(ItemUseData key, object value)
    {
        if (ItemUseData.ContainsKey(key))
            ItemUseData[key] = value;
        else
            ItemUseData.Add(key, value);
    }

    public object GetItemUseDate(ItemUseData key)
    {
        if (ItemUseData.TryGetValue(key, out var data))
            return data;
        return -1;
    }

    protected void CheckHealth()
    {
        if(CurrentHealth <= 0)
           Kill();
    }

    protected Color GameObjectEntityCollision(Object entity, GameObject source, float damage, float immunityAmount)
    {
        if (Immunity > 0)
            return deathParticlesColor;
        
        if (gameObject == entity)
        { 
            SetImmunity(immunityAmount);
            TakeDamage(damage);
            EnableCombat();
            return deathParticlesColor;
        }

        if (gameObject == source)
        {
            EnableCombat();
        }

        return new Color();
    }

    protected abstract void Kill();

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if(_hasHealthBar)
            healthBar.TakeDamage(damage);
    }

    protected void PlayDeathParticles(Vector3 position, Color color)
    {
        if (_isDeathParticlesNull) return;

        var particles = Instantiate(deathParticles, position, Quaternion.identity);
            
        // Colors...
        /*var renderer = particles.GetComponent<Renderer>();
        renderer.material.color = color;
        renderer.materials[1].color = color;*/
        var particlesMain = particles.main;
        particlesMain.startColor = color;
            
        particles.Play();
    }

    public void EnableCombat()
    {
        CombatTimer = combatCooldown;
    }

    public void SetImmunity(float amount)
    {
        Immunity = amount;
    }

    protected void UpdateTimers()
    {
        if (CombatTimer > 0)
        {
            CombatTimer -= Time.deltaTime;
        }

        if (Immunity > 0)
        {
            Immunity -= Time.deltaTime;
        }
    }
    
    protected void RegenerateHealth()
    {
        if (CombatTimer > 0 || !(CurrentHealth < maxHealth)) return;
            
        var addHealth = healthRegenRate * Time.deltaTime;
        if (CurrentHealth + addHealth > maxHealth)
            addHealth = maxHealth - CurrentHealth;
                
        CurrentHealth += addHealth;
        healthBar.TakeDamage(-addHealth);
    }
}