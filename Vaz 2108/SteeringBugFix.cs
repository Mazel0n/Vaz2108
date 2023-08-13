using NWH.VehiclePhysics2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace F100
{
    internal class SteeringBugFix : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(SteeringCheck());
        }

        IEnumerator SteeringCheck()
        {
            VehicleController vc = GetComponent<VehicleController>();
            foreach (var i in vc.powertrain.wheelGroups)
            {
                i.FindBelongingWheels();
            }

            yield return 0;
            yield return 0;
            yield return 0;
            yield return 0;
            yield return 0;

            foreach (var i in vc.powertrain.wheelGroups)
            {
                i.FindBelongingWheels();
            }

            yield return 0;

            GameObject.DestroyImmediate(GetComponent<SteeringBugFix>());
        }
    }
}
