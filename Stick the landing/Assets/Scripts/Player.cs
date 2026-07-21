using Unity.VisualScripting;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpforce = 10f;
    public float movespeed = 8f;
    private float generated_boost_force;
    private Rigidbody2D rb;

    [Header("Slam & Boost Tuning")]
    [Tooltip("Minimum force required to break a platform. Idle bounces won't break it.")]
    public float minSlamForceToDestroy = 5f;
    [Tooltip("Fixed upward force applied when destroying a platform.")]
    public float platformDestroyBoostForce = 14f;

    [Header("Timing Windows (Juice & Forgiveness)")]
    [Tooltip("How long before hitting the ground a player can release LMB and still get a Super Boost.")]
    public float earlyReleaseBuffer = 0.15f;
    [Tooltip("How long after hitting the ground a player can release LMB to trigger the boost.")]
    public float lateReleaseWindow = 0.12f;

    private float earlyReleaseTimer = 0f;
    private float lateReleaseTimer = 0f;
    private bool wasGravitatingOnImpact = false;

    // Ground & Gravity state
    public Transform ray_origin;
    private float ray_distance = 0.5f;
    private bool isGrounded;
    public bool is_gravitating;
    private bool can_gravitate = true;

    [Header("Visuals & Audio")]
    public SpriteRenderer background;
    public float force = 10f; // Target rotation lerp speed
    private float targetrotation = 0f;

    // Trail & Particles
    private TrailRenderer trail;
    public TrailRenderer[] trails;
    public ParticleSystem[] particles;
    private ParticleSystem.EmissionModule emission;

    [Header("Camera Shake & Effects")]
    public ShakeData shake_small;
    public ShakeData shake_medium;
    public ShakeData shake_great;

    public Transform particle_spawn_point;
    public GameObject explosion_heavy;
    public GameObject explosion_medium;
    public GameObject explosion_small;

    [Header("Audio")]
    public AudioSource bounce;
    public AudioSource boom1;
    public AudioSource boom2;
    public AudioSource ping;
    public AudioSource wind;
    public AudioSource platform_exp;
    public AudioSource button_press;
    private bool can_play_wind = true;

    [Header("Targeting & Systems")]
    public GameObject targeter;
    private bool can_destroy_platform = false;
    public Level_platform_manager level_manager;

    // Freeze & Score
    public float freeze_duration = 0.08f;
    private bool is_frozen = false;
    private float pending_freeze_duration = 0f;

    public Timer Timer;
    private bool has_scored_before = false;
    public Image combo_fill;

    // Explosion
    public float filed_of_impact = 3f;
    public float explosion_force = 500f;
    public LayerMask layertohit;

    // Tutorial & Camera
    public bool in_tutorial;
    public Transform tut_pos;
    public bool slowmo_tut = false;
    public GameObject blocks;
    public GameObject block_particles;
    public Camera maincam;
    public GameObject flash;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        int trailNum = PlayerPrefs.HasKey("TRAIL_INT") ? PlayerPrefs.GetInt("TRAIL_INT") : 0;
        update_trail(trailNum);

        foreach (ParticleSystem fires in particles)
        {
            emission = fires.emission;
        }
        update_fire_amount(0);
    }

    public void update_trail(int trail_num)
    {
        PlayerPrefs.SetInt("TRAIL_INT", trail_num);

        foreach (TrailRenderer t in trails) { t.gameObject.SetActive(false); }
        trail = trails[trail_num];
        trail.gameObject.SetActive(true);

        foreach (ParticleSystem t in particles) { t.gameObject.SetActive(false); }
        particles[trail_num].gameObject.SetActive(true);

        if (button_press) button_press.Play();
    }

    void Update()
    {
        // Combo bar depletion
        if (combo_fill.fillAmount > 0) { combo_fill.fillAmount -= Time.deltaTime / 3f; }
        if (combo_fill.fillAmount == 0) { Timer.reset_multiplier(); has_scored_before = false; }

        if (pending_freeze_duration > 0 && !is_frozen)
        {
            StartCoroutine(DoFreeze());
        }

        // Align targeter position
        targeter.transform.position = new Vector3(transform.position.x, targeter.transform.position.y, targeter.transform.position.z);

        // Horizontal Movement
        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * movespeed, rb.linearVelocity.y);
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetrotation, force * Time.deltaTime));

        // Background Flash Decay
        Color current_color = background.color;
        if (current_color.a > 0f)
        {
            current_color.a -= Time.deltaTime * 2f;
            background.color = current_color;
        }

        // Air Tilt & Trail Logic
        HandleAirRotationAndTrails();

        // --- SLAM / GRAVITY INPUT PROCESSING ---
        HandleSlamInput();

        // Update Release Timers
        if (earlyReleaseTimer > 0) earlyReleaseTimer -= Time.deltaTime;
        if (lateReleaseTimer > 0) lateReleaseTimer -= Time.deltaTime;
    }

    private void HandleSlamInput()
    {
        if (Input.GetMouseButton(0) && can_gravitate)
        {
            generated_boost_force += Time.deltaTime;
            wind.pitch = Mathf.Clamp(wind.pitch + Time.deltaTime, 1f, 2.5f);
            wind.volume = Mathf.Clamp01(wind.volume + Time.deltaTime / 1.5f);
            play_wind();

            targeter.transform.localScale = Vector3.Lerp(targeter.transform.localScale, new Vector3(0.25f, 0.25f, 0.25f), 0.1f);
            targeter.transform.Rotate(0, 0, 3.0f);

            rb.gravityScale += 12f * Time.deltaTime;
            ray_distance = 2f;
            is_gravitating = true;

            maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 108f, 0.05f);
        }
        else
        {
            rb.gravityScale = 1f;
            ray_distance = 0.5f;
            targeter.transform.localScale = Vector3.Lerp(targeter.transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), 0.1f);
            is_gravitating = false;
            maincam.fieldOfView = Mathf.Lerp(maincam.fieldOfView, 113f, 0.06f);
        }

        if (Input.GetMouseButtonUp(0))
        {
            earlyReleaseTimer = earlyReleaseBuffer;

            if (lateReleaseTimer > 0 && wasGravitatingOnImpact)
            {
                TriggerSuperBoost();
            }
        }
    }

    private void HandleAirRotationAndTrails()
    {
        float horiz = Input.GetAxis("Horizontal");

        if (rb.linearVelocityY > 0f)
        {
            trail.emitting = false;
            targetrotation = horiz < 0 ? 35f : (horiz > 0 ? -35f : 0f);
        }
        else if (rb.linearVelocityY < 0f)
        {
            trail.emitting = true;
            targetrotation = horiz < 0 ? -35f : (horiz > 0 ? 35f : 0f);
        }
    }

    private void FixedUpdate()
    {
        if (slowmo_tut && Input.GetMouseButton(0))
        {
            Time.timeScale = 1f;
        }

        RaycastHit2D hit2D = Physics2D.Raycast(ray_origin.position, Vector2.down, ray_distance);
        isGrounded = hit2D.collider != null;

        if (isGrounded && hit2D.collider.CompareTag("Platform") && can_destroy_platform)
        {
            HandlePlatformDestruction(hit2D);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        wasGravitatingOnImpact = is_gravitating;
        lateReleaseTimer = lateReleaseWindow;

        // Reset Y velocity first so bounce heights are predictable
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // Default bounce force
        rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
        can_gravitate = false;
        wind.Stop();
        wind.volume = 0f;

        if (earlyReleaseTimer > 0 || Input.GetMouseButtonUp(0))
        {
            TriggerSuperBoost();
        }
    }

    private void TriggerSuperBoost()
    {
        can_play_wind = true;
        rb.gravityScale = 0f;

        float mpd_boost_force = Mathf.Clamp(generated_boost_force * 13f, 2f, 12f);

        // Reset Y velocity before applying boost force so it's strictly uniform
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * mpd_boost_force, ForceMode2D.Impulse);

        // Only allow platform destruction if they generated enough slam force
        can_destroy_platform = mpd_boost_force >= minSlamForceToDestroy;

        generated_boost_force = 0f;
        earlyReleaseTimer = 0f;
        lateReleaseTimer = 0f;

        if (mpd_boost_force < 4f)
        {
            CameraShakerHandler.Shake(shake_small);
            if (explosion_small) Instantiate(explosion_small, particle_spawn_point.position, Quaternion.identity);
        }
        else if (mpd_boost_force < 7f)
        {
            CameraShakerHandler.Shake(shake_medium);
            if (explosion_medium) Instantiate(explosion_medium, particle_spawn_point.position, Quaternion.identity);
            boom1.pitch = Random.Range(1f, 1.5f);
            boom1.Play();
        }
        else
        {
            CameraShakerHandler.Shake(shake_great);
            if (explosion_heavy) Instantiate(explosion_heavy, particle_spawn_point.position, Quaternion.identity);
            boom1.pitch = Random.Range(1f, 1.5f);
            boom1.Play();
            explode();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!is_gravitating)
        {
            can_gravitate = true;
            bounce.pitch = Random.Range(1f, 1.5f);
            bounce.Play();
        }
    }

    private void HandlePlatformDestruction(RaycastHit2D hit2D)
    {
        // Override current bounce with a uniform maximum extra boost force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * platformDestroyBoostForce, ForceMode2D.Impulse);

        LeanTween.scale(hit2D.collider.gameObject, new Vector3(3, 3, 3), 0.1f);
        level_manager.spawn_platform();

        Color current_color = background.color;
        current_color.a = 0.5f;
        background.color = current_color;

        platform_exp.pitch = Random.Range(1f, 1.25f);
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

        if (has_scored_before) { Timer.add_multiplier(); }
        has_scored_before = true;
        combo_fill.fillAmount = 1f;

        if (slowmo_tut) { Time.timeScale = 0.25f; }
        Freeze();
    }

    public IEnumerator DoFreeze()
    {
        is_frozen = true;
        float original = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(freeze_duration);

        Time.timeScale = original;
        pending_freeze_duration = 0f;
        is_frozen = false;
        can_destroy_platform = true;
    }

    private void play_wind()
    {
        if (can_play_wind)
        {
            wind.pitch = Random.Range(1f, 1.25f);
            wind.Play();
            can_play_wind = false;
        }
    }

    public void Freeze() => pending_freeze_duration = freeze_duration;

    public void update_fire_amount(int amount)
    {
        foreach (ParticleSystem particle in particles)
        {
            var em = particle.emission;
            em.rateOverTime = amount;
        }
    }

    private void explode()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, filed_of_impact, layertohit);
        foreach (Collider2D obj in objects)
        {
            if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D objRb))
            {
                Vector2 direction = obj.transform.position - transform.position;
                objRb.AddForce(direction * force);
            }
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