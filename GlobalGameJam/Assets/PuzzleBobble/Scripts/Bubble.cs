using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class Bubble : MonoBehaviour
    {

        public string bubbleType = "Green";

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
                    Debug.Log("Collision!");

                    // turn off rigidbody motion
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    GetComponent<Rigidbody2D>().isKinematic = true;
                    // Find correct place in grid and go to it
                    Vector3 targetPos = currentGrid.GetClosestLocation(transform.position);
                    transform.position = targetPos;
                    // TODO: Lerp instead of instant snap

                    // TODO: Tell bubble grid that this bubble is now in place

                    // TODO: squash/stretch bubble effect for bubble setting (maybe also squash/stretch the bubble(s) we hit

                    // TODO: If the bubble grid says we are touching enough other bubbles...

                    // TODO: clear bubbles
                }
            }
        }

    }
}
