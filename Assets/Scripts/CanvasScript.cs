using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
    public void OnBack()
    {
        SceneManager.LoadScene("GameScene");
    }
}
