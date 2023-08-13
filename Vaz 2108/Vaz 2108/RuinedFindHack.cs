using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace F100
{
    internal class RuinedFindHack
    {
        public static void SetupRuinedFind()
        {
            int value = ModMain.random.NextInt(1, 12);
            if (value == 4)
            {
                MainCarProperties[] cars = UnityEngine.Object.FindObjectsOfType<MainCarProperties>();
                foreach (MainCarProperties mcp in cars)
                {
                    if (mcp.Owner == "None")
                    {
                        Vector3 creationPos = mcp.transform.position + new Vector3(0f, 3.5f, 0f);
                        GameObject.Destroy(mcp);

                        GameObject instanciated = GameObject.Instantiate(ModMain.VehiclePrefab_v8, creationPos, Quaternion.identity);
                        instanciated.GetComponent<MainCarProperties>().CreatingRuinedFind();
                    }
                }
            }
        }
    }
}
