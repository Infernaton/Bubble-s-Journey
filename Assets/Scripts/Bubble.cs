using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Rigidbody _rb;
    private List<Wind> _winds = new();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        foreach (Wind wind in _winds)
        {
            _rb.AddForce(wind.WindForce);
        }
    }

    public void AddWindEffect(Wind wind)
    {
        if (_winds.Contains(wind)) return;
        _winds.Add(wind);
    }

    public void RemoveWindEffect(Wind wind)
    {
        if (_winds.Contains(wind)) _winds.Remove(wind);
    }
}