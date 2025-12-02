using UnityEngine;

public class Fash : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeanTween.scale(gameObject,  new Vector3(3, 3, 3), 0.05f).setOnComplete(scaledown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void scaledown()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.05f).setOnComplete(gone);
    }

    void gone()
    {
        Destroy(gameObject);
    }
}
