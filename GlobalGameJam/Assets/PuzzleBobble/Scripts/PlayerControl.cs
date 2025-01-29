using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBobble
{

    public class PlayerControl : MonoBehaviour
    {
        public string axisH = "Horizontal";
        public string axisV = "Vertical";
        public string buttonFire = "Fire";
        public string buttonSwap = "Swap";
        public float rotateSpeed = 1.0f;
        public Transform aimRoot = null;
        public Transform currentBubbleRoot;
        public Transform nextBubbleRoot;
        public float maxAngle = 75f;

        public float firingVelocity = 10f;

        public Bubble[] possibleBubbles;

        private Bubble currentBubble;
        private Bubble nextBubble;

        private float currentAngle = 0;
        

        private int player = 1;

        public bool canFire = true;

        bool IsControllerConnected(int index)
        {
            string[] controllers = Input.GetJoystickNames();
            if (controllers.Length >= index && !string.IsNullOrEmpty(controllers[index]))
                return true; // A controller is connected
            return false; // No controller found
        }

        private void Start()
        {
            player = GetComponentInParent<Side>().player;
            nextBubble = InstantiateRandomBubble();
            currentBubble = InstantiateRandomBubble();
            currentBubble.transform.position = currentBubbleRoot.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // Swap ze bubble!
            if (Input.GetButtonDown(player + "-" + buttonSwap))
            {
                SwapBubbles();
            }

            // Fire ze bubble!
            if (Input.GetButtonDown(player + "-" + buttonFire) && canFire)
            {
                canFire = false;

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

            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                Debug.Log("Player 1 fired!");
            }

            if (Input.GetKeyDown(KeyCode.Joystick2Button0))
            {
                Debug.Log("Player 2 fired!");
            }

            // Aiming
            if (IsControllerConnected(player-1))
            {
                Vector2 aim = new Vector2(Input.GetAxis(player + "-" + axisH), Input.GetAxis(player + "-" + axisV));
                if (aim.sqrMagnitude > 0.1)
                {
                    aim = aim.normalized;

                    // Determine angle
                    currentAngle = Mathf.Rad2Deg * Mathf.Atan2(aim.y, aim.x);
                    currentAngle -= 90f;
                    currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
                    aimRoot.rotation = Quaternion.Euler(0, 0, currentAngle);
                }
            }
            else
            {
                float axisVal = Input.GetAxis(player + "-" + axisH);
                currentAngle += axisVal * Time.deltaTime * rotateSpeed;
                currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
                aimRoot.rotation = Quaternion.Euler(0, 0, currentAngle);
            }
        }

        Bubble InstantiateRandomBubble()
        {
            Bubble newBubble = Instantiate(possibleBubbles[Random.Range(0, possibleBubbles.Length)], nextBubbleRoot.position, Quaternion.identity);
            newBubble.control = this;
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
