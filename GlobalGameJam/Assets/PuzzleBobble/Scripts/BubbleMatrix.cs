using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class BubbleMatrix : MonoBehaviour
    {
        private int player = 1;
        public float startSpeed = 1f;
        public float speedIncreaseDur = 10f;
        public float speedIncreaseAmmount = 0.5f;
        public float spacing = 1f;
        public int startingRows = 8;
        public int filledRowIndex = 15;
        public float offset = 0.25f;

        private List<BubbleRow> rows = new();

        public BubbleRow rowPrefab = null;
        public Bubble[] possibleBubbles;

        private float speed;
        private float lastSpeedChange = 0f;

        private bool lastRowLeft = false;

        private float GetRightOffset()
        {
            return offset;// * 0.5f;
        }
        private float GetLeftOffset()
        {
            return 0;// GetRightOffset() * - 1f;
        }

        private BubbleRow SpawnNewRow(bool populate = false)
        {
            BubbleRow row = Instantiate(rowPrefab,transform);

            // Check what offset this should be
            float offset = GetLeftOffset();
            if (lastRowLeft)
            {
                offset = GetRightOffset();
            }
            lastRowLeft = !lastRowLeft;

            if (populate)
            {
                PopulateRow(row);
            }
            else
            {
                // this will set up the row with 0 bubbles in
                row.SpawnBubbles();
            }

            Vector3 pos = transform.position + spacing * Vector3.up;
            if (rows.Count > 0)
            {
                pos = rows[rows.Count-1].transform.position + spacing * Vector3.up;
            }
            pos.x = transform.position.x+offset;
            row.transform.position = pos;

            rows.Add(row);

            return row;
        }

        void PopulateRow(BubbleRow row)
        {
            // set up random bubbles
            for (int i = 0; i < row.bubblePrefabs.Length; i++)
            {
                row.bubblePrefabs[i] = possibleBubbles[Random.Range(0, possibleBubbles.Length)];
            }

            row.SpawnBubbles();
        }


        private void Start()
        {

            player = GetComponentInParent<Side>().player;
            speed = startSpeed;
            lastSpeedChange = Time.time;

            for(int i = 0;i < startingRows; i++)
            {
                SpawnNewRow (i >= filledRowIndex);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Move
            for(int i = 0; i < rows.Count; i++)
            {
                rows[i].transform.Translate(Vector3.down * speed * Time.deltaTime);

            }

            // row has reached bottom, time to move up
            if (rows[0].transform.position.y < transform.position.y)
            {
                BubbleRow row = rows[0];

                // Check loss!
                if (row.HasBubbles())
                {
                    Debug.LogError("Player "+player+" loses!");
                }
                else
                {
                    // Return back up
                    rows.RemoveAt(0);
                    Destroy(row.gameObject);
                    SpawnNewRow(true);
                }

            }

            // Increase speed
            if (Time.time > lastSpeedChange + speedIncreaseDur)
            {
                lastSpeedChange = Time.time;
                speed += speedIncreaseAmmount;
            }
        }
    }
}
