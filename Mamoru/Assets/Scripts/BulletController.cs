using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        this.gameObject.SetActive(false);
        //Destroy Partical System
        Destroy(this.gameObject);
    }

}
