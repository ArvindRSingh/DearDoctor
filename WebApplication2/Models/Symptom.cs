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

        public string Symptom { get { return this.PartitionKey; } set { this.PartitionKey = value; } }


        AgeGroup _ageGroup;
        public AgeGroup AgeGroup
        {
            get { return this._ageGroup; }
            set
            {
                this._ageGroup = value;
                this.RowKey = SymptomEntity.GetRowKey(this.AgeGroup, this.Gender, this.Region);
            }
        }

        Gender _gender;
        public Gender Gender
        {
            get { return this._gender; }
            set
            {
                this._gender = value;
                this.RowKey = SymptomEntity.GetRowKey(this.AgeGroup, this.Gender, this.Region);
            }
        }

        Region _region;
        public Region Region
        {
            get
            {
                return this._region;
            }
            set
            {
                this._region = value;
                this.RowKey = SymptomEntity.GetRowKey(this.AgeGroup, this.Gender, this.Region);
            }
        }


        public string LikelyDiagnosis { get; set; }
        public string Prescription { get; set; }
        public string Notes { get; set; }

    }

    public class SymptomSearchViewModel
    {
        public IList<SymptomEntity> SearchResult { get; set; }
        public SymptomEntity SearchParameters { get; set; }
    }

    public class SymptomSearchViewModelPure
    {
        public SymptomSearchViewModelPure()
        {
            this.Diagnosis = new List<SymptomEntity>();
            this.Symptoms = new List<string>();
        }
        public Gender Gender { get; set; }
        public Region Region { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public string RowKey { get; set; }
        public List<string> Symptoms { get; set; }
        public List<SymptomEntity> Diagnosis { get; set; }
        public string Prescription { get; set; }
        public string Notes { get; set; }
    }
}