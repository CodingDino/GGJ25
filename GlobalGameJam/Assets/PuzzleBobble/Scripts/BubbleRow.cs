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

        public void AddBubble(Bubble bubble)
        {
            bubbleSlots[GetClosestLocationIndex(bubble.transform.position)] = bubble;
        }

        public bool HasBubbles()
        {
            for (int i = 0; i < bubbleSlots.Length; i++)
            {
                if (bubbleSlots[i] != null)
                    return true;
            }
            return false;
        }

        public float GetWidth()
        {
            return bubblePrefabs.Length * spacing;
        }

        public Vector3 GetBasePos()
        {
            return transform.position + Vector3.right * (offset - GetWidth() * 0.5f);
        }

        public Vector3 GetSlotPos(int i)
        {
            Vector3 basePos = GetBasePos();
            return basePos + i * spacing * Vector3.right;
        }

        public int GetClosestLocationIndex(Vector3 realPos)
        {
            Vector3 basePos = GetBasePos();
            Vector3 closestPos = basePos;
            int closestIndex = 0;

            for (int i = 0; i < bubbleSlots.Length; ++i)
            {
                Vector3 newPos = GetSlotPos(i);
                if ((newPos - realPos).sqrMagnitude < (closestPos - realPos).sqrMagnitude)
                {
                    // New closest!
                    closestPos = newPos;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        public Vector3 GetClosestLocation(Vector3 realPos)
        {
            Vector3 basePos = GetBasePos();
            Vector3 closestPos = basePos;

            for (int i = 0; i < bubbleSlots.Length; ++i)
            {
                Vector3 newPos = GetSlotPos(i);
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
            points[0] = new Vector2(-GetWidth() * 0.5f, 0);
            points[1] = new Vector2(GetWidth()*0.5f, 0);
            col.points = points;
            col.offset = Vector2.right * (-spacing * 0.5f + offset);
        }

        [Button]
        public void AlignBubbles()
        {
            for (int i = 0; i < bubbleSlots.Length; ++i)
            {
                if (bubbleSlots[i])
                {
                    bubbleSlots[i].transform.position = GetSlotPos(i);
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
