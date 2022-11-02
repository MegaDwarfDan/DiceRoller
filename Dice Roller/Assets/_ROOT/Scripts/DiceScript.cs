using System.ComponentModel.Design;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    [Tooltip("Enter the value that appears on the die when facing in the X,Y,Z axes.")]
    public Vector3Int diceSides;
    [Tooltip("Make sure to keep Y and Z values positive so the die is thrown into the pit.")]
    public Vector3 minForce;
    public Vector3 maxForce;

    int diceValue = 0;
    Rigidbody rgbd;
    bool isMoving = false;
    Vector3 prevVelocity = Vector3.zero;
    Vector3 startPos;

    // Awake sets the RigidBody variable, and saves the original position of the die so we can reset it after a throw.
    private void Awake()
    {
        rgbd = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    // If the die isn't currently moving, check if the player presses the mouse button down.
    // If they do, Roll the Dice.
    private void Update()
    {
        if (!isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RollDice();
            }
        }
    }

    // Check if the die is actively moving. If it is, check if it has stopped, and make sure it wasn't stopped the previous frame.
    // This is to make sure the dice isn't immediately checked after being thrown.
    // Then check to see if the die value check returns a valid result. If it does, stop the die, and update the UI.
    // If it doesn't, it means the die isn't sitting flush on the terrain. So give it a little push to try to settle it down.
    // Then save the velocity of the die so we can use it again later.
    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (rgbd.velocity == Vector3.zero && prevVelocity != Vector3.zero)
            {
                diceValue = DetermineSideUp();
                if (diceValue == 0)
                {
                    rgbd.AddForce(new Vector3(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f)), ForceMode.Impulse);
                }
                else
                {
                    isMoving = false;
                    rgbd.isKinematic = true;
                    UIManager.Instance.DisplayDieResult(diceValue);
                }
            }
            prevVelocity = rgbd.velocity;
        }
    }

    // This checks to see which side of the die is face up.
    // It is only called if the die has come to rest after being thrown.
    int DetermineSideUp()
    {
        float upThreshold = 0.99f;

        float dotFwd = Vector3.Dot(transform.forward, Vector3.up);
        if (dotFwd >= upThreshold) return diceSides.z;
        if (dotFwd <= -(upThreshold)) return 7 - diceSides.z;

        float dotRight = Vector3.Dot(transform.right, Vector3.up);
        if (dotRight >= upThreshold) return diceSides.x;
        if (dotRight <= -(upThreshold)) return 7 - diceSides.x;

        float dotUp = Vector3.Dot(transform.up, Vector3.up);
        if (dotUp >= upThreshold) return diceSides.y;
        if (dotUp <= -(upThreshold)) return 7 - diceSides.y;

        return 0;
    }

    // This function rolls the die. It sets the die's position and rotation to their starting spots, in case the die has already been thrown.
    // Then it adds force and torque to the die to help generate a random and realistic feeling of being thrown.
    void RollDice()
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        rgbd.isKinematic = false;
        rgbd.AddForce(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.y, maxForce.y), Random.Range(minForce.z, maxForce.z), ForceMode.Impulse);
        rgbd.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        isMoving = true;
    }
}