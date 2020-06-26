using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Main : BaseScript
    {
        private static dynamic ESX;
        public Main()
        {

            Debug.WriteLine("############################");
            Debug.WriteLine("#                          #");
            Debug.WriteLine("#          vNoisy          #");
            Debug.WriteLine("#                          #");
            Debug.WriteLine("############################");
            Tick += onTick;
            EventHandlers["blackmarket:gotodriverserver"] += new Action<Player>(GotoDriver);
            EventHandlers["blackmarket:startjobserver"] += new Action<Player>(StartJob);
        }
        private void StartJob([FromSource] Player source)
        {
            var xPlayer = ESX.GetPlayerFromId(source.Handle);
            if ((string)xPlayer.getGroup() == "admin" || (string)xPlayer.getGroup() == "superadmin")
            {
                TriggerClientEvent("blackmarket:startjob");
            }
        }

        private void GotoDriver([FromSource] Player source)
        {
            var xPlayer = ESX.GetPlayerFromId(source.Handle);
            if ((string)xPlayer.getGroup() == "admin" || (string)xPlayer.getGroup() == "superadmin")
            {
                TriggerClientEvent("blackmarket:gotodriver");
            }

        }

        private async Task onTick()
        {
            while (ESX == null)
            {
                TriggerEvent("esx:getSharedObject", new object[] { new Action<dynamic>(esx => {
                    ESX = esx;
                })});
                await Delay(1000);
            }
        }
    }
}
