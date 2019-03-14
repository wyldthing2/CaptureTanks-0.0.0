using UnityEngine;

public class Move : MonoBehaviour
{

    [SerializeField] float speed = 10;
    [SerializeField] float jumpForce = 40;
    float jumpSpeed;
    
    public Transform target;

    /*
    public void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        jumpSpeed = jumpForce;

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }

    }
    */




    void Update()
    {
        // Move the object forward along its z axis 1 unit/second.
        //transform.Translate(Vector3.left * speed * Time.deltaTime);

        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        // Move our position a step closer to the target.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 3, 250), step);

        /*
        transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime);
        jumpSpeed = jumpSpeed - 9.8f;
        */





        /*
        if (transform.position.y <= 5)
        {
            jumpSpeed = jumpForce;
            transform.position.Set (transform.position.x, 5,transform.position.z);
        }
        */


    }
}