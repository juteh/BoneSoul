using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // Config
    [SerializeField] float attackRate = 1.0f;
    [SerializeField] Transform attackPoint = null;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayers = 0;

    // State
    public int attackDamage = 10;
    bool isAttacking = false;

    // Cache
    Animator playerAnimator;
    AnimationHandler myAnimationHandler;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        myAnimationHandler = FindObjectOfType<AnimationHandler>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                Attack();
                Invoke("AttackComplete", attackRate);
            }
        }        
    }
    void Attack()
    {
        myAnimationHandler.ChangeAnimationState(AnimationName.PLAYER_ATTACK, playerAnimator);
        // detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    void AttackComplete()
    {
        isAttacking = false;
    }

    // draw sphere to see the attackrange
    private void OnDrawGizmosSelected()
    {
        if (!attackPoint) { return; }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}
