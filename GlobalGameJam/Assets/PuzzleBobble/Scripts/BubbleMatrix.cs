using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace PuzzleBobble
{
    public class BubbleMatrix : MonoBehaviour
    {
        public float startSpeed = 1f;
        public float speedIncreaseDur = 10f;
        public float speedIncreaseAmmount = 0.5f;
        public float spacing = 1f;
        public int startingRows = 8;

        private List<BubbleRow> rows = new();

        public BubbleRow rowPrefab = null;
        public Bubble[] possibleBubbles;

        private float speed;
        private float lastSpeedChange = 0f;

        private BubbleRow SpawnNewRow(bool populate = false)
        {
            BubbleRow row = Instantiate(rowPrefab,transform);

            // Check what offset this should be
            if (rows.Count > 0 && rows[rows.Count - 1].offset == 0f)
            {
                row.offset = 0.5f;
            }

            if (populate)
            {
                PopulateRow(row);
            }

            Vector3 pos = transform.position + spacing * Vector3.up;
            if (rows.Count > 0)
            {
                pos = rows[rows.Count-1].transform.position + spacing * Vector3.up;
            }
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
            speed = startSpeed;
            lastSpeedChange = Time.time;

            for(int i = 0;i < startingRows; i++)
            {
                SpawnNewRow (i == startingRows - 1);
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
                // Check loss!

                // Return back up
                BubbleRow row = rows[0];
                rows.RemoveAt(0);
                Destroy(row.gameObject);
                SpawnNewRow(true);
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
