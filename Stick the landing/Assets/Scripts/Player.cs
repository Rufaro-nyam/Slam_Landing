using Unity.VisualScripting;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;
using System.Collections;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //mvmt
    public float jumpforce = 10f;
    private float boost_force = 10f;
    private float generated_boost_force;
    private Rigidbody2D rb;
    public float movespeed;
    

    //BALANCING
    private float targetrotation = 0f;
    public float force;

    //ground check
    public Transform ray_origin;
    private float ray_distance = 0.5f;
    private bool can_boost;
    private bool can_gravitate = true;

    //GFX
    public TrailRenderer trail;
    public SpriteRenderer background;

    //CAMSHAKE
    public ShakeData shake_small;
    public ShakeData shake_medium;
    public ShakeData shake_great;

    //PARTICLES
    public Transform particle_spawn_point;
    public GameObject explosion_heavy;
    public GameObject explosion_medium;
    public GameObject explosion_small;

    public float stretch;
    public Transform squash_parent;

    //SOUND
    public AudioSource bounce;
    public AudioSource boom1;
    public AudioSource boom2;
    public AudioSource ping;
    public AudioSource wind;
    public AudioSource platform_exp;
    private bool can_play_wind = true;

    //TARGETING
    public GameObject targeter;

    //Platform
    private bool can_destroy_platform = false;
    public Level_platform_manager level_manager;

    //FRAME FREEZE
    public float freeze_duration = 1f;
    bool is_frozen = false;
    float pending_freeze_duration = 0f;

    //SCORE SYSTEM
    public Timer Timer;
    private bool has_scored_before = false;
    private float multiplyer_loss_timer = 0f;
    public Image combo_fill;

    //EXPLOSION
    public float filed_of_impact;
    public float explosion_force;
    public LayerMask layertohit;

    //RESET
    public bool is_gravitating;
    //TUTORIAL
    public bool in_tutorial;
    public Transform tut_pos;
    public bool slowmo_tut = false;
    private bool tut_floor_hit = false;

    public GameObject blocks;
    public GameObject block_particles;

    public Camera maincam;
    public GameObject flash;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    
    void Update()
    {
        if (combo_fill.fillAmount > 0) { combo_fill.fillAmount -= Time.deltaTime/3; }
        if(combo_fill.fillAmount == 0) { Timer.reset_multiplier(); has_scored_before = false; }
        if(pending_freeze_duration > 0 && is_frozen == false)
        {
            //print("freezing");
            StartCoroutine(DoFreeze());
        }

        targeter.transform.position = new Vector3(transform.position.x, targeter.transform.position.y, targeter.transform.position.z);

        float mpd_boost_force = generated_boost_force * 13;
        mpd_boost_force = Mathf.Clamp(mpd_boost_force, 0, 10);
        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * movespeed, rb.linearVelocity.y);
        

        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetrotation, force * Time.deltaTime));

        Color current_color = background.color;
        if(current_color.a > 0f)
        {
            current_color.a -= Time.deltaTime * 4;
        }
        background.color = current_color;
        /*Vector3 velocity = rb.linearVelocity;
        var scalex = 0.25f + (velocity.magnitude * stretch);
        var scaley = 0.25f / scalex;
        squash_parent.localScale = new Vector3(scalex, scaley, 1.0f);*/

        if (rb.linearVelocityY > 0f) 
        {
            trail.emitting = false;


            if ((Input.GetAxis("Horizontal")) < 0) 
            {
                targetrotation = 35f;
            }
            else if((Input.GetAxis("Horizontal")) > 0)
            {
                targetrotation = -35f;
            }
            else
            {
                targetrotation = 0f;
            }
            
        }
        else if(rb.linearVelocityY < 0f)
        {
            trail.emitting = true;
            if ((Input.GetAxis("Horizontal")) < 0)
            {
                targetrotation = -35f;
            }
            else if ((Input.GetAxis("Horizontal")) > 0)
            {
                targetrotation = 35f;
            }
            else
            {
                targetrotation = 0f;
            }
        }

        if (Input.GetMouseButton(0) && can_gravitate ) 
        {
            generated_boost_force += Time.deltaTime;
            wind.pitch += Time.deltaTime;
            wind.volume += Time.deltaTime/1.5f;
            play_wind();
            targeter.transform.localScale = Vector3.Lerp(targeter.transform.localScale, new Vector3(0.25f, 0.25f, 0.25f), 0.02f);
            targeter.transform.Rotate(0, 0, 3.0f);

            rb.gravityScale += 8f * Time.deltaTime;
            ray_distance = 2f;
            is_gravitating = true;

            maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 108, 0.0005f);
        }
        else
        {
            rb.gravityScale = 1f;
            ray_distance = 0.5f;
            targeter.transform.localScale = Vector3.Lerp(targeter.transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), 0.02f);
            is_gravitating = false;
            maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 113, 0.06f);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (can_boost) 
            {
                can_play_wind = true;

                rb.gravityScale = 0f;
                rb.AddForce(Vector2.up * mpd_boost_force , ForceMode2D.Impulse);
                //print(mpd_boost_force);

                generated_boost_force = 0;
                //shake.Magnitude = mpd_boost_force / 10;
                if (mpd_boost_force < 3f) 
                {
                    CameraShakerHandler.Shake(shake_small);
                    Instantiate(explosion_small, particle_spawn_point.position, Quaternion.identity);
                }
                else if (mpd_boost_force < 6f && mpd_boost_force > 3f)
                {
                    CameraShakerHandler.Shake(shake_medium);
                    Instantiate(explosion_medium, particle_spawn_point.position, Quaternion.identity);
                    boom1.pitch = UnityEngine.Random.Range(1f, 1.5f);
                    boom1.Play();
                    can_destroy_platform = true;
                }
                else if(mpd_boost_force > 6f) 
                {
                    CameraShakerHandler.Shake(shake_great);
                    Instantiate(explosion_heavy,particle_spawn_point.position, Quaternion.identity);
                    boom1.pitch = UnityEngine.Random.Range(1f, 1.5f);
                    boom1.Play();
                    explode();

                    can_destroy_platform = true;
                }
                
            }
        }

    }

    public IEnumerator DoFreeze()
    {

        is_frozen = true;
        var original = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(freeze_duration);

        Time.timeScale = original;
        pending_freeze_duration = 0;
        is_frozen = false;
        can_destroy_platform = true;

    }

    private void play_wind()
    {
        if (can_play_wind) 
        {
            wind.pitch = UnityEngine.Random.Range(1f, 1.25f);
            wind.Play();
            can_play_wind = false;
        }

    }


    private void FixedUpdate()
    {
        if (slowmo_tut)
        {
            if (Input.GetMouseButton(0))
            {
                Time.timeScale = 1f;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Time.timeScale = 1f;
            }
        }
        RaycastHit2D hit2D = Physics2D.Raycast(ray_origin.position, Vector2.down, ray_distance);
        Debug.DrawRay(ray_origin.position, Vector2.down * ray_distance, Color.red);

        if (hit2D.collider != null) 
        {
            can_boost = true;
            if (hit2D.collider.tag == "Platform" && can_destroy_platform) 
            {
                LeanTween.scale(hit2D.collider.gameObject, new Vector3(3, 3, 3), 0.1f);
                
                //print("i can destroy this");

                
                level_manager.spawn_platform();
                Color current_color = background.color;
                current_color.a = 1f;
                background.color = current_color;
                if(ping.pitch < 2)
                {
                    ping.pitch += 0.1f;
                }
                platform_exp.pitch = UnityEngine.Random.Range(1f, 1.25f);
                platform_exp.Play();
                ping.Play();
                Timer.add_point();
                Instantiate(blocks, hit2D.transform.position, Quaternion.identity);
                Instantiate(block_particles, hit2D.transform.position, Quaternion.identity);
                Instantiate(flash, hit2D.transform.position, Quaternion.identity);
                explode();
                Destroy(hit2D.collider.gameObject);
                if (combo_fill.fillAmount < 0.15f && has_scored_before)
                {
                    Timer.close_call();
                }
                
                if (has_scored_before) 
                {
                    Timer.add_multiplier();
                }
                has_scored_before = true;
                multiplyer_loss_timer = 1f;
                combo_fill.fillAmount = 1f;
                if (slowmo_tut)
                {
                    Time.timeScale = 0.25f;
                }
                
                //combo_fill.color = Random.ColorHSV(0f, 1f, 1f, 0.5f, 0.5f, 1f);
                //combo_fill.color.a = new float 0.5f;
                //rb.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
                //hit2D.collider.TryGetComponent<Platform>(out Platform platform);

                //platform.destroy();
                Freeze();
            }
            if(hit2D.collider.tag == "Spike_Platform")
            {
                Timer.reset_multiplier();
                print("Spike Hit");
                Destroy(hit2D.collider.gameObject);
                
            }

        }
        else
        {
            can_boost = false;
            can_destroy_platform = false;
            

        }

    }

    public void reset_ping_pitch()
    {
        ping.pitch = 1f;
    }

    
    public void Freeze()
    {
        //print("freeze function called");
        pending_freeze_duration = freeze_duration;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        float mpd_boost_force = generated_boost_force * 13;
        rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
        //print("bounce");
        can_gravitate = false;
        wind.Stop();
        wind.volume = 0f;


        /*if (collision.transform.tag == "Manager"  && grace_bounces > 1)
        {

            Timer.reset_multiplier();
            has_scored_before = false;
            grace = false;

        }
        if (collision.transform.tag == "Platform" && mpd_boost_force < 4)
        {

            Timer.reset_multiplier();
            has_scored_before = false;
            grace = false;

        }*/
        



    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        if (is_gravitating == false)
        {
            float mpd_boost_force = generated_boost_force * 13;
            can_gravitate = true;
            bounce.pitch = UnityEngine.Random.Range(1f, 1.5f);
            bounce.Play();
        }

    }

    private void explode()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, filed_of_impact, layertohit);
        foreach (Collider2D obj in objects) 
        {
            Vector2 Direction = obj.transform.position - transform.position;
            obj.GetComponent<Rigidbody2D>().AddForce(Direction * force);
        }
    }

    public void start_tutorial()
    {
        transform.position = tut_pos.position;
        slowmo_tut = true;
        Time.timeScale = 0.25f;
    }

    public void stop_tutorial()
    {
        transform.position = tut_pos.position;
        slowmo_tut = false;
        Time.timeScale = 1f;
    }
}
