using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float fearLevel = 0;
    public float delayForSteeringAtStart = 2;
    public List<Transform> tires;
    public GameObject canvas;
    public GameObject getReadyText;
    public Slider fearSlider;
    public float stableWheelTimer = 0;
    [HideInInspector]
    public bool enableCollision = true;
    public List<AudioClip> screemSoundClips;
    private Rigidbody playerRigidbody;
    private bool isCenterDirectionSelected = false;
    private int direction = 0;
    private int isLastPressedKeyRight = 0; // 1 right, -1 left
    private float keyCoolDownTimer = 0;
    private bool enableMovement = true;
    private bool leftButtonHold = false;
    private bool rightButtonHold = false;
    private bool startSteering = false;
    private AudioSource audioSource;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        enableCollision = false;
        IEnumerator coroutine = EnableSteering(delayForSteeringAtStart);
        StartCoroutine(coroutine);
    }

    // Update is called once per frame
    void Update()
    {
        if(canvas)
        {
            canvas.transform.LookAt(Camera.main.transform);
            fearSlider.value = fearLevel;
        }
        if(enableMovement == true)
        {
            if(playerRigidbody)
            {
                playerRigidbody.velocity = transform.forward*forwardSpeed;
            }
            
            if(startSteering == true)
            {
                SteerWheel();
            }
        }

        if(playerRigidbody.velocity.magnitude > 0)
        {
            for (int i = 0; i < tires.Count; i++)
            {
                tires[i].RotateAround(tires[i].position, transform.right, Time.deltaTime * tireRotateSpeed);
            }
        }
    }

    IEnumerator EnableSteering(float secs)
    {
        if(getReadyText != null)
        {
            getReadyText.SetActive(true);
        }
        yield return new WaitForSeconds(secs);
        enableCollision = true;
        startSteering = true;
        if(getReadyText != null)
        {
            getReadyText.SetActive(false);
        }
    }
    public void IncreaseFear(float val)
    {
        fearLevel += val;
        if(fearLevel >= 1)
        {
            enableMovement = false;
            StopAfterSecs(2, false);
            GameManager.Instance.GameOverMenu.SetActive(true);
        }
    }
    public void DecreaseFear(float val)
    {
        fearLevel -= val;
    }
    public void StopMovement()
    {
        enableMovement = false;
        GameManager.Instance.transform.GetChild(0).GetComponent<AudioSource>().Stop();
        playerRigidbody.velocity = Vector3.zero;
    }
    public void PlayScreamSound()
    {
        audioSource.clip = screemSoundClips[Random.Range(0, screemSoundClips.Count)];
        audioSource.Play();
    }
    void SteerWheel()
    {
        keyCoolDownTimer -= Time.deltaTime;
        keyCoolDownTimer = Mathf.Max(0, keyCoolDownTimer);
        stableWheelTimer -= Time.deltaTime;
        stableWheelTimer = Mathf.Max(0, stableWheelTimer);

        float randomSpeed = Random.Range(randomYankSpeedRange/2, randomYankSpeedRange);
        float offsetFromCenter = transform.position.x;

        if(Input.GetKey(KeyCode.RightArrow) || rightButtonHold == true || Input.acceleration.x > 0.02f)
        {
            if(keyCoolDownTimer <= 0 || (isLastPressedKeyRight == 1 || isLastPressedKeyRight == 0 ))
            {
                isLastPressedKeyRight = 1;
                keyCoolDownTimer = keyCoolDownTime;
                isCenterDirectionSelected = false;
                transform.RotateAround(transform.position, transform.up, Time.deltaTime * verticleSpeed);
            }
        }
        else if(Input.GetKey(KeyCode.LeftArrow) || leftButtonHold == true || Input.acceleration.x < -0.02f)
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
    }

    public Rigidbody GetRigidBody()
    {
        return playerRigidbody;
    }
    public void StartStableWheel()
    {
        stableWheelTimer = stableWheelCoolDownTime;
    }

    public void LeftButtonOnClick()
    {
        leftButtonHold = true;
    }
    public void LeftButtonOnRelease()
    {
        leftButtonHold = false;
    }

    public void RightButtonOnClick()
    {
        rightButtonHold = true;
    }
    public void RightButtonOnRelease()
    {
        rightButtonHold = false;
    }

    public void StopAfterSecs(float seconds, bool isWin)
    {
        IEnumerator coroutine = HaltAfterSeconds(seconds, isWin);
        StartCoroutine(coroutine);
    }

    IEnumerator HaltAfterSeconds(float seconds, bool isWin)
    {
        float startVelocityX = 0;
        float startVelocityY = 0;
        float startVelocityZ = 0;
        yield return new WaitForSeconds(seconds/4);

        startVelocityX = playerRigidbody.velocity.x;
        startVelocityY = playerRigidbody.velocity.y;
        startVelocityZ = playerRigidbody.velocity.z;

        float decreasedVelocityX = ((startVelocityX)*(3))/4;
        float decreasedVelocityY = ((startVelocityY)*(3))/4;
        float decreasedVelocityZ = ((startVelocityZ)*(3))/4;

        playerRigidbody.velocity = new Vector3(decreasedVelocityX, decreasedVelocityY, decreasedVelocityZ);

        yield return new WaitForSeconds(seconds/4);
        startVelocityX = playerRigidbody.velocity.x;
        startVelocityY = playerRigidbody.velocity.y;
        startVelocityZ = playerRigidbody.velocity.z;
        decreasedVelocityX = ((startVelocityX)*(2))/4;
        decreasedVelocityY = ((startVelocityY)*(2))/4;
        decreasedVelocityZ = ((startVelocityZ)*(2))/4;

        playerRigidbody.velocity = new Vector3(decreasedVelocityX, decreasedVelocityY, decreasedVelocityZ);

        yield return new WaitForSeconds(seconds/4);
        startVelocityX = playerRigidbody.velocity.x;
        startVelocityY = playerRigidbody.velocity.y;
        startVelocityZ = playerRigidbody.velocity.z;
        decreasedVelocityX = ((startVelocityX)*(1))/4;
        decreasedVelocityY = ((startVelocityY)*(1))/4;
        decreasedVelocityZ = ((startVelocityZ)*(1))/4;

        playerRigidbody.velocity = new Vector3(decreasedVelocityX, decreasedVelocityY, decreasedVelocityZ);

        yield return new WaitForSeconds(seconds/4);
        GameManager.Instance.transform.GetChild(0).GetComponent<AudioSource>().Stop();
        playerRigidbody.velocity = Vector3.zero;

        if(isWin == false && GameManager.Instance.isFinished == false)
        {
            GameManager.Instance.OpenGameOverMenu();
        }
        else
        {
            GameManager.Instance.OpenLevelFinishedMenu();
        }

    }
}
