using UnityEngine.InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Init Game")]
    [SerializeField] float m_NbStartBubble;
    [SerializeField] float m_SpawnTime;
    [SerializeField] Bubble m_BubblePrefab;
    [SerializeField] Vector3 m_SpawnPos1;
    [SerializeField] Vector3 m_SpawnPos2;

    [Header("During Game")]
    [SerializeField] Camera m_Camera;
    [SerializeField] GameObject m_Background;
    [SerializeField] Wind m_WindPrefab;

    [SerializeField] float m_CameraStartScroll;
    [SerializeField] float m_ScrollingSpeed;

    private Vector3 _startWind;
    private Vector3 _endWind;

    private void Start()
    {
        float time = m_SpawnTime / m_NbStartBubble;
        InvokeRepeating(nameof(SpawnBubble), 0, time);
        Invoke(nameof(CancelInvoke), m_SpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        float startedTime = Time.time - m_SpawnTime;
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

        if (startedTime - m_CameraStartScroll < 0) return;
        m_Camera.transform.position += Vector3.up * m_ScrollingSpeed * Time.deltaTime;
    }

    void SpawnBubble()
    {
        Vector3 pos = Vector3.Lerp(m_SpawnPos1, m_SpawnPos2, Random.value);
        Instantiate(m_BubblePrefab, pos, Quaternion.identity, transform);
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
