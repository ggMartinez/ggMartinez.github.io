using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class PlayerPlatformerController : PhysicsObject {

    float deathPositionX = -9.59f;

    float hitLenght = 0.5f;
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public CinemachineVirtualCamera vcam;

    public Transform AttackPoint;
    public float AttackRange = 0.5f;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    public LayerMask EnemyLayers;
    public LayerMask RepairLayers;

    public SpriteRenderer flareRender;

    

    [SerializeField] private Canvas PauseScreen;

    public GameObject bulletPrefab;
    public Transform firepoint;

    public float blltspeed = 5f;
    Vector2 movement;
    public Canvas GameOverScreen;
    public Canvas WinScreen;

    public int Score = 0;
    public float lastY;

    public Background Background;

    public AudioSource ParticlesSound;

     



    // Use this for initialization
    void Awake () 
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();    
        animator = GetComponent<Animator> ();
        flareRender = GetComponent<SpriteRenderer> ();

    }

    protected override void UpdateFrame()
    {

        if(Input.GetKeyDown(KeyCode.F)){
            TriggerBackgroundChange();
        }
        
        if(Input.GetKeyDown("escape")){
            PauseScreen.gameObject.SetActive(true);
        }


        checkInput();    

        moveForward();    


        checkBreak();
        checkRepair();

      
       
    }
    


    void checkInput(){
        checkJump();        
    }


    void checkRepair(){
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position,AttackRange,RepairLayers);
        
        foreach(Collider2D enemy in hitEnemies){
            Debug.Log(enemy.name);
            enemy.GetComponent<Objects>().HitRepair();  
            if (Input.GetButtonDown("Fire1")){
                enemy.GetComponent<Objects>().Hit();
                Score +=5;
            }
        } 
    }



    void checkBreak() {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position,AttackRange,EnemyLayers);
            foreach(Collider2D enemy in hitEnemies){
                Debug.Log("Hit "+ enemy.name);
                enemy.GetComponent<Objects>().HitBreak();
                if (Input.GetButtonDown("Fire2")){
                    enemy.GetComponent<Objects>().Break();
                    Score -=10;
            }
        }     
            
    }




    void OnDrawGizmosSelected(){
        if(AttackPoint == null){
            return;
        }
        Gizmos.DrawWireSphere(AttackPoint.position,AttackRange);
    }




    void moveForward(){
        Vector2 move = Vector2.zero;
        //move.x = 1;
        move.x = Input.GetAxis("Horizontal") + 1.3f; 
        if (move.x == 0f){
            move.x += 0.3f;
        }


        targetVelocity = move * maxSpeed;

    }

   
    void checkJump(){
        //Debug.Log(vcam.VirtualCameraGameObject.name);
        //vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY += 0.3f;
        Vector2 move = Vector2.zero;
        

        if (Input.GetButtonDown ("Jump") && grounded) {
            animator.SetBool("Jumping", true);
            animator.SetBool("Float", false);

            ParticlesSound.Stop();
            

            velocity.y = jumpTakeOffSpeed;
        } else if (Input.GetButtonUp ("Jump")) 
        {   animator.SetBool("Jumping", false);
            animator.SetBool("Float", true);
            if (velocity.y > 0) {
                velocity.y = velocity.y * 0.5f;
            }
        }        
        targetVelocity = move * maxSpeed;
    }

    
    void die(){
        Destroy(gameObject);
        GameOverScreen.gameObject.SetActive(true);
    }



    void OnCollisionEnter2D (Collision2D other){
       if  ((other.gameObject.layer != 11) && (other.gameObject.layer != 12)) {
            animator.SetBool("Float", false);
            ParticlesSound.Play();
        }
    }
    void win(){
        WinScreen.gameObject.SetActive(true);

    }

    void OnTriggerEnter2D (Collider2D other){

        switch(other.gameObject.name){
            case "Pit":
                die();
                break;
            case "touchTrans":
              //Instantiate(prefab,ransport.position,transport.rotation);
              TriggerBackgroundChange();
              break;
            case "Finish":
                win();
                break;
            

        }
    }

    void TriggerBackgroundChange(){
      Background.TriggerTransition();
    }

    

    


}