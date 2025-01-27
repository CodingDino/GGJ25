using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{
    public class BubbleMatrix : MonoBehaviour
    {
        public float startSpeed = 1f;
        public float speedIncreaseDur = 10f;
        public float speedIncreaseAmmount = 0.5f;
        public float spacing = 1f;
        public int startingGrids = 0;

        private List<BubbleGrid> grids = new();

        public BubbleGrid gridPrefab = null;
        public Bubble[] possibleBubbles;

        private float speed;
        private float lastSpeedChange = 0f;

        private BubbleGrid SpawnNewGrid(bool populate = false)
        {
            BubbleGrid grid = Instantiate(gridPrefab,transform);

            if (populate)
            {
                // set up random bubbles
                for (int i = 0; i < grid.bubblePrefabs.Length; i++)
                {
                    grid.bubblePrefabs[i] = possibleBubbles[Random.Range(0, possibleBubbles.Length)];
                }

                grid.SpawnBubbles();
            }

            Vector3 pos = transform.position;
            if (grids != null && grids.Count > 0)
            {
                pos = grids[grids.Count-1].transform.position + spacing * Vector3.up;
            }
            grid.transform.position = pos;

            grids.Add(grid);

            return grid;
        }


        private void Start()
        {
            speed = startSpeed;
            lastSpeedChange = Time.time;

            for(int i = 0;i < startingGrids; i++)
            {
                SpawnNewGrid (i == startingGrids-1);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Move
            for(int i = 0; i < grids.Count; i++)
            {
                grids[i].transform.Translate(Vector3.down * speed * Time.deltaTime);
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
