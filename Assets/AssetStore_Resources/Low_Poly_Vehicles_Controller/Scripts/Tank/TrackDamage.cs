using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackDamage : MonoBehaviour
{
    public Transform spawnPoint;
    public TanksDamageController.Tracks track;
    private TanksDamageController damage;

    private void Start()
    {
        damage = GetComponentInParent<TanksDamageController>();
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("TRRIGGGERRR");

        if(col.gameObject.tag == "Mine")
        {
            Mine mine = col.transform.GetComponentInParent<Mine>();

            damage.damage(track, spawnPoint.position, spawnPoint.rotation, mine.explosionForce, mine.transform.position);
            Destroy(mine.transform.gameObject);
        }
    }
}
