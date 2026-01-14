using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Timer : MonoBehaviour
{
    public Player player;
    private float start_time;
    public TextMeshProUGUI timetext;
    public TextMeshProUGUI score;
    public TextMeshProUGUI real_score_text;
    public TextMeshProUGUI HIGHSCORE;
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

    public GameObject[] difficulty_buttons;
    public GameObject[] menu_buttons;
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
    // GOAL
    private float maxgoal = 100;
    public float currentgoal = 0;

    [SerializeField] private Image currentgoalbarfill;
    [SerializeField] private Image currentgoalbarcopy;
    public bool can_reduce = true;
    public AudioSource ping;
    public TextMeshProUGUI max_goal_text;

    //MOVING PLATFORMS
    public Level_platform_manager level_manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_time = 1;
        Current_multiplier = 1;
        currentgoal = 0;
        max_goal_text.text = maxgoal.ToString();
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
        //print(num);

        
    }

    // Update is called once per frame
    void Update()
    {
        currentgoalbarfill.fillAmount = Mathf.Clamp(currentgoal / maxgoal, 0, 1);

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

        if (track1.active)
        {
            
        }
        if(Current_multiplier < 1)
        {
            Current_multiplier = 1;
        }
       

        //goal_fill.fillAmount = current_goal_fill_amount/10;

    }

    public void display_end()
    {
        add_multiplier();
        end_panel.SetActive(true);
        end_score.text = real_score.ToString();
        time_display.SetActive(false);
        score_display.SetActive(false);
        real_score_display.SetActive(false);
        update_high_score(); 
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
        currentgoal += 10f;
        if(currentgoal >= maxgoal)
        {
            currentgoal = 0;
            maxgoal += 50;
        }
        ping.pitch = (currentgoal / maxgoal) + 1;
        max_goal_text.text = maxgoal.ToString();
    }

    public void update_music()
    {
        if (maxgoal == 100)
        {
            music_1.volume = 1;
            music_2.volume = 0;
            music_3.volume = 0;
            music_4.volume = 0;
            music_5.volume = 0;
            //print("music 1");
            music_1_second.volume = 1;
            music_2_second.volume = 0;
            music_3_second.volume = 0;
            music_4_second.volume = 0;
            music_5_second.volume = 0;
        }
        else if (maxgoal == 150 ) 
        {
            music_1.volume = 0;
            music_2.volume = 1;
            music_3.volume = 0;
            music_4.volume = 0;
            music_5.volume = 0;
            //print("music 2");
            music_1_second.volume = 0;
            music_2_second.volume = 1;
            music_3_second.volume = 0;
            music_4_second.volume = 0;
            music_5_second.volume = 0;
        }
        else if (maxgoal == 200)
        {
            music_1.volume = 0;
            music_2.volume = 0;
            music_3.volume = 1;
            music_4.volume = 0;
            music_5.volume = 0;
           // print("music 3");
            music_1_second.volume = 0;
            music_2_second.volume = 0;
            music_3_second.volume = 1;
            music_4_second.volume = 0;
            music_5_second.volume = 0;
            level_manager.can_spawn_movers = true;
            level_manager.speed_to_set_to_moving_platforms = 1f;
        }
        else if (maxgoal == 250)
        {
            music_1.volume = 0;
            music_2.volume = 0;
            music_3.volume = 0;
            music_4.volume = 1;
            music_5.volume = 0;
            //print("music 4");
            music_1_second.volume = 0;
            music_2_second.volume = 0;
            music_3_second.volume = 0;
            music_4_second.volume = 1;
            music_5_second.volume = 0;
            level_manager.speed_to_set_to_moving_platforms = 2.5f;
        }
        else if (maxgoal== 300 )
        {
            music_1.volume = 0;
            music_2.volume = 0;
            music_3.volume = 0;
            music_4.volume = 0;
            music_5.volume = 1;
            //print("music 5");
            music_1_second.volume = 0;
            music_2_second.volume = 0;
            music_3_second.volume = 0;
            music_4_second.volume = 0;
            music_5_second.volume = 1;
            level_manager.speed_to_set_to_moving_platforms = 5f;
            
        }
        reset_player_ping_pitch();
    }

    public void update_high_score()
    {
        if (PlayerPrefs.HasKey("HIGHSCORE"))
        {
            if(real_score > PlayerPrefs.GetInt("HIGHSCORE"))
            {
                PlayerPrefs.SetInt("HIGHSCORE", real_score);
            }
        }
        else
        {
            PlayerPrefs.SetInt("HIGHSCORE", real_score);
        }
        HIGHSCORE.text = PlayerPrefs.GetInt("HIGHSCORE").ToString();
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
        //print("music 1");
        music_1_second.volume = 1;
        music_2_second.volume = 0;
        music_3_second.volume = 0;
        music_4_second.volume = 0;
        music_5_second.volume = 0;

        currentgoal = 0;
        maxgoal = 100;
        max_goal_text.text = maxgoal.ToString();
    }

    public void close_call()
    {

        Close_Call.SetActive(true);
        add_multiplier();
        LeanTween.scale(Close_Call, new Vector3(3f, 3f, 3f), 0.05f).setOnComplete(close_call_scaledown);
        close_call_sound.Play();
        currentgoal += 10f;
        if (currentgoal >= maxgoal)
        {
            currentgoal = 0;
            maxgoal += 50;
        }
        ping.pitch = (currentgoal / maxgoal) + 1;
    }

    public void close_call_scaledown()
    {
        //Close_Call.SetActive(true);
        LeanTween.scale(Close_Call, new Vector3(1f, 1f, 1f), 0.05f);
    }

    public void start_game()
    {
        foreach (GameObject m in menu_buttons) { m.SetActive(false); }
        foreach (GameObject d in difficulty_buttons) { d.SetActive(true); }
        


    }

    

    public void start_game_easy()
    {
        start_time = 21;
        started = true;
        time_display.SetActive(true);
        score_display.SetActive(true);
        real_score_display.SetActive(true);
        main_menu.SetActive(false);
        foreach (GameObject s in start_blocks) { s.SetActive(true); }
        start_txt.SetActive(true);
        can_pause = true;
        button_press.Play();
    }

    public void start_game_mideum()
    {
        start_time = 14;
        started = true;
        time_display.SetActive(true);
        score_display.SetActive(true);
        real_score_display.SetActive(true);
        main_menu.SetActive(false);
        foreach (GameObject s in start_blocks) { s.SetActive(true); }
        start_txt.SetActive(true);
        can_pause = true;
        button_press.Play();
    }

    public void start_game_hard()
    {
        start_time = 7;
        started = true;
        time_display.SetActive(true);
        score_display.SetActive(true);
        real_score_display.SetActive(true);
        main_menu.SetActive(false);
        foreach (GameObject s in start_blocks) { s.SetActive(true); }
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
        foreach (GameObject d in difficulty_buttons) { d.SetActive(false); }
        foreach (GameObject m in menu_buttons) { m.SetActive(true); }

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
