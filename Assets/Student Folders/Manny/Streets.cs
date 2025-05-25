using UnityEngine;

public class Streets : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y > -13f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 13.4f, transform.position.z);
        }
    }
}
