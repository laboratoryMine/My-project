using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public string nextScene;


    public void PlayNext()
    {


        SceneManager.LoadScene(nextScene);

    }
}
