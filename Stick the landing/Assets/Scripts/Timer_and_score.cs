using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public Player player;
    public float start_time;
    public TextMeshProUGUI timetext;
    public TextMeshProUGUI score;
    public TextMeshProUGUI real_score_text;
    private int real_score;
    private int current_score = 0;
    public TextMeshProUGUI Multiple;
    public GameObject multiplier;
    public GameObject Close_Call;
    private int Current_multiplier = 1;
    private float close_timer = 3f;
    public Level_platform_manager manager;

    //MUSIC
    public AudioSource music_1;
    public AudioSource music_2;
    public AudioSource music_3;
    public AudioSource music_4;
    public AudioSource music_5;

    public AudioSource music_1_second;
    public AudioSource music_2_second;
    public AudioSource music_3_second;
    public AudioSource music_4_second;
    public AudioSource music_5_second;

    public GameObject track1;
    public GameObject track2;
    //SCORE BREAK
    public AudioSource s_b_1;
    public AudioSource s_b_2;
    //Close_call
    public AudioSource close_call_sound;
    //STARTING
    public bool started = false;
    public GameObject start_txt;
    //main menu
    public GameObject main_menu;
    public GameObject time_display;
    public GameObject score_display;
    public GameObject real_score_display;
    public GameObject[] start_blocks;
    //TUTORIAL
    public GameObject tutorial;
    //PAUSING
    public bool can_pause = false;
    public GameObject pause_ui;
    public static bool is_paused = false;
    //ENDGAME
    private bool can_display_end = true;
    public GameObject end_panel;
    public TextMeshProUGUI end_score;
    //JUICE
    public GameObject score_obj;

    public AudioSource button_press;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float num = Random.Range(0, 2);
        if (num > 0)
        {
            track1.SetActive(true);
            track2.SetActive(false);
        }
        else
        {
            track1.SetActive(false);
            track2.SetActive(true);
        }
        print(num);
    }

    // Update is called once per frame
    void Update()
    {
        if (started) { start_time -= Time.deltaTime; }
        
        timetext.text = start_time.ToString("0.00");
        score.text = current_score.ToString();
        real_score_text.text = real_score.ToString();
        //timetext.text = Mathf.FloorToInt(start_time).ToString();
        if(Close_Call.activeSelf == true)
        {
            close_timer -= Time.deltaTime;
            if (close_timer < 0) 
            { 
                Close_Call.SetActive(false);
                close_timer = 3;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && can_pause) 
        {
            if (is_paused) 
            {
                resume();
            }
            else
            {
                pause();
            }
        }

    }

    public void display_end()
    {
        add_multiplier();
        end_panel.SetActive(true);
        end_score.text = real_score.ToString();
        time_display.SetActive(false);
        score_display.SetActive(false);
        real_score_display.SetActive(false);
    }
    public void resume()
    {
        pause_ui.SetActive(false);
        Time.timeScale = 1f;
        is_paused = false;
        button_press.Play();
    }

    public void pause()
    {
        pause_ui.SetActive(true);
        Time.timeScale = 0f;
        is_paused=true;
        button_press.Play();
    }

    public void quit()
    {
        Application.Quit();
    }

    private void FixedUpdate()
    {
        if (start_time <= 0)
        {
            Time.timeScale = 0.05f;
            if (can_display_end) { display_end(); can_display_end = false; }
            
        }
    }

    public void add_point()
    {
        score.enabled = true;
        current_score++;
        start_time += 1.0f;
    }

    public void update_music()
    {
        if (Current_multiplier < 10)
        {
            music_1.volume = 1;
            music_2.volume = 0;
            music_3.volume = 0;
            music_4.volume = 0;
            music_5.volume = 0;
            print("music 1");
            music_1_second.volume = 1;
            music_2_second.volume = 0;
            music_3_second.volume = 0;
            music_4_second.volume = 0;
            music_5_second.volume = 0;
        }
        else if (Current_multiplier >= 10 && Current_multiplier < 20 ) 
        {
            music_1.volume = 0;
            music_2.volume = 1;
            music_3.volume = 0;
            music_4.volume = 0;
            music_5.volume = 0;
            print("music 2");
            music_1_second.volume = 0;
            music_2_second.volume = 1;
            music_3_second.volume = 0;
            music_4_second.volume = 0;
            music_5_second.volume = 0;
        }
        else if (Current_multiplier >= 20 && Current_multiplier < 30)
        {
            music_1.volume = 0;
            music_2.volume = 0;
            music_3.volume = 1;
            music_4.volume = 0;
            music_5.volume = 0;
            print("music 3");
            music_1_second.volume = 0;
            music_2_second.volume = 0;
            music_3_second.volume = 1;
            music_4_second.volume = 0;
            music_5_second.volume = 0;
        }
        else if (Current_multiplier >= 30 && Current_multiplier < 40)
        {
            music_1.volume = 0;
            music_2.volume = 0;
            music_3.volume = 0;
            music_4.volume = 1;
            music_5.volume = 0;
            print("music 4");
            music_1_second.volume = 0;
            music_2_second.volume = 0;
            music_3_second.volume = 0;
            music_4_second.volume = 1;
            music_5_second.volume = 0;
        }
        else if (Current_multiplier >= 40 )
        {
            music_1.volume = 0;
            music_2.volume = 0;
            music_3.volume = 0;
            music_4.volume = 0;
            music_5.volume = 1;
            print("music 5");
            music_1_second.volume = 0;
            music_2_second.volume = 0;
            music_3_second.volume = 0;
            music_4_second.volume = 0;
            music_5_second.volume = 1;
        }
        reset_player_ping_pitch();
    }

    public void reset_player_ping_pitch()
    {
        if (Current_multiplier % 10 == 0  )
        {
            player.reset_ping_pitch();
        }
    }
    public void add_multiplier()
    {
        multiplier.SetActive(true);
        Current_multiplier ++;
        Multiple.text = Current_multiplier.ToString();
        update_music();
        LeanTween.scale(score_obj, new Vector3(2f, 2f, 2f), 0.05f).setOnComplete(score_obj_scaledown);
        //print("multiplier added");
    }
    public void score_obj_scaledown()
    {
        LeanTween.scale(score_obj, new Vector3(1f, 1f, 1f), 0.1f);
    }
    public void reset_multiplier()
    {
        player.reset_ping_pitch();
        if (Current_multiplier > 1)
        {
            s_b_1.Play();
            s_b_2.Play();
        }
        multiplier.SetActive(false);
        score.enabled = false;
        real_score += current_score * Current_multiplier;
        current_score = 0;
        Current_multiplier = 1;
        manager.reset_platforms();

        music_1.volume = 1;
        music_2.volume = 0;
        music_3.volume = 0;
        music_4.volume = 0;
        music_5.volume = 0;
        print("music 1");
        music_1_second.volume = 1;
        music_2_second.volume = 0;
        music_3_second.volume = 0;
        music_4_second.volume = 0;
        music_5_second.volume = 0;
    }

    public void close_call()
    {

        Close_Call.SetActive(true);
        add_multiplier();
        LeanTween.scale(Close_Call, new Vector3(3f, 3f, 3f), 0.05f).setOnComplete(close_call_scaledown);
        close_call_sound.Play();
    }

    public void close_call_scaledown()
    {
        //Close_Call.SetActive(true);
        LeanTween.scale(Close_Call, new Vector3(1f, 1f, 1f), 0.05f);
    }

    public void start_game()
    {
        started = true;
        time_display.SetActive(true);
        score_display.SetActive(true);
        real_score_display.SetActive(true);
        main_menu.SetActive(false);
        foreach(GameObject s in start_blocks) { s.SetActive(true); }
        start_txt.SetActive(true);
        can_pause = true;
        button_press.Play();
    }

    public void start_tutorial()
    {
        //started = true;
        //time_display.SetActive(true);
        //score_display.SetActive(true);
        //real_score_display.SetActive(true);
        main_menu.SetActive(false);
        //foreach (GameObject s in start_blocks) { s.SetActive(true); }
        player.start_tutorial();
        tutorial.SetActive(true);
        button_press.Play();
    }

    public void back_to_menu()
    {
        player.stop_tutorial();
        main_menu.SetActive(true);
        tutorial.SetActive(false);
        Time.timeScale = 1f;
        button_press.Play();
    }

    public void back_to_menu_fromstart()
    {
        player.stop_tutorial();
        main_menu.SetActive(true);
        tutorial.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        button_press.Play();
    }
}
