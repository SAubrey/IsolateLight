using UnityEngine;
using UnityEngine.UI;

public class Player : Body {
    public UnityEngine.Experimental.Rendering.Universal.Light2D light2d;
    private float defaultOuterLightRadius = 3.5f;
    private float defaultInnerLightRadius = .3f;
    private Vector3 mousePos, prevMousePos;
    private Camera cam;
    private bool _alive = true;
    public bool alive { get { return _alive; } }

    private GameObject lockedOnEnemy = null;
    private float sprintMaxVel;


    void Start() {
        _isPlayer = true;
        cam = Camera.main;
        prevMousePos = new Vector3(1, 1, 1);
        Init();
        
        MAX_VEL = 3;
        sprintMaxVel = MAX_VEL * 2;
    }

    public override void Update() {
        if (!alive)
            return;
        base.Update();

        if (Input.GetMouseButtonDown(0)) { // Left click
            if (stamina > 0)
                Attack();
        } else if (Input.GetKeyDown(KeyCode.E)) {
            if (lockedOnEnemy == null) {
                lockedOnEnemy = FindNearestEnemyInSight();
            } else {
                lockedOnEnemy = null;
            }
        }
    }

    void FixedUpdate() {
        if (!alive || stunned)
            return;
        
        // Determine rotation
        if (lockedOnEnemy != null) {
            RotateTowards(lockedOnEnemy.transform.position);
        } else {
            mousePos = cam.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            Input.mousePosition.z - cam.transform.position.z));
            if (prevMousePos != mousePos) {
                // Rotate towards the mouse. 
                RotateTowards(mousePos);
                prevMousePos = mousePos;
            }
        }
        HandlePhysicsInput();
    }

    private void HandlePhysicsInput() {
        if (!Input.anyKey)
            return;

        // Dash
        if (Input.GetKey(KeyCode.Space)) {
            if (canDash) {
                Dash(StaticOperations.TargetUnitVec(body.position, 
                cam.ScreenToWorldPoint(Input.mousePosition)));

            }
        }

        // Regular movement
        if (canMove) {
            Vector2 vec = GetMovementInput();
            if (vec.magnitude > 0) {
                if (Input.GetKey(KeyCode.LeftShift) && SprintUpdate()) {
                    // sprinting
                    Move(vec, movementForce * 1.5f, sprintMaxVel);
                }
                else
                    Move(vec, movementForce, MAX_VEL);
            }
        }
    }

    private Vector2 GetMovementInput() {
        Vector2 vec = EmptyVec2;
        if (Input.GetKey(KeyCode.W))
            vec += VEC_UP;
         else if (Input.GetKey(KeyCode.S))
            vec += VEC_DOWN;
        if (Input.GetKey(KeyCode.A)) 
            vec += VEC_LEFT;
        else if (Input.GetKey(KeyCode.D)) 
            vec += VEC_RIGHT;
        return vec;
    }

    private bool SprintUpdate() {
        if (stamina >= Time.deltaTime * 20) {
            stamina -= Time.deltaTime * 20;
            return true;
        }
        return false;
    }
    
    public override void TakeDamage(int dmg) {
        base.TakeDamage(dmg);
        if (invulnerable)
            return;
        
        if (hp <= 0) {
            Die();
        }
        Stun(timers.normalStunTime);
    }

    private GameObject FindNearestEnemyInSight() {
        int mask = 1 << 9;
        // Scan for enemies with enemy layer mask.
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(
            new Vector2(transform.position.x, transform.position.y), light2d.pointLightOuterRadius, mask);

        Collider2D closestEnemy = StaticOperations.DetermineClosestCollider(enemiesInRange, 
            new Vector2(transform.position.x, transform.position.y));
        if (closestEnemy == null)
            return null;

        return closestEnemy.gameObject;
    }

    private void Die() {
        _alive = false;
        //Destroy(gameObject, 0);
        Respawn();
    }

    private void Respawn( ) {
        _alive = true;
        body.position = new Vector3(0, 0, 0);
        body.velocity = new Vector2(0, 0);
        hp = MAX_HP;
        stamina = MAX_STAMINA;
        light2d.pointLightOuterRadius -= 1f;
        
    }

    /*
        GRAPHICAL
    */
    public void ScaleLight() {
        light2d.pointLightOuterRadius = Mathf.Max(defaultOuterLightRadius * (hp / MAX_HP), 3f);
        light2d.pointLightInnerRadius = Mathf.Min(defaultInnerLightRadius * (hp / MAX_HP), 5f);

        light2d.intensity = Mathf.Max(hp / MAX_HP, .5f);
    }

    public override void UpdateHealthbar() {
        ScaleLight();
        healthbar.value = hp;

        float red = ((float)hp / (float)MAX_HP);
        healthbar.fillRect.GetComponent<Image>().color = new Color(.8f, .1f, .1f, red);
    }
}
