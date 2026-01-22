using UnityEngine;

public class Customisation : MonoBehaviour
{
    public GameObject[] eyes;
    public GameObject[] mouths;
    public AudioSource button_press;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("EYE_INT"))
        {

            update_eyes(PlayerPrefs.GetInt("EYE_INT"));

        }
        else
        {
            update_eyes(0);
        }

        if (PlayerPrefs.HasKey("MOUTH_INT"))
        {

            update_mouth(PlayerPrefs.GetInt("MOUTH_INT"));

        }
        else
        {
            update_mouth(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void play_button_sound()
    {
        
    }

    public void update_eyes(int eye_num)
    {
        button_press.Play();
        button_press.Play();
        foreach (GameObject go in eyes) 
        {
            go.SetActive(false);

        }
        eyes[eye_num].SetActive(true);
        print(eye_num);
        if (PlayerPrefs.HasKey("EYE_INT"))
        {

                PlayerPrefs.SetInt("EYE_INT", eye_num);

        }
        else
        {
            PlayerPrefs.SetInt("EYE_INT", eye_num);
        }

    }

    public void update_mouth(int mouth_num)
    {
        button_press.Play();
        foreach (GameObject go in mouths)
        {
            go.SetActive(false);

        }
        mouths[mouth_num].SetActive(true);
        print(mouth_num);

        if (PlayerPrefs.HasKey("MOUTH_INT"))
        {

            PlayerPrefs.SetInt("MOUTH_INT", mouth_num);

        }
        else
        {
            PlayerPrefs.SetInt("MOUTH_INT", mouth_num);
        }

    }
}
