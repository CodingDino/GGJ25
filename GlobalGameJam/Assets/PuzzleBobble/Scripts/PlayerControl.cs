using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

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

        public Transform firingPoint = null;
        

        private int player = 1;

        public bool canFire = true;

        public Animator canonAnim = null;

        public AudioSource canonSFX = null;

        public VisualEffect canonVFX = null;

        public AudioSource turningSFX = null;

        public SpriteRenderer[] pips = null;

        public float shotCooldown;
        private float lastShot = 0;

        bool IsControllerConnected(int index)
        {
            string[] controllers = Input.GetJoystickNames();
            if (controllers.Length > index && !string.IsNullOrEmpty(controllers[index]))
                return true; // A controller is connected
            return false; // No controller found
        }

        private void Start()
        {
            player = GetComponentInParent<Side>().player;
            nextBubble = InstantiateRandomBubble();
            currentBubble = InstantiateRandomBubble();
            currentBubble.transform.position = currentBubbleRoot.transform.position;
            // Update pip colours
            foreach (var pip in pips)
            {
                LeanTween.color(pip.gameObject, currentBubble.color, 0.5f)
                    .setEase(LeanTweenType.easeInOutSine);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.timeScale == 0)
            {
                turningSFX.Pause();
                return;
            }

            // Swap ze bubble!
            if (Input.GetButtonDown(player + "-" + buttonSwap))
            {
                SwapBubbles();
            }

            // Fire ze bubble!
            if (Input.GetButtonDown(player + "-" + buttonFire) && canFire && Time.time >= lastShot+shotCooldown)
            {
                canonAnim.SetTrigger("Fire");

                canonSFX.pitch = Random.Range(0.9f, 1.1f);
                canonSFX.Play();

                if (canonVFX)
                    canonVFX.Play();

                canFire = false;

                lastShot = Time.time;

                // TODO
                Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();

                // Turn on physics for the bubble
                rb.isKinematic = false;
                Collider2D collider2D = rb.GetComponent<Collider2D>();
                collider2D.enabled = true;

                // Calculate vector in direction aimed
                Vector2 rotatedVector = aimRoot.rotation * Vector2.up;

                // Set the bubble location
                currentBubble.transform.position = firingPoint.transform.position;

                // Shoot the bubble!
                rb.velocity = rotatedVector * firingVelocity;

                // Tween the size

                LeanTween.scale(currentBubble.gameObject, currentBubble.ogScale * Vector3.one, 0.1f)
                .setEase(LeanTweenType.easeInBack);

                // Load next bubble
                currentBubble = nextBubble;
                currentBubble.transform.position = currentBubbleRoot.position;

                // Update pip colours
                foreach (var pip in pips)
                {
                    LeanTween.color(pip.gameObject, currentBubble.color, 0.5f)
                        .setEase(LeanTweenType.easeInOutSine);
                }

                // new next bubble
                nextBubble = InstantiateRandomBubble();

            }

            // Aiming
            if (IsControllerConnected(player-1))
            {
                Vector2 aim = new Vector2(Input.GetAxis(player + "-" + axisH), Input.GetAxis(player + "-" + axisV));
                if (aim.sqrMagnitude > 0.1f)
                {
                    aim = aim.normalized;

                    // Determine angle
                    currentAngle = Mathf.Rad2Deg * Mathf.Atan2(aim.y, aim.x);
                    currentAngle -= 90f;
                    currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
                    aimRoot.rotation = Quaternion.Euler(0, 0, currentAngle);

                    if (!turningSFX.isPlaying)
                        turningSFX.Play();
                }
                else
                    turningSFX.Pause();
            }
            else
            {

                float axisVal = Input.GetAxis(player + "-" + axisH);
                currentAngle += axisVal * Time.deltaTime * rotateSpeed;
                currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
                aimRoot.rotation = Quaternion.Euler(0, 0, currentAngle);

                if (Mathf.Abs(axisVal) >= 0.1f)
                {
                    if (!turningSFX.isPlaying)
                        turningSFX.Play();
                }
                else
                    turningSFX.Pause();

            }
        }

        Bubble InstantiateRandomBubble()
        {
            Bubble newBubble = Instantiate(possibleBubbles[Random.Range(0, possibleBubbles.Length)], nextBubbleRoot.position, Quaternion.identity);
            newBubble.control = this;
            newBubble.SetOnShip();
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
