using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Wind : MonoBehaviour
{
    [SerializeField] float m_LifeTimeMax = 5;
    [SerializeField] Vector3 m_StartPos;
    [SerializeField] Vector3 m_EndPos;

    private float _currentLifeTime;

    public void Init(Vector3 startPos, Vector3 endPos)
    {
        m_StartPos = startPos;
        m_EndPos = endPos;
    }

    private void Start()
    {
        _currentLifeTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(m_StartPos, m_EndPos, Color.yellow);

        if (Time.time - _currentLifeTime >= m_LifeTimeMax) Destroy(gameObject);
    }
}