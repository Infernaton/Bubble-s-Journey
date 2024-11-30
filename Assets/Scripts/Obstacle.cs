using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    void Awake()
    {
        transform.rotation = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);
    }
}
