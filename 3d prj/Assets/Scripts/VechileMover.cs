using UnityEngine;

public class VechileMover : MonoBehaviour
{

    [SerializeField]
    private GameObject waypoint1;

    [SerializeField]
    private GameObject waypoint2;


    [SerializeField]
    private GameObject target;


    private const float CLOSE_DISTANCE = 1;
    private const float SPEED = 10.0F;

    [SerializeField]
    private bool flipLookDirection = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance > 0)
        {
            Quaternion rotation;
            if (flipLookDirection)
            {
                rotation = Quaternion.LookRotation(-direction, Vector3.up);
            }
            else
            {
                rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            transform.rotation = rotation;
        }
        Vector3 normDirection = direction / distance;

        transform.position = transform.position + normDirection * SPEED * Time.deltaTime;

        if (distance < CLOSE_DISTANCE)
        {

            if (target.Equals(waypoint1))
            {
                target = waypoint2;
            }
            else if (target.Equals(waypoint2))
            {
                target = waypoint1;
            }

        }
    }
}
