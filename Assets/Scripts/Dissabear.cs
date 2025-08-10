using System.Collections;
using UnityEngine;

public class Dissabear : MonoBehaviour
{


    public float maxdealy = 5f;
    public float mindealy = 1f;


    public void Start()
    {
        StartCoroutine(inActive());
       

    }

    IEnumerator inActive()
    {
        while (true)
        {

            yield return new WaitForSeconds(Random.Range(maxdealy, mindealy));

            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
