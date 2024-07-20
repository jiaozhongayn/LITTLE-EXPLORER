using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public List<GameObject> Weapons = new List<GameObject>();

    public void DropSwords()
    {
        foreach (var weapon in Weapons)
        {
            weapon.AddComponent<Rigidbody>();
            weapon.AddComponent<BoxCollider>();
            weapon.transform.SetParent(null);
        }
    }
}
