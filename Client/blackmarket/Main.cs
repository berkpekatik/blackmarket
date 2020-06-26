using CitizenFX.Core;
using CitizenFX.Core.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using Debug = CitizenFX.Core.Debug;

namespace Client
{
    public class Main : BaseScript
    {
        private static ConfigModel config = new ConfigModel();
        private static bool pause = false;
        private static bool pressed = false;
        private static int driver;
        private static int vehicle;
        private static int currentMis;
        public Main()
        {
            EventHandlers["blackmarket:startjob"] += new Action(Start);
            EventHandlers["blackmarket:gotodriver"] += new Action(GotoDriver);
            RegisterCommand("startjob", new Action(sJob), false);
            RegisterCommand("gotodriver", new Action(gDriver), false);
            var data = LoadResourceFile(GetCurrentResourceName(), "config.json");
            try
            {
                config = JsonConvert.DeserializeObject<ConfigModel>(data);
            }
            catch
            {
                Debug.WriteLine("Config File cannot read!");
            }
            
        }

        private void gDriver()
        {
            TriggerServerEvent("blackmarket:gotodriverserver");
           
        }

        private void sJob()
        {
            TriggerServerEvent("blackmarket:startjobserver");
        }

        private void GotoDriver()
        {
            Game.PlayerPed.SetIntoVehicle(new Vehicle(vehicle), VehicleSeat.Passenger);
        }

        private async Task OnMarketTick()
        {
            var trunkPos = GetWorldPositionOfEntityBone(vehicle, GetEntityBoneIndexByName(vehicle, "taillight_l"));
            if (!pause && Game.PlayerPed.Position.DistanceToSquared2D(trunkPos) > 1.8f && pressed)
            {
                pressed = false;
            }
            if (pause && Game.PlayerPed.Position.DistanceToSquared2D(trunkPos) < 1.8f && IsControlJustPressed(0, 46))
            {
                TriggerEvent("esx_blackmarket:openMenu", 1);
                pressed = true;
            }
            else if (pause && Game.PlayerPed.Position.DistanceToSquared2D(trunkPos) < 1.8f && !pressed)
            {
                SetTextComponentFormat("STRING");
                AddTextComponentString(config.Msg);
                DisplayHelpTextFromStringLabel(0, false, true, -1);
            }
        }

        private async void Start()
        {
            await Delay(500);
            Tick += OnTick;
            Tick += OnMarketTick;
            await Delay(5000);
            await LoadModel((uint)GetHashKey(config.CarModel));
            var veh = await World.CreateVehicle(config.CarModel, config.SpawnCoords);
            await LoadModel((uint)GetHashKey(config.DriverModel));
            var vehDriver = await World.CreatePed(config.DriverModel, config.SpawnCoords);
            vehDriver.BlockPermanentEvents = true;
            driver = vehDriver.Handle;
            vehicle = veh.Handle;
            veh.LockStatus = VehicleLockStatus.LockedForPlayer;
            GodMode();
            await Delay(100);
            vehDriver.Task.WarpIntoVehicle(veh, VehicleSeat.Driver);
            await Delay(50);
            vehDriver.Task.DriveTo(veh, config.Coords[currentMis], 2f, 15f, 317);
        }
        public static async Task<bool> LoadModel(uint model)
        {
            if (!IsModelInCdimage(model))
            {
                //Debug.WriteLine($"Invalid model {model}");
                return false;
            }
            RequestModel(model);
            while (!HasModelLoaded(model))
            {
                //Debug.WriteLine($"Waiting for model {model}");
                await Delay(100);
            }
            return true;
        }

        private async Task OnTick()
        {
            if (currentMis > config.Coords.Count - 1)
            {
                currentMis = 0;
            }
            var van = GetEntityCoords(vehicle, true);
            if (!pause && van.DistanceToSquared2D(config.Coords[currentMis]) < 2f)
            {
                var veh = new Vehicle(vehicle);
                await Delay(1500);
                var ped = new Ped(driver);
                await Delay(1500);

                while (ped.IsInVehicle())
                {
                    ped.Task.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    await Delay(200);
                }
                await Delay(200);
                ped.Task.ClearAllImmediately();
                await Delay(150);
                ped.Task.GoTo(GetWorldPositionOfEntityBone(vehicle, GetEntityBoneIndexByName(vehicle, "taillight_l")));
                await Delay(900);
                if (veh.ClassType == VehicleClass.Vans)
                {
                    SetVehicleDoorOpen(vehicle, 3, false, false);
                    SetVehicleDoorOpen(vehicle, 2, false, false);
                }
                else
                {
                    SetVehicleDoorOpen(vehicle, 5, false, false);
                }
                await Delay(150);
                pause = true;
                await Delay(150);
                await Delay(config.Timeout);//bekleme süresi
                pause = false;
                await Delay(150);
                currentMis++;
                if (currentMis > config.Coords.Count - 1)
                {
                    currentMis = 0;
                }
                await Delay(800);
                ped.Task.DriveTo(veh, config.Coords[currentMis], 2f, 15f, 317);
                await Delay(150);
                while (!ped.IsInVehicle())
                {
                    await Delay(150);
                }
                await Delay(250);
                SetVehicleDoorsShut(veh.Handle, false);
            }
        }

        private void GodMode()
        {
            SetEntityInvincible(vehicle, true);
            SetEntityInvincible(driver, true);
        }
    }
}
