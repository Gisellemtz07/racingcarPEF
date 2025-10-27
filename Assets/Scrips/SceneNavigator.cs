// Opcional, por si te gusta tener uno utilitario:
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public void GoTo(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
