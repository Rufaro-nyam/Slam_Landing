using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Running_out_of_time : MonoBehaviour
{
    public GameObject time;

    public TextMeshProUGUI time_text;
    public Color[] colours;
    private int currentcolorindex = 0;
    private int targetcolourindex = 0;
    public AudioSource tick;
    public bool is_warning = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        expand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void expand()
    {
        if (is_warning)
        {
            LeanTween.scale(time, new Vector3(1.2f, 1.2f, 1.2f), 0.25f).setOnComplete(reduce);
            time_text.color = Color.red;
            if (tick.isPlaying == false)
            {
                tick.Play();
            }
        }
        else
        {
            reduce();
        }

    }

    public void reduce()
    {
        if (is_warning)
        {
            LeanTween.scale(time, new Vector3(1f, 1f, 1f), 0.25f).setOnComplete(expand);
            time_text.color = Color.white;
        }
        else
        {
            LeanTween.scale(time, new Vector3(1f, 1f, 1f), 0.25f).setOnComplete(expand); ;
            time_text.color = Color.white;
            if (tick.isPlaying == true)
            {
                tick.Stop();
            }
        }

    }
}
