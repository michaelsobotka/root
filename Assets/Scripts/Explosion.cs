using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        Invoke("Boom", 5f);
    }

    void Boom(){
        Destroy(gameObject);
    }
}
