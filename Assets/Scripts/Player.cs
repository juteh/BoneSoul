using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Config
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float climbSpeed = 5.0f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    // State
    bool isAlive = true;
    private int jumps = 0;
    private bool isTouchingGround = false;
    private bool isTouchingHazards = false;

    // Cache
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    PlayerCombat myPlayerCombat;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeetCollider2D;
    float gravityScaleAtStart;
    AnimationHandler myAnimationHandler;



    // attack
    [SerializeField] float attackRate = 1.0f;

    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    public int attackDamage = 10;
    bool isAttacking = false;

    void Start() {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myPlayerCombat = GetComponent<PlayerCombat>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        myAnimationHandler = FindObjectOfType<AnimationHandler>();
    }

    // Update is called once per frame
    void Update() {
        if (!isAlive) { return; }
        isTouchingGround = myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Forground"));
        isTouchingHazards = myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards"));
        Run();
        Jump();
        FlipSprite();
        Die();
    }

    private void Run() {
        var controlThrow = Input.GetAxis("Horizontal"); ;
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        if (isTouchingGround && !myPlayerCombat.IsAttacking())      
        {
            if (Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon)
            {
                myAnimationHandler.ChangeAnimationState(AnimationName.PLAYER_RUN, myAnimator);
            }
            else 
            {
                myAnimationHandler.ChangeAnimationState(AnimationName.PLAYER_IDLE, myAnimator);
            }
        }
    }

    private void FlipSprite() {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

    private void Jump() {
        if (!isTouchingGround)
        {
            // Make DoubleJump
            if (Input.GetButtonDown("Jump") && jumps < 1)
            {
                jumps++;
                Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
                // myRigidBody.velocity += jumpVelocityToAdd;
                myRigidBody.velocity = jumpVelocityToAdd;
            }
            if (!myPlayerCombat.IsAttacking())
            {
                if (myRigidBody.velocity.y > Mathf.Epsilon)
                {
                    myAnimationHandler.ChangeAnimationState(AnimationName.PLAYER_JUMP_UP, myAnimator);
                }
                else
                {
                    myAnimationHandler.ChangeAnimationState(AnimationName.PLAYER_JUMP_DOWN, myAnimator);
                }
            }

            return;
        }
        if (Input.GetButtonDown("Jump")) {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            // myRigidBody.velocity += jumpVelocityToAdd;
            myRigidBody.velocity = jumpVelocityToAdd;
        }
        jumps = 0;
    }

    private void Die()
    {
        if (isTouchingHazards)
        {
            isAlive = false;
            myAnimator.SetTrigger("dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
            myAnimationHandler.ChangeAnimationState(AnimationName.PLAYER_DEATH, myAnimator);
            // FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
