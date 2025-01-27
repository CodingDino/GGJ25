using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace PuzzleBobble
{
    public class BubbleRow : MonoBehaviour
    {
        public Bubble[] bubblePrefabs;
        [HideInInspector]
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

        [Button]
        public void SetUpEdgeCollider()
        {
            EdgeCollider2D col = GetComponent<EdgeCollider2D>();
            Vector2[] points = col.points;
            points[1] = new Vector2(bubbleSlots.Length*spacing, 0);
            col.points = points;
            col.offset = Vector2.right * (-spacing * 0.5f + offset);
        }

        [Button]
        public void AlignBubbles()
        {
            Vector3 basePos = transform.position + Vector3.right * offset;
            for (int i = 0; i < bubbleSlots.Length; ++i)
            {
                if (bubbleSlots[i])
                {
                    Vector3 newPos = basePos + i * spacing * Vector3.right;
                    bubbleSlots[i].transform.position = newPos;
                }
            }
        }
        [Button]
        public void SpawnBubbles()
        {
            for (int i = 0; i < bubbleSlots.Length; ++i)
            { 
                if (bubbleSlots[i])
                    DestroyImmediate(bubbleSlots[i].gameObject,true);
            }
            bubbleSlots = new Bubble[bubblePrefabs.Length];
            for (int i = 0; i < bubblePrefabs.Length; ++i)
            {
                if (bubblePrefabs[i])
                {
                    bubbleSlots[i] = Instantiate(bubblePrefabs[i],transform);
                }
            }
            AlignBubbles();
        }

    }
}
