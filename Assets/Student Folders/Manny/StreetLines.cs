using UnityEngine;

public class StreetLines : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y > -10f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 7f, transform.position.z);
        }
    }
}
