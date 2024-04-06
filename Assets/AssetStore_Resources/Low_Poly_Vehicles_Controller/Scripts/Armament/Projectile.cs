using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int projectileDestroyTime = 25;
    public GameObject hitPrefab;
    public int hitDestroyTime = 10;

    private void Start()
    {
        //Destroy(gameObject, projectileDestroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = Instantiate(hitPrefab, transform.position, Quaternion.FromToRotation(hitPrefab.transform.up, transform.forward)) as GameObject;
        Destroy(hit, hitDestroyTime);
        Destroy(gameObject);
    }
}
