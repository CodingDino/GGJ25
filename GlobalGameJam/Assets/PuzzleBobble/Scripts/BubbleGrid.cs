using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class BubbleGrid : MonoBehaviour
    {
        public Bubble[] bubbleSlots;
        public float spacing = 1.0f;
        public float offset = 0f;

        public Vector3 GetClosestLocation(Vector3 realPos)
        {
            Vector3 basePos = transform.position + Vector3.right * offset;
            Vector3 closestPos = basePos;

            for(int i = 0; i < bubbleSlots.Length; ++i)
            {
                Vector3 newPos = basePos + i * spacing * Vector3.right;
                if ((newPos - realPos).sqrMagnitude < (closestPos - realPos).sqrMagnitude)
                {
                    // New closest!
                    closestPos = newPos;
                }
            }

            return closestPos;
        }

    }
}
