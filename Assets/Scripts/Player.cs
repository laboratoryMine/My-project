using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{


    public float speed = 10f;

 

    Rigidbody rb;


    public TextMeshProUGUI coin;

    int cointtext = 0;

    public string nextLevel;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        coin.text = cointtext.ToString();
    }

    private void Update()
    {
        float leftRigt = Input.GetAxis("Horizontal");

        float frontBack = Input.GetAxis("Vertical");


       Vector3 move = new Vector3(leftRigt * speed * Time.deltaTime , frontBack * speed * Time.deltaTime);

        Vector3 movement = new Vector3(leftRigt, 0f, frontBack) * speed * Time.deltaTime;

        rb.MovePosition(rb.position + movement);

        if(Input.GetKey(KeyCode.LeftShift))
        {

            speed = 20;

        }
        else
        {
            speed = 15;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coins")
        {
            cointtext++;
            coin.text = cointtext.ToString();
            Destroy(other.gameObject);
        }

        if(other.tag == "Level")
        {
            SceneManager.LoadScene(nextLevel);
        }

        if(other.tag == "Kill")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }



}
