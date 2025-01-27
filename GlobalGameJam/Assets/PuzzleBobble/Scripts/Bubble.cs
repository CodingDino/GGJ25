using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class Bubble : MonoBehaviour
    {

        public string bubbleType = "Green";

        List <Bubble> connectedBubbles = new();
        BubbleGrid currentGrid = null;

        private void OnTriggerEnter2D(Collider2D collision)

        {
            BubbleGrid grid = collision.GetComponent<BubbleGrid>();
            if (grid)
            {
                currentGrid = grid;
            }


            Bubble otherBubble = collision.GetComponent<Bubble>();
            if (otherBubble)
            {
                // Only act if we were the moving bubble:
                if (!GetComponent<Rigidbody2D>().isKinematic)
                {

                    // turn off rigidbody motion
                    GetComponent<Rigidbody2D>().isKinematic = true;
                    // Find correct place in grid and go to it
                    Vector3 targetPos = currentGrid.GetClosestLocation(transform.position);
                    transform.position = targetPos;
                    // TODO: Lerp instead of instant snap

                    // TODO: squash/stretch bubble effect for bubble setting (maybe also squash/stretch the bubble(s) we hit

                    // If the other bubble is the same type as this bubble
                    if (bubbleType == otherBubble.bubbleType)
                    {
                        // track what bubbles we're conected to
                        connectedBubbles.AddRange(otherBubble.connectedBubbles);
                        otherBubble.connectedBubbles.Add(this);

                        // TODO: determine if we should take any actions based on bubble type
                        // May depend on alien type etc (some may take more "hits" aka bubbles)
                        // clear bubbles, make other bubbles fall, etc
                    }
                }
            }
        }

    }
}
