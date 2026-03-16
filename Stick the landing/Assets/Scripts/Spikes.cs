using UnityEngine;

public class Spikes : MonoBehaviour
{
    private float speed = 5;
    public bool is_moving;
    private bool going_right;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float dir_num = Random.Range(0, 2);
        if (is_moving && dir_num == 0)
        {
            going_right = false;
            speed = -speed;
        }
        else
        {
            going_right = true;
            speed = speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (is_moving)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        if (transform.position.x >= 10.37f)
        {
            going_right = false;
            speed = -speed;
        }
        if (transform.position.x <= -10.02f)
        {
            going_right = true;
            speed = Mathf.Abs(speed);
        }
    }
}
