using UnityEngine;
using System.Collections;

public class BulletFire : MonoBehaviour
{

    public float bulletSpeed;

    public GameObject m_shooter;
    public GameObject m_target;

    private Vector3 destination;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_target != null)
        {
            Vector3 vec2Dir = m_target.transform.position - transform.position;
            vec2Dir.Normalize();

            Vector3 vec2Temp = vec2Dir * (Time.deltaTime * bulletSpeed);

            vec2Temp = transform.position + vec2Temp;

            float distance1 = Vector3.Distance(vec2Temp, transform.position);
            float distance2 = Vector3.Distance(m_target.transform.position, transform.position);

            if (distance1 >= distance2)
            {
                transform.position = m_target.transform.position;
                Destroy(gameObject);
            }
            else
            {
                transform.position = vec2Temp;
            }

            destination = m_target.transform.position;
        }

        else
        {
            Vector3 vec2Dir = destination - transform.position;
            vec2Dir.Normalize();

            Vector3 vec2Temp = vec2Dir * (Time.deltaTime * bulletSpeed);

            vec2Temp = transform.position + vec2Temp;

            float distance1 = Vector3.Distance(vec2Temp, transform.position);
            float distance2 = Vector3.Distance(destination, transform.position);

            if (distance1 >= distance2)
            {
                transform.position = destination;
                Destroy(gameObject);
            }
            else
            {
                transform.position = vec2Temp;
            }
        }
    }
}
