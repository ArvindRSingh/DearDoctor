using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public enum AgeGroup
    {
        NewBorn,
        YoungChild,
        OldChild,
        Adolesent,
        YoungAdult,
        AdultIn30s,
        AdultIn40s,
        AdultIn50s,
        AdultIn60s,
        Senior

    }
    public enum Region
    {
        NorthAmerica,
        WesternEurope,
        EasternEurope,
        NorthAfrica,
        EastAfrica,
        WestAfrica,
        SouthAfrica,
        CentralAfrica,
        EastAsia,
        SouthAsia,
        SouthEastAsia,
        Australia,
        MiddleEast,
        Caribbean

    }

    public enum Gender
    {
        Male,
        Female

    }
    public class SymptomEntity : TableEntity
    {
        public static string GetRowKey(AgeGroup? ageGroup, Gender? gender, Region? region)
        {
            return string.Concat(
                ageGroup.HasValue ? Enum.GetName(typeof(AgeGroup), ageGroup.Value) : "",
                gender.HasValue ? Enum.GetName(typeof(Gender), gender.Value) : "",
                region.HasValue ? Enum.GetName(typeof(Region), region.Value) : ""
                );
        }

        public SymptomEntity(string symptom, AgeGroup ageGroup, Gender gender, Region region)
        {
            this.PartitionKey = symptom;
            this.RowKey = SymptomEntity.GetRowKey(ageGroup, gender, region);
        }
        public SymptomEntity() { }
        public Gender Gender { get; set; }
        public Region Region { get; set; }
        public string LikelyDiagnosis { get; set; }

    }
}