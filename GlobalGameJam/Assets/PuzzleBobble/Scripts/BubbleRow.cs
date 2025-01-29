using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace PuzzleBobble
{
    public class BubbleRow : MonoBehaviour
    {
        public Bubble[] bubblePrefabs;
        [HideInInspector]
        public Bubble[] bubbleSlots;
        public float spacing = 1.0f;
        public bool offset = false;

        public BubbleMatrix matrix = null;

        static int bubbleNum = 0;

        public bool IsTopRow()
        {
            return GetComponentInParent<BubbleMatrix>().IsTopRow(this);
        }

        public void AddBubble(Bubble bubble, int row, int col)
        {
            bubble.name = "Bubble " + bubbleNum + " (" + bubble.bubbleType + ")";
            bubble.canBeStuckTo = true;
            bubbleSlots[col] = bubble;
            bubble.rowCol.x = row;
            bubble.rowCol.y = col;
            bubble.parentRow = this;
            bubble.canBeStuckTo = true;
            bubble.GetComponent<Rigidbody2D>().isKinematic = true;
            bubble.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            ++bubbleNum;
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



        public bool GetClosestValidSnapLocation(Vector3 realPos, int row, out Vector3 closestPos, out int index)
        {
            index = -1;
            closestPos = Vector3.zero;
            bool validPos = false;

            for (int i = 0; i < bubbleSlots.Length; ++i)
            {
                Vector3 newPos = GetSlotPos(i);
                // Only allow slots that have 
                bool openSpace = bubbleSlots[i] == null;
                // Only allow slots that are connected to another bubble
                bool connected = matrix.GetConnectedBubbles(row, i).Count > 0;

                if (openSpace && connected)
                {
                    if (!validPos || (newPos - realPos).sqrMagnitude < (closestPos - realPos).sqrMagnitude)
                    {
                        // New closest!
                        index = i;
                        closestPos = newPos;
                        validPos = true;
                    }
                }
            }

            return validPos;
        }

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

        public void SpawnBubbles(int row)
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
                    AddBubble(Instantiate(bubblePrefabs[i], transform), row, i);
                }
            }
            AlignBubbles();
        }

    }
}
