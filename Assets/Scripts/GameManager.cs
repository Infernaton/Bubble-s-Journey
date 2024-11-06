using UnityEngine.InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Init Game")]
    [SerializeField] float m_NbStartBubble;
    [SerializeField] float m_SpawnBubbleTime;
    [SerializeField] Bubble m_BubblePrefab;
    [SerializeField] Vector3 m_SpawnPos1;
    [SerializeField] Vector3 m_SpawnPos2;

    [Header("During Game")]
    [SerializeField] GameObject AscendingObject;
    [SerializeField] GameObject m_Background;
    [SerializeField] Wind m_WindPrefab;

    [SerializeField] float m_TimeBeforeScroll;
    [SerializeField] float m_ScrollingSpeed;

    private Vector3 _startWind;
    private Vector3 _endWind;

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

    public static GameManager Instance = null;

    private void Awake()
    {
        if (Instance == null) // If there is no instance already
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        float time = m_SpawnBubbleTime / m_NbStartBubble;
        InvokeRepeating(nameof(SpawnBubble), 0, time);
        Invoke(nameof(GameStart), m_SpawnBubbleTime);
    }

    private void GameStart()
    {
        CancelInvoke();
        StartCoroutine(Utils.Anim.SlideOut(0.4f, UIManager.Instance.CinematicView));
    }

    void Update()
    {
        float startedTime = Time.time - m_SpawnBubbleTime;
        Physics.simulationMode = startedTime < 0 ? SimulationMode.Script : SimulationMode.FixedUpdate;
        if (startedTime < 0) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            _startWind = GetMousePosition();
        if (Mouse.current.leftButton.wasReleasedThisFrame){
            _endWind = GetMousePosition();
            //rotate the collider along the two point
            float angle = Mathf.Atan2(_endWind.y - _startWind.y, _endWind.x - _startWind.x) * 180 / Mathf.PI - 90;
            Wind w = Instantiate(m_WindPrefab, Vector3.Lerp(_startWind, _endWind, 0.5f), Quaternion.AngleAxis(angle, Vector3.forward));
            w.Init(_startWind, _endWind);
        }

        if (startedTime - m_TimeBeforeScroll < 0) return;
        AscendingObject.transform.position += Vector3.up * m_ScrollingSpeed * Time.deltaTime;
    }

    void SpawnBubble()
    {
        Vector3 pos = Vector3.Lerp(m_SpawnPos1, m_SpawnPos2, Random.value);
        Instantiate(m_BubblePrefab, pos, Quaternion.identity, transform);
        NbBubble++;
    }

    Vector3 GetMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.value;
        Vector3 startPos = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane);

        //Get ray from mouse postion
        Ray rayCast = Camera.main.ScreenPointToRay(startPos);

        //Raycast and check if any object is hit
        Physics.Raycast(rayCast, out RaycastHit hit, Camera.main.farClipPlane);
        Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        return new Vector3(hit.point.x, hit.point.y, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_SpawnPos1, m_SpawnPos2);
    }
}
