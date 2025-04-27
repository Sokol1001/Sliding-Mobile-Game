using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class SlideSpawner : MonoBehaviour
{
    [Header("Prefabs and gameobjects")]
    public GameObject obstaclesHolder;
    private GameObject slideSpawner;
    private GameObject backGroundSpawner;
    public GameObject obstacleSpawner;
    public GameObject gameCanvas;
    [SerializeField] GameObject startingSlide;

    public GameObject slidePrefab;
    public Transform spawnPoint;
    public ParticleSystem bubbles;
    public GameObject waterX;
    public GameObject bitchBallPref;

    public GameObject spaceShip;
    public GameObject fireRing;
    public GameObject iceRing;
    public GameObject spacePendulum;
    public GameObject axePendulum;
    public GameObject asteroidsArrows;
    public GameObject snowballArrows;
    public GameObject jungleArrows;
    public GameObject waterArrows;
    public ParticleSystem snow;
    [SerializeField] Material levelMat;
    [SerializeField] Material jungleMat;
    [SerializeField] Material beachMat;
    [SerializeField] Material waterMat;
    [SerializeField] Bot[] bots;
    [SerializeField] GameObject zigzags;

    [SerializeField] Sprite notMutedSound;
    [SerializeField] Sprite mutedSound;
    [SerializeField] GameObject soundButton;

    private Vector2 touchStartPos;

    public GameObject player;  // Reference to the player's transform

    [Header("Spawn points")]
    public Transform ObstaclePoint_pos;
    public Transform spawnPoints_a_transform;
    public Transform spawnPoints_b_transform;
    public Transform spawnPoint_a;
    public Transform spawnPoint_b;
    public Transform arrowSpawnPoint;
    [SerializeField] GameObject spawnPointRB_a;
    [SerializeField] GameObject spawnPointRB_b;
    public Rigidbody[] spawnPoint_Back;
    public GameObject bitchBallSP;
    public Transform zigzagSP;
    public GameObject[] treesSP;
    
    public float slideSpawnInterval = 2.0f;
    public float forwardSpeed = 50f;    // Adjust the forward speed as needed
    public float regularForwardSpeedSpaceShip = 25f;    // Adjust the forward speed as needed for space ship
    public float newForwardSpeedSpaceShip = 25f;
    private bool canSpawnZigzags = true;

    private List<int> excludedNumbers = new List<int>();
    private float timeSinceLastSpawn = 0.0f;
    public bool canSpawn = true;
    private int zigzagCount = -1;
    private int canSpawnObstacles = 0;
    private int callCounter = 1;

    int n = 0, k = 0;

    [Header("Background")]
    public List<GameObject> backGroundPrefabsSpace;
    public List<GameObject> backGroundPrefabsSnow;
    public List<GameObject> backGroundPrefabsJungle;
    public List<GameObject> backGroundPrefabsBitch;
    public List<GameObject> backGroundPrefabsWater;
    public bool isSpace;
    public bool isSnow;
    public bool isDefault;
    public bool isJungle;
    private int levelIndex;
    public GameObject spaceSkyBox;

    [Header("Material Color")]
    public Material pinkMaterial;
    public Material greenMaterial;
    public Material grayMaterial;
    public Material orangeMaterial;
    public Material blueMat;

    [Header("Sounds + Music")]
    // Reference to the AudioSource component
    private AudioSource audioSource;
    public AudioClip[] soundTracks;
    // Choose the appropriate soundtrack based on the level index
    AudioClip selectedSoundtrack = null;
    public AudioSource mainCumSource;
    public AudioClip starShip;
    private bool isMuted = false;

    private void Awake()
    {
        levelIndex = 0;
        forwardSpeed = 0;
        canSpawn = false;
        newForwardSpeedSpaceShip = 0;
        audioSource = GetComponent<AudioSource>();

        // Get the AudioSource component attached to this GameObject
        selectedSoundtrack = soundTracks[0];
        audioSource.clip = selectedSoundtrack;
        audioSource.Play();
    }
    private void Start()
    {
        if (PlayerPrefs.GetInt("CompletedTutorial", 0) == 1)
        {
            Time.timeScale = 1;
            if (gameCanvas.activeSelf)
            {
                StartGame();
            }
        }
        int muted = PlayerPrefs.GetInt("Muted", 0);
        isMuted = muted == 1;
        audioSource.volume = isMuted ? 0 : 1;

        bubbles.Stop();
        snow.Stop();
        slideSpawner = GameObject.FindGameObjectWithTag("SlideSpawner");
        backGroundSpawner = GameObject.FindGameObjectWithTag("BackGroundSpawner");
    }
    private void FixedUpdate()
    {
        //Debug.Log("Level index = "+ levelIndex);

        Vector3 forwardMovement = Vector3.forward * forwardSpeed;
        Vector3 forwardMovementSpaceShip = Vector3.forward * newForwardSpeedSpaceShip;

        //Move space ship
        spaceShip.transform.Translate(forwardMovementSpaceShip * Time.deltaTime, Space.World);

        spawnPointRB_a.transform.Translate(forwardMovement * Time.deltaTime, Space.World);
        spawnPointRB_b.transform.Translate(forwardMovement * Time.deltaTime, Space.World);

        Debug.Log("Can spawn = " + canSpawn);
        Debug.Log("Zigzag count = " + zigzagCount);

        //Debug.Log(levelIndex);
        // Check if it's time to spawn a new slide
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= slideSpawnInterval)
        {
            if (zigzagCount == 0)
            {
                canSpawn = true;
                SpawnSlide();
            }
            if (canSpawn)
            {
                Debug.Log("Spawning slides");
                SpawnSlide();
            }
            timeSinceLastSpawn = 0.0f;
        }
    }

    public void StopResumeGame(bool isStoppedGame)
    {
        if (isStoppedGame)
        {
            Time.timeScale = 0;
        }
        else if(!isStoppedGame)
        {
            Time.timeScale = 1;
        }
    }
    private int GetRandomNumber(int min, int max)
    {
        // Create a list of all possible numbers in the specified range
        List<int> possibleNumbers = new List<int>();

        // Increment the counter
        callCounter++;

        // Check if the counter has reached 5, then reset
        if (callCounter >= 6)
        {
            callCounter = 0;
            excludedNumbers.Clear(); // Reset the excluded numbers list
        }

        for (int i = min; i <= max; i++)
        {
            possibleNumbers.Add(i);
        }

        // Remove the previously excluded numbers from the list
        possibleNumbers.RemoveAll(excludedNumbers.Contains);

        // If all numbers are excluded, reset the list to retry all numbers
        if (possibleNumbers.Count == 0)
        {
            possibleNumbers.AddRange(excludedNumbers);
            excludedNumbers.Clear(); // Reset the excluded numbers list
        }

        // Randomly select a number from the remaining possibilities
        int randomNumber = possibleNumbers[Random.Range(0, possibleNumbers.Count)];

        // Add the selected number to the excluded list
        excludedNumbers.Add(randomNumber);

        return randomNumber;
    }
    
    public void SpaceShipExit()
    {
        newForwardSpeedSpaceShip = 80f;
        StartCoroutine(SpaceShip());
    }
    private void StartGame()
    {
        levelIndex = GetRandomNumber(2, 6);
        canSpawnObstacles = -1;
        canSpawnZigzags = true;

        snow.Stop();
        switch (levelIndex)
        {
            case 0:
                selectedSoundtrack = soundTracks[0];
                break;
            case 1:
                selectedSoundtrack = soundTracks[1];
                break;
            case 2:
                selectedSoundtrack = soundTracks[2];
                break;
            case 3:
                selectedSoundtrack = soundTracks[3];
                break;
            case 4:
                selectedSoundtrack = soundTracks[4];
                break;
            case 5:
                selectedSoundtrack = soundTracks[5];
                break;
            case 6:
                selectedSoundtrack = soundTracks[6];
                break;
            // Add more cases as needed

            default:
                Debug.LogError("Unknown level index");
                break;
        }

        // Change the audio clip and play the new soundtrack
        if (selectedSoundtrack != null)
        {
            audioSource.clip = selectedSoundtrack;
            audioSource.Play();
        }
        SpawnSlide();
    }
    private IEnumerator SpaceShip()
    {
        //audioSource.volume = 1;
        ResetSpawnPoints();
        levelIndex = GetRandomNumber(2, 6);
        canSpawnObstacles = 0;
        zigzagCount = 0;
        canSpawnZigzags = true;
        snow.Stop();
        bubbles.Stop();
        if (!isMuted)
        {
            switch (levelIndex)
            {
                case 0:
                    selectedSoundtrack = soundTracks[0];
                    break;
                case 1:
                    selectedSoundtrack = soundTracks[1];
                    break;
                case 2:
                    selectedSoundtrack = soundTracks[2];
                    break;
                case 3:
                    selectedSoundtrack = soundTracks[3];
                    break;
                case 4:
                    selectedSoundtrack = soundTracks[4];
                    break;
                case 5:
                    selectedSoundtrack = soundTracks[5];
                    break;
                case 6:
                    selectedSoundtrack = soundTracks[6];
                    break;
                // Add more cases as needed

                default:
                    Debug.LogError("Unknown level index");
                    break;
            }

            // Change the audio clip and play the new soundtrack
            if (selectedSoundtrack != null)
            {
                audioSource.clip = selectedSoundtrack;
                audioSource.Play();
            }
        }

        player.transform.position = spaceShip.transform.position;
        player.GetComponent<Player>().LaunchPlayerIntoAir();
        yield return new WaitForSeconds(3);
        newForwardSpeedSpaceShip = regularForwardSpeedSpaceShip;
    }

    public void MuteOrUnmute()
    {
        if (!isMuted)
        {
            isMuted = true;
            audioSource.volume = 0;
            PlayerPrefs.SetInt("Muted", 1); // Save the mute state
                                            //soundButton.GetComponent<RawImage>().texture = mutedSound.texture;
        }
        else
        {
            isMuted = false;
            audioSource.volume = 1;
            PlayerPrefs.SetInt("Muted", 0); // Save the mute state
                                            //soundButton.GetComponent<RawImage>().texture = notMutedSound.texture;
        }
        PlayerPrefs.Save(); // Don't forget to save the changes
    }

    public void GetBoolSpawnSlide(bool canSpawnThis)
    {
        canSpawn = canSpawnThis;
    }
    public void ResetSpawnPoints()
    {
        // Reset the spawn points to their initial positions
        spawnPoint_a.position = spawnPoints_a_transform.position;
        spawnPoint_b.position = spawnPoints_b_transform.position;
    }
    public void SpawnSlide()
    {
        selectedSoundtrack = null;
        
        // Instantiate a new slide at the spawn point
        var slid_a = Instantiate(slidePrefab, spawnPoint_a.position, transform.rotation * Quaternion.Euler(10f, 50.8f, 83.383f));

        //right slide
        //slid_a.transform.SetParent(slideSpawner.transform);
        for (int i = 0; i < 5; i++)
           slid_a.transform.GetChild(1).GetChild(i).gameObject.SetActive(true);

       //left slide
        var slid_b = Instantiate(slidePrefab, spawnPoint_b.position, transform.rotation * Quaternion.Euler(-13.258f, 65.783f, 94.636f));
       //slid_b.transform.SetParent(slideSpawner.transform);
       for (int i = 0; i < 5; i++)
           slid_b.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);

        n++;
        k++;
        slid_a.name = "Slide a " + n;
        slid_b.name = "Slide b " + k;

        for (int i = 0; i < bots.Length; i++)
        {
            bots[i].AddSlides(slid_a.transform);
            bots[i].AddSlides(slid_b.transform);
        }

            if (levelIndex == 1)
            {
                isSnow = false;
                isSpace = false;
                spaceSkyBox.SetActive(false);
                isDefault = true;
            }
            else if (levelIndex == 2)
            {
                isDefault = true;
                isSnow = false;
                isSpace = true;
                canSpawnObstacles++;
                ChangeMaterial(slid_a, pinkMaterial);
                ChangeMaterial(slid_b, pinkMaterial);

                if (canSpawnObstacles == 3)
                {
                    Instantiate(asteroidsArrows, arrowSpawnPoint.position, transform.rotation * Quaternion.Euler(0, -180f, 0));
                    canSpawnObstacles = 0;
                }

                if (canSpawnObstacles == 1)
                {
                    if (Random.Range(1, 3) == 1)
                    {
                        var spacePendulum_ = Instantiate(spacePendulum, ObstaclePoint_pos);
                        spacePendulum_.transform.SetParent(obstaclesHolder.transform);
                    }
                    else
                    {
                        var fireRing_ = Instantiate(fireRing, ObstaclePoint_pos);
                        fireRing_.transform.SetParent(obstaclesHolder.transform);
                    }

                }
                if (startingSlide != null)
                {
                ChangeMaterial(startingSlide, pinkMaterial);
                }

                spaceSkyBox.SetActive(true);
                for (int i = 0; i < spawnPoint_Back.Length - 1; i++)
                {
                    var backGround = Instantiate(backGroundPrefabsSpace[Random.Range(0, backGroundPrefabsSpace.Count)], spawnPoint_Back[Random.Range(0, spawnPoint_Back.Length - 1)].position, Quaternion.identity);
                    //backGround.transform.SetParent(backGroundSpawner.transform);
                    StartCoroutine(FadeInSlide(backGround));
                }

            }
            else if (levelIndex == 3)
            {
                isDefault = false;
                isSpace = false;
                spaceSkyBox.SetActive(false);
                snow.Play();
                canSpawnObstacles++;

                ChangeMaterial(slid_a, grayMaterial);
                ChangeMaterial(slid_b, grayMaterial);
            if (startingSlide != null)
            {
                ChangeMaterial(startingSlide, grayMaterial);
            }
            if (canSpawnObstacles == 3)
                {
                    if (Random.Range(1, 3) == 1)
                    {
                        slid_a.transform.GetChild(2).GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        slid_b.transform.GetChild(3).GetChild(2).gameObject.SetActive(true);
                    }
                    canSpawnObstacles = 0;
                }
                if (canSpawnObstacles == 2)
                {
                    Instantiate(snowballArrows, arrowSpawnPoint.position, transform.rotation * Quaternion.Euler(0, -180f, 0));
                }
                if (canSpawnObstacles == 1)
                {
                    var iceRingP = Instantiate(iceRing, ObstaclePoint_pos);
                    iceRingP.transform.SetParent(obstaclesHolder.transform);
                }
                isSnow = true;
                RenderSettings.skybox = levelMat;
                for (int i = 0; i < spawnPoint_Back.Length - 1; i++)
                {
                    var backGround = Instantiate(backGroundPrefabsSnow[Random.Range(1, backGroundPrefabsSnow.Count - 1)], spawnPoint_Back[i].position, transform.rotation * Quaternion.Euler(-90f, 150f, 0f));
                    var snowMan = Instantiate(backGroundPrefabsSnow[1], spawnPoint_Back[i + 1].position, Quaternion.identity);
                    var santa = Instantiate(backGroundPrefabsSnow[2], spawnPoint_Back[i].position, transform.rotation * Quaternion.Euler(-180f, 0f, -181.162f));
                    //backGround.transform.SetParent(backGroundSpawner.transform);
                    //snowMan.transform.SetParent(backGroundSpawner.transform);

                    Destroy(snowMan, 5);
                    Destroy(backGround, 5);
                    Destroy(santa, 5);
                }
                for (int i = 0; i < treesSP.Length; i++)
                {
                    var tree = Instantiate(backGroundPrefabsSnow[0], treesSP[i].transform.position, transform.rotation * Quaternion.Euler(-90f, 0, 0f));
                }

            }
            else if (levelIndex == 4)
            {
                isDefault = false;
                isSpace = false;
                isSnow = false;
                isJungle = true;
                spaceSkyBox.SetActive(false);
                canSpawnObstacles++;
            if (startingSlide != null)
            {
                ChangeMaterial(startingSlide, greenMaterial);
            }
            if (canSpawnObstacles == 2)
                {
                    if (Random.Range(1, 3) == 1)
                    {
                        slid_a.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        slid_b.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
                    }
                    canSpawnObstacles = 0;
                }
                ChangeMaterial(slid_a, greenMaterial);
                ChangeMaterial(slid_b, greenMaterial);
                   
                if (canSpawnObstacles == 1)
                {
                    var axe = Instantiate(axePendulum, ObstaclePoint_pos);
                    axe.transform.SetParent(obstaclesHolder.transform);

                    Instantiate(jungleArrows, arrowSpawnPoint.position, Quaternion.identity/*transform.rotation * Quaternion.Euler(0, 180f, 0)*/);
                }
                RenderSettings.skybox = jungleMat;
                for (int i = 0; i < spawnPoint_Back.Length - 1; i++)
                {
                    var backGround = Instantiate(backGroundPrefabsJungle[Random.Range(0, 0)], spawnPoint_Back[Random.Range(0, spawnPoint_Back.Length - 1)].position, transform.rotation * Quaternion.Euler(-90f, 150f, 0f));
                    var bird = Instantiate(backGroundPrefabsJungle[1], spawnPoint_Back[Random.Range(0, spawnPoint_Back.Length - 1)].position, transform.rotation * Quaternion.Euler(6f, -88.567f, 0f));

                    //backGround.transform.SetParent(backGroundSpawner.transform);
                    //bird.transform.SetParent(backGroundSpawner.transform);

                    Destroy(backGround, 5);
                    Destroy(bird, 5);
                }

            }
            if (levelIndex == 5)
            {
                spaceSkyBox.SetActive(false);
                RenderSettings.skybox = beachMat;
                ChangeMaterial(slid_a, orangeMaterial);
                ChangeMaterial(slid_b, orangeMaterial);
                canSpawnObstacles++;
                if (startingSlide != null)
                {
                    ChangeMaterial(startingSlide, orangeMaterial);
                }

                if (canSpawnObstacles == 2)
                {
                    var waterRing = Instantiate(waterX, ObstaclePoint_pos);
                    waterRing.transform.SetParent(obstaclesHolder.transform);

                    var bitchBall = Instantiate(bitchBallPref, bitchBallSP.transform);
                    //bitchBall.transform.SetParent(obstaclesHolder.transform);
                    canSpawnObstacles = 0;
                }
                if (canSpawnObstacles == 1)
                {
                    if (Random.Range(1, 3) == 1)
                    {
                        slid_a.transform.GetChild(2).GetChild(3).gameObject.SetActive(true);
                    }
                    else
                    {
                        slid_b.transform.GetChild(3).GetChild(3).gameObject.SetActive(true);
                    }
                }
                for (int i = 0; i < spawnPoint_Back.Length - 1; i++)
                {
                    var sandCastle = Instantiate(backGroundPrefabsBitch[0], spawnPoint_Back[i].position, transform.rotation * Quaternion.Euler(-186.3f, 0f, 180f));
                    //sandCastle.transform.SetParent(backGroundSpawner.transform);

                    Destroy(sandCastle, 5);
                }

            }
            if(levelIndex == 6)
            {
            zigzagCount++;
            spaceSkyBox.SetActive(false);
            RenderSettings.skybox = waterMat;
            ChangeMaterial(slid_a, blueMat);
            ChangeMaterial(slid_b, blueMat);
            canSpawnObstacles++;
            bubbles.Play();
            if(zigzagCount == 3 && canSpawnZigzags)
            {
                var zigzag = Instantiate(zigzags, zigzagSP.transform);
                zigzag.transform.parent = null;
                GameObject[] slides = GameObject.FindGameObjectsWithTag("Slide");
                foreach (var slide in slides)
                {
                    Destroy(slide);
                }

                zigzag.transform.Rotate(0, 90, 0);
                canSpawn = false;
                canSpawnZigzags = false;
                zigzagCount = 0;
            }
            if (startingSlide != null)
            {
                ChangeMaterial(startingSlide, blueMat);
            }
            for (int i = 0; i < spawnPoint_Back.Length - 1; i++)
            {
                var dolphin = Instantiate(backGroundPrefabsWater[0], spawnPoint_Back[i].position, Quaternion.identity);
                var fishClown = Instantiate(backGroundPrefabsWater[1], spawnPoint_Back[i].position, Quaternion.identity);
                var fishKoy = Instantiate(backGroundPrefabsWater[2], spawnPoint_Back[i].position, Quaternion.identity);
                Destroy(dolphin, 5);
                Destroy(fishKoy, 5);
                Destroy(fishClown, 5);
            }
            if (canSpawnObstacles == 3)
            {
                Instantiate(waterArrows, arrowSpawnPoint.position, transform.rotation * Quaternion.Euler(0, 180f, 0));
                canSpawnObstacles = 0;
            }
            if (canSpawnObstacles == 1)
            {
                if (Random.Range(1, 3) == 1)
                {
                    slid_a.transform.GetChild(2).GetChild(4).gameObject.SetActive(true);
                }
                else
                {
                    slid_b.transform.GetChild(3).GetChild(4).gameObject.SetActive(true);
                }
            }
        }
            StartCoroutine(FadeInSlide(slid_a));
            StartCoroutine(FadeInSlide(slid_b));

        //for(int k = 0; k < bots.Length; k++)
        //{
        //    bots[k].RemoveleftSlides(slid_a.transform);
        //    bots[k].RemoveRightSlides(slid_b.transform);
        //}

    }
    public void getSceneManagmentData(bool canSpa, int spaceShipSpeed, int levelInd, int forwardSpeedSP)
    {
        selectedSoundtrack = soundTracks[levelInd];
        levelIndex = levelInd;
        canSpawn = canSpa;
        forwardSpeed = forwardSpeedSP;
        newForwardSpeedSpaceShip = spaceShipSpeed;
        regularForwardSpeedSpaceShip = spaceShipSpeed;
        audioSource.clip = selectedSoundtrack;
        audioSource.Play();
    }
    private void ChangeMaterial(GameObject gameObject ,Material mat)
    {
        // Get the Renderer component attached to this GameObject
        Renderer renderer = gameObject.GetComponent<Renderer>();

        // Check if the Renderer component exists
        if (renderer != null)
        {
            // Assign the pink material to the renderer
            renderer.material = mat;
        }
        else
        {
            Debug.LogError("Renderer component not found on this GameObject!");
        }
    }
    private IEnumerator FadeInSlide(GameObject slide)
    {
        Renderer renderer = slide.GetComponent<Renderer>();
        Material material = renderer.material;

        // Get the initial alpha value
        Color startColor = material.color;
        float startAlpha = startColor.a;

        // Set the initial alpha value to 0 (fully transparent)
        startColor.a = 0;
        material.color = startColor;

        float elapsedTime = 0f;

        while (elapsedTime < slideSpawnInterval)
        {
            elapsedTime += Time.deltaTime;

            // Gradually increase the alpha value
            Color newColor = material.color;
            newColor.a = Mathf.Lerp(0, startAlpha, elapsedTime / slideSpawnInterval);
            material.color = newColor;

            yield return null;
        }

        // Ensure a precise final alpha value
        startColor.a = startAlpha;
        material.color = startColor;
    }
}