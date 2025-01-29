using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PuzzleBobble
{
    public class Bubble : MonoBehaviour
    {
        
        public string bubbleType = "Green";
        public int clearRequirement = 3;

        [HideInInspector]
        public bool canBeStuckTo = false;

        public bool cleared = false;

        public PlayerControl control = null;

        public Vector2Int rowCol = new();

        public BubbleRow parentRow = null;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            Bubble otherBubble = collision.GetComponent<Bubble>();
            if (otherBubble && otherBubble.canBeStuckTo)
            {
                // Only act if we were the moving bubble:
                if (!GetComponent<Rigidbody2D>().isKinematic)
                {
                    Debug.Log("Collision!");

                    // Get the bubble matrix for this side
                    BubbleMatrix[] matrices = FindObjectsByType<BubbleMatrix>(FindObjectsSortMode.None);
                    BubbleMatrix matrix = matrices[0];
                    if ((matrices[1].transform.position-transform.position).sqrMagnitude < (matrices[0].transform.position - transform.position).sqrMagnitude)
                    {
                        matrix = matrices[1];
                    }

                    // Snap the bubble in place
                    matrix.SnapBubble(this);

                    // Tell the player they can fire again
                    control.canFire = true;


                    // If the bubble grid says we are touching enough other bubbles...
                    List<Bubble> chain = new();
                    GetChainBubblesOfSameType(ref chain);
                    if (chain.Count >= clearRequirement) // TODO: might need a better way to calculate this if our aliens require higher chains!
                    {
                        // Will need to check every bubble connected to the chain of bubbles that is being removed
                        List<Bubble> toCheckForRemoval = GetConnectedBubblesForGroup(chain);

                        // Clear the chain!
                        foreach (var clearBub in chain)
                        {
                            clearBub.ClearBubble();
                        }

                        // Check if clearing this chain has caused other bubbles to fall
                        // Check every bubble connected to the chain of bubbles that is being removed
                        List<Bubble> markForRemoval = new();
                        foreach(var checkFall in toCheckForRemoval)
                        {
                            // IF the bubble isn't already marked for deletion
                            if (!markForRemoval.Contains(checkFall) && !checkFall.cleared)
                            {
                                // Check if there is a path to the top row ()
                                List<Bubble> searched = new();
                                bool hasPathToTopRow = checkFall.HasPathToTopRow(ref searched);

                                // If there is no path, mark the bubble (and all connected bubbles) for deletion
                                if (!hasPathToTopRow)
                                {
                                    List<Bubble> fallingChain = new();
                                    checkFall.GetConnectedChain(ref fallingChain);
                                    markForRemoval.AddRange(fallingChain);
                                }
                            }
                        }

                        // Remove those marked
                        foreach (var toRemove in markForRemoval)
                        {
                            // TODO: May need it's own function later
                            toRemove.ClearBubble();
                        }
                    }
                }
            }
        }

        void GetConnectedChain(ref List<Bubble> chain)
        {
            chain.Add(this);

            List<Bubble> connectedBubbles = parentRow.matrix.GetConnectedBubbles(rowCol.x, rowCol.y);
            foreach (var connectedBubble in connectedBubbles)
            {
                if (!chain.Contains(connectedBubble))
                {
                    connectedBubble.GetConnectedChain(ref chain);
                }
            }
        }

        void ClearBubble()
        {
            cleared = true;

            // Remove from grid
            GetComponentInParent<BubbleRow>().RemoveBubble(this);

            // TODO: Animate first
            Destroy(gameObject);
        }

        bool IsInTopRow()
        {
            return GetComponentInParent<BubbleRow>().IsTopRow();
        }

        bool HasPathToTopRow(ref List<Bubble> searched)
        {
            // We will return true if a root has been found
            bool rootFound = false;

            // Add current bubble to search pool
            searched.Add(this);

            // Check if current bubble is at the top, if so, return true.
            // Cleared bubbles don't count
            rootFound = IsInTopRow();
            if (rootFound) return rootFound;

            // If not, we need to check the bubbles we're connected to to try and find a path

            // Get bubbles connected to this one
            if(!parentRow || !parentRow.matrix)
            {
                Debug.LogError("not initialised correctly");
            }
            List<Bubble> connectedBubbles = parentRow.matrix.GetConnectedBubbles(rowCol.x, rowCol.y);

            // Sort by bubble with the highest y value
            // comparing b to a sorts in descending order
            connectedBubbles.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

            // Check each connected bubble in order from highest y to lowest (highest y most likely to get close to 
            foreach (var connected in connectedBubbles)
            {
                // If this bubble hasn't already beel searched...
                // Only bubbles that are not cleared can be considered
                if (!searched.Contains(connected) && !connected.cleared)
                {
                    // Check if there is a path to the top row from this bubble ()
                    // --- uses a heuristic-based depth-first search to find the connected bubble with the highest y value
                    rootFound = connected.HasPathToTopRow(ref searched);

                    // if we found a path, return now!
                    if (rootFound) return rootFound;
                }
                // Otherwise move on to the next bubble connected to this one.
            }

            return rootFound;
        }

        List<Bubble> GetConnectedBubblesForGroup(List<Bubble> group)
        {
            List<Bubble> connectedBubbles = new();
            foreach (var groupBubble in group)
            {
                List<Bubble> currentConnectedBubbles = groupBubble.parentRow.matrix.GetConnectedBubbles(groupBubble.rowCol.x, groupBubble.rowCol.y);
                foreach (var currentBubble in currentConnectedBubbles)
                {
                    if (!connectedBubbles.Contains(currentBubble) && !group.Contains(currentBubble))
                        connectedBubbles.Add(currentBubble);
                }

            }
            return connectedBubbles;
        }


        List<Bubble> GetConnectedBubbles()
        {
            List<Bubble> connectedBubbles = new();
            List<Bubble> currentConnectedBubbles = parentRow.matrix.GetConnectedBubbles(rowCol.x, rowCol.y);
            foreach (var currentBubble in currentConnectedBubbles)
            {
                if (!connectedBubbles.Contains(currentBubble))
                    connectedBubbles.Add(currentBubble);
            }

            return connectedBubbles;
        }

        void GetChainBubblesOfSameType(ref List<Bubble> chain)
        {
            chain.Add(this);

            if (!parentRow || !parentRow.matrix)
            {
                Debug.LogError("not initialised correctly");
            }
            List<Bubble> connectedBubbles = parentRow.matrix.GetConnectedBubbles(rowCol.x, rowCol.y);

            for (int i = 0; i < connectedBubbles.Count; ++i)
            {
                // If the bubble is the same type and not contained in the chain already...
                if (connectedBubbles[i].bubbleType == bubbleType && !chain.Contains(connectedBubbles[i]))
                {
                    // Recursively check that bubble
                    // This is depth-first search
                    connectedBubbles[i].GetChainBubblesOfSameType(ref chain);
                }
            }            
        }

    }
}
