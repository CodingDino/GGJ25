using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PuzzleBobble;

public class BubbleConnections : MonoBehaviour
{
    public List<Bubble> GetConnectedBubbles()
    {
        // Clear the list before checking overlaps
        List<Bubble> connectedBubbles = new();
        List<Collider2D> touchingColliders = new();

        // Contact filter to determine which layers and conditions to check
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true; // Set to false if you don't want to include triggers
        contactFilter.useLayerMask = false; // Set to true to use a specific layer mask

        // Get all colliders currently overlapping with this collider
        GetComponent<Collider2D>().OverlapCollider(contactFilter, touchingColliders);

        // Debug log the names of all touching colliders
        foreach (Collider2D collider in touchingColliders)
        {
            Bubble otherBubble = collider.GetComponent<Bubble>();
            if (otherBubble && otherBubble.canBeStuckTo)
            {
                connectedBubbles.Add(otherBubble);
            }
        }

        return connectedBubbles;
    }

}
