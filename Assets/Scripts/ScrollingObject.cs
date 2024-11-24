using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    [SerializeField] Vector3 m_PositionOffset;
    [SerializeField] float m_ScrollingSpeed;
    [SerializeField] float m_MaxHeight;

    public void UpdatePosition(Vector3 destination)
    {
        float destinationHeight = m_MaxHeight < 0 ? destination.y : Mathf.Min(m_MaxHeight, destination.y);
        transform.position = Vector3.Lerp(
            transform.position,
            m_PositionOffset + Vector3.up * destinationHeight,
            Time.smoothDeltaTime * m_ScrollingSpeed);
    }
}
