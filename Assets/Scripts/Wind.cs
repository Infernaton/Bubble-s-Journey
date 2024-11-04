using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] ParticleSystem m_WindParticle;
    [SerializeField] float m_LifeTimeMax = 5;
    [SerializeField] Vector3 m_StartPos;
    [SerializeField] Vector3 m_EndPos;

    [Header("Wind force reducer")]
    [SerializeField] float m_OverallSpeedOffset;
    [SerializeField] float m_DistanceSpeedReducer;

    public Vector3 WindForce => new((m_EndPos.x - m_StartPos.x) / m_OverallSpeedOffset, (m_EndPos.y - m_StartPos.y) / m_OverallSpeedOffset, 0);

    private float _currentLifeTime;
    private CapsuleCollider _collider;

    /**
     * Modify the force of the wind, depending on the distance between the object and the Start position
     */
    public float GetWindForceModifier(Vector3 contactPosition)
    {
        float distance = Vector3.Distance(m_StartPos, contactPosition);
        return 1 / distance * m_DistanceSpeedReducer;
    }

    public void Init(Vector3 startPos, Vector3 endPos)
    {
        m_StartPos = startPos;
        m_EndPos = endPos;
        float height = Vector3.Distance(startPos, endPos);
        _collider.height = height;

        // m_WindParticle
        // Change the start pos + z rotation + height
        var editableShape = m_WindParticle.shape;
        editableShape.position = new Vector3(0, -height/2, 0);

        var editableMain = m_WindParticle.main;
        editableMain.startLifetime = height / m_WindParticle.main.startSpeed.constant;
    }

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        _currentLifeTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _currentLifeTime >= m_LifeTimeMax) Destroy(gameObject);


    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.TryGetComponent<Bubble>(out var bubble))
        {
            bubble.AddWindEffect(this);
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<Bubble>(out var bubble))
        {
            bubble.RemoveWindEffect(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_StartPos, m_EndPos);
    }
}
