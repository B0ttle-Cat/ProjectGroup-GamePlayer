using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclesManager : MonoBehaviour
{
    public CameraOrbit cam;
    public KeyCode nextVehicle = KeyCode.C;
    public KeyCode previousVehicle = KeyCode.V;
    public GameObject[] vehicles;
    public string vehiclesTag = "Vehicles";
    public int vehicleIndex = 0;

    private void Start()
    {
        vehicles = GameObject.FindGameObjectsWithTag(vehiclesTag);
        vehicleIndex = Random.Range(0, vehicles.Length);
        SelectVehicle();
    }

    private void Update()
    {
        int previousVehicleIndex = vehicleIndex;

        if (Input.GetKeyDown(nextVehicle))
        {
            if (vehicleIndex >= vehicles.Length - 1)
                vehicleIndex = 0;
            else
                vehicleIndex++;
        }

        if (Input.GetKeyDown(previousVehicle))
        {
            if (vehicleIndex <= 0)
                vehicleIndex = vehicles.Length -1;
            else
                vehicleIndex--;
        }

        if (previousVehicleIndex != vehicleIndex)
            SelectVehicle();
    }

    void SelectVehicle()
    {
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (i == vehicleIndex)
            {
                cam.target = vehicles[i].GetComponent<TurretController>().cameraTarget;

                VehicleController vController = vehicles[i].GetComponent<VehicleController>();

                if (vController)
                    vController.controlVehicle = true;

                TanksController tController = vehicles[i].GetComponent<TanksController>();

                if (tController)
                    tController.controlVehicle = true;
            }
            else
            {
                VehicleController vController = vehicles[i].GetComponent<VehicleController>();

                if (vController)
                    vController.controlVehicle = false;

                TanksController tController = vehicles[i].GetComponent<TanksController>();

                if (tController)
                    tController.controlVehicle = false;
            }
        }
    }
}
