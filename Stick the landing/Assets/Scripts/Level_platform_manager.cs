using UnityEngine;

public class Level_platform_manager : MonoBehaviour
{
    public GameObject platform;
    public GameObject moving_platformer;
    public GameObject spike_platform;
    public Transform player;
    private int platforms_destroyed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            spawn_spiker();
        }
    }

    public void spawn_platform()
        
    {
        if (platforms_destroyed > 50)
        {
            float pla_to_spawn = Random.Range(0, 4);
            if (pla_to_spawn == 0)
            {
                Instantiate(platform);
                float y_range2 = Random.Range(-13.7f, 11.5f);
                //print(y_range - player.transform.position.y);
                platform.transform.position = new Vector3(Random.Range(-10.02f, 10.37f), -5.25f, 0);
                platforms_destroyed += 1;
            }
            else
            {
                Instantiate(moving_platformer);
                float y_range2 = Random.Range(-13.7f, 11.5f);
                //print(y_range - player.transform.position.y);
                moving_platformer.transform.position = new Vector3(Random.Range(-10.02f, 10.37f), -5.25f, 0);
                platforms_destroyed += 1;
            }
        }
        else
        {
            Instantiate(platform);
            float y_range = Random.Range(-13.7f, 11.5f);
            //print(y_range - player.transform.position.y);
            platform.transform.position = new Vector3(Random.Range(-10.02f, 10.37f), -5.25f, 0);
            platforms_destroyed += 1;

        }

        if (platforms_destroyed == 70) { spawn_spiker(); }
        if (platforms_destroyed == 100) { spawn_spiker(); }

    }

    public void spawn_spiker()
    {
        Instantiate(spike_platform);
        spike_platform.transform.position = new Vector3(Random.Range(-10.02f, 10.37f), -4.2f, 0);
    }

    public void reset_platforms()
    {
        platforms_destroyed = 0;
    }
}
