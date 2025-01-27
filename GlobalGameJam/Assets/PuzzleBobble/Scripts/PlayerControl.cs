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
        public float rotateSpeed = 1.0f;
        public Transform aimRoot = null;
        public GameObject bubblePrefab = null;
        public float firingVelocity = 10f;


        private float currentAngle = 0;


        // Update is called once per frame
        void Update()
        {
            // Fire ze bubble!
            if (Input.GetButtonDown(playerNum + "-" + buttonFire))
            {
                // TODO
                GameObject bubble = Instantiate(bubblePrefab, aimRoot.position, Quaternion.identity);

                Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

                // Calculate vector in direction aimed
                Vector2 rotatedVector = aimRoot.rotation * Vector2.up;

                // Shoot the bubble!
                rb.velocity = rotatedVector * firingVelocity;

                // TODO: Cooldown to avoid rapid fire (or does that matter?)
                // TODO: Load next bubble
            }

            // Aiming
            float axisVal = Input.GetAxis(playerNum + "-" + axisH);
            currentAngle += axisVal * Time.deltaTime * rotateSpeed;
            aimRoot.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

}
