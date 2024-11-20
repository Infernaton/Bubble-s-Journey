using System;
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
            _rb.AddForce(wind.WindForce * wind.GetWindForceModifier(transform.position));
        }
        if (Math.Abs(transform.position.x) >= GameManager.Instance.GetBorderTPOffset())
            transform.position = new Vector3(transform.position.x * -1, transform.position.y, transform.position.z);

        GameManager.Instance.HighScore = transform.position.y;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BubbleDestroyer"))
        {
            Destroy(gameObject);
            GameManager.Instance.SpawnBubble();
            ParticleSystem particle = GameManager.Instance.GetParticles(Particle.DestroyedBubble);
            particle.gameObject.transform.position = transform.position;
            particle.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("WinObject"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
