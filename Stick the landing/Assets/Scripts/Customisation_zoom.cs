using UnityEngine;

public class Customisation_zoom : MonoBehaviour
{
    public Transform zoom_pos;
    public bool is_customising = false;
    public Vector3 initial_pos;
    public float initial_fov;
    public float zoom_fov;
    private Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_customising) 
        {
            transform.position = Vector3.Lerp(transform.position, zoom_pos.position, 0.05f);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom_fov, 0.05f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, initial_pos, 0.05f);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initial_fov, 0.05f);
        }
    }

    public void customize()
    {
        is_customising =true;
    }

    public void exit_customize()
    {
        is_customising = false ;
    }


}
