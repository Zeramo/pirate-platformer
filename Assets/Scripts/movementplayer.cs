using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementplayer : MonoBehaviour
{
    public bool drawRaycasts;

    [Header("Animator Variables")]
    public Animator animator;
    private bool isDashing = false;
    private bool animationDashing = false;
    private bool isStabbing = false;

    [Header("Player Properties")]
    public int playerSpeed = 10;
    public int playerJumpPower = 750;
    public float playerDashPower = 15f;
    private float dashTime;
    public float startDashTime;
    public int playerHealth = 1;
    public int meleeDamage = 1;
    public int shotDamage = 2;
    public float maxShootingDistance = 25f;

    private float moveX;

    [Header("Player Status")]
    public bool isGrounded;
    public bool doubleJump;                 //Player has another jump available mid-air

    [Header("Collision Checks")]
    public float coyote = .08f;
    public float groundBuffer = .15f;
    public float footOffset = .41f;         //Adjust, if Box Collider X size is changed
    public LayerMask platformsLayer;
    public LayerMask enemyLayer;

    float playerHeight;

    BoxCollider2D boxCollider;
    BoxCollider2D swordCollider;
    Rigidbody2D rigidBody;
    int direction = 1;
    float spawnXScale;
    private BoxCollider2D boxCollider2d;

    GameObject[] goArray;
    ArrayList enemies = new ArrayList();

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        swordCollider = GetComponentInChildren<BoxCollider2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();

        platformsLayer = LayerMask.GetMask("Platforms");
        enemyLayer = LayerMask.GetMask("Enemies");

        spawnXScale = transform.localScale.x;

        dashTime = startDashTime;
        isGrounded = true;
        doubleJump = false;

        playerHeight = boxCollider.size.y;

        //If enemies are spawned while the game is running, move to Update
        EnemiesCheck();
    }

    
    void Update()
    {
        CheckJumpOrDash();
        CheckShootOrMelee();
        AnimatePlayer();
        if (isDashing)
        {
            animationDashing = false;
            Dash();
        }
            
    }

    void FixedUpdate()
    {
        GroundedCheck();
        MovementXAxis();
    }

    void CheckJumpOrDash(){


        if(Input.GetButtonDown("Dash") && isGrounded == true)
        {
            doubleJump = false;
            animationDashing = true;
            Dash();
        } else if (Input.GetButtonDown("Dash") && doubleJump)
        {
            doubleJump = false;
            animationDashing = true;
            Dash();
        } else if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            doubleJump = true;
            Jump();
        } else if (Input.GetButtonDown("Jump") && doubleJump)
        {
            doubleJump = false;
            Jump();
        }
    }

    void CheckShootOrMelee(){
        if(Input.GetButtonDown("Melee") && isDashing == false)
        {
            Debug.Log("Melee");
            isStabbing = true;
        } else if (Input.GetButtonDown("Shoot") && isDashing == false)
        {
            Debug.Log("shooting");
            isStabbing = false;
            Shoot();
        } else{
            isStabbing = false;
        }
        
    }

    void GroundedCheck()
    {
        /*
        //Default value is false, we want to check every update cycle
        isGrounded = false;

        //Calculate Raycast for both feet, with footOffset and half player height as offset
        RaycastHit2D leftLegHit = MyRaycast(new Vector2(-footOffset, -(playerHeight/2)), Vector2.down, groundBuffer, platformsLayer);
        RaycastHit2D rightLegHit = MyRaycast(new Vector2(footOffset, -(playerHeight/2)), Vector2.down, groundBuffer, platformsLayer);
        
        //If one foot touches the ground, player is considered on the ground
        if (leftLegHit || rightLegHit)
        {
            isGrounded = true;
        }
        */       

        float extraHeightText = 0.51f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeightText, platformsLayer);

        Color rayColor;
        if (raycastHit.collider != null) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y + extraHeightText), Vector2.right * (boxCollider2d.bounds.extents.x * 2f), rayColor);

        if(raycastHit.collider != null){
            isGrounded = true;
        } else{
            isGrounded = false;
        }
              
    }

    void EnemiesCheck()
    {
        goArray = FindObjectsOfType<GameObject>();
        enemies = new ArrayList();
        Debug.Log(goArray.Length + " total game objects");
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].name.Equals("Colliders"))
                enemies.Add(goArray[i]);
        }
        Debug.Log(enemies.Count + " enemies detected");
    }

    void MovementXAxis(){


        if(rigidBody.velocity.x < -10.0 || rigidBody.velocity.x > 10.0 )
        {
            Debug.Log("End Dash");
        }
        if(!isDashing)  //Move on x Axis if Dash is Over
        {
            //get input speed
            moveX = Input.GetAxis("Horizontal");
            //Player direction update
            if (moveX * direction < 0)
                FlipPlayer();

            //Physics
            rigidBody.velocity = new Vector2 (moveX * playerSpeed, rigidBody.velocity.y);

        }
    }

    void AnimatePlayer() {
        animator.SetFloat("SpeedX", Mathf.Abs(Input.GetAxis("Horizontal")));
        animator.SetFloat("SpeedY", Mathf.Abs(rigidBody.velocity.y));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("animationDashing", animationDashing);
        animator.SetBool("isStabbing", isStabbing);
    }

    void Jump(){
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.AddForce(Vector2.up * playerJumpPower);
    }

    void Dash(){
        if (dashTime <= 0)
        {
            dashTime = startDashTime;
            isDashing = false;
            rigidBody.velocity = Vector2.zero;
            return;
        }           
        else
        {
            isDashing = true;
            dashTime -= Time.deltaTime;

            rigidBody.velocity = new Vector2(direction, 0) * playerDashPower;
        }
    }

    void Shoot()
    {
        //Check enemies again, in case any have died
        EnemiesCheck();
        CameraShaker.Instance.ShakeCamera(3f, .075f);
        Vector2 enemyDir;
        float distance = maxShootingDistance;
        GameObject closestEnemy = null;
        Vector2 closestEnemyDir = Vector2.zero;
        foreach (GameObject enemy in enemies)
        {
            enemyDir = new Vector2(enemy.transform.position.x - this.transform.position.x, enemy.transform.position.y - this.transform.position.y);
            if (enemyDir.magnitude < distance)
            {
                distance = enemyDir.magnitude;
                Debug.Log("New closest enemy found");
                closestEnemy = enemy;
                closestEnemyDir = enemyDir;
            }                
        }
        if (closestEnemy)
        {
            Vector2 offset = new Vector2(.3f, .5f);
            RaycastHit2D hitObstacle = MyRaycast(offset, closestEnemyDir, distance, platformsLayer);
            Color color = hitObstacle ? Color.red : Color.green;
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y) + offset, closestEnemyDir, color, 5f);
            if (hitObstacle)
                return;

            Debug.Log(closestEnemy.name);
            closestEnemy.GetComponentInParent<SwordfishBehavior>().EnemyTakeDamage(shotDamage);
        }
    }

    void FlipPlayer()
    {
        //Flip direction
        direction *= -1;
        //Get current scale
        Vector3 scale = transform.localScale;
        //Set x scale to scale recorded on spawn times the direction
        scale.x = spawnXScale * direction;
        //Apply scale
        transform.localScale = scale;
    }

    public void PlayerTakeDamage(int damage, Collider2D col)
    {
        playerHealth -= damage;

        //If player health is above 0, do nothing else
        if (playerHealth > 0)
            return;
        
        //Invoke(nameof(DestroyPlayer), .5f);
        gameObject.SetActive(false);
        
    }

    void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col){
        //Debug.Log("Player has collided with " + col.collider);
        if(col.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            Debug.Log("Player has collided with enemy");
         
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            //Debug.Log("Player has collided with platforms");
        }
        
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Enemies") && (animator.GetCurrentAnimatorStateInfo(0).IsName("pirate_stab") || animator.GetCurrentAnimatorStateInfo(0).IsName("pirate_dash")))
        {
            Debug.Log("A trigger has collided with " + col.gameObject.name);
            col.gameObject.GetComponentInParent<SwordfishBehavior>().EnemyTakeDamage(meleeDamage);
        }
    }
    

    RaycastHit2D MyRaycast(Vector2 offset, Vector2 direction, float length, LayerMask mask)
    {
        //Get position of player
        Vector2 pos = transform.position;
        //Calculate Raycast hit with offset from pos
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, direction, length, mask);

        //REMOVE after game debugging is finished
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, direction * length, color);
        
        return hit;
    }
}