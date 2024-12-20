using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using Utils;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    PlayAnimation,
    CanInterract,
    EndGame
}

public enum Particle
{
    DestroyedBubble,
}

public class GameManager : MonoBehaviour
{
    public GameState State;

    [Header("Init Game")]
    [SerializeField] float m_SpawnBubbleTime;
    [SerializeField] Bubble m_BubblePrefab;
    [SerializeField] Vector3 m_SpawnPos1;
    [SerializeField] Vector3 m_SpawnPos2;
    [SerializeField] GameObject m_FinishArea;

    [Header("During Game")]
    [SerializeField] ScrollingObject[] AscendingObjects;
    [SerializeField] Wind m_WindPrefab;
    [SerializeField] float m_BorderTPOffset;

    [Header("ParticlesPool")]
    [SerializeField] ParticleSystem m_DestroyedBubble;

    private float _nbBubble;
    public float NbBubble
    {
        get => _nbBubble;
        set 
        {
            _nbBubble = value;
            UIManager.Instance.UpdateBubbleCounter(_nbBubble);
        }
    }

    private float _highScore = 0f;
    public float HighScore
    {
        get => _highScore;
        set
        {
            if (value > _highScore)
            {
                _highScore = value;
                float percentage = (_highScore / m_FinishArea.transform.position.y) * 100;
                UIManager.Instance.UpdateHighScoreCounter(Mathf.Round(percentage)); 
            }
        }
    }

    private Vector2 _startWind;
    public Vector2 MousePositionOnScreen => Mouse.current.position.value;
    public float GetBorderTPOffset() => m_BorderTPOffset;

    public static GameManager Instance = null;

    public void SpawnBubble()
    {
        Vector3 pos = Vector3.Lerp(m_SpawnPos1, m_SpawnPos2, Random.value);
        Instantiate(m_BubblePrefab, pos, Quaternion.identity, transform);
        NbBubble++;
    }

    public ParticleSystem GetParticles(Particle name)
    {
        switch (name)
        {
            case Particle.DestroyedBubble:
                return m_DestroyedBubble;
            default:
                return null;
        }
    }

    private void Awake()
    {
        if (Instance == null) // If there is no instance already
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        State = GameState.Menu;
        UIManager.Instance.DisplayTitleScreen(true);
    }

    void Update()
    {
        Physics.simulationMode = State != GameState.CanInterract ? SimulationMode.Script : SimulationMode.FixedUpdate;
        switch (State)
        {
            case GameState.Menu:
                if (Input.GetMouseButtonDown(0))
                {
                    State = GameState.PlayAnimation;
                    UIManager.Instance.DisplayTitleScreen(false);
                    StartGame();
                }
                break;
            case GameState.CanInterract:
                UpdateGame();
                break;
            case GameState.EndGame:
                if (Input.GetMouseButtonDown(0))
                {
                    ReloadScene();
                }
                break;
            default:
                break;
        }
    }

    #region Handle State game
    private void StartGame()
    {
        float time = m_SpawnBubbleTime;
        Invoke(nameof(SpawnBubble), time / 2);
        StartCoroutine(GameStartAnimation());
    }

    private IEnumerator GameStartAnimation()
    {
        yield return new WaitForSeconds(m_SpawnBubbleTime);
        yield return Anim.SlideOut(0.4f, UIManager.Instance.CinematicView);
        StartCoroutine(UIManager.Instance.ClickAnimation());
        State = GameState.CanInterract;
    }

    private void UpdateGame()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            _startWind = MousePositionOnScreen;
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Delayed the position In Game of the mouse position at pressed event
            // Because their a constant camera movement: the initial position when pressed is not really respected when we release the button, and feal a little uncanny
            Vector3 startPosIG = GetMousePositionIG(_startWind);
            Vector3 endPosIG = GetMousePositionIG(MousePositionOnScreen);
            //rotate the collider along the two point
            float angle = Mathf.Atan2(endPosIG.y - startPosIG.y, endPosIG.x - startPosIG.x) * 180 / Mathf.PI - 90;
            Wind w = Instantiate(m_WindPrefab, Vector3.Lerp(startPosIG, endPosIG, 0.5f), Quaternion.AngleAxis(angle, Vector3.forward));
            w.Init(startPosIG, endPosIG);
        }

        UIManager.Instance.UpdateTimeCounter(Time.time);

        UpdateAscendingObjectsPosition();
    }

    private void UpdateAscendingObjectsPosition()
    {
        Vector3 dest = AverageBubblePosition();
        foreach (var item in AscendingObjects)
        {
            item.UpdatePosition(dest);
        }
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        State = GameState.PlayAnimation;
        StartCoroutine(GameOverAnimation());
    }

    private IEnumerator GameOverAnimation()
    {
        yield return Anim.SlideIn(0.3f, UIManager.Instance.CinematicView);
        UIManager.Instance.DisplayVictoryScreen(true);
        State = GameState.EndGame;
    }
    #endregion

    Vector3 GetMousePositionIG(Vector2 mousePositionOnScreen)
    {
        Vector3 startPos = new (mousePositionOnScreen.x, mousePositionOnScreen.y, Camera.main.nearClipPlane);

        //Get ray from mouse postion
        Ray rayCast = Camera.main.ScreenPointToRay(startPos);

        //Raycast and check if any object is hit
        Physics.Raycast(rayCast, out RaycastHit hit, Camera.main.farClipPlane);
        Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        return new Vector3(hit.point.x, hit.point.y, 0);
    }

    private Vector3 AverageBubblePosition()
    {
        //Because bubble are stored as child of the gamemanager
        Vector3 average = Vector3.zero;
        foreach (Transform child in transform)
        {
            average += child.position;
        }
        return average / transform.childCount;
    }

    //Wind Effect are still stored in the bubble's wind list if he diseapear before the bubble exit by itself
    public void RemoveGlobalWindEffect(Wind wind)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent<Bubble>(out var bubble))
                bubble.RemoveWindEffect(wind);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_SpawnPos1, m_SpawnPos2);
    }
}
