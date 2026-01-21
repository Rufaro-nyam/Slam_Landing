using UnityEngine;

public class Panel_lerp_mvt : MonoBehaviour
{
    public Vector3 Lerp_to;
    public Vector3 Lerp_from;
    public float lerp_speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void panel_appear()
    {
        LeanTween.moveLocal(gameObject, Lerp_to, lerp_speed);
    }

    public void panel_dissapear()
    {
        LeanTween.moveLocal(gameObject, Lerp_from, lerp_speed);
    }
}
