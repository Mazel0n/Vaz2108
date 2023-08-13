using ModUI.Settings;
using PaintIn3D;
using SimplePartLoader;
using SimplePartLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static ModUI.Settings.ModSettings;
using static ModUI.Settings.ModSettings.Dropdown;

namespace Vaz_2108
{
    public class ModMain : Mod, IModSettings
    {
        // Looking for docs? https://fedearre.github.io/my-garage-modding-docs/
        public override string ID => "Vaz2108VehicleMod";
        public override string Name => "Vaz2108";
        public override string Author => "RAMB";
        public override string Version => "1.0.0"; // remember to change on assemblyinfo
        public string TestVersionIs = "v1.0.0-test1";
        public static bool TestVersion = false;

        public static GameObject VehiclePrefab_i4f, EmptyVehiclePrefab, TransparentsToAdd;

        public static Material BlackMaterial, RustMaterial;

        public static Unity.Mathematics.Random random = new Unity.Mathematics.Random();
        
        // Body
        public static Part MainFloorPanel, FirewallInterior, FirewallExterior, FloorPanelL, FloorPanelR, FrontpanelL, FrontpanelR, Frontpanel;
        public static Part FrameRailL, FrameRailR, FrameFrontSupport, FrameMiddleSupport, FrameRearSupport, FrameBedSupport, MiddleBrace, RearFrameBrace;
        public static Part FloorBed, WheelWellL, WheelWellR, QuarterpanelRL, QuarterpanelRR, CabinWall, Tailgate;
        public static Part TailgateHolder;

        // Suspension
        public static Part SteeringColumn, SteeringLink, SteeringArm;
        public static Part Spring, SpringLow, SpringHigh, /**/ HalfAxle;
        public static Mesh HandbrakeCable;

        // Engine
        public static Part FuelTank, Radiator, ExhaustI4f;
        public static Mesh FuelLine;

        // Brakes
        public static GameObject BrakeLineMesh;

        // Interior
        public static Part Dashboard;

        // Cabin
        public static Part RearWall, Roof, Hood, QuarterpanelFR, QuarterpanelFL, Grill, FrontValanace, BumperF, BumperR, HeadlightCoverL, HeadlightCoverR;
        public static Part BlinkerL, BlinkerR, DoorPanelL, DoorL, DoorR, DoorPanelR;
        public static Part RockerL, RockerR;
        public static Part Headliner, MirrorL, MirrorR, Antenna, RearMirror, CabinLights, WiperMotor;
        public static Part Windshield, RearWindow, WindowLiftFR, WindowR, WindowLiftFL, WindowL, SunvisorL, SunvisorR, RearLightL, RearLightR;
        public static Part BedCover, SteeringWheel, ExtraGrill;
        public static Part SmallMirrorL, SmallMirrorR;

        ModInstance ThisMod;

        Material boltMat = null;

        public static bool HighQualityPaintEnabled = true;

        public ModMain()
        {
            ModMain.PrintIfDebug("VAZ 2108 - version is " + TestVersionIs);
            if (File.Exists("./Mods/vaz2108paint.txt"))
                HighQualityPaintEnabled = false;

            Debug.Log("VAZ 2108 is now loading :)");
            Debug.Log("Done by RANB with Federico Arredondo support");

            random.InitState((uint)DateTime.Now.Ticks);

            ThisMod = ModUtils.RegisterMod(this);

            //ThisMod.EnableEarlyAccessCheck();
            //ThisMod.GenerateThumbnails();

            ThisMod.Settings.PreciseCloning = true;
            ThisMod.Settings.EnableImmediateDestroys = true;
            ThisMod.Settings.PaintResolution = HighQualityPaintEnabled ? PaintingSystem.PartPaintResolution.High : PaintingSystem.PartPaintResolution.Low;
            ThisMod.Settings.UseBackfaceShader = true;

            //CarBuilding.ENABLE_CARBUILDING_DEBUG = false;

            ModUtils.RegisterCarCategory("VAZ 2108");
            //SPL.DEVELOPER_LOG = true;
            //SPL.ENABLE_SAVE_DISSASAMBLE = true;

        }

        public void PartSetup()
        {

        }

        public void PaintSetup2(Part p)
        {

        }

        public void FixMaterials(Part p, bool doubleSide = false)
        {

        }

        public void SetBlackMaterial(Part p)
        {

        }

        public void SetBlackMaterial(Part p, int index)
        {

        }

        public void SetChildChrome(Part p, Material m = null)
        {

        }
        public void SetChildBlack(Part p)
        {

        }
        public void PartFixes(Part p)
        {

        }
        public override void OnLoad()
        {

        }

        public void SetupPainting(Part p, bool dirt = true)
        {
            PaintingSystem.SetMaterialsForObject(p, 2, 0, 1);
            p.EnablePartPainting(dirt ? PaintingSystem.Types.FullPaintingSupport : PaintingSystem.Types.OnlyPaintAndRust);
        }

        public static void PrintIfDebug(string s)
        {
            if (TestVersion)
                Debug.Log(s);
        }

        Dropdown PaintResolutionSetting;
        public void CreateModSettings(ModUI.Settings.ModSettings modSettings)
        {
            PaintResolutionSetting = modSettings.AddDropdown("Paint res.", "VAZ2108_paintResDropdown", 1, UpdatePaintRes, new Option[] { Option.Create("Very low"), Option.Create("Low"), Option.Create("Medium"), Option.Create("High") });
        }

        public void UpdatePaintRes(int value)
        {
            if (Dashboard.Prefab == null) // any prefab gen part...
                return;

            foreach (Part p in ThisMod.Parts)
            {
                if (p.CarProps.Paintable)
                {
                    P3dPaintableTexture tex = p.Prefab.GetComponent<P3dPaintableTexture>();
                    if (tex)
                    {
                        tex.Width = GetValueByIndex(value);
                        tex.Height = GetValueByIndex(value);
                    }
                }
            }
        }

        public int GetValueByIndex(int index)
        {
            switch (index)
            {
                case 0: return 256;
                case 1: return 512;
                case 2: return 1024;
                case 3: return 2048;
            }

            return 512;
        }

        public void ModSettingsLoaded() { }
    }
}