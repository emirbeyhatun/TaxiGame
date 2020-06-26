using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10;
    public float verticleSpeed = 10;
    public float tireRotateSpeed = 300;
    [Range(0, 100)]
    public float randomYankSpeedRange;//min value is not 0, it is half of max
    public float centerOffset = 2f;
    public float keyCoolDownTime = 0.1f;
    public float stableWheelCoolDownTime = 0.3f;
    public List<Transform> tires;
    Rigidbody playerRigidbody;
    bool isCenterDirectionSelected = false;
    int direction = 0;
    int isLastPressedKeyRight = 0; // 1 right, -1 left
    float keyCoolDownTimer = 0;
    public float stableWheelTimer = 0;
    bool enableMovement = true;
    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enableMovement == true)
        {
            if(playerRigidbody)
            {
                playerRigidbody.velocity = transform.forward*forwardSpeed;
            }
            
            SteerWheel();
        }
    }

    public void StopMovement()
    {
        enableMovement = false;
        playerRigidbody.velocity = Vector3.zero;
    }
    void SteerWheel()
    {
        keyCoolDownTimer -= Time.deltaTime;
        keyCoolDownTimer = Mathf.Max(0, keyCoolDownTimer);
        stableWheelTimer -= Time.deltaTime;
        stableWheelTimer = Mathf.Max(0, stableWheelTimer);

        float randomSpeed = Random.Range(randomYankSpeedRange/2, randomYankSpeedRange);
        float offsetFromCenter = transform.position.x;

        if(Input.GetKey(KeyCode.RightArrow))
        {
            if(keyCoolDownTimer <= 0 || (isLastPressedKeyRight == 1 || isLastPressedKeyRight == 0 ))
            {
                isLastPressedKeyRight = 1;
                keyCoolDownTimer = keyCoolDownTime;
                isCenterDirectionSelected = false;
                transform.RotateAround(transform.position, transform.up, Time.deltaTime * verticleSpeed);
            }
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            if(keyCoolDownTimer <= 0 || (isLastPressedKeyRight == -1 || isLastPressedKeyRight == 0 ))
            {
                isLastPressedKeyRight = -1;
                keyCoolDownTimer = keyCoolDownTime;
                isCenterDirectionSelected = false;
                transform.RotateAround(transform.position, transform.up, Time.deltaTime * -verticleSpeed);
            }
        }
        else if(stableWheelTimer <= 0)
        {
            if(Mathf.Abs(offsetFromCenter) <= centerOffset)
            {
                if(isCenterDirectionSelected == false)
                {
                    float randomDirection = Random.value;
                    if(randomDirection > 0.5f)
                    {
                        direction = 1;
                    }
                    else
                    {
                        direction = -1;
                    }
                    isCenterDirectionSelected = true;
                }

                transform.RotateAround(transform.position, transform.up, Time.deltaTime * direction*randomSpeed);

            }
            else
            {
                isCenterDirectionSelected = false;
                //not abs
                if(offsetFromCenter > centerOffset)
                {
                    direction = 1;
                    transform.RotateAround(transform.position, transform.up, Time.deltaTime * direction * randomSpeed);
                }
                else if(offsetFromCenter < -centerOffset)
                {
                    direction = -1;
                    transform.RotateAround(transform.position, transform.up, Time.deltaTime * direction * randomSpeed);
                }
            }
        }

        for (int i = 0; i < tires.Count; i++)
        {
            tires[i].RotateAround(tires[i].position, transform.right, Time.deltaTime * tireRotateSpeed);
        }
    }

    public Rigidbody GetRigidBody()
    {
        return playerRigidbody;
    }
    public void StartStableWheel()
    {
        stableWheelTimer = stableWheelCoolDownTime;
    }
}
