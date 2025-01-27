using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuzzleBobble;

public class BubbleConnections : MonoBehaviour
{
    public List<Bubble> connectedBubbles = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bubble bubble = collision.GetComponent<Bubble>();
        if (bubble)
        {
            connectedBubbles.Add(bubble);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Bubble bubble = collision.GetComponent<Bubble>();
        if (bubble)
        {
            connectedBubbles.Remove(bubble);
        }
    }

}
