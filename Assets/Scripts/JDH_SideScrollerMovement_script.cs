using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JDH_SideScrollerMovement_script : MonoBehaviour
{
    /// <summary>
    /// Script created by Joshua "JDSherbert" Herbert 29/01/2021
    /// Control script for a side scroller. Tailored for 2.5d play. 
    /// Created for Global Game Jam 2021.
    /// </summary>

    [System.Serializable]
    public class PlayerParameters
    {
        public float movementSpeed = 5.0f;
        public float dashSpeed = 15.0f;
        public float jumpHeight = 10.0f;
        public float wallJumpHeight = 10.0f;
        public float groundDetectionRange = 0.55f;
        public float groundDetectionWidth = 0.55f;
        public float gravityMultiplier = 20f;
        public LayerMask platformDetection;
        public int buddylayer = 8;

        public float currentVelocity;
        public bool isGrounded;
        public bool isDashing;
        public float dashdelay = 5.0f;
        public float dashLength = 5.0f;
        public float dashLengthTime;
        public float dashtime;
        public float jumpresetdelay = 0;
        public int jumps = 0;
        public float airtime = 0;

        public bool isDamaged;
        public bool isDead;
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.05f;
        public string VERTICAL_AXIS = "Vertical"; //^v
        public string HORIZONTAL_AXIS = "Horizontal"; //<>
        public string JUMP_AXIS = "Jump"; //_

        public float verticalInput;
        public float horizontalInput;
        public float jumpInput;
    }

    [System.Serializable]
    public class Components
    {
        public Rigidbody rb;
        public Collider collider;
        public Animator anim;

        public AudioSource audiosource;
        public AudioClip dashsfx;
        public AudioClip jumpsfx;
        public AudioClip hurtsfx;
        public AudioClip deadsfx;

        public SpriteRenderer spriterenderer;
        public ParticleSystem feathers;
        public ParticleSystem smoke;

        public JDH_Follow_script followscript;
    }

    [System.Serializable]
    public class Abilities
    {
        public bool doublejump;
        public bool walljump;
        public bool dash;
        public bool glide;
    }


    [System.Serializable]
    public class Cheats
    {
        public bool immortal = false; //ignore destroy colliders
        public bool invisible = false; //undetectable by enemies
        public bool flying = false; //no jump limiting
        public bool ghost = false; //go through walls
    }

    public PlayerParameters player = new PlayerParameters();
    public InputSettings input = new InputSettings();
    public Components component = new Components();
    public Abilities ability = new Abilities();
    public Cheats cheat = new Cheats();

    public void Start()
    {
        Get();
    }

    public void Update()
    {
        if(player.isDead == false)
        {
            GetInput();
            Movement();
            Dash();
            Jump();
            Cheat();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Damage(collision);
        if (collision.gameObject.tag == "Buddies")
            GetComponent<JDH_Follow_script>().AddThing(collision.gameObject);
        if (collision.gameObject.tag == "Cage")
            collision.gameObject.GetComponent<JDH_CagedBuddy_script>().CheckDestroy();
        if(collision.gameObject.tag == "Vent")
        {
            Debug.Log("Saving buddies");
            component.followscript.FreeBuddy();   
        }
    }

    public void GetInput()
    {
        input.verticalInput = Input.GetAxis(input.VERTICAL_AXIS);
        input.horizontalInput = Input.GetAxis(input.HORIZONTAL_AXIS);
        input.jumpInput = Input.GetAxis(input.JUMP_AXIS);
    }

    public void Movement()
    {
        player.currentVelocity = component.rb.velocity.magnitude; //returns speed from rb
        component.anim.SetFloat("Speed", player.currentVelocity);

        //Speed limiter
        if (component.rb.velocity.x > player.movementSpeed)
            component.rb.velocity = new Vector3(player.movementSpeed, component.rb.velocity.y, component.rb.velocity.z);
        else if (component.rb.velocity.x < -player.movementSpeed)
            component.rb.velocity = new Vector3(-player.movementSpeed, component.rb.velocity.y, component.rb.velocity.z);

        //On move input detect
        if (Mathf.Abs(input.horizontalInput) > input.inputDelay)
        {
            Vector3 movement = new Vector3(input.horizontalInput, 0);

            //Maxspeed limit
            if(CanJump())
            {
                //Move Player
                component.rb.AddForce(player.movementSpeed * movement * player.movementSpeed);
            }
            else
            {
                //Move Player
                component.rb.AddForce(player.movementSpeed * movement);
            }

            //Sprite
            if (input.horizontalInput > 0.1f)
                component.spriterenderer.flipX = false;
            if (input.horizontalInput < -0.1f)
                component.spriterenderer.flipX = true;
        }
        else if(player.isDashing)
        {

        }
        else //Clean stopping
        {
            component.rb.velocity = new Vector3(input.horizontalInput, component.rb.velocity.y, component.rb.velocity.z);
        }
    }

    public void Dash()
    {
        if(ability.dash == true)
        {
            float temp_speed = player.movementSpeed;

            //Delay
            if (!Input.GetButton("Dash") && player.dashtime >= 0)
            {
                player.dashtime -= 0.05f;
            }

            if(player.isDashing == true && player.dashLengthTime >= 0)
            {
                player.dashLengthTime -= 0.1f;
            }

            if (Input.GetButtonDown("Dash") && input.horizontalInput != 0 && player.dashtime <= 0)
            {
                component.audiosource.PlayOneShot(component.dashsfx);
                component.feathers.Play();
                component.smoke.Play();
                player.dashtime = player.dashdelay;
                player.isDashing = true;
                player.dashLengthTime = player.dashLength;

                
                //Shadow sprint calculation
                //component.rb.AddForce((input.horizontalInput * (player.dashSpeed * player.movementSpeed) * 2), 0, 0, ForceMode.Impulse);
                //component.rb.velocity = new Vector3(component.rb.velocity.x * Time.deltaTime, 0, 0);
            }
            if(player.dashLengthTime > 0 && player.isDashing)
            {
                //Normal
                component.rb.velocity += new Vector3(input.horizontalInput * player.dashSpeed, 0, 0);
            }
            if(player.dashLengthTime <= 0)
            {
                player.isDashing = false;
                player.movementSpeed = temp_speed;
            }
        }
        else
        {
            //player.isDashing = false;
        }

    }

    public bool CanJump() //Jump range detection
    {

        if (cheat.flying == true)
        {
            return false;
        }
        else
        {
            RaycastHit ground;
            if (Physics.SphereCast(transform.position, player.groundDetectionWidth, transform.TransformDirection(Vector3.down), out ground, player.groundDetectionRange, player.platformDetection))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * ground.distance, Color.yellow);
                player.isGrounded = true;
                component.anim.SetBool("isGrounded", player.isGrounded);
                return true;
            }
            if(ability.walljump)
            {
                //Right Wall
                if (Physics.SphereCast(transform.position, player.groundDetectionWidth, transform.TransformDirection(Vector3.right), out ground, player.groundDetectionRange, player.platformDetection))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * ground.distance, Color.yellow);
                    player.isGrounded = true;
                    if(Input.GetButtonDown("Jump"))
                    {
                        Debug.Log("Wall jump");
                        component.rb.AddForce(new Vector3(-player.wallJumpHeight, player.wallJumpHeight, 0), ForceMode.VelocityChange);
                    }
                    component.anim.SetBool("isGrounded", player.isGrounded);
                    return true;
                }
                //Left Wall
                else if (Physics.SphereCast(transform.position, player.groundDetectionWidth, transform.TransformDirection(Vector3.left), out ground, player.groundDetectionRange, player.platformDetection))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * ground.distance, Color.yellow);
                    player.isGrounded = true;
                    if (Input.GetButtonDown("Jump"))
                    {
                        Debug.Log("Wall jump");
                        component.rb.AddForce(new Vector3(player.wallJumpHeight, player.wallJumpHeight, 0), ForceMode.VelocityChange);
                    }
                    component.anim.SetBool("isGrounded", player.isGrounded);
                    return true;
                }
                //None
                else
                {
                    player.isGrounded = false;
                    component.anim.SetBool("isGrounded", player.isGrounded);
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * ground.distance, Color.green);
                player.isGrounded = false;
                component.anim.SetBool("isGrounded", player.isGrounded);
                return false;
            }

        }
    }
    public void Jump()
    {
        CanJump();

        //Ground systems
        if (!CanJump())
        {
            component.rb.AddForce(0, -player.gravityMultiplier * Time.deltaTime, 0, ForceMode.Impulse);
            player.airtime += 0.5f;
            player.jumpresetdelay += 0.1f;

        }
        else
        {
            player.airtime = 0;
            if (player.isGrounded && player.jumpresetdelay >= 1)
            {
                player.jumpresetdelay = 0;
                player.jumps = 0;
            }
        }

        //Simple jump
        if(!ability.doublejump)
        {
            if (CanJump() == true && input.jumpInput > 0)
            {
                component.feathers.Play();
                component.rb.velocity += input.jumpInput * player.jumpHeight * Vector3.up;
            }
        }
        //Double jump
        if (Input.GetButtonDown("Jump") && player.jumps == 0 && CanJump() == true)
        {
            component.audiosource.PlayOneShot(component.jumpsfx);
            component.feathers.Play();
            component.rb.velocity += input.jumpInput * (player.jumpHeight) * Vector3.up;
            player.jumps = 1;
            player.jumpresetdelay = 0;
        }

        if (Input.GetButtonDown("Jump") && player.jumps == 1 && player.airtime > 0.0f)
        {
            component.audiosource.PlayOneShot(component.jumpsfx);
            component.feathers.Play();
            component.rb.velocity += input.jumpInput * (player.jumpHeight / 1.5f) * Vector3.up;
            player.jumps = 2;
        }
    }

    public void Damage(Collision collision)
    {
        if(collision.gameObject.tag == "Instakill")
        {
            Debug.Log("Killed by " + collision.gameObject.name);
            component.audiosource.PlayOneShot(component.deadsfx);
            player.isDamaged = true;
            component.feathers.Play();
            KillPlayer();
        }

        if (collision.gameObject.tag == "Damage")
        {
            Debug.Log("hurt by " + collision.gameObject.name);
            component.audiosource.PlayOneShot(component.hurtsfx);
            player.isDamaged = true;
            component.feathers.Play();
        }
        else if (collision.gameObject.tag == "Baddies")
        {
            Debug.Log("hurt by " + collision.gameObject.name);
            component.audiosource.PlayOneShot(component.hurtsfx);
            player.isDamaged = true;
            component.feathers.Play();
        }
        else
        {
            player.isDamaged = false;
            component.feathers.Stop();
        }
    }


    public void Cheat()
    {
        float gravMultiplier_Temp = player.gravityMultiplier;

        if (cheat.flying == true)
        {
            component.rb.useGravity = !cheat.flying;
            player.gravityMultiplier = 0;

            //Fly Player
            if (input.verticalInput != 0)
            {
                gameObject.transform.position +=
               (Vector3.up
               * Time.deltaTime
               * input.verticalInput
               * player.movementSpeed); ;
            }
        }
        else if (cheat.flying == false)
        {
            component.rb.useGravity = !cheat.flying;
            player.gravityMultiplier = gravMultiplier_Temp;
        }
    }

    public void KillPlayer()
    {
        player.isDead = true;
        component.anim.SetBool("isDead", player.isDead);
        //component.spriterenderer.enabled = false;
        StartCoroutine(ResetLevel());
    }

    public IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(3);
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void Get()
    {
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
        if (GetComponentInChildren<ParticleSystem>())
            component.feathers = GetComponentInChildren<ParticleSystem>();
        if (GetComponent<JDH_Follow_script>())
            component.followscript = GetComponent<JDH_Follow_script>();
    }
}
