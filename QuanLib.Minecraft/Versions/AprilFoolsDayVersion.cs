using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public class AprilFoolsDayVersion : MinecraftVersion
    {
        private static readonly string[] VERSIONS = [
            "2.0_purple",           //2013
            "2.0_red",              //2013
            "2.0_blue",             //2013
            "15w14a",               //2015
            "1.RV-Pre1",            //2016
            "3D Shareware v1.34",   //2019
            "20w14infinite",        //2020
            "22w13oneblockatatime", //2022
            "23w13a_or_b",          //2023
            "24w14potato",          //2024
            "25w14craftmine"        //2025
        ];

        private static readonly int[] YEARS = [
            2013,
            2013,
            2013,
            2015,
            2016,
            2019,
            2020,
            2022,
            2023,
            2024,
            2025
        ];

        public AprilFoolsDayVersion(string versionNumber, DateTime releaseTime) : base(versionNumber, VersionType.AprilFoolsDay, releaseTime)
        {
            int index = Array.IndexOf(VERSIONS, versionNumber);
            if (index < 0)
                throw new ArgumentException("Invalid April Fools' Day version number.", nameof(versionNumber));

            Year = YEARS[index];
        }

        public override bool IsReleaseVersion => false;

        public override bool IsSnapshotVersion => true;

        public override bool IsAncientVersion => false;

        public int Year { get; }

        public static bool IsAprilFoolsDayVersion(string versionNumber)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionNumber, nameof(versionNumber));

            return VERSIONS.Contains(versionNumber);
        }

        public static string[] GetVersions()
        {
            string[] result = new string[VERSIONS.Length];
            VERSIONS.CopyTo(result, 0);
            return result;
        }
    }
}
