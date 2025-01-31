using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public string[] playerNames;
    public Color[] playerColors;
    public Image panel;

    public CanvasGroup group;
    public TMP_Text titleText;
    public float transInDur = 0.3f;

    private bool shown = false;
    bool loading = false;

    public string titlescene;

    public CanvasGroup cover;

    private void Start()
    {
        LeanTween.alphaCanvas(cover, 0f, 0.5f);
    }
    public void ShowPanel(int playerWin)
    {
        titleText.text = playerNames[playerWin-1] + " wins!";
        panel.color = playerColors[playerWin - 1];

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

        Time.timeScale = 1f;
        SceneManager.LoadScene(titlescene);
    }

}
