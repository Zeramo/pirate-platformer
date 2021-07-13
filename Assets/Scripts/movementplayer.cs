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
    private bool hasBeenDamaged = false;

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
    public float startInvincibilityTime;
    private float invincibilityTime;
    public int gold;

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

    public AudioManager audioManager;

    GameObject[] goArray;
    ArrayList enemies = new ArrayList();

    void Start()
    {
        //Get colliders
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        swordCollider = GetComponentInChildren<BoxCollider2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();

        //Get layers
        platformsLayer = LayerMask.GetMask("Platforms");
        enemyLayer = LayerMask.GetMask("Enemies");

        //Get spawn direction, to later determine whether the player needs to be flipped
        spawnXScale = transform.localScale.x;

        //StartDashTime holds total dash time, dashTime how much time there is left in the dash
        dashTime = startDashTime;

        //startInvincibilityTime holds total invincibilityTime, called when the player gets damaged
        invincibilityTime = startInvincibilityTime;

        //At the start, assume player is grounded and has no double jump 
        isGrounded = true;
        doubleJump = false;

        playerHeight = boxCollider.size.y;

        //If enemies are spawned while the game is running, move to Update
        EnemiesCheck();
    }

    
    void Update()
    {
        if (GameManager.IsGameOver())
            return;

        CheckJumpOrDash();
        CheckShootOrMelee();
        AnimatePlayer();         
    }

    void FixedUpdate()
    {
        if (GameManager.IsGameOver())
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0f;
            return;
        }
            

        GroundedCheck();
        MovementXAxis();
    }

    void CheckJumpOrDash(){
        //If player is currently dashing, disable animation trigger and continue the dash
        if (isDashing)
        {
            animationDashing = false;
            Dash();
        }
        //If player starts a dash, and player is on the ground, double jumping becomes
        //unavailable. Start dashing animation and dash movement
        if (Input.GetButtonDown("Dash") && isGrounded == true)
        {
            doubleJump = false;
            animationDashing = true;
            Dash();

            //plays according audio cue
            audioManager.Play("playerDash");
        }
        //If player is in the air and wants to dash, disable double jump, start dashing animation and movement
        else if (Input.GetButtonDown("Dash") && doubleJump)
        {
            doubleJump = false;
            animationDashing = true;
            Dash();

            //plays according audio cue
            audioManager.Play("playerDash");
        }
        //If player jumps while on the ground, enable double jump and start jump movement
        else if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            doubleJump = true;
            Jump();
        }
        //If player jumps while midair and has a double jump available, disable double jump and start jump movement
        else if (Input.GetButtonDown("Jump") && doubleJump)
        {
            doubleJump = false;
            Jump();
        }
    }

    void CheckShootOrMelee(){
        //When player starts a melee attack and is not dashing, start stabbing animation
        if(Input.GetButtonDown("Melee") && isDashing == false)
        {
            Debug.Log("Melee");
            isStabbing = true;

            //plays according audio cue
            audioManager.Play("playerStab");
        }
        //When player wants to shoot and is not dashing, reset animation trigger and player shoots
        else if (Input.GetButtonDown("Shoot") && isDashing == false)
        {
            Debug.Log("shooting");
            isStabbing = false;
            Shoot();

            //plays according audio cue
            audioManager.Play("playerShoot");
        }
        //Reset animation trigger
        else {
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

        //Buffer for the distance the player is considered to be on the ground
        float extraHeightText = 0.51f;
        //Draw Raycast box
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeightText, platformsLayer);

        //Debug information
        Color rayColor;
        if (raycastHit.collider != null) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y + extraHeightText), Vector2.right * (boxCollider2d.bounds.extents.x * 2f), rayColor);

        //If Raycast box hit, player is on the ground
        if(raycastHit.collider != null){
            isGrounded = true;
        } else{
            isGrounded = false;
        }
              
    }

    //Loop through all game objects to find enemies. Used to determine closest enemy for the Shoot() function
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
        /*
        if(rigidBody.velocity.x < -10.0 || rigidBody.velocity.x > 10.0 )
        {
            Debug.Log("End Dash");
        }
        */
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
        animator.SetBool("hasBeenDamaged", hasBeenDamaged);
        if (hasBeenDamaged) hasBeenDamaged = false;
    }

    void Jump(){
        //Set velocity to 0 so forces are not added on top of each other (for example on double jump)
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.AddForce(Vector2.up * playerJumpPower);

        //plays according audio cue
        audioManager.Play("playerJump");
    }

    void Dash(){
        //StartDashTime (i.e. dash duration) has been exceeded.
        if (dashTime <= 0)
        {
            //Reset dashTime
            dashTime = startDashTime;
            //End dash
            isDashing = false;
            //Ensure dash velocity does not carry over to normal movement speed
            rigidBody.velocity = Vector2.zero;
            return;
        }           
        else
        {
            //Player is considered to be dashing
            isDashing = true;
            //Decrease remaining dash time
            dashTime -= Time.deltaTime;
            //Set velocity to dash power
            rigidBody.velocity = new Vector2(direction, 0) * playerDashPower;
        }
    }

    void Shoot()
    {
        //Check enemies again, in case any have died
        EnemiesCheck();

        //Foreach loop variable for direction from player to enemy
        Vector2 enemyDir;
        //This distance is used to determine, whether enemies are withing shooting range
        float distance = maxShootingDistance;
        //Store GO of closest enemy 
        GameObject closestEnemy = null;
        //Store direction of closest enemy
        Vector2 closestEnemyDir = Vector2.zero;

        //Loop over all enemies
        foreach (GameObject enemy in enemies)
        {
            //Update enemy direction
            enemyDir = new Vector2(enemy.transform.position.x - this.transform.position.x, enemy.transform.position.y - this.transform.position.y);
            //If length of the vector between player and enemy is lower than distance to
            //the previously closest enemy or max shooting distance
            if (enemyDir.magnitude < distance)
            {
                //Distance in which enemies can be considered closest is updated
                distance = enemyDir.magnitude;
                Debug.Log("New closest enemy found");
                //Store values 
                closestEnemy = enemy;
                closestEnemyDir = enemyDir;
            }                
        }
        if (closestEnemy)
        {
            //Offset for origin of the shot
            Vector2 offset = new Vector2(.3f, .5f);

            //When there is something in the way, do not fire the shot
            RaycastHit2D hitObstacle = MyRaycast(offset, closestEnemyDir, distance, platformsLayer);
            Color color = hitObstacle ? Color.red : Color.green;
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y) + offset, closestEnemyDir, color, 5f);
            if (hitObstacle)
                return;

            Debug.Log(closestEnemy.name);
            //Shake the camera
            CameraShaker.Instance.ShakeCamera(3f, .075f);
            //Enemy takes damage
            closestEnemy.GetComponentInParent<EnemyHealth>().EnemyTakeDamage(shotDamage);
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

    public void PlayerTakeDamage(int damage)
    {

        if (invincibilityTime <= 0) {
            //Reset invincibilityTime
            invincibilityTime = startInvincibilityTime;
            hasBeenDamaged = false;
        }           
        else
        {
            //playerHealth -= damage;
            //test: if the player runs out of gold, they die
            gold -= damage;
            hasBeenDamaged = true;
            
            invincibilityTime -= Time.deltaTime;

            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0f;
            rigidBody.AddForce(Vector2.up * 500);
            //rigidBody.velocity = new Vector2(direction, 0) * playerDashPower;

            GameManager.DecreaseScore(damage);

            //plays according audio cue
            audioManager.Play("playerHurt");
        }

        //If player health is above 0, do nothing else
        if (/*playerHealth*/ gold > 0)
            return;
        
        gameObject.SetActive(false);
        DestroyPlayer();
        GameManager.PlayerDied();
    }

    public void incrementGold (int amount)
    {
        gold += amount;
    }

    private void decrementGold (int amount)
    {
        gold -= amount;
    }

    void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    //This function is called automatically by Unity's physics engine/detector/whatever
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

    //This function is called automatically by Unity's physics engine/detector/whatever
    //The player's sword is marked as a trigger. When it collides with something, this function is called
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Enemies") && (animator.GetCurrentAnimatorStateInfo(0).IsName("pirate_stab") || animator.GetCurrentAnimatorStateInfo(0).IsName("pirate_dash")))
        {
            Debug.Log("A trigger has collided with " + col.gameObject.name);
            col.gameObject.GetComponentInParent<EnemyHealth>().EnemyTakeDamage(meleeDamage);
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