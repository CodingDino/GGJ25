using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{

    public class PlayerControl : MonoBehaviour
    {
        public int playerNum = 1;
        public string axisH = "Horizontal";
        public string buttonFire = "Fire";
        public string buttonSwap = "Swap";
        public float rotateSpeed = 1.0f;
        public Transform aimRoot = null;
        public Transform currentBubbleRoot;
        public Transform nextBubbleRoot;

        public float firingVelocity = 10f;

        public Bubble[] possibleBubbles;

        private Bubble currentBubble;
        private Bubble nextBubble;

        private float currentAngle = 0;

        private void Start()
        {
            nextBubble = InstantiateRandomBubble();
            currentBubble = InstantiateRandomBubble();
            currentBubble.transform.position = currentBubbleRoot.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // Swap ze bubble!
            if (Input.GetButtonDown(playerNum + "-" + buttonSwap))
            {
                SwapBubbles();
            }

            // Fire ze bubble!
            if (Input.GetButtonDown(playerNum + "-" + buttonFire))
            {
                // TODO
                Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();

                // Turn on physics for the bubble
                rb.isKinematic = false;
                Collider2D collider2D = rb.GetComponent<Collider2D>();
                collider2D.enabled = true;

                // Calculate vector in direction aimed
                Vector2 rotatedVector = aimRoot.rotation * Vector2.up;

                // Shoot the bubble!
                rb.velocity = rotatedVector * firingVelocity;

                // TODO: Cooldown to avoid rapid fire (or does that matter?)

                // Load next bubble
                currentBubble = nextBubble;
                currentBubble.transform.position = currentBubbleRoot.position;

                nextBubble = InstantiateRandomBubble();

            }


            // Aiming
            float axisVal = Input.GetAxis(playerNum + "-" + axisH);
            currentAngle += axisVal * Time.deltaTime * rotateSpeed;
            aimRoot.rotation = Quaternion.Euler(0, 0, currentAngle);
        }

        Bubble InstantiateRandomBubble()
        {
            Bubble newBubble = Instantiate(possibleBubbles[Random.Range(0, possibleBubbles.Length)], nextBubbleRoot.position, Quaternion.identity);
            Rigidbody2D rb = newBubble.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            Collider2D collider2D = newBubble.GetComponent<Collider2D>();
            collider2D.enabled = false;
            return newBubble;
        }

        void SwapBubbles()
        {
            Bubble wasCurrentBubble = currentBubble;
            currentBubble = nextBubble;
            currentBubble.transform.position = currentBubbleRoot.position;
            // TODO: tween

            nextBubble = wasCurrentBubble;
            nextBubble.transform.position = nextBubbleRoot.position;
            // TODO: tween
        }
    }

}
