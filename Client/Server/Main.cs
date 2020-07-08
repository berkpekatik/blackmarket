using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Server
{
    public class Main : BaseScript
    {
        private static CurrentMission mission = new CurrentMission();
        public Main()
        {
            ResetConfig();
            Debug.WriteLine("############################");
            Debug.WriteLine("#                          #");
            Debug.WriteLine("#          vNoisy          #");
            Debug.WriteLine("#                          #");
            Debug.WriteLine("############################");
            EventHandlers["blackmarket:savePayload"] += new Action<int, int, int>(SaveConfig);
            EventHandlers["blackmarket:readPayload"] += new Action(ReadConfig);
            EventHandlers["blackmarket:restartPayload"] += new Action(ResetConfig);
        }

        private void ResetConfig()
        {
            File.WriteAllText(@GetResourcePath(GetCurrentResourceName()) + "//mission.json", "{,}");
        }
        private void SaveConfig(int veh, int vehDriver, int currentMission)
        {
            mission.Driver = vehDriver;
            mission.Mission = currentMission;
            mission.Vehicle = veh;
            File.WriteAllText(@GetResourcePath(GetCurrentResourceName()) + "//mission.json", JsonConvert.SerializeObject(mission));
        }
        private void ReadConfig()
        {
            var data = File.ReadAllText(@GetResourcePath(GetCurrentResourceName()) + "//mission.json");
            TriggerClientEvent("blackmarket:addMissionFile", data);
        }
    }
}
