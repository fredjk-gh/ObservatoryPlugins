﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observatory.Framework.Files;
using Observatory.Framework.Files.Journal;

namespace com.github.fredjk_gh.ObservatoryHelm
{
    class CommanderData
    {
        private NavRouteFile _lastNavRoute = null;

        public CommanderData(string commander)
        {
            Commander = commander;
            Scans = new();
        }

        public string Commander { get; }
        public double FuelCapacity { get; set; }
        public double FuelRemaining { get; set; }
        public double MaxFuelPerJump { get; set; }
        public string? CurrentSystem { get; set; }
        public double DistanceTravelled { get; set; }
        public long JumpsCompleted { get; set; }
        public long DockedCarrierJumpsCompleted { get; set; }
        public StartJump LastStartJumpEvent { get; set; }
        public FSDJump LastJumpEvent { get; set; }
        public NavRouteFile LastNavRoute
        {
            get => _lastNavRoute;
            set
            {
                _lastNavRoute = value;
                JumpsRemainingInRoute = 0;
                if (_lastNavRoute != null)
                {
                    Destination = value.Route[value.Route.Count - 1].StarSystem;
                    JumpsRemainingInRoute = value.Route.Count - 1; // Route includes origin system.
                }
            }
        }
        public string Destination { get; set; }
        public int JumpsRemainingInRoute { get; set; }
        public string? NeutronPrimarySystem { get; set; }
        public Scan? ScoopableSecondaryCandidateScan { get; set; }
        public string? NeutronPrimarySystemNotified { get; set; }
        public string? FuelWarningNotifiedSystem { get; set; }
        public bool IsDockedOnCarrier { get; set; }
        public Dictionary<string, Scan> Scans { get; private set; }
        public bool AllBodiesFound { get; set; }
        public bool UndiscoveredSystem { get; set; }
        public bool? IsInSuppressionZone(HelmSettings settings, FSDJump jump = null)
        {
            jump ??= LastJumpEvent;
            if (jump == null) return null;

            if (jump.StarPos == null) return null;

            // Yeah -- FDev done goofed up, these should be all be &&... But anyway.
            var radius = settings.SuppressionZoneRadiusLy;
            if (Math.Abs(jump.StarPos.y) < radius && (Math.Abs(jump.StarPos.x) < radius || Math.Abs(jump.StarPos.z) < radius)) return true;

            return false;
        }

        public bool? IsInBubble(FSDJump jump = null)
        {
            jump ??= LastJumpEvent;
            if (jump == null) return null;

            if (jump.StarPos == null) return null;

            // The bubble is the area ~250 Ly around Sol. Some may say it's larger.
            var radius = 250;
            if (Math.Abs(jump.StarPos.y) < radius && Math.Abs(jump.StarPos.x) < radius && Math.Abs(jump.StarPos.z) < radius) return true;

            return false;
        }

        public void SystemReset(string systemName, double fuelLevel, double jumpDist)
        {
            Scans.Clear();
            CurrentSystem = systemName;
            FuelRemaining = fuelLevel;
            DistanceTravelled += jumpDist;
            JumpsCompleted++;
            AllBodiesFound = false;
            UndiscoveredSystem = false;
            IsDockedOnCarrier = false;
        }

        public void SystemResetDockedOnCarrier(string systemName, double distanceTravelled = 0)
        {
            Scans.Clear();
            CurrentSystem = systemName;
            IsDockedOnCarrier = true;
            DockedCarrierJumpsCompleted++;
            DistanceTravelled += distanceTravelled;
            AllBodiesFound = false;
            UndiscoveredSystem = false;
        }

        public void SessionReset(double fuelCapacity, double fuelLevel)
        {
            FuelCapacity = fuelCapacity;
            FuelRemaining = fuelLevel;
            JumpsRemainingInRoute = 0;
            DistanceTravelled = 0;
            JumpsCompleted = 0;
            DockedCarrierJumpsCompleted = 0;
            LastNavRoute = null;
        }
    }
}
