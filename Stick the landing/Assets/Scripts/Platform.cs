using UnityEngine;

public class Platform : MonoBehaviour
{

    private GameObject plaform_manager;
    private float speed = 5f;
    public bool is_moving;
    private bool going_right;

    public GameObject[] eyes;
    public GameObject[] squint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        float dir_num = Random.Range(0, 2);
        if(is_moving && dir_num == 0)
        {
            going_right = false;
            speed = -5f;
        }
        else
        {
            going_right=true;
            speed = 5f;
        }
        plaform_manager = GameObject.FindGameObjectWithTag("Manager");
        plaform_manager.TryGetComponent<Level_platform_manager>(out Level_platform_manager platformer);
        LeanTween.scale(gameObject, new Vector3(3f, 3f, 3f), 0.1f).setOnComplete(scalsedown);
    }

    // Update is called once per frame
    void Update()
    {
        if (is_moving)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        if(transform.position.x >= 10.37f)
        {
            going_right = false;
            speed = -5f;
        }
        if (transform.position.x <= -10.02f)
        {
            going_right = true;
            speed = 5f;
        }

    }

    public void destroy()
    {
        plaform_manager = GameObject.FindGameObjectWithTag("Manager");
        plaform_manager.TryGetComponent<Level_platform_manager>(out Level_platform_manager platformer);
        platformer.spawn_platform();
    }

    public void scalsedown()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LeanTween.scale(gameObject, new Vector3(2f, 2f, 2f), 0.05f).setOnComplete(scalsedown);
        if(collision.transform.tag == "Wall_Left")
        {

        }
        if (collision.transform.tag == "Wall_Right")
        {
            print("hit right");
        }
        foreach(GameObject eye in eyes)
        {
            eye.gameObject.SetActive(false);
        }
        foreach (GameObject sq in squint)
        {
            sq.gameObject.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        foreach (GameObject eye in eyes)
        {
            eye.gameObject.SetActive(true);
        }
        foreach (GameObject sq in squint)
        {
            sq.gameObject.SetActive(false);
        }
    }
}
