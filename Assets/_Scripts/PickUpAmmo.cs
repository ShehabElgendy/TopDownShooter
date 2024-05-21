using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAmmo : Interactable
{
    public override void Interaction()
    {
        Debug.Log($"Added Ammo to Weapon");
    }
}
