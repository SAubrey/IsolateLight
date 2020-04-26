using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sword : MonoBehaviour {
    public Body owner;
    private bool _swinging = false;
    public bool swinging {
        get { return _swinging; }
        set {
            _swinging = value;
            owner.attacking = value;
        }
    }
    private Animator animator;
    public BoxCollider2D collider2d;
    public TrailRenderer trailRenderer;
    private int leftClickHash = Animator.StringToHash("Left Click");
    protected int damage = 10;
    public int swingCost = 30;
    protected float swingTime = 0.38f;
    protected float swingTimeCounter = 0;
    private float animSpeed = 1f;

    // Note that the total length from the center of the unit to the target
    // is calculated from centerpoints and thus 
    // includes roughly half of the unit's length + full weapon length;
    // Enemy attacks should be calculated such that they will attack when
    // more than just the tip can reach the player.
    public float length;

    void Start() {
        Init();
        //animator.GetCurrentAnimatorStateInfo.
    }

    protected void Init() {
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
        collider2d.enabled = false;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        owner = GetComponentInParent<Body>();
        swingTime *= owner.attAnimSpeedMultiplier; // ??
        Debug.Log("swing time for " + owner + ": " + swingTime);
        animator.speed = animSpeed * owner.attAnimSpeedMultiplier;

        length = GetComponent<SpriteRenderer>().bounds.size.y;
        Debug.Log("sword length: " + length);
    }

    void FixedUpdate() {
        UpdateSwingTimer();
    }

    protected void UpdateSwingTimer() {
        if (swinging) {
            swingTimeCounter += Time.deltaTime;
            if (swingTimeCounter > swingTime) {
                //DoneSwinging();
                swingTimeCounter = 0;
            }
        }
    }

    public void Swing() {
        collider2d.enabled = true;
        swinging = true;
        trailRenderer.emitting = true;
        animator.SetTrigger(leftClickHash);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        Body b = col.GetComponentInParent<Body>();
        if (b == null)
            return;
        if (b.isEnemy && !owner.isEnemy) {
            b.TakeDamage(damage);
        } else if (b.isPlayer && !owner.isPlayer) {
            b.TakeDamage(damage);
        }
    }

    public void DoneSwinging() {
        Debug.Log("done");
        swinging = false;
        collider2d.enabled = false;
        trailRenderer.emitting = false;
    }

    public UnityEvent OnLandEvent;
}
