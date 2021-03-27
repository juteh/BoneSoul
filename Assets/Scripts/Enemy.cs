using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Config
    [SerializeField] int maxHealth = 1;
    [SerializeField] float fadingTime = 3f;

    // State
    private int currentHealth;

    // Cache
    Animator enemyAnimator;
    AnimationHandler myAnimationHandler;

    void Start()
    {
        currentHealth = maxHealth;
        enemyAnimator = GetComponent<Animator>();
        myAnimationHandler = FindObjectOfType<AnimationHandler>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die()
    {
        myAnimationHandler.ChangeAnimationState("ENEMY_DIE", enemyAnimator);
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
        StartCoroutine(Fading());
    }

    // enemy lies around and fading after time away
    IEnumerator Fading()
    {
        yield return new WaitForSeconds(fadingTime);
        Destroy(gameObject);
    }
}
