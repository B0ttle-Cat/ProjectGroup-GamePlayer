using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanksDamageController : MonoBehaviour
{
    public GameObject destroyTrack;
    public GameObject leftTrack;
    public GameObject rightTrack;

    Rigidbody rb;

    public enum Tracks
    {
        leftTrack,
        rightTrack
    }

    TanksController controller;
    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        controller = GetComponent<TanksController>();
    }

    public void damage(Tracks tracks, Vector3 pos, Quaternion rot, float explosionForce , Vector3 explosifPosition)
    {
        switch (tracks)
        {
            case Tracks.leftTrack:
                leftTrack.SetActive(false);
                controller.leftTrack.enable = false;
                break;
            case Tracks.rightTrack:
                rightTrack.SetActive(false);
                controller.rightTrack.enable = false;
                break;
            default:
                break;
        }

        rb.AddForceAtPosition(Vector3.one * explosionForce, explosifPosition, ForceMode.Impulse);

        Instantiate(destroyTrack, pos, rot);
    }
}
