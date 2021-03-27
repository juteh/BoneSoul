using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Config
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] int maxAirJumps = 1;
    [SerializeField] Transform respawnPoint = null;


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
    AnimationHandler myAnimationHandler;

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myPlayerCombat = GetComponent<PlayerCombat>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        myAnimationHandler = FindObjectOfType<AnimationHandler>();
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (!isAlive) {
            // in death -> freeze position
            myRigidBody.velocity = Vector2.zero;
            if (myRigidBody.gravityScale != 0f)
            {
                myRigidBody.gravityScale = 0f;
            }
            return; 
        }
        if (myRigidBody.gravityScale != 1f)
        {
            myRigidBody.gravityScale = 1f;
        }
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
            // Make AirJump
            if (Input.GetButtonDown("Jump") && jumps < maxAirJumps)
            {
                jumps++;
                Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
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
            myRigidBody.velocity = jumpVelocityToAdd;
        }
        jumps = 0;
    }

    private void Die()
    {
        if (isTouchingHazards)
        {
            isAlive = false;
            myAnimationHandler.ChangeAnimationState(AnimationName.PLAYER_DEATH, myAnimator);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() {
        yield return new WaitForSeconds(1f);
        gameObject.transform.position = respawnPoint.position;
        // delay of buggy respawn. sometimes after respawn the player hit the hazard again and dies again
        yield return new WaitForSeconds(0.1f);
        isAlive = true;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
