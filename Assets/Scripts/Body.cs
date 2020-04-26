using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
public abstract class Body : MonoBehaviour {
    protected static Vector2 VEC_UP = new Vector2(0, 1f);
    protected static Vector2 VEC_DOWN = new Vector2(0, -1f);
    protected static Vector2 VEC_RIGHT = new Vector2(1f, 0);
    protected static Vector2 VEC_LEFT = new Vector2(-1f, 0);
    public static Vector2 EmptyVec2 = new Vector2(0, 0);
    public const int ALIVE = 0;
    public const int INJURED = 1;
    public const int DEAD = 2;
    
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;
    protected Quaternion targetRotation;
    public TrailRenderer trailRenderer;
    protected BodyTimers timers = new BodyTimers();
    public Color defaultColor = Color.white;
    public bool invulnerable = false;
    public bool attacking = false;

    // Movement
    protected float movementForce = 10f;
    protected float dashForce = 250f;
    public bool dashing = false;
    public bool stunned = false;
    public float attAnimSpeedMultiplier = 1f;
   
    // Health
    public Slider healthbar;
    private Color healthbarFillColor = new Color(1, 1, 1, .8f);
    public Color healthbarBGColor;
    protected int healthbarUnitWidth = 1;
    protected float MAX_VEL = 3;
    protected bool _isEnemy, _isPlayer = false;
    protected float MAX_HP = 100f;
    protected float hp = 100f;
    public bool showingDamage = false;

    // Stamina
    public Slider staminaBar;
    public float MAX_STAMINA = 100f;
    public float stamina;
    protected float stamRegenAmount = .25f;
    protected float stamDashCost = 30f;

    protected Sword sword;

    public bool canDash {
        get { return !dashing && stamina >= stamDashCost && canMove; }
    }

    public bool canMove {
        get { return !attacking; }
    }

    // Called at the beginning of a subclasses Start method.
    protected void Init() {
        body = gameObject.GetComponent<Rigidbody2D>();
        sword = GetComponentInChildren<Sword>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        hp = MAX_HP;
        stamina = MAX_STAMINA;
        healthbar.maxValue = MAX_HP;
        staminaBar.maxValue = MAX_STAMINA;
        
        timers.Init(this);
        UpdateHealthbar();
        UpdateStamina();
    }

    public virtual void Update() {
        timers.UpdateTimers(Time.deltaTime);
        if (showingDamage) {
            ShowDamage();
        }
    }

    protected void Move(Vector2 vec, float addedForce, float maxVel) {
        Vector2 vel = body.velocity;
        Vector2 absVel = new Vector2(Mathf.Abs(vel.x), Mathf.Abs(vel.y));
        if (absVel.x >= maxVel)
            vec.x = 0;
        if (absVel.y >= maxVel)
            vec.y = 0;

        vec.Normalize();
        vec *= addedForce;
        //body.AddRelativeForce(vec);
        body.AddForce(vec);
    } 

    protected void Dash(Vector2 vec) {
        if (!canDash || vec.magnitude == 0)
            return;
        dashing = true;
        invulnerable = true;
        Color c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.r, c.b, .5f); // transparency
        trailRenderer.emitting = true;
        Move(vec, dashForce, MAX_VEL + dashForce + 1);
        stamina -= stamDashCost; 
    }
    
    protected void Attack() {
        if (sword) {
            sword.Swing();
            stamina = Mathf.Max(stamina - sword.swingCost, 0);
            UpdateStamina();
        }
    }

    // Disables input for some length of time.
    protected void Stun(float stunTime) {
        timers.stunTime = stunTime;
        stunned = true;
    }
    
    public virtual void TakeDamage(int damage) {
        if (invulnerable)
            return;
        hp -= damage;
        UpdateHealthbar();
        showingDamage = true;
    }

    protected void RotateTowards(Vector3 target) {
        if (!canMove)
            return;
        targetRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2((target.y - transform.position.y), 
            (target.x - transform.position.x)) * Mathf.Rad2Deg - 90));
        
        // Lerp smooths and thus limits rotation speed.
        float str = Mathf.Min(5 * Time.deltaTime, 1);
        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, targetRotation, str);
    }
    
    public void RegenStamina(float amount) {
        stamina += StaticOperations.GetAdjustedIncrease(stamina, amount, MAX_STAMINA);
        UpdateStamina();
    }

    public float GetStamRegenAmount() {
        float vel = Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.y);
        // Double regeneration when not moving.
        return vel < MAX_VEL / 10 ? stamRegenAmount * 2 : stamRegenAmount;
    }
    
    public void UpdateStamina() {
        staminaBar.maxValue = MAX_STAMINA;
        staminaBar.value = stamina;

        float green = ((float)stamina / (float)MAX_STAMINA);
        staminaBar.fillRect.GetComponent<Image>().color = new Color(.1f, .8f, .1f, green);
    }

    public bool isEnemy { get { return _isEnemy; } }
    public bool isPlayer { get { return _isPlayer; } }

    public Sword GetSword() {
        return sword;
    }

    /*
        GRAPHICAL
    */
    public virtual void UpdateHealthbar() {
        healthbar.value = hp;

        float red = ((float)hp / (float)MAX_HP);
        healthbar.fillRect.GetComponent<Image>().color = new Color(1f, .1f, .1f, red);
    }

    protected void SizeHealthbar(float maxHP) {
        // Adjust width based on max hp.
        RectTransform t = healthbar.transform as RectTransform;
        t.sizeDelta = new Vector2(healthbarUnitWidth * maxHP, t.sizeDelta.y);
    }

    protected void SizeStambar(float maxStam) {
        // Adjust width based on max stamina.
        RectTransform t = staminaBar.transform as RectTransform;
        t.sizeDelta = new Vector2(healthbarUnitWidth * maxStam, t.sizeDelta.y);
    }

    public void ShowDamage() {
        // Assumes that default color is white.
        float red = (timers.showDamageTimer.counterTime / 
            timers.showDamageTimer.threshhold);  
        spriteRenderer.color = new Color(1f, red, red);
    }
}
