using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Wind : MonoBehaviour
{
    [SerializeField] float m_LifeTimeMax = 5;
    [SerializeField] Vector3 m_StartPos;
    [SerializeField] Vector3 m_EndPos;
    [SerializeField] float m_SpeedOffset;

    public Vector3 WindForce => new((m_EndPos.x - m_StartPos.x) / m_SpeedOffset, (m_EndPos.y - m_StartPos.y) / m_SpeedOffset, 0);

    private float _currentLifeTime;

    private CapsuleCollider _collider;

    public void Init(Vector3 startPos, Vector3 endPos)
    {
        m_StartPos = startPos;
        m_EndPos = endPos;
        _collider.height = Vector3.Distance(startPos, endPos);
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
