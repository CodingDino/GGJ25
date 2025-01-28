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

        public bool IsTopRow()
        {
            return GetComponentInParent<BubbleMatrix>().IsTopRow(this);
        }

        public void AddBubble(Bubble bubble)
        {
            bubble.canBeStuckTo = true;
            bubbleSlots[GetClosestLocationIndex(bubble.transform.position)] = bubble;
        }

        public void RemoveBubble(Bubble bubble)
        {
            for (int i = 0; i < bubbleSlots.Length; ++i)
            {
                if (bubbleSlots[i] == bubble)
                    bubbleSlots[i] = null;
            }
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
            return (bubblePrefabs.Length-1) * spacing;
        }

        public Vector3 GetBasePos()
        {
            return transform.position - Vector3.right * GetWidth() * 0.5f;
        }

        public Vector3 GetSlotPos(int i)
        {
            return GetBasePos() + i * spacing * Vector3.right;
        }

        public int GetClosestLocationIndex(Vector3 realPos)
        {
            Vector3 closestPos = GetBasePos();
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
            Vector3 closestPos = GetBasePos();

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
            points[1] = new Vector2( GetWidth() * 0.5f + spacing, 0);
            col.points = points;
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
            SetUpEdgeCollider();
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
                    bubbleSlots[i].canBeStuckTo = true;
                    bubbleSlots[i].GetComponent<Rigidbody2D>().isKinematic = true;
                }
            }
            AlignBubbles();
        }

    }
}
