using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace F100
{
    internal class HandbrakeFixF100 : MonoBehaviour
    {
        Rigidbody rb, rbvc;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rbvc = transform.root.GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (rb.mass == 300)
            {
                rbvc.Sleep();
            }
        }
    }
}
