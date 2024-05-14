using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        Destroy(gameObject);
    }
}
