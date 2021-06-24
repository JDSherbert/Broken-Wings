using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JDH_EnemyAI_script : MonoBehaviour
{
    [System.Serializable]
    public class AIParams
    {
        public int HP;
        public int MaxHP = 3;
        public float movementSpeed = 10.0f;
        public float damage = 1;
        public float attackspeed = 1.0f;

        public float groundDetectionRange = 0.5f;
        public float groundDetectionWidth = 0.1f;
        public float gravityMultiplier = 10.0f;
        public LayerMask platformDetection;

        public float currentVelocity;
        public Vector3 lastFramePosition = Vector3.zero;
        public Vector3 delta;
        public float delay = 3.0f;

        public float invulnerabledelaymax = 1.0f;
        public float invulntimer;

        [System.Serializable]
        public class Behaviours
        {
            public bool isAttacking;
            public bool isGrounded;
            public bool isDead;
            public bool isAggro;
            public bool walkRight;
        }

        public Behaviours behaviour = new Behaviours();
    }


    [System.Serializable]
    public class Components
    {
        public Collider collider;
        public Rigidbody rb;
        public GameObject healthdisplay;
        public Animator anim;
        public AudioSource audiosource;
        public AudioClip attacksound;
        public SpriteRenderer spriterenderer;
    }

    public Components component = new Components();
    public AIParams ai = new AIParams();



    // Start is called before the first frame update
    void Start()
    {
        Get();
    }

    // Update is called once per frame
    void Update()
    {
        if(ai.behaviour.isDead == false)
        {
            Movement();
            ObstacleCheck();
            if (ai.invulntimer > ai.invulnerabledelaymax)
                ai.invulntimer += 0.05f;
        }
        else
        {
            KillPlayer();
        }

    }

    public void Movement()
    {
        ai.currentVelocity = component.rb.velocity.magnitude; //returns speed from rb
        component.anim.SetFloat("Speed", ai.currentVelocity);

        //Speed limiter
        if (component.rb.velocity.x > ai.movementSpeed)
            component.rb.velocity = new Vector3(ai.movementSpeed, component.rb.velocity.y, component.rb.velocity.z);
        else if (component.rb.velocity.x < -ai.movementSpeed)
            component.rb.velocity = new Vector3(-ai.movementSpeed, component.rb.velocity.y, component.rb.velocity.z);

        if(!ai.behaviour.isGrounded)
            component.rb.AddForce(0, -ai.gravityMultiplier * Time.deltaTime, 0, ForceMode.Impulse);

        //Sprite get last frame
        if (ai.behaviour.isAttacking == false)
        {
            ai.delta = transform.position - ai.lastFramePosition;
            if (ai.delta.x < 0)
            {
                component.spriterenderer.flipX = true;
            }
            else if (ai.delta.x > 0)
            {
                component.spriterenderer.flipX = false;
            }
            ai.lastFramePosition = transform.position;

            //Patrol
            if (ai.behaviour.walkRight == true)
            {
                component.rb.AddForce(ai.movementSpeed * Vector3.right);
            }
            else
            {
                component.rb.AddForce(ai.movementSpeed * Vector3.left);
            }
        }
        else
        {
            //Lock into attack
        }
    }
    //Detect ground
    public bool ObstacleCheck()
    {
        RaycastHit side;
        //Right Side
        if (Physics.SphereCast(transform.position, ai.groundDetectionWidth, transform.TransformDirection(Vector3.right), out side, ai.groundDetectionRange, ai.platformDetection))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * side.distance, Color.yellow);
            if(side.collider.gameObject.tag == "Player")
            {
 
                ai.behaviour.isAttacking = true;
                component.spriterenderer.flipX = false;
                StartCoroutine(Attack());
            }
            else if(side.collider.gameObject.tag == "Buddies")
            {
                ai.behaviour.isAttacking = true;
                component.spriterenderer.flipX = false;
                StartCoroutine(Attack());
            }
            else
            {
                ai.behaviour.isAttacking = false;
                ai.behaviour.walkRight = false;
            }
        }
        //Left Side
        else if (Physics.SphereCast(transform.position, ai.groundDetectionWidth, transform.TransformDirection(Vector3.left), out side, ai.groundDetectionRange, ai.platformDetection))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * side.distance, Color.yellow);
            if (side.collider.gameObject.tag == "Player")
            {
                ai.behaviour.isAttacking = true;
                component.spriterenderer.flipX = true;
                StartCoroutine(Attack());
            }
            else if (side.collider.gameObject.tag == "Buddies")
            {
                ai.behaviour.isAttacking = true;
                component.spriterenderer.flipX = true;
                StartCoroutine(Attack());
            }
            else
            {
                ai.behaviour.isAttacking = false;
                ai.behaviour.walkRight = true;
            }
        }

        //Groundcast
        RaycastHit ground;
        if (Physics.SphereCast(transform.position, ai.groundDetectionWidth, transform.TransformDirection(Vector3.down), out ground, ai.groundDetectionRange, ai.platformDetection))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * ground.distance, Color.yellow);
            ai.behaviour.isGrounded = true;
            component.anim.SetBool("isGrounded", ai.behaviour.isGrounded);
            return true;
        }

        else
        {
            ai.behaviour.isGrounded = false;
            component.anim.SetBool("isGrounded", ai.behaviour.isGrounded);
            return false;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Instakill")
        {
            KillPlayer();
        }
        else
        {
            if (collision.gameObject.tag == "Player")
            {
                component.audiosource.PlayOneShot(component.attacksound);
                collision.gameObject.GetComponent<JDH_SideScrollerMovement_script>().player.isDamaged = true;
                StartCoroutine(Attack());
            }
            else if (collision.gameObject.tag == "Buddies")
            {
                component.audiosource.PlayOneShot(component.attacksound);
                StartCoroutine(Attack());
            }
            else if (collision.gameObject.tag == "Damage")
            {
                if (ai.invulntimer >= ai.invulnerabledelaymax)
                    ai.HP--;
                ai.invulntimer = 0;
            }
            else if (collision.gameObject.tag == "Baddies")
            {
                if(component.spriterenderer.flipX == true)
                    component.rb.AddForce(ai.movementSpeed * Vector3.right, ForceMode.Impulse);
                if (component.spriterenderer.flipX == false)
                    component.rb.AddForce(ai.movementSpeed * Vector3.left, ForceMode.Impulse);
            }
        }
    }


    public IEnumerator Attack()
    {
        component.anim.SetBool("isAttacking", true);
        ai.behaviour.isAttacking = true;
        yield return new WaitForSeconds(ai.attackspeed);
        component.anim.SetBool("isAttacking", false);
        ai.behaviour.isAttacking = false;
    }

    public void KillPlayer()
    {
        ai.behaviour.isDead = true;
        component.anim.SetBool("isDead", ai.behaviour.isDead);
        //component.spriterenderer.enabled = false;
        gameObject.tag = "Untagged";
        Destroy(gameObject, ai.invulnerabledelaymax);
    }

    public void Get()
    {
        ai.HP = ai.MaxHP;
        if (GetComponent<Rigidbody>())
            component.rb = GetComponent<Rigidbody>();
        if (GetComponent<Collider>())
            component.collider = GetComponent<Collider>();
        if (GetComponentInChildren<Animator>())
            component.anim = GetComponentInChildren<Animator>();
        if (GetComponent<AudioSource>())
            component.audiosource = GetComponent<AudioSource>();
        if (GetComponentInChildren<SpriteRenderer>())
            component.spriterenderer = GetComponentInChildren<SpriteRenderer>();
    }
}
