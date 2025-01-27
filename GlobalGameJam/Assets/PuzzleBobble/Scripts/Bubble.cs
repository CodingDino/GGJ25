using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class Bubble : MonoBehaviour
    {
        
        public string bubbleType = "Green";

        BubbleRow currentGrid = null;

        private void OnTriggerEnter2D(Collider2D collision)

        {
            BubbleRow grid = collision.GetComponent<BubbleRow>();
            if (grid)
            {
                currentGrid = grid;
            }


            Bubble otherBubble = collision.GetComponent<Bubble>();
            if (otherBubble)
            {
                // Only act if we were the moving bubble:
                if (!GetComponent<Rigidbody2D>().isKinematic && currentGrid)
                {
                    Debug.Log("Collision!");

                    // Make the bubble a child of the grid
                    transform.parent = currentGrid.transform;

                    // turn off rigidbody motion
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    GetComponent<Rigidbody2D>().isKinematic = true;
                    // Find correct place in grid and go to it
                    Vector3 targetPos = currentGrid.GetClosestLocation(transform.position);
                    transform.position = targetPos;
                    // TODO: Lerp instead of instant snap

                    // Tell bubble grid that this bubble is now in place
                    currentGrid.AddBubble(this);

                    // TODO: squash/stretch bubble effect for bubble setting (maybe also squash/stretch the bubble(s) we hit

                    // TODO: If the bubble grid says we are touching enough other bubbles...

                    // TODO: clear bubbles
                }
            }
        }

    }
}
