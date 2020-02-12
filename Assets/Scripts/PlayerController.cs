using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float pullForce = 80f;
    public float rotateSpeed = 300f;
    private GameObject closestTower;
    private GameObject hookedTower;

    private bool isPulled = false;
    private Rigidbody2D rigidbody2D;
    private UIControllerScript uiController;
    private AudioSource myAudio;
    private bool isCrashed = false;
    private Vector2 startPosition;
    private Tower tower;
    // Start is called before the first frame update
    void Start()
    {
        tower = GameObject.Find("Tower").GetComponent<Tower>();
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>(); 
        myAudio = this.gameObject.GetComponent<AudioSource>();
        uiController = GameObject.Find("Canvas").GetComponent<UIControllerScript>();  
        startPosition = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody2D.velocity = -transform.up * moveSpeed;
        if (Input.GetKeyDown(KeyCode.Z) && !isPulled)
        {
            if (closestTower != null && hookedTower == null)
            {
                hookedTower = closestTower;
            }

            if (hookedTower)
            {
                float distance = Vector2.Distance(transform.position, hookedTower.transform.position);

                //Gravitation toward tower
                Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;
                float newPullForce = Mathf.Clamp(pullForce / distance, 10, 50);
                rigidbody2D.AddForce(pullDirection * newPullForce);

                //Angular velocity
                rigidbody2D.angularVelocity = -rotateSpeed / distance;
                isPulled = true;

            }
        }

        else if (Input.GetKeyUp(KeyCode.Z))
        {
            isPulled = false;
        }

        if (isCrashed)
        {
            if (!myAudio.isPlaying)
            {
                //Restart Scene
                RestartPosition();
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if(!isCrashed)
            {
                myAudio.Play();
                rigidbody2D.velocity = new Vector3(0f, 0f, 0f);
                rigidbody2D.angularVelocity = 0f;
                isCrashed = true;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Kena");
            uiController.EndGame();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tower" && tower.isClicked )
        {
            Debug.Log("CLICKED");
            closestTower = collision.gameObject;

            //Change tower color black to green as indicator
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isPulled) return;
        if (collision.gameObject.tag == "Tower" || !tower.isClicked)
        {
            closestTower = null;

            //Change tower to normal
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void RestartPosition()
    {
        //set to start position
        this.transform.position = startPosition;

        //Restart rotation 
        this.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        isCrashed = false;
        if (closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }
    }
}
