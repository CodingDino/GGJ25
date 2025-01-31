using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public SpriteRenderer sprite;

    public string nextScene;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        LeanTween.alpha(sprite.gameObject, 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        // wait for scene to be seen

        yield return new WaitForSeconds(3f);

        LeanTween.alpha(sprite.gameObject, 1f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(nextScene);
    }
}
