using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBraker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Spawn Broken Peaces
        //Play Breaking Sound
        Destroy(transform.parent.gameObject);
    }

}
