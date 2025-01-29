using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public CanvasGroup group;
    public TMP_Text titleText;
    public float transInDur = 0.3f;

    private bool shown = false;
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
        if (shown && Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
