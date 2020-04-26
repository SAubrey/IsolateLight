using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTimers {
    Body body;

    // Stun
    public float normalStunTime = .35f;
    public float stunTime = .35f; // To be changed by unit.
    public Timer stunTimer;    

    // Stamina
    protected float stamRegenFrequency = .01f; 
    public Timer staminaTimer;

    // Show Damage
    public float showDamageTime = 0.4f;
    public Timer showDamageTimer;

    // Dash
    protected float dashInterval = .5f;
    public Timer dashTimer;

    public void Init(Body body) {
        this.body = body;
        showDamageTimer = new Timer(showDamageTime);
        stunTimer = new Timer(normalStunTime);
        staminaTimer = new Timer(stamRegenFrequency);
        dashTimer = new Timer(dashInterval);
    }

    public void UpdateTimers(float deltaTime) {
        if (body.showingDamage) {
            if (showDamageTimer.Update(deltaTime)) {
                body.showingDamage = false;
                body.spriteRenderer.color = Color.white;
            }
        }
        
        // Do not time what is below the stun timer when stunned.
        if (body.stunned) {
            if (stunTimer.Update(deltaTime))
                body.stunned = false;
            return;
        }

        if (body.stamina <= body.MAX_STAMINA && !body.GetSword().swinging) {
            if (staminaTimer.Update(deltaTime)) {
                body.RegenStamina(body.GetStamRegenAmount());
            }
        }

        if (body.dashing) {
            if (dashTimer.Update(deltaTime)) {
                body.dashing = false;
                body.invulnerable = false;
                body.spriteRenderer.color = body.defaultColor;
                body.trailRenderer.emitting = false;
            }
        }
    }
}

public class Timer {
    public float counterTime, threshhold;

    public Timer(float threshhold) {
        this.threshhold = threshhold;
    }

    public bool Update(float deltaTime) {
        counterTime += deltaTime;
        // Reset timer, condition has been met.
        if (counterTime >= threshhold) {
            counterTime = 0;
            return true;
        }
        return false;
    }
}
