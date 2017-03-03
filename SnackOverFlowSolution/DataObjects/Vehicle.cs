﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public string VIN { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Mileage { get; set; }
        public string Year { get; set; }
        public string Color { get; set; }
        public bool Active { get; set; }
        public DateTime? LatestRepair { get; set; }
        public int? LastDriver { get; set; }
        public string VehicleTypeID { get; set; }
    }
}