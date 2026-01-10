using UnityEngine;

public class BPM_BACKGROUND_1 : MonoBehaviour
{
    public float wait_time;
    public Vector3 size_from;
    public Vector3 size_to;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GROW();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GROW()
    {
        LeanTween.scale(gameObject, size_to, 0.176f).setOnComplete(SHRINK);
    }
    void SHRINK()
    {
        LeanTween.scale(gameObject, size_from, 0.176f).setOnComplete(GROW);
    }
}
