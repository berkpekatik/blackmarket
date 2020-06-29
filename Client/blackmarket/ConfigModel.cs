using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ConfigModel
    {
        public string[] Identifiers { get; set; }
        public string CarModel { get; set; }
        public string DriverModel { get; set; }
        public int Timeout { get; set; }
        public string Msg { get; set; }
        public List<Vector3> Coords { get; set; }
        public Vector3 SpawnCoords { get; set; }
    }
}
