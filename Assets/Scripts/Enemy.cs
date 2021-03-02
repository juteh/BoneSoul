using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int maxHealth = 1;
    private int currentHealth;
    [SerializeField] Animator animator;
    [SerializeField] float fadingTime = 3f;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("IsDead", true);
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
