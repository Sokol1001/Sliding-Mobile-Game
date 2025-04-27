using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Cinemachine;


[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("References")]

    //Animators
    [SerializeField] private Animator anim;
    [SerializeField] private Animator animChef;
    [SerializeField] private Animator animAngel;
    [SerializeField] private Animator animBitch;
    [SerializeField] private Animator animScientist;
    [SerializeField] private Animator animSoldierBoy;
    [SerializeField] private Animator animCoolBoy;
    [SerializeField] private Material spaceMat;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private TextMeshProUGUI textCoins;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private WarpSpeed ws;
    [SerializeField] private GameObject spaceShip;
    [SerializeField] private Slider slider;
    [SerializeField] private CinemachineVirtualCamera camera1;
    [SerializeField] private CinemachineVirtualCamera camera2;
    [SerializeField] private Camera mainMenuCamera;
    [SerializeField] private SlideSpawner slideSpawner;
    [SerializeField] private GameObject space;
    [SerializeField] private NumberCounter numCounter;
    [SerializeField] private ParticleSystem killedEffect;
    public GameObject tutorialPanel; // Assign in inspector
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject[] bots;
    [SerializeField] private GameObject playerSpawnSS;

    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float slideSpeed = 10.0f;
    [SerializeField] private float smoothFactor = 10.0f;
    [SerializeField] private float maxTiltAngle = 30.0f;
    [SerializeField] private float transitionDuration = 1.0f;
    [SerializeField] private float screenSpeed = 2f;
    //[SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float slideAngleThreshold = 30f;
    [SerializeField] private float maxVerticalVelocity = 10f;
    [SerializeField] private float gravityScale = 2f;

    private int coinCountOneTry = 0;
    private int coinCount;
    private Rigidbody rb;
    private bool isDead = false;
    private bool canTouchSpaceShip = true;
    private Vector2 swipeStartPos;
    private Touch touch;
    private Vector3 slideDirection;
    private Vector3 touchStartPos;
    private bool isSliding = false;
    private float horizontalInput;

    public GameObject swipeRightSign;
    public GameObject swipeLeftSign;
    public ParticleSystem snow;
    public ParticleSystem[] trails;
    private bool canMovePlayer = true;
    public TextMeshProUGUI TextC;


    private bool hasSwipedRight = false;
    private bool hasSwipedLeft = false;

    public AudioSource slideSpawnAudioSource;
    public AudioSource mainCumSource;
    public AudioClip moneyAudio;
    public AudioClip deathAudio;
    public AudioClip starShip;
    [SerializeField] GameObject skins;

    [SerializeField] private RewardedAdsButton RAB;
    [SerializeField] InterstitialAdsButton IABHome;
    [SerializeField] InterstitialAdsButton IABRestart;

    private void Awake()
    {
        GameObject skin = GameObject.FindGameObjectWithTag("Skin");
        RAB.SetSkin(skin);
        coinCountOneTry = 0;
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        //PlayerPrefs.DeleteKey("CompletedTutorial");

        if (PlayerPrefs.GetInt("CompletedTutorial", 0) == 1)
        {
            Time.timeScale = 1;
            tutorialPanel.SetActive(false);
        }
        else
        {
            StartCoroutine(ShowTutorial());
        }

        camera1.Priority = 2;
        camera2.Priority = 0;

        PlayerDataManager.LoadPlayerData();
        PlayerDataManager.SavePlayerData();
        coinCount = PlayerDataManager.CoinCount;
        UpdateCoinCountText(); // Update the UI with the loaded coin count
    }
    private IEnumerator ShowTutorial()
    {
        if (gameCanvas.activeSelf)
        {
            Debug.Log("gameCanvas active");
            yield return new WaitForSeconds(0.5f);
            tutorialPanel.SetActive(true);
            while (!hasSwipedRight)
            {
                swipeRightSign.SetActive(true);
                Time.timeScale = 0.2f;
                // Check for swipe right
                if (SwipeDetected() && SwipeDirection() > 0)
                {
                    hasSwipedRight = true;
                    tutorialPanel.SetActive(false);
                    swipeRightSign.SetActive(false);
                    Time.timeScale = 1f;
                }
                yield return null;
            }

            yield return new WaitForSeconds(2f);
            tutorialPanel.SetActive(true);
            while (!hasSwipedLeft)
            {
                Time.timeScale = 0.2f;
                swipeLeftSign.SetActive(true);
                // Check for swipe left
                if (SwipeDetected() && SwipeDirection() < 0)
                {
                    hasSwipedLeft = true;
                    tutorialPanel.SetActive(false);
                    swipeLeftSign.SetActive(false);
                    Time.timeScale = 1f;
                }
                yield return null;
            }

            tutorialPanel.SetActive(false);
            PlayerPrefs.SetInt("CompletedTutorial", 1);
        }
    }
    private bool SwipeDetected()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    return true;
            }
        }
        return false;
    }

    private float SwipeDirection()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                return touch.position.x - touchStartPos.x;
            }
        }
        return 0;
    }
    private void Update()
    {
        GameObject skin = GameObject.FindGameObjectWithTag("Skin");
        if (skin != null)
        {
            isDead = false;
        }

        if (!isDead)
          HandleTouchInput();

        if (transform.position.y < -13f && !isDead)
        {
            ActivateDeathScreen();
        }
        if (IsCollidingWithSlide())
        {
            anim.SetBool("IsJumping", false);
            animChef.SetBool("IsJumping", false);
            animAngel.SetBool("IsJumping", false);
            animBitch.SetBool("IsJumping", false);
            animScientist.SetBool("IsJumping", false);
            animSoldierBoy.SetBool("IsJumping", false);
            animCoolBoy.SetBool("IsJumping", false);
        }
        else
        {
            anim.SetBool("IsJumping", true);
            animChef.SetBool("IsJumping", true);
            animAngel.SetBool("IsJumping", true);
            animBitch.SetBool("IsJumping", true);
            animScientist.SetBool("IsJumping", true);
            animSoldierBoy.SetBool("IsJumping", true);
            animCoolBoy.SetBool("IsJumping", true);
        }

        if (loseCanvas.activeSelf)
        {
            gameCanvas.SetActive(false);
        }
        else if(!loseCanvas.activeSelf && canMovePlayer)
        {
            gameCanvas.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if(canMovePlayer)
        MovePlayer();
    }
    
    public void getCanMovePlayer(bool canMovePlayer)
    {
        this.canMovePlayer = canMovePlayer;
        rb.isKinematic = !canMovePlayer;

        if (canMovePlayer)
        {
            gameCanvas.SetActive(true);
            TurnOffMainMenuCamera();
        }
    }
    private void MovePlayer()
    {
        float currentSpeed = isSliding ? slideSpeed : moveSpeed;
        Vector3 targetVelocity = new Vector3(horizontalInput * currentSpeed, rb.velocity.y, transform.forward.z * currentSpeed);
        targetVelocity.y = Mathf.Clamp(targetVelocity.y, -maxVerticalVelocity, maxVerticalVelocity);

        // rb.velocity += targetVelocity;  // Keep this line as it is

        // Apply custom gravity
        rb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);

        // Lerp the velocity towards the target velocity using Time.deltaTime
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * smoothFactor);
    }


    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    Vector2 touchDelta = new Vector2(touch.position.x - touchStartPos.x, touch.position.y - touchStartPos.y);
                    horizontalInput = (touchDelta.x / Screen.width) * screenSpeed;
                    RotatePlayer(horizontalInput);

                    if (!isSliding)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f))
                        {
                            if (hit.collider.CompareTag("Slide") && Vector3.Angle(hit.normal, Vector3.up) < slideAngleThreshold)
                            {

                                isSliding = true;
                            }
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    isSliding = false;
                    horizontalInput = 0;
                    break;
            }
        }
    }
    private bool IsCollidingWithSlide()
    {
        // Get the Collider component of the sphere
        Collider sphereCollider = GetComponent<Collider>();

        // Check for collisions with slide objects
        Collider[] hitColliders = Physics.OverlapSphere(sphereCollider.bounds.center, sphereCollider.bounds.extents.magnitude);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Slide"))
            {
                return true;
            }
        }

        return false;
    }


    private void RotatePlayer(float direction)
    {
        float targetTiltAngle = direction * maxTiltAngle;
        Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, targetTiltAngle);
        playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation, targetRotation, Time.deltaTime * smoothFactor);
    }

    private void ActivateDeathScreen()
    {
        mainCumSource.PlayOneShot(deathAudio);
        space.SetActive(false);
        RAB.LoadAd();
        IABHome.LoadAd();
        IABRestart.LoadAd();
        loseCanvas.SetActive(true);
        TextC.text = "" + coinCountOneTry / 2  + " <sprite name=\"coin_0\">";
        DestroyBackGround();
        for (int i = 0; i < trails.Length; i++)
        {
            trails[i].Stop();
        }
        killedEffect.Play();
        GameObject skin = GameObject.FindGameObjectWithTag("Skin");
        RAB.SetSkin(skin);
        skins.SetActive(false);
        isDead = true;
    }

    private void DestroyBackGround()
    {
        GameObject[] backGround = GameObject.FindGameObjectsWithTag("BackGround");
        GameObject[] slides = GameObject.FindGameObjectsWithTag("Slide");
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        slideSpawner.GetBoolSpawnSlide(false);

        for (int i = 0; i < bots.Length; i++)
        {
            bots[i].GetComponent<Bot>().RemoveSlides();
            bots[i].SetActive(false);
        }

        foreach (var bg in backGround)
        {
            Destroy(bg);
        }

        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        foreach (var slide in slides)
        {
            Destroy(slide);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("FireRing"))
        {
            ActivateDeathScreen();
        }
        else if (other.CompareTag("Coin"))
        {
            mainCumSource.PlayOneShot(moneyAudio);
            coinCount++;
            coinCountOneTry++;
            UpdateCoinCountText();
            other.GetComponentInChildren<ParticleSystem>().Play();
            Destroy(other.gameObject, 0.2f);

            // Save the updated coin count using the PlayerDataManager
            PlayerDataManager.CoinCount = coinCount;
            PlayerDataManager.SavePlayerData();
        }
        else if (other.CompareTag("SpaceShip") && canTouchSpaceShip)
        {
            StartSpaceShip();
        }
        else if (other.CompareTag("Slide"))
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }
    }

    private void StartSpaceShip()
    {
        slideSpawnAudioSource.volume = 0;
        mainCumSource.PlayOneShot(starShip);
        snow.Stop();
        RenderSettings.skybox = spaceMat;
        ws.WarpSpeedVFX(true);
        SwitchToCamera2();
        DestroyBackGround();
        canTouchSpaceShip = false;
        gameObject.SetActive(false);
    }

    private void SwitchToCamera1()
    {
        StartCoroutine(TransitionToCamera(camera1));
    }

    private void SwitchToCamera2()
    {
        StartCoroutine(TransitionToCamera(camera2));
    }
    private void TurnOffMainMenuCamera()
    {
        mainMenuCamera.enabled = false;
    }

    private IEnumerator TransitionToCamera(CinemachineVirtualCamera targetCamera)
    {
        float elapsedTime = 0f;

        int initialPriority = targetCamera.Priority;
        int otherPriority = (initialPriority == camera1.Priority) ? camera2.Priority : camera1.Priority;

        targetCamera.Priority = 20;
        CinemachineVirtualCamera otherCamera = (targetCamera == camera1) ? camera2 : camera1;
        otherCamera.Priority = otherPriority;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            targetCamera.Priority = Mathf.RoundToInt(Mathf.Lerp(20, initialPriority, elapsedTime / transitionDuration));
            otherCamera.Priority = Mathf.RoundToInt(Mathf.Lerp(otherPriority, 0, elapsedTime / transitionDuration));

            yield return null;
        }

        targetCamera.Priority = initialPriority;
        otherCamera.Priority = 0;
    }

    private void UpdateCoinCountText()
    {
        if (textCoins != null)
        {
            textCoins.text = "Coins: " + coinCount;
        }
    }
    private void JumpPlayerInAir()
    {
        // Set the desired jump force and direction
        Vector3 jumpForce = new Vector3(0, 15000, 0); // Adjust the force as needed
        Vector3 jumpForceForBot = new Vector3(0, 500, 0); // Adjust the force as needed

        for (int i = 0; i < bots.Length; i++)
        {
            bots[i].SetActive(true);
            bots[i].transform.position = spaceShip.transform.position;
            bots[i].GetComponent<Rigidbody>().AddForce(jumpForceForBot, ForceMode.Impulse);
        }
        rb.AddForce(jumpForce, ForceMode.Impulse);
    }

    public void LaunchPlayerIntoAir()
    {
        StartCoroutine(LaunchPlayerIntoAirCaru());
    }

    public IEnumerator LaunchPlayerIntoAirCaru()
    {
        // Call the JumpPlayerInAir function
        yield return new WaitForSeconds(0.5f);
        JumpPlayerInAir();

        SwitchToCamera1();

        // Wait for a specific duration or condition
        yield return new WaitForSeconds(5f);

        canTouchSpaceShip = true;
    }
}

