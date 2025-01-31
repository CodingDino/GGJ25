using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public CanvasGroup cover;


    public string scene;

    private bool loading = false;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.alphaCanvas(cover, 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey && !loading)
        {
            loading = true;
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        LeanTween.alphaCanvas(cover, 1f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(scene);
    }
}
