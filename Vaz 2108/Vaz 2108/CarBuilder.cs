using NWH.VehiclePhysics2;
using NWH.VehiclePhysics2.Modules.Aerodynamics;
using NWH.VehiclePhysics2.Modules.Rigging;
using NWH.VehiclePhysics2.Powertrain;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.WheelController3D;
using PaintIn3D;
using RVP;
using SimplePartLoader;
using SimplePartLoader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Vaz_2108
{
    internal class CarBuilder
    {
        public static void Start()
        {
            CreateCarPrefabs();
        }

        internal static void CreateCarPrefabs()
        {
            CarBuilding.CopyCarToPrefab((GameObject)cachedResources.Load("Chad"), ModMain.EmptyVehiclePrefab);

            // A bit of car setup
            ModMain.EmptyVehiclePrefab.name = "F100";
            ModMain.EmptyVehiclePrefab.GetComponent<MainCarProperties>().CarName = "F100";

            // Chad transparents delete
            //DeleteTransparent("WiresMain06");
            DeleteTransparent("V8_american_classic");
            DeleteTransparent("InnerPanelRL07");
            DeleteTransparent("InnerPanelRR07");
            DeleteTransparent("MiddlePanelLipC07");
            DeleteTransparent("WheelhouseR07");
            DeleteTransparent("WheelhouseL07");
            DeleteTransparent("LRockerpanelC07");
            DeleteTransparent("RRockerpanelC07");
            DeleteTransparent("MiddlePanel07");
            DeleteTransparent("CowlPanel07");
            DeleteTransparent("TorsionSupport07");
            DeleteTransparent("RearFrameBrace07");
            DeleteTransparent("FrameRailL07");
            DeleteTransparent("FrameRailR07");
            DeleteTransparent("MIddleFrameBrace07");
            DeleteTransparent("Firewall07");
            DeleteTransparent("floor07");
            DeleteTransparent("LFrontpanel07");
            DeleteTransparent("QuarterpanelRL07");
            DeleteTransparent("QuarterpanelRR07");
            DeleteTransparent("RadiatorSupport07");
            DeleteTransparent("RearValance07");
            DeleteTransparent("RFrontpanel07");
            DeleteTransparent("Roof07");
            DeleteTransparent("RearPanel07");
            DeleteTransparent("RearFloor07");
            DeleteTransparent("FloorTrunk07");
            DeleteTransparent("InsideItems");

            // Adding custom transparents
            CarBuilding.AttachPrefabChilds(ModMain.EmptyVehiclePrefab, ModMain.TransparentsToAdd);
            CarBuilding.UpdateTransparentsReferences(ModMain.EmptyVehiclePrefab);

            // Building our car

            ModMain.EmptyVehiclePrefab.name = "F100_empty";
            SetupTemplate(ModMain.EmptyVehiclePrefab);

            BuildCar(ModMain.EmptyVehiclePrefab, ModMain.VehiclePrefab_i4f, 2);

            ModMain.VehiclePrefab_i4f.name = "F100";
            ModMain.EmptyVehiclePrefab.name = "F100";

            Saver.modParts.Add("F100", ModMain.EmptyVehiclePrefab);

            ModMain.EmptyVehiclePrefab.GetComponent<MainCarProperties>().PREFAB = ModMain.EmptyVehiclePrefab;
            ModMain.VehiclePrefab_i4f.GetComponent<MainCarProperties>().PREFAB = ModMain.EmptyVehiclePrefab;

            ModMain.EmptyVehiclePrefab.AddComponent<SteeringBugFix>();
            ModMain.VehiclePrefab_i4f.AddComponent<SteeringBugFix>();

            AddInsideItemCollider(ModMain.EmptyVehiclePrefab);
            AddInsideItemCollider(ModMain.VehiclePrefab_i4f);
        }

        internal static void AddInsideItemCollider(GameObject car)
        {
            GameObject insideItems = new GameObject("InsideItems");
            insideItems.transform.SetParent(car.transform);
            insideItems.transform.localPosition = Vector3.zero;
            insideItems.transform.localEulerAngles = Vector3.zero;

            GameObject insideCollider = new GameObject("InsideCollider");
            insideCollider.transform.SetParent(insideItems.transform);
            insideCollider.transform.localPosition = Vector3.zero;
            insideCollider.transform.localEulerAngles = Vector3.zero;

            InsideItems ii = insideItems.AddComponent<InsideItems>();
            InsideCollider ic = insideCollider.AddComponent<InsideCollider>();

            ii.InsideCollider = insideCollider;
            ic.insideitems = ii;

            insideItems.layer = LayerMask.NameToLayer("Ignore Raycast");
            insideCollider.layer = LayerMask.NameToLayer("Ignore Raycast");

            BoxCollider bx1 = insideCollider.AddComponent<BoxCollider>();
            bx1.isTrigger = true;
            bx1.center = new Vector3(-0.000481724739f, 0.923041999f, 0.32507515f);
            bx1.size = new Vector3(1.49405694f, 1.15488684f, 1.1796236f);

            BoxCollider bx2 = insideCollider.AddComponent<BoxCollider>();
            bx2.isTrigger = true;
            bx2.center = new Vector3(0.044660449f, 0.774989307f, -1.58376646f);
            bx2.size = new Vector3(1.89309955f, 0.858781457f, 2.55485535f);
        }

        internal static void DeleteTransparent(string transparentToDel)
        {
            Transform t = ModMain.EmptyVehiclePrefab.transform.Find(transparentToDel);
            if (!t)
            {
                ModMain.PrintIfDebug("[F100]: Invalid transparent delete - " + transparentToDel);
            }
            else
            {
                ModMain.PrintIfDebug("[F100]: Destroying transparent " + transparentToDel);
                GameObject.DestroyImmediate(t.gameObject);
            }
        }

        internal static void BuildCar(GameObject template, GameObject objective, int engineType)
        {
            CarBuilding.CopyCarToPrefab(template, objective);
            CarBuilding.UpdateTransparentsReferences(objective);
            SetupTemplate(objective);
            /*
             * 
            template.name = "F100_empty";
            ModMain.PrintIfDebug(template);
            ModMain.PrintIfDebug(objective);
            //LoadEnginesIntoCatalog();
            ModMain.PrintIfDebug("sTemplate");
            SetupTemplate(template);
            ModMain.PrintIfDebug("sTemplate POST");
            
            // Now we build the whole car

            ModMain.PrintIfDebug(template);
            ModMain.PrintIfDebug(objective);

            ModMain.PrintIfDebug("starting");
            objective.name = "F100_builded";
            SetupTemplate(objective);
            SetupTemplate(template);
            ModMain.PrintIfDebug("RECURSIVE CAR BUILDING");
            */
            // Build car
            RecursiveCarBuilding(objective, engineType);

            CarBuilding.UpdateTransparentsReferences(objective);

            // MyBoneSCR fixes
            foreach (MyBoneSCR scr in objective.GetComponentsInChildren<MyBoneSCR>())
            {
                if (scr.thisTransform != null)
                {
                    if (scr.thisTransform.root != objective.transform)
                    {
                        scr.thisTransform = scr.transform;
                        ModMain.PrintIfDebug("MyBoneSCR thisTransform fix applied to " + scr.thisTransform);
                    }
                }
            }

            foreach (MyBoneSCR scr in objective.GetComponentsInChildren<MyBoneSCR>())
            {
                // Control arms use all same formula for this.
                // The error reporter shows that there are issues with these but actually the targetTransform is calculated on runtime!
                // Same happens with shock absorbers & springs
                if (scr.transform.name == "LowerControlArmFL08" || scr.transform.name == "LowerControlArmFR08" || scr.transform.name == "UpperControlArmFL08" || scr.transform.name == "UpperControlArmFR08" || scr.transform.name == "F100Spring" || scr.transform.name == "ShockAbsorberF08" || scr.transform.name == "BrakeLine" || scr.transform.name == "F100Leafspring" || scr.transform.name == "ShockAbsorberR07" || scr.transform.name == "TieRod08" || scr.transform.name == "F100HalfAxle")
                {
                    scr.LocalStrtetchTarget = scr.transform.GetChild(0);
                    ModMain.PrintIfDebug("SCR LST fix applied to " + scr.name);
                }
                else if (scr.transform.name == "HandbbrakeCable07")
                {
                    scr.LocalStrtetchTarget = scr.transform.parent.Find("DummyPivHbrak");
                }

            }

            // MyBoneSCR issue finder
            // Very useful code btw :)
            foreach (MyBoneSCR scr in objective.GetComponentsInChildren<MyBoneSCR>())
            {
                if (scr.targetTransform != null)
                {
                    if (!scr.targetTransform.root.name.Contains("F100") && scr.targetTransform.parent != null)
                    {
                        ModMain.PrintIfDebug("[F100]: targetTransform MyBoneSCR issue found in " + Functions.GetTransformPath(scr.transform));
                    }
                }

                if (scr.targetTransformB != null)
                {
                    if (!scr.targetTransformB.root.name.Contains("F100") && scr.targetTransformB.parent != null)
                    {
                        ModMain.PrintIfDebug("[F100]: targetTransformB MyBoneSCR issue found in " + Functions.GetTransformPath(scr.transform));
                    }
                }

                if (scr.thisTransform != null)
                {
                    if (!scr.thisTransform.root.name.Contains("F100") && scr.thisTransform.parent != null)
                    {
                        ModMain.PrintIfDebug("[F100]: thisTransform MyBoneSCR issue found in " + Functions.GetTransformPath(scr.transform));
                    }
                }

                if (scr.LocalStrtetchTarget != null)
                {
                    if (!scr.LocalStrtetchTarget.root.name.Contains("F100") && scr.LocalStrtetchTarget.parent != null)
                    {
                        ModMain.PrintIfDebug("[F100]: LocalStrtetchTarget MyBoneSCR issue found in " + Functions.GetTransformPath(scr.transform));
                    }
                }
            }

            // Fixing engine wrong references.
            // The wrong references happens because car prefab is done through reflection.
            // Engine specifics
            if (engineType == 1)
            {
                ModMain.PrintIfDebug("Oilpan fix F100 i4");
                objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock18").name = "CylinderBlock";
                CommonFixes.FixPart(objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/OilPan06/OilPan06").gameObject, FixType.Oilpan);
                FLUID OilContainerComponent = objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/OilPan06/OilPan06/OilFluidContainer").GetComponent<FLUID>();

                OilContainerComponent.FluidSize = 2f;
                OilContainerComponent.Condition = 1f;
                OilContainerComponent.transform.parent.GetComponent<CarProperties>().FluidSize = 2f;
                OilContainerComponent.transform.parent.GetComponent<CarProperties>().FluidCondition = 1f;
                // Cylinder head (also oil)
                ModMain.PrintIfDebug("Cylinderhead fix F100");
                CommonFixes.FixPart(objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/CylinderHead06/CylinderHead06/CylinderHeadCover06/CylinderHeadCover06").gameObject, FixType.CylinderHeadCover);

            }
            else if (engineType == 2)
            {
                ModMain.PrintIfDebug("Oilpan fix F100 v8");
                objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlockV8").name = "CylinderBlock";
                CommonFixes.FixPart(objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/OilPan07/OilPan07").gameObject, FixType.Oilpan);
                FLUID OilContainerComponent = objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/OilPan07/OilPan07/OilFluidContainer").GetComponent<FLUID>();

                OilContainerComponent.FluidSize = 2f;
                OilContainerComponent.Condition = 1f;

                OilContainerComponent.transform.parent.GetComponent<CarProperties>().FluidSize = 2f;
                OilContainerComponent.transform.parent.GetComponent<CarProperties>().FluidCondition = 1f;
                // Cylinder head (also oil)
                CommonFixes.FixPart(objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/CylinderHeadR07/CylinderHeadR07/CylinderHeadCoverR07/CylinderHeadCoverR07").gameObject, FixType.CylinderHeadCover);
            }
            else if (engineType == 3)
            {
                ModMain.PrintIfDebug("Oilpan fix F100 i6");
                objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlockI6").name = "CylinderBlock";
                CommonFixes.FixPart(objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/OilPanI6/OilPanI6").gameObject, FixType.Oilpan);
                FLUID OilContainerComponent = objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/OilPanI6/OilPanI6/OilFluidContainer").GetComponent<FLUID>();

                OilContainerComponent.FluidSize = 2f;
                OilContainerComponent.Condition = 1f;
                OilContainerComponent.transform.parent.GetComponent<CarProperties>().FluidSize = 2f;
                OilContainerComponent.transform.parent.GetComponent<CarProperties>().FluidCondition = 1f;

                // Cylinder head (also oil)
                ModMain.PrintIfDebug("Cylinderhead fix F100");
                CommonFixes.FixPart(objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock/CylinderHeadI6/CylinderHeadI6/CylinderHeadCoverI6/CylinderHeadCoverI6").gameObject, FixType.CylinderHeadCover);
            }

            // Dipstick
            CommonFixes.FixPart(objective.transform.Find("EngineTranny/CylinderBlock/CylinderBlock").gameObject, FixType.Dipstick);

            // Brake master cylinder fix
            ModMain.PrintIfDebug("Brake fluid fix f100");
            CommonFixes.FixPart(objective.transform.Find("F100FirewallInterior/F100FirewallInterior/BrakeMasterCylinder06/BrakeMasterCylinder06").gameObject, FixType.BrakeCylinder);
            FLUID BrakeFluidComponent = objective.transform.Find("F100FirewallInterior/F100FirewallInterior/BrakeMasterCylinder06/BrakeMasterCylinder06/BrakeFluidContainer").GetComponent<FLUID>();

            BrakeFluidComponent.FluidSize = BrakeFluidComponent.ContainerSize;
            BrakeFluidComponent.Condition = 1f;
            BrakeFluidComponent.transform.parent.GetComponent<CarProperties>().FluidSize = BrakeFluidComponent.ContainerSize;
            BrakeFluidComponent.transform.parent.GetComponent<CarProperties>().FluidCondition = 1f;

            // Fueltank fix
            ModMain.PrintIfDebug("Fuel tank fix F100");
            CommonFixes.FixPart(objective.transform.Find("F100MainFloorPanel/F100MainFloorPanel/F100FuelTank/F100FuelTank").gameObject, FixType.FuelTank);

            FLUID FuelTankComponent = objective.transform.Find("F100MainFloorPanel/F100MainFloorPanel/F100FuelTank/F100FuelTank/FuelContainer").GetComponent<FLUID>();
            FuelTankComponent.Condition = 1;
            FuelTankComponent.FluidSize = 25f;
            FuelTankComponent.transform.parent.GetComponent<CarProperties>().FluidSize = 25f;
            FuelTankComponent.transform.parent.GetComponent<CarProperties>().FluidCondition = 1f;

            // Coolant fix
            ModMain.PrintIfDebug("Coolant fix");
            CommonFixes.FixPart(objective.transform.Find("F100Frontpanel/F100Frontpanel/F100Radiator/F100Radiator").gameObject, FixType.Radiator);
            FLUID CoolantFluid = objective.transform.Find("F100Frontpanel/F100Frontpanel/F100Radiator/F100Radiator/CoolantFluidContainer").GetComponent<FLUID>();

            CoolantFluid.FluidSize = 6f;
            CoolantFluid.Condition = 1f;
            CoolantFluid.transform.parent.GetComponent<CarProperties>().FluidSize = 6f;
            CoolantFluid.transform.parent.GetComponent<CarProperties>().FluidCondition = 1f;

            // Seats fix
            ModMain.PrintIfDebug("Seats fix F100");
            Transform DriverSeat = objective.transform.Find("F100MainFloorPanel/F100MainFloorPanel/SeatFL06/SeatFL06");
            CommonFixes.FixPart(DriverSeat.gameObject, FixType.DriverPassangerSeat);

            ModMain.PrintIfDebug("ShatterFix");
            // Fixing windows shatter particle (This is apparently a thing lol)
            CommonFixes.FixPart(objective, FixType.Windows);

            ModMain.PrintIfDebug("Painting support");

            EnableFullSupport(objective.transform.Find("F100FloorBed/F100FloorBed").gameObject);
            EnableFullSupport(objective.transform.Find("F100WheelWellL/F100WheelWellL").gameObject);
            EnableFullSupport(objective.transform.Find("F100WheelWellR/F100WheelWellR").gameObject);
            EnableFullSupport(objective.transform.Find("F100FenderRL/F100FenderRL").gameObject);
            EnableFullSupport(objective.transform.Find("F100FenderRR/F100FenderRR").gameObject);
            EnableFullSupport(objective.transform.Find("F100BedCabinWall/F100BedCabinWall").gameObject);
            EnableFullSupport(objective.transform.Find("F100TailgateHolder/F100TailgateHolder/F100BedTailgate/F100BedTailgate").gameObject);

            // Cabin update
            EnableFullSupport(objective.transform.Find("F100FirewallExterior/F100FirewallExterior").gameObject);
            EnableFullSupport(objective.transform.Find("F100FirewallExterior/F100FirewallExterior/F100DoorL/F100DoorL").gameObject);
            EnableFullSupport(objective.transform.Find("F100FirewallExterior/F100FirewallExterior/F100DoorR/F100DoorR").gameObject);

            EnableFullSupport(objective.transform.Find("F100FenderFL/F100FenderFL/F100QuarterpanelFL/F100QuarterpanelFL").gameObject);
            EnableFullSupport(objective.transform.Find("F100FenderFR/F100FenderFR/F100QuarterpanelFR/F100QuarterpanelFR").gameObject);
            EnableFullSupport(objective.transform.Find("F100Frontpanel/F100Frontpanel/F100FrontValance/F100FrontValance").gameObject);

            EnableFullSupport(objective.transform.Find("F100RockerL/F100RockerL").gameObject);
            EnableFullSupport(objective.transform.Find("F100RockerR/F100RockerR").gameObject);
            EnableFullSupport(objective.transform.Find("F100Roof/F100Roof").gameObject);
            EnableFullSupport(objective.transform.Find("F100RearPanel/F100RearPanel").gameObject);

            EnableFullSupport(objective.transform.Find("F100FenderFR/F100FenderFR/F100Hood/F100Hood").gameObject);

            EnableDirtOnly(objective.transform.Find("F100Roof/F100Roof/F100Windshield/F100Windshield").gameObject);
            EnableDirtOnly(objective.transform.Find("F100Roof/F100Roof/F100RearWindow/F100RearWindow").gameObject);

            // Autofix car lights
            ModMain.PrintIfDebug("Autofix car lights");
            CommonFixes.FixPart(objective, FixType.CarLights);

            // Fixing window lifts
            Transform WindowLiftHandleRight = objective.transform.Find("F100FirewallExterior/F100FirewallExterior/F100DoorR/F100DoorR/F100WindowLiftRight/F100WindowLiftRight/WIndowHandle");
            Transform WindowLiftTransparentRight = objective.transform.Find("F100FirewallExterior/F100FirewallExterior/F100DoorR/F100DoorR/F100WindowLiftRight/F100WindowLiftRight/F100WindowR");

            WindowLiftHandleRight.GetComponent<WindowLift>().Window = WindowLiftTransparentRight.gameObject;

            Transform WindowLiftHandleLeft = objective.transform.Find("F100FirewallExterior/F100FirewallExterior/F100DoorL/F100DoorL/F100WindowLiftLeft/F100WindowLiftLeft/WIndowHandle.003");
            Transform WindowLiftTransparentLeft = objective.transform.Find("F100FirewallExterior/F100FirewallExterior/F100DoorL/F100DoorL/F100WindowLiftLeft/F100WindowLiftLeft/F100WindowL");

            WindowLiftHandleLeft.GetComponent<WindowLift>().Window = WindowLiftTransparentLeft.gameObject;

            /*WindowLiftHandleRight.localPosition = new Vector3(-0.013032198f, -0.0886586756f, 0.304578304f);

            WindowLiftTransparentRight.name = "F100WindowR";
            WindowLiftTransparentRight.localPosition = new Vector3(-0.0205740929f, 0.242237329f, 0.074375622f);
            WindowLiftTransparentRight.localEulerAngles = new Vector3(-90f, 0f, 0f);
            */

            // Last thing to do, actually attach everything.
            foreach (Partinfo partinfo in objective.GetComponentsInChildren<Partinfo>())
            {
                partinfo.fixedImportantBolts = 0f;
                partinfo.fixedwelds = 0f;
                partinfo.attachedwelds = 0f;
                partinfo.ImportantBolts = 0f;
                partinfo.tightnuts = 0f;
                partinfo.attachedbolts = 0f;

                if (!String.IsNullOrEmpty(partinfo.RenamedPrefab))
                    partinfo.gameObject.name = partinfo.RenamedPrefab;
            }

            MainCarProperties mcp = objective.GetComponent<MainCarProperties>();
            foreach (CarProperties carProps in objective.transform.GetComponentsInChildren<CarProperties>())
            {
                carProps.MainProperties = mcp;
            }

            foreach (HexNut hexNut in objective.GetComponentsInChildren<HexNut>())
            {
                hexNut.tight = true;
                hexNut.gameObject.transform.parent.GetComponent<Partinfo>().attachedbolts += 1f;
                hexNut.gameObject.transform.parent.GetComponent<Partinfo>().tightnuts += 1f;
            }
            ModMain.PrintIfDebug("HEXNUT FINISH");
            foreach (FlatNut flatNut in objective.GetComponentsInChildren<FlatNut>())
            {
                flatNut.tight = true;
                if (!flatNut.gameObject.transform.parent.GetComponent<Partinfo>())
                {
                    ModMain.PrintIfDebug("FN error:" + flatNut);
                }
                flatNut.gameObject.transform.parent.GetComponent<Partinfo>().attachedbolts += 1f;
                flatNut.gameObject.transform.parent.GetComponent<Partinfo>().tightnuts += 1f;
            }
            ModMain.PrintIfDebug("FlatNut FINISH");
            foreach (BoltNut boltNut in objective.GetComponentsInChildren<BoltNut>())
            {
                boltNut.ReStart();
                boltNut.tight = true;

                if (!boltNut.gameObject.transform.parent.GetComponent<Partinfo>())
                {
                    ModMain.PrintIfDebug("BNERROR1");
                    ModMain.PrintIfDebug("" + boltNut);
                    ModMain.PrintIfDebug("" + boltNut.transform.parent);
                    continue;
                }
                boltNut.gameObject.transform.parent.GetComponent<Partinfo>().ImportantBolts += 1f;
                boltNut.gameObject.transform.parent.GetComponent<Partinfo>().fixedImportantBolts += 1f;
                if (!boltNut.otherobject)
                {
                    ModMain.PrintIfDebug("BNERROR2");
                    ModMain.PrintIfDebug($"Missing otherobject BOLTNUT in paart {boltNut.transform.parent} - missing: {boltNut.otherobjectName}");
                    continue;
                }
                if (!boltNut.otherobject.GetComponent<Partinfo>())
                {
                    ModMain.PrintIfDebug("3");
                    ModMain.PrintIfDebug("" + boltNut);
                    ModMain.PrintIfDebug("" + boltNut.transform.parent);
                    continue;
                }
                boltNut.otherobject.GetComponent<Partinfo>().fixedImportantBolts += 1f;
                boltNut.otherobject.GetComponent<Partinfo>().ImportantBolts += 1f;
            }
            ModMain.PrintIfDebug("BoltNut FINISH");
            foreach (WeldCut weldCut in objective.GetComponentsInChildren<WeldCut>())
            {
                weldCut.ReStart();
                weldCut.welded = true;
                weldCut.gameObject.transform.parent.GetComponent<Partinfo>().fixedwelds += 1f;
                weldCut.gameObject.transform.parent.GetComponent<Partinfo>().attachedwelds += 1f;

                if (!weldCut.otherobject)
                {
                    ModMain.PrintIfDebug($"Missing otherobject in WELDCUT paart {weldCut.transform.parent} - missing: {weldCut.otherobjectName}");
                    continue;
                }

                if (weldCut.otherobject.name == "F100FirewallExterior")
                {
                    ModMain.PrintIfDebug($"{weldCut.transform.parent} weldcut on exterior");
                }

                weldCut.otherobject.GetComponent<Partinfo>().fixedwelds += 1f;
                weldCut.otherobject.GetComponent<Partinfo>().attachedwelds += 1f;
            }

            // Fixing missing glass shater sound
            AudioSource glassSoundSource = objective.transform.Find("GlassShatterSound").GetComponent<AudioSource>();
            foreach (RVP.ShatterPart part in objective.GetComponentsInChildren<RVP.ShatterPart>())
            {
                if (!part.name.Contains("LightBulb"))
                    part.shatterSnd = glassSoundSource;
            }

            // Fixing cluster lights
            Transform clusterGo = objective.transform.Find("F100FirewallInterior/F100FirewallInterior/F100Dashboard/F100Dashboard/Cluster06/Cluster06");
            CarProperties clusterProps = clusterGo.GetComponent<CarProperties>();

            clusterProps.ClusterBat = clusterGo.Find("BatLight").gameObject;
            clusterProps.ClusterHigh = clusterGo.Find("High").gameObject;
            clusterProps.ClusterL = clusterGo.Find("Left").gameObject;
            clusterProps.ClusterR = clusterGo.Find("Right").gameObject;

            // For fuck ups lol
            /*foreach (CarProperties c in objective.GetComponentsInChildren<CarProperties>())
            {
                if(c.Paintable)
                {
                    if(!c.gameObject.GetComponent<P3dPaintableTexture>())
                    {
                        ModMain.PrintIfDebug("MESSUP " + c + ", " + c.name);
                    }
                }
            }*/

            // Setting part count (adapted code of JobManager btw) AND ALSO fixing VISUALOBJECTS
            int partCount = 0;
            float totalCost = 0;
            CarProperties[] componentsInChildren = objective.GetComponentsInChildren<CarProperties>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (componentsInChildren[i].PREFAB)
                {
                    totalCost += componentsInChildren[i].GetComponent<Partinfo>().price;
                }

                if (componentsInChildren[i].SinglePart)
                {
                    partCount++;
                }

                if (componentsInChildren[i].VisualObject)
                {
                    if (componentsInChildren[i].name == componentsInChildren[i].VisualObject.name)
                    {
                        componentsInChildren[i].VisualObject = componentsInChildren[i].gameObject;
                    }

                    foreach (Transform t in componentsInChildren[i].GetComponentsInChildren<Transform>())
                    {
                        if (t.name == componentsInChildren[i].VisualObject.name)
                        {
                            componentsInChildren[i].VisualObject = t.gameObject;
                            break;
                        }
                    }
                }

            }


            // AWD: 4
            // Mirrors: 6
            // Cabin light
            template.GetComponent<MainCarProperties>().PartsCount = partCount;
            objective.GetComponent<MainCarProperties>().PartsCount = partCount;

            template.GetComponent<MainCarProperties>().CarPrice = 18500;
            objective.GetComponent<MainCarProperties>().CarPrice = 18500;

            AddRemainingPartsToCatalog();
            // I used this to look for some error on the SetMesh function.
            // Since this does not occur with brake line from the game and only occurs with new 3d model of it i think the issue is that
            // Something related to the fact that the childrenmesh is not the mesh of the object? idk its 5am and i dont care i'm not even using that model anymore
            // Dont use custom brake line if you dont need, set properly the pivots of the brake lines and will be good.
            // update from the future: i was an idiot.
            // replace childrenmesh to your new mesh, if is a brake line set to all of them (1,2,3 atm but varies on the engine that the car uses)
            /*
            foreach(CarProperties cp in objective.GetComponentsInChildren<CarProperties>())
            {
                if (cp.RemovedDifferentMesh)
                {
                    ModMain.PrintIfDebug("PART " + cp + " / MCP trigger");
                }
                
                if(cp.transform.parent && cp.RemovedDifferentMesh && cp.transform.parent.GetComponent<transparents>() && cp.MainProperties)
                {
                    ModMain.PrintIfDebug("PART " + cp + " / CarProp trigger");
                }
            }*/
        }

        internal static void SetupTemplate(GameObject objective)
        {
            ModMain.PrintIfDebug("11");
            Transform FrontSusp = objective.transform.Find("FrontSusp");
            Transform RearSusp = objective.transform.Find("RearSusp");
            Transform EnginneTranny = objective.transform.Find("EngineTranny");

            MyBoneSCR MyBoneComponentF = FrontSusp.GetComponent<MyBoneSCR>();
            MyBoneSCR MyBoneComponentR = RearSusp.GetComponent<MyBoneSCR>();
            MyBoneSCR MyBoneComponentEngine = EnginneTranny.GetComponent<MyBoneSCR>();

            MyBoneComponentF.thisTransform = FrontSusp;
            MyBoneComponentF.targetTransform = objective.transform.Find("FRSuspensionPosition");
            MyBoneComponentF.targetTransformB = objective.transform.Find("FLSuspensionPosition");

            MyBoneComponentR.thisTransform = RearSusp;
            MyBoneComponentR.targetTransform = objective.transform.Find("RRSuspensionPosition");
            MyBoneComponentR.targetTransformB = objective.transform.Find("RLSuspensionPosition");

            MyBoneComponentEngine.thisTransform = EnginneTranny;
            MyBoneComponentEngine.targetTransform = objective.transform.Find("FRSuspensionPosition");
            MyBoneComponentEngine.targetTransformB = objective.transform.Find("FLSuspensionPosition");
            ModMain.PrintIfDebug("12");
            // Crossmember setup.
            // We have to rename it to make it unique.
            // The coords here were setup as shown in the prefab on the Unity project
            Transform crossmemberTransparent = objective.transform.Find("FrontSusp/Crossmemmber07");
            if (crossmemberTransparent)
            {
                crossmemberTransparent.name = "CrossmemmberF100";
                //crossmemberTransparent.localPosition = new Vector3(0.0312098283f, -0.0574216098f, 0.0163185652f);
                crossmemberTransparent.localPosition = new Vector3(0.0312098283f, -0.057f, 0.0163185652f);
            }

            // Driveshaft setup
            Transform driveshaftTransparent = objective.transform.Find("EngineTranny/DriveShaft07");
            if (driveshaftTransparent)
            {
                //driveshaftTransparent.name = "F100Driveshaft"; dont! diff07 dependency
                driveshaftTransparent.localPosition = new Vector3(0.957887173f, 0.0553110391f, -0.0145417452f);
                driveshaftTransparent.localEulerAngles = new Vector3(4.71347666f, 87.3576355f, 179.646561f);
            }
            ModMain.PrintIfDebug("13");
            // Brake line & pivots setup
            Transform brakeLine = objective.transform.Find("MainBrakeLine");
            brakeLine.localPosition = new Vector3(-0.337344408f, 0.590443611f, 1.38899231f);
            transparents brakeLineTransparent = brakeLine.GetComponent<transparents>();
            brakeLineTransparent.ChildrenMesh = ModMain.BrakeLineMesh.GetComponent<MeshFilter>().sharedMesh;
            brakeLineTransparent.ChildrenMesh1 = ModMain.BrakeLineMesh.GetComponent<MeshFilter>().sharedMesh;
            brakeLineTransparent.ChildrenMesh2 = ModMain.BrakeLineMesh.GetComponent<MeshFilter>().sharedMesh;

            Transform brakePivotFR = objective.transform.Find("MainBrakeLine/FRbrakePivot");
            Transform brakePivotFL = objective.transform.Find("MainBrakeLine/FLbrakePivot");
            Transform brakePivotRR = objective.transform.Find("MainBrakeLine/RRbrakePivot");
            Transform brakePivotRL = objective.transform.Find("MainBrakeLine/RLbrakePivot");

            brakePivotFR.localPosition = new Vector3(0.847f, -0.355f, 0.06f);

            brakePivotFL.localPosition = new Vector3(-0.14f, -0.3552f, 0.06f);
            brakePivotFL.localEulerAngles = new Vector3(0.3553f, 351.4458f, 0.005f);

            brakePivotRR.localPosition = new Vector3(-0.1f, -0.32f, -3.0399f);

            brakePivotRL.localPosition = new Vector3(-0.161f, -0.36f, -3.041f);
            ModMain.PrintIfDebug("14");
            // Wires main setup
            GameObject CarsParent = GameObject.Find("CarsParent");
            GameObject[] GameCars = CarsParent.GetComponent<CarList>().Cars;
            GameObject Niva = null;
            foreach (GameObject car in GameCars)
            {
                if (car.name.StartsWith("NIV"))
                {
                    Niva = car;
                    break;
                }
            }

            Transform wiresMain = objective.transform.Find("WiresMain06");
            transparents NivTransparent = Niva.transform.Find("WiresMain06").GetComponent<transparents>();
            transparents F100Transparent = wiresMain.GetComponent<transparents>();

            F100Transparent.ChildrenMesh = NivTransparent.ChildrenMesh;
            F100Transparent.ChildrenMesh1 = NivTransparent.ChildrenMesh1;
            F100Transparent.ChildrenMesh2 = NivTransparent.ChildrenMesh2;

            wiresMain.localPosition = new Vector3(0.37020278f, 0.64535141f, 1.69255114f);
            wiresMain.localEulerAngles = new Vector3(0f, 0f, 180f);
            ModMain.PrintIfDebug("15");
            // Fuel line setup
            Transform fuelLine = objective.transform.Find("FuelLine");
            transparents fuelLineTransparent = fuelLine.GetComponent<transparents>();
            fuelLineTransparent.ChildrenMesh = ModMain.FuelLine;

            // Fixing car suspension position
            // OLD HEIGHT WAS 0.19
            ModMain.PrintIfDebug("155");
            objective.transform.Find("FRSuspensionPosition").localPosition = new Vector3(0.4f, 0.2119f, 1.604f);
            objective.transform.Find("FLSuspensionPosition").localPosition = new Vector3(-0.4f, 0.2119f, 1.604f);
            ModMain.PrintIfDebug("16");
            // Rear suspension setup
            RearSusp.transform.localPosition = new Vector3(0.00525641441f, 0.178413868f, -1.56136656f);
            objective.transform.Find("RRSuspensionPosition").localPosition = new Vector3(0.755256414f, 0.19f, -1.56136656f);
            objective.transform.Find("RLSuspensionPosition").localPosition = new Vector3(-0.744743586f, 0.19f, -1.56136656f);

            // Fixing broken FrontSuspension
            objective.transform.Find("FrontSusp/CrossmemmberF100/WheelContParentFR").localPosition = new Vector3(0.72f, 0.0149178505f, -0.0186132193f);
            //objective.transform.Find("FrontSusp/CrossmemmberF100/WheelContParentFL").localPosition = new Vector3(-0.827665925f, 0.0149178505f, -0.0186132193f);
            objective.transform.Find("FrontSusp/CrossmemmberF100/WheelContParentFL").localPosition = new Vector3(-0.72f, 0.0149178505f, -0.0186132193f);
            ModMain.PrintIfDebug("18");

            // Fix HBrake
            Transform Hbrake = objective.transform.Find("Hbrake");
            Hbrake.GetComponent<HingeJoint>().connectedBody = objective.transform.GetComponent<Rigidbody>();
            objective.GetComponent<MainCarProperties>().HandbrakeObject = Hbrake.gameObject;
            Hbrake.GetComponent<Rigidbody>().mass = 1500f;
            Hbrake.GetComponent<MeshCollider>().isTrigger = false;
            Hbrake.gameObject.AddComponent<HandbrakeFixF100>();
            ModMain.PrintIfDebug("19");

            // Changing vehicle dmg
            VehicleDamage dmg = objective.GetComponent<VehicleDamage>();
            dmg.maxCollisionMagnitude = 35f;
            dmg.maxCollisionPoints = 5;
            ModMain.PrintIfDebug("20");
            // Fixing broken Wheel controllers & powertrain
            MainCarProperties mainCarProps = objective.GetComponent<MainCarProperties>();
            WheelController WheelFR, WheelFL, WheelRL, WheelRR;

            WheelComponent NWH_WheelFR, NWH_WheelFL, NWH_WheelRR, NWH_WheelRL;
            WheelGroup FrontGroup = new WheelGroup(), RearGroup = new WheelGroup();
            List<WheelComponent> WheelControllers = new List<WheelComponent>();
            List<WheelGroup> WheelGroups = new List<WheelGroup>();

            VehicleController vehController = objective.GetComponent<VehicleController>();
            Powertrain oldPowertrain = vehController.powertrain;
            Powertrain vehPowertrain = new Powertrain();

            //vehController.centerOfMass = new Vector3(0, -0.013f, 0.2f);

            TransmissionComponent newTransmission = new TransmissionComponent();
            ClutchComponent newClutch = new ClutchComponent();
            EngineComponent newEngine = new EngineComponent();

            Steering newSteering = new Steering();
            Steering oldSteering = vehController.steering;

            newSteering.linearity = oldSteering.linearity;
            newSteering.maximumSteerAngle = 44;
            newSteering.returnToCenter = true;
            newSteering.smoothing = 0.05f;
            newSteering.steeringWheelTurnRatio = 5;
            newSteering.useDirectInput = false;

            vehController.steering = newSteering;

            OutputSelector outputSelectorEngine = new OutputSelector();
            OutputSelector outputSelectorTransmission = new OutputSelector();
            OutputSelector outputSelectorClutch = new OutputSelector();
            outputSelectorEngine.name = "Engine";
            outputSelectorClutch.name = "Clutch";
            outputSelectorTransmission.name = "Transmission";

            SetPrivatePropertyValue(newClutch, "outputASelector", outputSelectorTransmission);
            SetPrivatePropertyValue(newEngine, "outputASelector", outputSelectorClutch);

            vehPowertrain.clutch = newClutch;
            vehPowertrain.differentials = new List<DifferentialComponent>();
            vehPowertrain.engine = newEngine;
            vehPowertrain.transmission = newTransmission;

            newClutch.baseEngagementRPM = 1200f;
            newClutch.clutchEngagement = 0f;
            newClutch.engagementSpeed = 2.5f;
            newClutch.finalEngagementRPM = 1201.434f;
            newClutch.fwdAcceleration = 0.0195f;
            newClutch.slipTorque = 5000;
            newClutch.variableEngagementRPMRange = 1400;
            newClutch.inertia = 0.04f;
            newClutch.name = "Clutch";
            newClutch.startSignal = false;
            newClutch.shiftSignal = false;
            newClutch.isAutomatic = true;
            newClutch.PID_Kd = 2;
            newClutch.PID_Ki = 0.8f;
            newClutch.PID_Kp = 8;

            newEngine.engineType = EngineComponent.EngineType.ICE;
            newEngine.engineLayout = EngineComponent.EngineLayout.Longitudinal;
            newEngine.idleRPM = 800;
            newEngine.lossTorque = 0f;
            newEngine.maxLossTorque = 100;
            newEngine.maxRPM = 8040;
            newEngine.minRPM = -50;
            newEngine.powerCurve = oldPowertrain.engine.powerCurve;
            newEngine.revLimiterEnabled = true;
            newEngine.revLimiterActive = false;
            newEngine.revLimiterRPM = 6700;
            newEngine.slipTorque = 5000;
            newEngine.stallingEnabled = true;
            newEngine.stallRPM = 300;
            newEngine.starterRPMLimit = 500;
            newEngine.starterRunTime = 1;
            newEngine.starterTorque = 60;
            newEngine.name = "Engine";
            newEngine.inertia = 0.23f;
            newEngine.componentInputIsNull = true;
            newEngine.autoStartOnThrottle = false;

            newTransmission.finalGearRatio = 3.8f;
            newTransmission.ignorePostShiftBanInManual = true;
            newTransmission.postShiftBan = 0.5f;
            newTransmission.revMatch = true;
            newTransmission.shiftCheckCooldown = 0.1f;
            newTransmission.shiftDuration = 0.2f;
            newTransmission.variableShiftIntensity = 0.3f;
            newTransmission.variableShiftPoint = true;
            newTransmission.name = "Transmission";
            newTransmission.inertia = 0.02f;

            vehController.powertrain = vehPowertrain;
            ModMain.PrintIfDebug("22");
            WheelFR = objective.transform.Find("FrontSusp/CrossmemmberF100/WheelContParentFR/WheelControllerFR").GetComponent<WheelController>();
            WheelFL = objective.transform.Find("FrontSusp/CrossmemmberF100/WheelContParentFL/WheelControllerFL").GetComponent<WheelController>();
            WheelRR = objective.transform.Find("RearSusp/WheelControllerRR").GetComponent<WheelController>();
            WheelRL = objective.transform.Find("RearSusp/WheelControllerRL").GetComponent<WheelController>();

            /* testing stuff...            WheelFR.springMaximumForce = 12000;
                        WheelFL.springMaximumForce = 12000;
                        WheelRR.springMaximumForce = 12000;
                        WheelRL.springMaximumForce = 12000;*/

            ModMain.PrintIfDebug("DOINGMAGICC");
            mainCarProps.FRwhellcontroller = WheelFR.gameObject;
            mainCarProps.FLwhellcontroller = WheelFL.gameObject;
            mainCarProps.RRwhellcontroller = WheelRR.gameObject;
            mainCarProps.RLwhellcontroller = WheelRL.gameObject;

            WheelFR.parent = objective;
            WheelFL.parent = objective;
            WheelRR.parent = objective;
            WheelRL.parent = objective;

            // Now we fix the list in VehicleController
            NWH_WheelFR = new NWH.VehiclePhysics2.Powertrain.WheelComponent();
            NWH_WheelFR.wheelController = WheelFR;
            NWH_WheelFR.wheelGroupSelector.index = 0;
            NWH_WheelFR.name = "WheelWheelControllerFR";

            NWH_WheelFL = new NWH.VehiclePhysics2.Powertrain.WheelComponent();
            NWH_WheelFL.wheelController = WheelFL;
            NWH_WheelFL.wheelGroupSelector.index = 0;
            NWH_WheelFL.name = "WheelWheelControllerFL";

            NWH_WheelRR = new NWH.VehiclePhysics2.Powertrain.WheelComponent();
            NWH_WheelRR.wheelController = WheelRR;
            NWH_WheelRR.wheelGroupSelector.index = 1;
            NWH_WheelRR.name = "WheelWheelControllerRR";

            NWH_WheelRL = new NWH.VehiclePhysics2.Powertrain.WheelComponent();
            NWH_WheelRL.wheelController = WheelRL;
            NWH_WheelRL.wheelGroupSelector.index = 1;
            NWH_WheelRL.name = "WheelWheelControllerRL";

            WheelControllers.Add(NWH_WheelRR);
            WheelControllers.Add(NWH_WheelRL);
            WheelControllers.Add(NWH_WheelFR);
            WheelControllers.Add(NWH_WheelFL);

            FrontGroup.name = "Front";
            FrontGroup.ackermanPercent = 0.14f;
            FrontGroup.brakeCoefficient = 0.9f;
            FrontGroup.camberAtBottom = 1;
            FrontGroup.camberAtTop = -5;
            FrontGroup.steerCoefficient = 1;
            FrontGroup.trackWidth = 1.4559f;
            FrontGroup.antiRollBarForce = 2500;

            RearGroup.name = "Rear";
            RearGroup.ackermanPercent = 0;
            RearGroup.antiRollBarForce = 0F;
            RearGroup.brakeCoefficient = 0.4f;
            RearGroup.camberAtBottom = 0;
            RearGroup.camberAtTop = 0;
            RearGroup.handbrakeCoefficient = 2;
            RearGroup.steerCoefficient = 0;
            RearGroup.toeAngle = 0;
            RearGroup.trackWidth = 1.5f;

            /*List<WheelComponent> FrontWheels = new List<WheelComponent>();
            FrontWheels.Add(NWH_WheelFL);
            FrontWheels.Add(NWH_WheelFR);

            List<WheelComponent> RearWheels = new List<WheelComponent>();
            RearWheels.Add(NWH_WheelRL);
            RearWheels.Add(NWH_WheelRR);

            FrontGroup.SetWheels(FrontWheels);
            RearGroup.SetWheels(RearWheels);
            */
            WheelGroups.Add(FrontGroup);
            WheelGroups.Add(RearGroup);
            ModMain.PrintIfDebug("25");
            // Now we fix the powertrain too.
            vehPowertrain.wheelGroups = WheelGroups;
            vehPowertrain.wheels = WheelControllers;

            // Differentials fix
            DifferentialComponent RearDiff = new DifferentialComponent("Rear Differential", NWH_WheelRL, NWH_WheelRR), FrontDiff = new DifferentialComponent("Front Differential", NWH_WheelFL, NWH_WheelFR), TransferCase = new DifferentialComponent("TransferCase", FrontDiff, RearDiff);
            List<DifferentialComponent> Diffs = new List<DifferentialComponent>();
            Diffs.Add(RearDiff);
            Diffs.Add(TransferCase);
            Diffs.Add(FrontDiff);

            TransferCase.biasAB = 1;
            TransferCase.coastRamp = 0.5f;
            TransferCase.powerRamp = 1;
            TransferCase.preload = 13;
            TransferCase.slipTorque = 1000;
            TransferCase.stiffness = 1;
            TransferCase.inertia = 0.02f;
            TransferCase.input = vehPowertrain.transmission;
            ModMain.PrintIfDebug("ASDDASSD " + vehPowertrain.transmission);
            TransferCase.differentialType = DifferentialComponent.Type.Locked;

            RearDiff.input = TransferCase;
            RearDiff.biasAB = 0.5f;
            RearDiff.coastRamp = 0.5f;
            RearDiff.powerRamp = 1;
            RearDiff.preload = 5;
            RearDiff.slipTorque = 3600;
            RearDiff.stiffness = 0.5f;

            FrontDiff.input = TransferCase;
            FrontDiff.biasAB = 0.5f;
            FrontDiff.coastRamp = 0.5f;
            FrontDiff.powerRamp = 1;
            FrontDiff.preload = 2;
            FrontDiff.slipTorque = 1200;
            FrontDiff.stiffness = 0.5f;
            FrontDiff.differentialType = DifferentialComponent.Type.Open;

            OutputSelector outputSelectorFR = new OutputSelector();
            OutputSelector outputSelectorFL = new OutputSelector();
            OutputSelector outputSelectorRL = new OutputSelector();
            OutputSelector outputSelectorRR = new OutputSelector();
            OutputSelector outputSelectorFrontDiff = new OutputSelector();
            OutputSelector outputSelectorRearDiff = new OutputSelector();
            OutputSelector outputSelectorTransfer = new OutputSelector();
            outputSelectorFR.name = "WheelWheelControllerFR";
            outputSelectorFL.name = "WheelWheelControllerFL";
            outputSelectorRL.name = "WheelWheelControllerRL";
            outputSelectorRR.name = "WheelWheelControllerRR";
            outputSelectorFrontDiff.name = "Front Differential";
            outputSelectorRearDiff.name = "Rear Differential";
            outputSelectorTransfer.name = "TransferCase";

            vehPowertrain.transmission.outputA = TransferCase;

            SetPrivatePropertyValue(newTransmission, "outputASelector", outputSelectorTransfer);
            SetPrivatePropertyValue(FrontDiff, "outputASelector", outputSelectorFL);
            SetPrivatePropertyValue(FrontDiff, "outputBSelector", outputSelectorFR);
            SetPrivatePropertyValue(RearDiff, "outputASelector", outputSelectorRL);
            SetPrivatePropertyValue(RearDiff, "outputBSelector", outputSelectorRR);
            SetPrivatePropertyValue(TransferCase, "outputASelector", outputSelectorFrontDiff);
            SetPrivatePropertyValue(TransferCase, "outputBSelector", outputSelectorRearDiff);
            SetPrivatePropertyValue(vehPowertrain.transmission, "outputASelector", outputSelectorTransfer);

            vehPowertrain.differentials = Diffs;

            objective.GetComponent<MainCarProperties>().AWD = true;
            ModMain.PrintIfDebug("331");
            // Some stuff
            AerodynamicsModuleWrapper AeroWrapper = objective.GetComponent<AerodynamicsModuleWrapper>();
            AerodynamicsModule aerodynamicModule = AeroWrapper.module;
            aerodynamicModule.maxDownforceSpeed = 120;
            ModMain.PrintIfDebug("33311");

            /*
            // DELETED BECAUSE GAME 06/04/2023 UPDATE REMOVED THIS!
            // Rigind module fix.
            RiggingModuleWrapper RigidWrapper = objective.GetComponent<RiggingModuleWrapper>();
            RiggingModule RigidModule = new RiggingModule();
            Bone Bone1 = new Bone(), Bone2 = new Bone();
            Transform NonRot1 = null, NonRot2 = null;

            foreach (Transform child in objective.transform.Find("RearSusp").GetComponentsInChildren<Transform>())
            {
                if (child.name == "NonROtVIsualANDrAxlePivot")
                {
                    if (NonRot1)
                        NonRot2 = child;
                    else
                        NonRot1 = child;
                }

                if (NonRot1 && NonRot2)
                    break;
            }
            ModMain.PrintIfDebug("113333");
            Transform DriveshaftPivot = objective.transform.Find("RearSusp/RearAxle07/Pivotdriveshaft");
            ModMain.PrintIfDebug($"DPst: {DriveshaftPivot} | {Bone1} {Bone2} {objective.transform.Find("RearSusp/RearAxle07")} {objective.transform.Find("EngineTranny/DriveShaft07")}");
            ModMain.PrintIfDebug($"second line {DriveshaftPivot} {NonRot1} {NonRot2} {RigidModule} {RigidWrapper}");

            Bone1.doubleSided = true;
            Bone1.thisTransform = objective.transform.Find("RearSusp/RearAxle07");
            Bone1.targetTransform = NonRot2;
            Bone1.targetTransformB = NonRot1;

            Bone2.lookAtTarget = true;
            Bone2.stretchToTarget = true;
            Bone2.targetTransform = DriveshaftPivot;
            Bone2.thisTransform = objective.transform.Find("EngineTranny/DriveShaft07");

            RigidModule.bones = new List<Bone>();
            RigidModule.bones.Add(Bone1);
            RigidModule.bones.Add(Bone2);
            RigidWrapper.module = RigidModule;

            ModMain.PrintIfDebug("BONESFIXED!");
            */

            //DriveshaftPivot.localPosition = new Vector3(0.155f, -0.05f, 0.005f);

            ModMain.PrintIfDebug("133333");
            // Fixing crash sound
            try
            {
                objective.GetComponent<RVP.VehicleDamage>().crashSnd = objective.transform.Find("CrashSound").GetComponent<AudioSource>();
            }
            catch (Exception) { }
            // Fixing broken particle systems due to SMOKE component
            SMOKE smokeComponent = objective.transform.Find("ExhaustSmoke").GetComponent<SMOKE>();
            SMOKE smokeComponent2 = objective.transform.Find("ExhaustSmoke2").GetComponent<SMOKE>();
            smokeComponent.particleSystems = new List<ParticleSystem>();
            smokeComponent2.particleSystems = new List<ParticleSystem>();
            smokeComponent.particleSystems.Add(smokeComponent.GetComponent<ParticleSystem>());
            smokeComponent2.particleSystems.Add(smokeComponent2.GetComponent<ParticleSystem>());

            // This is called twice, fully intended.
            foreach (MyBoneSCR scr in objective.GetComponentsInChildren<MyBoneSCR>())
            {
                if (scr.thisTransform != null)
                {
                    if (!scr.thisTransform.root != objective.transform)
                    {
                        scr.thisTransform = scr.transform;
                    }
                }
            }
            ModMain.PrintIfDebug("11423342");
            // Fixing flipped LeafspringR due to mirrored model.
            //objective.transform.Find("RearSusp/RearAxle07/pivotRSpring").localEulerAngles = new Vector3(93f, 0f, 0f);
            foreach (MyBoneSCR scr in objective.GetComponentsInChildren<MyBoneSCR>())
            {
                // Control arms use all same formula for this.
                // The error reporter shows that there are issues with these but actually the targetTransform is calculated on runtime!
                // Same happens with shock absorbers & springs
                if (scr.transform.name == "LowerControlArmFL08" || scr.transform.name == "LowerControlArmFR08" || scr.transform.name == "UpperControlArmFL08" || scr.transform.name == "UpperControlArmFR08" || scr.transform.name == "F100Spring" || scr.transform.name == "ShockAbsorberF08" || scr.transform.name == "BrakeLine" || scr.transform.name == "F100Leafspring" || scr.transform.name == "ShockAbsorberR07" || scr.transform.name == "TieRod08")
                {
                    scr.LocalStrtetchTarget = scr.transform.GetChild(0);
                    scr.targetTransform = null;
                }
                else if (scr.transform.name == "HandbbrakeCable07")
                {
                    scr.LocalStrtetchTarget = scr.transform.parent.Find("DummyPivHbrak");
                }
                else if (scr.transform.name == "RearAxle07")
                {
                    Transform NonRot1 = null, NonRot2 = null;

                    foreach (Transform child in objective.transform.Find("RearSusp").GetComponentsInChildren<Transform>())
                    {
                        if (child.name == "NonROtVIsualANDrAxlePivot")
                        {
                            if (NonRot1)
                                NonRot2 = child;
                            else
                                NonRot1 = child;
                        }

                        if (NonRot1 && NonRot2)
                            break;
                    }

                    scr.targetTransform = NonRot2;
                    scr.targetTransformB = NonRot1;
                }
                else if (scr.transform.name == "DriveShaft07")
                {
                    scr.targetTransform = objective.transform.Find("RearSusp/RearAxle07/Pivotdriveshaft");
                }
            }
            ModMain.PrintIfDebug("11423432");
            // Fixing some broken transparents
            Transform FrontpanelR = objective.transform.Find("F100FenderFR");
            Transform FrontpanelL = objective.transform.Find("F100FenderFL");
            Transform MainFloorPanel = objective.transform.Find("F100MainFloorPanel");
            foreach (transparents transparent in objective.GetComponentsInChildren<transparents>())
            {
                if (transparent.name == "GearBox06")
                {
                    transparent.DEPENDANTS[1].dependant = MainFloorPanel.gameObject;
                }
                else if (transparent.name == "WiresMain06")
                {
                    transparent.DEPENDANTS[1].dependant = FrontpanelR.gameObject;
                }
                else if (transparent.name == "CrossmemmberF100")
                {
                    transparent.DEPENDANTS[0].dependant = FrontpanelL.gameObject;
                    transparent.DEPENDANTS[1].dependant = FrontpanelR.gameObject;
                }
            }

            // Step & InCarCollider
            /*Transform step = objective.transform.Find("Step");
            step.localPosition = new Vector3(0f, 0.274f, -0.1830f);
            
            BoxCollider bxStep = step.GetComponent<BoxCollider>();
            bxStep.center = new Vector3(0f, 0.6049201f, 0.4355f);
            bxStep.size = new Vector3(1.77f, 0.86967319f, 1.2158f);
            BoxCollider bx = objective.transform.Find("InCarCollider").GetComponent<BoxCollider>();
            bx.center = new Vector3(0f, -0.2947f, -1.54768f);
            bx.size = new Vector3(1.17f, 0.89f, 2.98096f);
            */
            ModMain.PrintIfDebug("11423432");
            // Stuff that is not on F350 (TODO ADD)
            vehController.centerOfMass = new Vector3(0.0157f, 0.2821f, -0.0036f);
            Brakes vehBrakes = new Brakes();
            vehBrakes.HandbrakeWorkingOrder = true;
            vehBrakes.StartedToBrake = true;
            vehBrakes.maxTorque = 700f;
            vehBrakes.brakeOffThrottleStrength = 0.1f;
            vehBrakes.MainProp = mainCarProps;
            vehBrakes.brakeWhileAsleep = true;
            vehBrakes.brakeWhileIdle = true;
            vehBrakes.HandbrakeDeployed = true;

            vehController.brakes = vehBrakes;
            foreach (WheelController wc in objective.GetComponentsInChildren<WheelController>())
            {
                wc.CustomSideSlip = 4f;
                wc.forwSlipCoef = 4f;
                wc.maxBrakeForce = 700f;
                wc.sideSlipCoef = 10f;
                wc.mass = 50f;
            }
        }

        internal static void AddRemainingPartsToCatalog()
        {
            string[] AllCarsList = null;
            foreach (GameObject part in PartManager.gameParts)
            {
                if (part.name == "SparkPlug")
                {
                    AllCarsList = part.GetComponent<Partinfo>().FitsToCar;
                    break;
                }
            }

            foreach (GameObject part in PartManager.gameParts)
            {
                Partinfo pi = part.GetComponent<Partinfo>();
                if (pi && pi.FitsToCar.Length == AllCarsList.Length - 1)
                {
                    pi.FitsToCar = AllCarsList;
                }

                if (part.name.Contains("tire15"))
                {
                    continue;
                }

                if (part.name.Contains("BrakePads"))
                {
                    if (pi.FitsToCar.Contains("Chad") && !pi.FitsToCar.Contains("F100"))
                    {
                        Array.Resize(ref pi.FitsToCar, pi.FitsToCar.Length + 1);
                        pi.FitsToCar[pi.FitsToCar.Length - 1] = "F100";
                    }
                }

                if (part.name == "LightBulbsH4" || part.name == "LightBubs")
                {
                    if (!pi.FitsToCar.Contains("F100"))
                    {
                        Array.Resize(ref pi.FitsToCar, pi.FitsToCar.Length + 1);
                        pi.FitsToCar[pi.FitsToCar.Length - 1] = "F100";
                    }
                }

                if (pi.RenamedPrefab == "ExhaustL07")
                {
                    CarProperties cp = part.GetComponent<CarProperties>();
                    if (cp.PrefabName == "ExhaustLstr")
                    {
                        Array.Resize(ref pi.FitsToCar, pi.FitsToCar.Length + 1);
                        pi.FitsToCar[pi.FitsToCar.Length - 1] = "F100";
                    }
                }

                if (pi.RenamedPrefab == "F100Leafspring" || pi.RenamedPrefab == "Pedals06" || (pi.RenamedPrefab == "TrailerHook06" || pi.name == "TrailerHook06"))
                {
                    bool isHere = false;
                    foreach (string s in pi.FitsToCar)
                    {
                        if (s == "F100")
                        {
                            isHere = true;
                            break;
                        }
                    }

                    if (!isHere)
                    {
                        Array.Resize(ref pi.FitsToCar, pi.FitsToCar.Length + 1);
                        pi.FitsToCar[pi.FitsToCar.Length - 1] = "F100";
                    }
                }

                if (pi.RenamedPrefab == "Diff07")
                {
                    bool isHere = false;
                    bool isChad = false;
                    foreach (string s in pi.FitsToCar)
                    {
                        if (s == "F100")
                        {
                            isHere = true;
                        }

                        if (s == "Chad")
                        {
                            isChad = true;
                        }

                        if (isChad && isHere)
                            break;
                    }

                    if (!isHere && isChad)
                    {
                        Array.Resize(ref pi.FitsToCar, pi.FitsToCar.Length + 1);
                        pi.FitsToCar[pi.FitsToCar.Length - 1] = "F100";
                    }
                }

                if (pi.FitsToCar.Contains("Chad") && pi.Engine)
                {
                    if (pi.name.Contains("Radiator") || pi.name.Contains("GasTank") || pi.name.StartsWith("Exhaust0") || pi.name.StartsWith("ExhaustL0") || pi.name.StartsWith("ExhaustR0"))
                        continue;

                    if (!pi.FitsToCar.Contains("F100"))
                    {
                        Array.Resize(ref pi.FitsToCar, pi.FitsToCar.Length + 1);
                        pi.FitsToCar[pi.FitsToCar.Length - 1] = "F100";
                    }
                }
            }
        }

        internal static void RecursiveCarBuilding(GameObject car, int engineType)
        {
            bool callAgain = false;
            foreach (transparents t in car.GetComponentsInChildren<transparents>())
            {
                ModMain.PrintIfDebug("[F100]: Checking for " + t.name);
                if (!IsTransparentEmpty(t))
                    continue;

                bool isParentCustom = t.transform.parent.GetComponent<SPL_Part>();
                if (t.transform.parent.tag == "Vehicle" || t.transform.parent.parent.tag == "Vehicle") // transparent -> car
                {
                    isParentCustom = true;
                    ModMain.PrintIfDebug("forcing " + t.name);
                }

                GameObject part = PartLookup(t.name, isParentCustom, engineType);

                if (!part)
                {
                    ModMain.PrintIfDebug("[F100]: Car building did not found part for " + t.name);
                    continue;
                }

                ModMain.PrintIfDebug("[F100]: Part found for " + t.name);

                CarBuilding.CopyPartIntoTransform(part, t.transform);
                callAgain = true;
            }

            if (callAgain)
            {
                RecursiveCarBuilding(car, engineType);
            }
        }

        internal static bool IsTransparentEmpty(transparents t)
        {
            foreach (Transform t2 in t.GetComponentsInChildren<Transform>())
            {
                if (t2 == t.transform)
                    continue;

                if (t2.GetComponent<CarProperties>() && !t2.name.ToLower().Contains("pivot") && !t2.name.ToLower().Contains("wheelcont"))
                    return false;

            }

            return true;
        }

        internal static GameObject PartLookup(string name, bool parentIsCustom, int engineType)
        {
            GameObject found = null;
            foreach (GameObject part in PartManager.gameParts)
            {
                if (part != null && part.name == name)
                {
                    found = part;
                    Partinfo pi = part.GetComponent<Partinfo>();
                    if (pi.RenamedPrefab == "Rim" || part.name == "Rim")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "Rim15Ch5")
                        {
                            found = null;
                            continue;
                        }
                    }

                    if (pi.RenamedPrefab == "HubF06" || part.name == "HubF06")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "HubF506")
                        {
                            found = null;
                            continue;
                        }
                    }

                    if (pi.RenamedPrefab == "CylinderBlock" || part.name == "CylinderBlock")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        switch (engineType)
                        {
                            case 1:
                                if (cp.PrefabName != "CylinderBlock")
                                {
                                    found = null;
                                    continue;
                                }
                                break;

                            case 2:
                                if (cp.PrefabName != "CylinderBlockV8")
                                {
                                    found = null;
                                    continue;
                                }
                                break;

                            case 3:
                                if (cp.PrefabName != "CylinderBlockI6")
                                {
                                    found = null;
                                    continue;
                                }
                                break;
                        }
                    }

                    if (pi.RenamedPrefab == "GearBox06" || part.name == "GearBox06")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (engineType != 1)
                        {
                            if (cp.PrefabName != "GearBox07")
                            {
                                found = null;
                                continue;
                            }
                        }
                    }

                    if (pi.RenamedPrefab == "Spacer" || pi.name == "Spacer")
                    {
                        found = null;
                        continue;
                    }

                    if (pi.RenamedPrefab == "F100MirrorSmallL" || pi.RenamedPrefab == "F100MirrorSmallR" || pi.name == "F100MirrorSmallL" || pi.name == "F100MirrorSmallR")
                    {
                        found = null;
                        continue;
                    }

                    if (pi.RenamedPrefab == "SteeringWheel06" || part.name == "SteeringWheel06")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (engineType != 1)
                        {
                            if (cp.PrefabName != "FJG_F100Mod_SteeringWheel")
                            {
                                found = null;
                                continue;
                            }
                        }
                    }

                    if (pi.RenamedPrefab == "SpeedoDigital" || part.name == "SpeedoDigital")
                    {
                        found = null;
                        continue;
                    }

                    if (pi.RenamedPrefab == "DriveShaft07" || part.name == "DriveShaft07")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "FJG_F100Mod_Driveshaft")
                        {
                            found = null;
                            continue;
                        }
                    }

                    if (pi.RenamedPrefab == "GearStick06" || part.name == "GearStick06")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "FJG_F100Mod_InteriorShifter")
                        {
                            found = null;
                            continue;
                        }
                    }
                    if (pi.RenamedPrefab == "SeatFL06" || part.name == "SeatFL06")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "FJG_F100Mod_LongSeat")
                        {
                            found = null;
                            continue;
                        }
                    }
                    if (pi.RenamedPrefab == "SeatFR06" || part.name == "SeatFR06")
                    {
                        found = null;
                        continue;
                    }

                    if (pi.RenamedPrefab == "F100TransferCase" || part.name == "F100TransferCase")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "FJG_F100Mod_4WDTransfercase")
                        {
                            found = null;
                            continue;
                        }
                    }

                    if (pi.RenamedPrefab == "ExhaustR07" || part.name == "ExhaustR07")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "FJG_F100Mod_ExhaustRight")
                        {
                            found = null;
                            continue;
                        }
                    }
                    if (pi.RenamedPrefab == "ExhaustL07" || part.name == "ExhaustL07")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "FJG_F100Mod_ExhaustLeft")
                        {
                            found = null;
                            continue;
                        }
                    }

                    if (pi.RenamedPrefab == "tire15" || part.name == "tire15")
                    {
                        CarProperties cp = found.GetComponent<CarProperties>();
                        if (cp.PrefabName != "tire15HF")
                        {
                            found = null;
                            continue;
                        }
                    }

                    if (pi.RenamedPrefab == "F100BedCover" || pi.RenamedPrefab == "FloorTrailer" || pi.name == "FloorTrailer")
                    {
                        found = null;
                        continue;
                    }

                    if (pi.RenamedPrefab == "TaxiSign")
                    {
                        found = null;
                        continue;
                    }

                    if (found.GetComponent<SPL_Part>() && !parentIsCustom)
                    {
                        ModMain.PrintIfDebug($"FOUND {found} BUT PARENT IS NOT CUSTOM");
                        found = null;
                        continue;
                    }

                    if (found.name.Contains("JackStand") || found.name.Contains("TrailerHook") || pi.RenamedPrefab == "F100FlatbedFrame" || pi.RenamedPrefab == "F100RearLightHolder")
                    {
                        found = null;
                        continue;
                    }

                    AddPartToCatalog(pi);
                    break;
                }
            }

            if (!found)
            {
                foreach (GameObject part in PartManager.gameParts)
                {
                    if (part != null)
                    {
                        Partinfo pi = part.GetComponent<Partinfo>();
                        if (pi)
                        {
                            if (pi.RenamedPrefab == name)
                            {
                                found = part;
                                if (pi.RenamedPrefab == "Rim" || part.name == "Rim")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "Rim15Ch5")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }

                                if (pi.RenamedPrefab == "HubF06" || part.name == "HubF06")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "HubF506")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }

                                if (pi.RenamedPrefab == "TaxiSign")
                                {
                                    found = null;
                                    continue;
                                }

                                if (pi.RenamedPrefab == "CylinderBlock" || part.name == "CylinderBlock")
                                {
                                    ModMain.PrintIfDebug(engineType + "FOUNDDD " + pi);
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    switch (engineType)
                                    {
                                        case 1:
                                            if (cp.PrefabName != "CylinderBlock18")
                                            {
                                                found = null;
                                                continue;
                                            }
                                            break;

                                        case 2:
                                            if (cp.PrefabName != "CylinderBlockV8")
                                            {
                                                found = null;
                                                continue;
                                            }
                                            break;

                                        case 3:
                                            if (cp.PrefabName != "CylinderBlockI6")
                                            {
                                                found = null;
                                                continue;
                                            }
                                            break;
                                    }
                                }
                                if (pi.RenamedPrefab == "DriveShaft07" || part.name == "DriveShaft07")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "FJG_F100Mod_Driveshaft")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }

                                if (pi.RenamedPrefab == "GearBox06" || part.name == "GearBox06")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (engineType != 1)
                                    {
                                        if (cp.PrefabName != "GearBox07")
                                        {
                                            found = null;
                                            continue;
                                        }
                                    }
                                }

                                if (pi.RenamedPrefab == "Spacer" || pi.name == "Spacer")
                                {
                                    found = null;
                                    continue;
                                }

                                if (pi.RenamedPrefab == "SpeedoDigital" || part.name == "SpeedoDigital")
                                {
                                    found = null;
                                    continue;
                                }

                                if (pi.RenamedPrefab == "SteeringWheel06" || part.name == "SteeringWheel06")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (engineType != 1)
                                    {
                                        if (cp.PrefabName != "FJG_F100Mod_SteeringWheel")
                                        {
                                            found = null;
                                            continue;
                                        }
                                    }
                                }

                                if (pi.RenamedPrefab == "F100TransferCase" || part.name == "F100TransferCase")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "FJG_F100Mod_4WDTransfercase")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }

                                if (pi.RenamedPrefab == "F100MirrorSmallL" || pi.RenamedPrefab == "F100MirrorSmallR" || pi.name == "F100MirrorSmallL" || pi.name == "F100MirrorSmallR")
                                {
                                    found = null;
                                    continue;
                                }

                                if (pi.RenamedPrefab == "ExhaustR07" || part.name == "ExhaustR07")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "FJG_F100Mod_ExhaustRight")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }
                                if (pi.RenamedPrefab == "ExhaustL07" || part.name == "ExhaustL07")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "FJG_F100Mod_ExhaustLeft")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }
                                if (pi.RenamedPrefab == "GearStick06" || part.name == "GearStick06")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "FJG_F100Mod_InteriorShifter")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }
                                if (pi.RenamedPrefab == "SeatFL06" || part.name == "SeatFL06")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "FJG_F100Mod_LongSeat")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }
                                if (pi.RenamedPrefab == "SeatFR06" || part.name == "SeatFR06")
                                {
                                    found = null;
                                    continue;
                                }

                                if (pi.RenamedPrefab == "tire15" || part.name == "tire15")
                                {
                                    CarProperties cp = found.GetComponent<CarProperties>();
                                    if (cp.PrefabName != "tire15HF")
                                    {
                                        found = null;
                                        continue;
                                    }
                                }

                                if (pi.RenamedPrefab == "F100BedCover" || pi.RenamedPrefab == "FloorTrailer" || pi.name == "FloorTrailer")
                                {
                                    found = null;
                                    continue;
                                }

                                if (found.GetComponent<SPL_Part>() && !parentIsCustom && !found.GetComponent<CarProperties>().PrefabName.Contains("F100Mod_Exhaust") && found.GetComponent<CarProperties>().PrefabName != "FJG_F100Mod_Spring")
                                {
                                    ModMain.PrintIfDebug($"FOUND {found} BUT PARENT IS NOT CUSTOM");
                                    found = null;
                                    continue;
                                }

                                if (found.name.Contains("JackStand") || found.name.Contains("TrailerHook") || pi.RenamedPrefab == "F100FlatbedFrame" || pi.RenamedPrefab == "F100RearLightHolder")
                                {
                                    found = null;
                                    continue;
                                }

                                AddPartToCatalog(pi);
                                break;
                            }
                        }
                    }
                }
            }

            return found;
        }

        private static void AddPartToCatalog(Partinfo pi)
        {
            if (pi.RenamedPrefab == "Rim")
                return;

            foreach (string s in pi.FitsToCar)
            {
                if (s == "F100")
                {
                    return;
                }
            }

            Array.Resize(ref pi.FitsToCar, pi.FitsToCar.Length + 1);
            pi.FitsToCar[pi.FitsToCar.Length - 1] = "F100";
        }

        // Adapted painting system
        internal static void EnableFullSupport(GameObject Prefab)
        {
            Prefab.AddComponent<P3dPaintable>();

            // Material checks
            Renderer prefabRenderer = Prefab.GetComponent<Renderer>();
            int l2Material_index = 0, alphaMaterial_index = 1;

            // Now we create our painting components.
            P3dMaterialCloner materialCloner_l2 = Prefab.AddComponent<P3dMaterialCloner>();
            P3dMaterialCloner materialCloner_paint = Prefab.AddComponent<P3dMaterialCloner>();

            P3dPaintableTexture paintableTexture_colorMap = Prefab.AddComponent<P3dPaintableTexture>();
            P3dPaintableTexture paintableTexture_rust = Prefab.AddComponent<P3dPaintableTexture>();
            P3dPaintableTexture paintableTexture_dirt = Prefab.AddComponent<P3dPaintableTexture>();
            P3dPaintableTexture paintableTexture_grungeMap = Prefab.AddComponent<P3dPaintableTexture>();

            P3dChangeCounter counter_dirt = Prefab.AddComponent<P3dChangeCounter>();
            P3dChangeCounter counter_rust = Prefab.AddComponent<P3dChangeCounter>();
            P3dChangeCounter counter_colorMap = Prefab.AddComponent<P3dChangeCounter>();

            P3dSlot p3dSlot_rustDirt = new P3dSlot(l2Material_index, "_L2MetallicRustDustSmoothness");
            P3dSlot p3dSlot_colorMap = new P3dSlot(l2Material_index, "_L2ColorMap");
            P3dSlot p3dSlot_grungeMap = new P3dSlot(l2Material_index, "_GrungeMap");
            P3dSlot p3dSlot_dirt = new P3dSlot(alphaMaterial_index, "_MainTex");

            // Setting up the components
            // Check for original mesh
            OriginalMesh orMesh = Prefab.GetComponent<OriginalMesh>();
            Mesh meshToUse = null;
            if (orMesh)
            {
                meshToUse = orMesh.Mesh;
            }
            // Material cloner
            materialCloner_l2.Index = l2Material_index;
            materialCloner_paint.Index = alphaMaterial_index;

            // Paintable textures
            paintableTexture_colorMap.Slot = p3dSlot_colorMap;
            paintableTexture_colorMap.Width = 1024;
            paintableTexture_colorMap.Height = 1024;

            paintableTexture_grungeMap.Slot = p3dSlot_grungeMap;
            paintableTexture_grungeMap.Group = 100;

            paintableTexture_rust.Slot = p3dSlot_rustDirt;
            paintableTexture_rust.Group = 100;

            paintableTexture_dirt.Slot = p3dSlot_dirt;
            paintableTexture_dirt.Group = 5;

            // Counters
            counter_rust.PaintableTexture = paintableTexture_rust;
            counter_rust.Threshold = 0.5f;
            counter_rust.enabled = false;
            counter_rust.Color = new Color(0, 0, 0, 1f);
            counter_rust.DownsampleSteps = 5;
            counter_rust.MaskMesh = meshToUse;

            counter_colorMap.PaintableTexture = paintableTexture_colorMap;
            counter_colorMap.Threshold = 0.1f;
            counter_colorMap.enabled = false;
            counter_colorMap.DownsampleSteps = 5;
            counter_colorMap.MaskMesh = meshToUse;

            counter_dirt.PaintableTexture = paintableTexture_dirt;
            counter_dirt.Threshold = 0.7f;
            counter_dirt.enabled = false;
            counter_dirt.Color = new Color(0.219f, 0.219f, 0.219f, 0f);
            counter_dirt.DownsampleSteps = 5;
            counter_dirt.MaskMesh = meshToUse;
        }

        internal static void EnableDirtOnly(GameObject Prefab)
        {
            Prefab.AddComponent<P3dPaintable>();

            // Material checks
            int alphaMaterial_index = 1;

            // Setting up the components

            // Material cloner
            P3dMaterialCloner materialCloner_dirt = Prefab.AddComponent<P3dMaterialCloner>();
            materialCloner_dirt.Index = alphaMaterial_index;

            // Paintable texture
            P3dPaintableTexture paintableTexture_dirt = Prefab.AddComponent<P3dPaintableTexture>();
            paintableTexture_dirt.Slot = new P3dSlot(alphaMaterial_index, "_MainTex");
            paintableTexture_dirt.Group = 5;

            // Check for original mesh
            OriginalMesh orMesh = Prefab.GetComponent<OriginalMesh>();
            Mesh meshToUse = null;
            if (orMesh)
            {
                meshToUse = orMesh.Mesh;
            }

            // Counter
            P3dChangeCounter counter_dirt = Prefab.AddComponent<P3dChangeCounter>();
            counter_dirt.PaintableTexture = paintableTexture_dirt;
            counter_dirt.Threshold = 0.7f;
            counter_dirt.enabled = false;
            counter_dirt.Color = new Color(0.219f, 0.219f, 0.219f, 0f);
            counter_dirt.DownsampleSteps = 5;
            counter_dirt.MaskMesh = meshToUse;
        }


        public static void SetPrivatePropertyValue<T>(T obj, string propertyName, object newValue)
        {
            // add a check here that the object obj and propertyName string are not null
            foreach (FieldInfo fi in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fi.Name.ToLower().Contains(propertyName.ToLower()))
                {
                    fi.SetValue(obj, newValue);
                    break;
                }
            }
        }
    }
}
