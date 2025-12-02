using UnityEngine;

public class rigids : MonoBehaviour
{
    private float start_time = 3;
    public Rigidbody2D[] rigid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Rigidbody2D rigidbody2 in rigid) { rigidbody2.AddForce(new Vector2(0, Random.value), ForceMode2D.Impulse); }
    }

    // Update is called once per frame
    void Update()
    {
        start_time -= Time.deltaTime;
        if (start_time < 0) { Destroy(gameObject); }
        
    }
}
