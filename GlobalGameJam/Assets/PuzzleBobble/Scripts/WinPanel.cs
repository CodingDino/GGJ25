using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public CanvasGroup group;
    public TMP_Text titleText;
    public float transInDur = 0.3f;

    private bool shown = false;
    bool loading = false;

    public CanvasGroup cover;

    private void Start()
    {
        LeanTween.alphaCanvas(cover, 0f, 0.5f);
    }
    public void ShowPanel(int playerWin)
    {
        titleText.text = "Player " + playerWin+" wins!";

        LeanTween.alphaCanvas(group, 1f, transInDur).setOnComplete(() => {
            group.interactable = true;
            group.blocksRaycasts = true;
            shown = true;
        }).setIgnoreTimeScale(true).setEase(LeanTweenType.easeInOutQuad);
    }

    private void Update()
    {
        if (shown && Input.anyKeyDown && !loading)
        {
            loading = true;
            StartCoroutine(LoadScene());
        }
    }
    IEnumerator LoadScene()
    {
        LeanTween.alphaCanvas(cover, 1f, 0.5f);

        yield return new WaitForSecondsRealtime(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

}
