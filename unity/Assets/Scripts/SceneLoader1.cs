using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader1 : MonoBehaviour
{
    public void LoadScene1(string Start)
    {
        SceneManager.LoadScene(Start);
    }
    public void LoadScene2(string SampleScene)
    {
        SceneManager.LoadScene(SampleScene);
    }
}
