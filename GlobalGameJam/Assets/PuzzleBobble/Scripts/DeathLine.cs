using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLine : MonoBehaviour
{

    public float falshDur = 0.3f;

    public SpriteRenderer[] sprites;

    private Color originalColor;

    bool flashing = false;

    private void Start()
    {
        originalColor = sprites[0].color;
    }

    public void StartFlash()
    {
        if (flashing) return;

        flashing = true;

        Color targetColor = new Color(1f, 0f, 0f, 1f);

        foreach (var sprite in sprites)
        {
            LeanTween.value(sprite.gameObject, originalColor, targetColor, falshDur)
                .setOnUpdate((Color color) => sprite.color = color)
                .setLoopPingPong()
                .setEase(LeanTweenType.easeInOutSine);
        }
    }
    public void StopFlash()
    {
        if (!flashing) return;


        flashing = false;

        foreach (var sprite in sprites)
        {
            LeanTween.cancel(sprite.gameObject);

            LeanTween.value(sprite.gameObject, sprite.color, originalColor, falshDur)
                .setOnUpdate((Color color) => sprite.color = color)
                .setEase(LeanTweenType.easeInOutSine);
        }
    }

}
