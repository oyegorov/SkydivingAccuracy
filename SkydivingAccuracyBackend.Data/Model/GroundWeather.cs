﻿using System;

namespace SkydivingAccuracyBackend.Data.Model
{
    public struct GroundWeather
    {
        public MetarStation MetarStation { get; set; }

        public double DistanceToStation { get; set; }

        public int Temperature { get; set; }

        public int Dewpoint { get; set; }

        public int WindSpeed { get; set; }

        public int WindGust { get; set; }

        public int WindHeading { get; set; }

        public double Pressure { get; set; }

        public double Visibility { get; set; }

        public SkyCondition[] SkyConditions { get; set; }
    }
}
