using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour
{
    private static string ENEMY_LAYER_PARAMETER = "Enemy";
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(ENEMY_LAYER_PARAMETER);
    }
}
