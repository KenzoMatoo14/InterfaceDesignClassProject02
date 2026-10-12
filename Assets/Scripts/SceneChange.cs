
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void SceneA()
    {
        SceneManager.LoadScene(1);
    }

    public void SceneB(){
        SceneManager.LoadScene(2);
    }
}