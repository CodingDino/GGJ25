using PuzzleBobble;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[System.Serializable]
public class MonsterArrangement
{
    public Bubble[] row1;
    public Bubble[] row2offset;
    public Bubble[] row3;
    public Bubble[] row4offset;
}

namespace PuzzleBobble
{
    public class BubbleMatrix : MonoBehaviour
    {
        private int player = 1;
        public float startSpeed = 1f;
        public float speedPerAlien = 0.5f;
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

        public WinPanel win;

        public MonsterArrangement[] monsters;

        public int minMonsterSpawn = 5;
        public int maxMonsterSpawn = 10;

        public AlienMeter opponentMeter;

        public bool IsTopRow(BubbleRow row)
        {
            return row == rows[rows.Count - 1];
        }

        private float GetRightOffset()
        {
            return offset * 0.5f;
        }
        private float GetLeftOffset()
        {
            return  GetRightOffset() * - 1f;
        }

        private BubbleRow SpawnNewRow(bool populate = false)
        {
            BubbleRow row = Instantiate(rowPrefab,transform);
            row.matrix = this;

            // Check what offset this should be
            float offset = GetLeftOffset();
            row.offset = false;
            if (lastRowLeft)
            {
                offset = GetRightOffset();
                row.offset = true;
            }
            lastRowLeft = !lastRowLeft;

            if (populate)
            {
                PopulateRow(row, rows.Count);
            }
            else
            {
                // this will set up the row with 0 bubbles in
                row.SpawnBubbles(rows.Count);
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

        void PopulateRow(BubbleRow row, int rowIndex)
        {
            // set up random bubbles
            for (int i = 0; i < row.bubblePrefabs.Length; i++)
            {
                row.bubblePrefabs[i] = possibleBubbles[Random.Range(0, possibleBubbles.Length)];
            }

            row.SpawnBubbles(rowIndex);
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
            // Get numberof aliens
            int numAliens = 0;
            foreach(var row in rows)
            {
                foreach (var bubble in row.bubbleSlots)
                {
                    if (bubble && bubble.monster)
                        ++numAliens;
                }
            }

            // Move
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].transform.Translate(Vector3.down * (speed+ speedPerAlien*numAliens) * Time.deltaTime);

            }

            // lose condition
            // (bottom row has stuff in)
            if (rows[0].HasBubbles())
            {
                Time.timeScale = 0;
                win.ShowPanel(player == 1 ? 2 : 1);
            }

            // row has reached bottom, time to move up
            if (rows[0].transform.position.y < transform.position.y)
            {
                BubbleRow row = rows[0];

                // Return back up
                rows.RemoveAt(0);
                Destroy(row.gameObject);
                SpawnNewRow(true);

            }
        }


        public void AddPoints(float points)
        {
            opponentMeter.AddToMeter(points);
        }

        int GetClosestRowIndex(Bubble _bubble)
        {
            int index = 0;
            float closestSquareDist = 1000;

            for (int i = 0; i < rows.Count; ++i)
            {
                float sqareDist = (rows[i].transform.position - _bubble.transform.position).sqrMagnitude;
                if (sqareDist < closestSquareDist)
                {
                    index = i;
                    closestSquareDist = sqareDist;
                }
            }

            return index;
        }

        public void AddToListIfExists(int row, int col, ref List<Bubble> connected)
        {
            if (row >= 0 && row < rows.Count)
            {
                if(col >= 0 && col < rows[row].bubbleSlots.Length)
                {
                    Bubble toCheck = rows[row].bubbleSlots[col];
                    if (toCheck)
                        connected.Add(toCheck);
                }
            }
        }

        public void AddToListIfValid(int row, int col, ref List<Vector2Int> neighbors)
        {
            if (row >= 0 && row < rows.Count)
            {
                if (col >= 0 && col < rows[row].bubbleSlots.Length)
                {
                    neighbors.Add(new Vector2Int(row, col));
                }
            }
        }

        public List<Bubble> GetConnectedBubbles(int row, int col)
        {
            List<Bubble> connected = new();

            // offset left or right?
            int offIndex = rows[row].offset ? +1 : -1;

            // Check row above
            AddToListIfExists(row-1, col, ref connected);
            AddToListIfExists(row-1, col + offIndex, ref connected);

            // Check this row
            AddToListIfExists(row, col - 1, ref connected);
            AddToListIfExists(row, col + 1, ref connected);

            // Check row below
            AddToListIfExists(row + 1, col, ref connected);
            AddToListIfExists(row + 1, col + offIndex, ref connected);

            return connected;
        }

        public List<Vector2Int> GetNeighborIndices(Vector2Int rowCol)
        {
            List<Vector2Int> neighbors = new();

            // offset left or right?
            int offIndex = rows[rowCol.x].offset ? -1 : +1;

            // Check row above
            AddToListIfValid(rowCol.x - 1, rowCol.y + 0, ref neighbors);
            AddToListIfValid(rowCol.x - 1, rowCol.y + offIndex, ref neighbors);

            // Check this row
            AddToListIfValid(rowCol.x, rowCol.y - 1, ref neighbors);
            AddToListIfValid(rowCol.x, rowCol.y + 1, ref neighbors);

            // Check row below
            AddToListIfValid(rowCol.x + 1, rowCol.y + 0, ref neighbors);
            AddToListIfValid(rowCol.x + 1, rowCol.y + offIndex, ref neighbors);

            return neighbors;
        }

        public int GetRowIndex(BubbleRow row)
        {
            for (int i = 0; i < rows.Count; ++i)
            {
                if (row == rows[i])
                    return i;
            }
            return -1;
        }


        public IEnumerator MakeOrphanBubblesFall(List<Bubble> toCheck)
        {
            yield return new WaitForSeconds(0.5f);

            List<Bubble> markForRemoval = new();
            foreach (var checkFall in toCheck)
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
                if (!toRemove.monster)
                    AddPoints(1);
                // TODO: May need it's own function later
                toRemove.ClearBubble();
            }

            yield break;
        }

        public void SnapBubble(Bubble _bubble)
        {
            // Find the right location for this bubble

            // Closest row
            int closestRowIndex = GetClosestRowIndex(_bubble);

            int minRowToCheck = closestRowIndex > 0 ? closestRowIndex - 1 : closestRowIndex;
            int maxRowToCheck = closestRowIndex < rows.Count - 1 ? closestRowIndex + 1 : closestRowIndex;

            // Get closes free position in rows
            int chosenCol = -1;
            int chosenRow = -1;
            Vector3 chosenPos = Vector3.zero;
            float shortestDist = 0;
            bool foundPos = false;
            for (int i = minRowToCheck; i <= maxRowToCheck; ++i)
            {
                Vector3 potentialClosestPos = Vector3.zero;
                int potentialCol = -1;
                bool rowPosExists = rows[i].GetClosestValidSnapLocation(_bubble.transform.position, i, out potentialClosestPos, out potentialCol);
                if (rowPosExists)
                {
                    Debug.Log("Actual position = " + _bubble.transform.position);
                    Debug.Log("Snap position = " + potentialClosestPos);
                    float distToPotentialPos = (potentialClosestPos - _bubble.transform.position).sqrMagnitude;
                    if (!foundPos || distToPotentialPos < shortestDist)
                    {
                        chosenCol = potentialCol;
                        chosenRow = i;
                        foundPos = true;
                        shortestDist = distToPotentialPos;
                        chosenPos = potentialClosestPos;
                    }
                }
            }

            if (!foundPos)
            {
                Debug.LogError("Couldn't find valid position for bubble snap");
            }

            // Make the bubble a child of the grid
            _bubble.transform.parent = rows[chosenRow].transform;

            // turn off rigidbody motion
            // Find correct place in grid and go to it
            float overshootTime = 0.005f;
            float arcTime = 0.05f;
            Rigidbody2D rb = _bubble.GetComponent<Rigidbody2D>();
            Vector3 overshootPos = _bubble.transform.position+new Vector3(rb.velocity.x * overshootTime, rb.velocity.y * overshootTime, 0);

            // First move: linear move for both x and y
            LeanTween.move(_bubble.gameObject, overshootPos, overshootTime)
                .setEase(LeanTweenType.linear)
                .setOnComplete(() =>
                {
                    // Tween the x position with linear easing after the first move is complete
                    LeanTween.value(_bubble.gameObject, overshootPos.x, chosenPos.x, arcTime)
                        .setOnUpdate((float val) =>
                        {
                            Vector3 newPos = _bubble.transform.position;
                            newPos.x = val; // Update only the x position
                            _bubble.transform.position = newPos;
                        })
                        .setEase(LeanTweenType.linear);

                    // Tween the y position with ease-in-out quadratic easing after the first move is complete
                    LeanTween.value(_bubble.gameObject, overshootPos.y, chosenPos.y, arcTime)
                        .setOnUpdate((float val) =>
                        {
                            Vector3 newPos = _bubble.transform.position;
                            newPos.y = val; // Update only the y position
                            _bubble.transform.position = newPos;
                        })
                        .setEase(LeanTweenType.easeInOutQuad)
                        .setOnComplete(() =>
                        {
                            // HERE, trigger further effects
                            _bubble.transform.position = chosenPos;
                        });
                });


            // Tell bubble grid that this bubble is now in place
            rows[chosenRow].AddBubble(_bubble, chosenCol);

            // TODO: squash/stretch bubble effect for bubble setting (maybe also squash/stretch the bubble(s) we hit

        }

        [Button]
        public void SpawnMonster()
        {
            int randMonIndex = Random.Range(0, monsters.Length);

            MonsterArrangement chosenMonster = monsters[randMonIndex];

            // Determine valid place for monster
            bool valid = false;
            int rowI = -1;
            int colI = -1;
            BubbleRow row = null;
            while (!valid)
            {
                rowI = Random.Range(minMonsterSpawn, maxMonsterSpawn + 1);
                row = rows[rowI];
                colI = Random.Range(0, row.bubbleSlots.Length);
                valid = CheckMonsterLocation(chosenMonster, rowI, colI);
            }

            // Place the monster
            // Row 1
            row = rows[rowI];
            for(int i = 0; i < chosenMonster.row1.Length; ++i)
            {
                if (chosenMonster.row1[i])
                {
                    SpawnMonsterPart(row, chosenMonster.row1[i], colI + i);
                }
            }
            // Row 2
            row = rows[rowI-1];
            for (int i = 0; i < chosenMonster.row2offset.Length; ++i)
            {
                if (chosenMonster.row2offset[i])
                {
                    SpawnMonsterPart(row, chosenMonster.row2offset[i], colI + i);
                }
            }
            // Row 3
            row = rows[rowI-2];
            for (int i = 0; i < chosenMonster.row3.Length; ++i)
            {
                if (chosenMonster.row3[i])
                {
                    SpawnMonsterPart(row, chosenMonster.row3[i], colI + i);
                }
            }
            // Row 4
            row = rows[rowI - 3];
            for (int i = 0; i < chosenMonster.row4offset.Length; ++i)
            {
                if (chosenMonster.row4offset[i])
                {
                    SpawnMonsterPart(row, chosenMonster.row4offset[i], colI + i);
                }
            }
        }

        public void SpawnMonsterPart(BubbleRow _row, Bubble _monsterPart, int _col)
        {
            // Remove any existing bubble
            if (_row.bubbleSlots[_col])
                Destroy(_row.bubbleSlots[_col].gameObject);

            // Add and position new monster bubble
            _row.AddBubble(Instantiate(_monsterPart, _row.transform), _col);
            _row.PositionBubble(_row.bubbleSlots[_col], _col);

            // TODO: Animate?
        }

        public bool CheckMonsterLocation(MonsterArrangement _monster, int rowI, int colI)
        {
            BubbleRow row = rows[rowI];

            // row1 must be NOT offset
            if (row.offset)
                return false;

            // Get the first index of the monster with something in it
            int firstRowIndex = 0;
            for (int i = 0; i < _monster.row1.Length; ++i)
            {
                if (_monster.row1[i])
                {
                    firstRowIndex = i;
                    break;
                }
            }

            // First item is out of bounds
            if (colI + firstRowIndex >= row.bubbleSlots.Length)
                return false;

            // Check that each part of the monster is within bounds

            // Check row1 col1 location contains a bubble and is connected to the top
            List<Bubble> topSearch = new();
            if (row.bubbleSlots[colI+ firstRowIndex] && row.bubbleSlots[colI+ firstRowIndex].HasPathToTopRow(ref topSearch))
            {
                // Check that each part of the monster is within bounds
                for (int i = 0; i < _monster.row1.Length; ++i)
                {
                    if (_monster.row1[i])
                    {
                        if (colI+i >= row.bubbleSlots.Length)
                        {
                            return false;
                        }
                    }
                }
                row = rows[rowI-1];
                for (int i = 0; i < _monster.row2offset.Length; ++i)
                {
                    if (_monster.row2offset[i])
                    {
                        if (colI + i >= row.bubbleSlots.Length)
                        {
                            return false;
                        }
                    }
                }
                row = rows[rowI-2];
                for (int i = 0; i < _monster.row3.Length; ++i)
                {
                    if (_monster.row3[i])
                    {
                        if (colI + i >= row.bubbleSlots.Length)
                        {
                            return false;
                        }
                    }
                }
                row = rows[rowI-3];
                for (int i = 0; i < _monster.row4offset.Length; ++i)
                {
                    if (_monster.row4offset[i])
                    {
                        if (colI + i >= row.bubbleSlots.Length)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
