using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using Utils;

public enum GameState
{
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

    [Header("During Game")]
    [SerializeField] Vector3 m_AscendingObjectOffset;
    [SerializeField] GameObject AscendingObject;
    [SerializeField] GameObject m_Background;
    [SerializeField] Wind m_WindPrefab;
    [SerializeField] float m_BorderTPOffset;

    [SerializeField] float m_ScrollingSpeed;

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
                UIManager.Instance.UpdateHighScoreCounter(Mathf.Round(value)); 
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
        State = GameState.PlayAnimation;
        float time = m_SpawnBubbleTime;
        Invoke(nameof(SpawnBubble), time/2);
        StartCoroutine(GameStart());
    }

    void Update()
    {
        Physics.simulationMode = State != GameState.CanInterract ? SimulationMode.Script : SimulationMode.FixedUpdate;
        if (State != GameState.CanInterract) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            _startWind = MousePositionOnScreen;
        if (Mouse.current.leftButton.wasReleasedThisFrame){
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

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        AscendingObject.transform.position = Vector3.Lerp(
            AscendingObject.transform.position,
            m_AscendingObjectOffset + Vector3.up * AverageBubblePosition().y,
            Time.smoothDeltaTime * m_ScrollingSpeed);
    }

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

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(m_SpawnBubbleTime);
        yield return Anim.SlideOut(0.4f, UIManager.Instance.CinematicView);
        StartCoroutine(UIManager.Instance.ClickAnimation());
        State = GameState.CanInterract;
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        State = GameState.EndGame;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_SpawnPos1, m_SpawnPos2);
    }
}
