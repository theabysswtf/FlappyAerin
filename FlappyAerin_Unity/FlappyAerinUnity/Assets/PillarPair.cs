using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarPair : MonoBehaviour
{
    public Transform top;
    public Transform bot;

    Hittable[] _hittableObjects;

    void Awake()
    {
        _hittableObjects = GetComponentsInChildren<Hittable>();
    }

    public void ResetHittable()
    {
        foreach (Hittable obj in _hittableObjects)
        {
            obj.ResetObject();
        }
    }
}
