using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int sceneToLoad;

    public void fadeToScene(int sceneIndex) {
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
    }

    public void onFadeComplete() {
        SceneManager.LoadScene(sceneToLoad);
    }
}
