using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class Bubble : MonoBehaviour
    {
        
        public string bubbleType = "Green";
        public int clearRequirement = 3;

        BubbleRow currentGrid = null;

        [HideInInspector]
        public bool canBeStuckTo = false;

        private void OnTriggerEnter2D(Collider2D collision)

        {
            BubbleRow grid = collision.GetComponent<BubbleRow>();
            if (grid)
            {
                currentGrid = grid;
            }


            Bubble otherBubble = collision.GetComponent<Bubble>();
            if (otherBubble && otherBubble.canBeStuckTo)
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
                    List<Bubble> chain = new();
                    chain.Add(this);
                    GetChainBubblesOfSameType(ref chain);
                    if (chain.Count >= clearRequirement) // TODO: might need a better way to calculate this if our aliens require higher chains!
                    {
                        // Clear the chain!
                        foreach(var clearBub in chain)
                        {
                            clearBub.ClearBubble();
                        }
                    }
                }
            }
        }

        void ClearBubble()
        {
            // Remove from grid
            GetComponentInParent<BubbleRow>().RemoveBubble(this);

            // TODO: Animate first
            Destroy(gameObject);
        }

        void GetChainBubblesOfSameType(ref List<Bubble> chain)
        {
            List<Bubble> connectedBubbles = GetComponentInChildren<BubbleConnections>().GetConnectedBubbles();

            for (int i = 0; i < connectedBubbles.Count; ++i)
            {
                // If the bubble is the same type and not contained in the chain already...
                if (connectedBubbles[i].bubbleType == bubbleType && !chain.Contains(connectedBubbles[i]))
                {
                    // Add the conneted bubble bubble
                    chain.Add(connectedBubbles[i]);

                    // Recursively check that bubble
                    // This is depth-first search
                    connectedBubbles[i].GetChainBubblesOfSameType(ref chain);
                }
            }            
        }

    }
}
