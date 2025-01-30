using PuzzleBobble;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public float bubbleResetTime = 0.2f;
    Bubble recentBubble = null;

    private float lastBubbleTime = 0;

    public AudioSource bounceSFX = null;

    private void Update()
    {
        if (Time.time > bubbleResetTime+lastBubbleTime)
        {
            recentBubble = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bubble bubble = collision.GetComponent<Bubble>();
        if (bubble != null && bubble != recentBubble && !bubble.canBeStuckTo)
        {
            bounceSFX.pitch = Random.Range(0.9f, 1.1f);
            bounceSFX.Play();

            recentBubble = bubble; // protect against repeated changes
            lastBubbleTime = Time.time;

            // change direction of bubble
            Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
            Vector2 vel = rb.velocity;
            vel.x = -vel.x;
            rb.velocity = vel;
        }
    }

}
