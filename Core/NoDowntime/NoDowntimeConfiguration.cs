using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDowntime
{
    class NoDowntimeConfiguration : ConfigurationSection
    {
        [ConfigurationProperty(nameof(ClassName))]
        public string ClassName
        {
            get
            {
                return (string)this[nameof(ClassName)];
            }
            set
            {
                this[nameof(ClassName)] = value;
            }
        }

        [ConfigurationProperty(nameof(DllName))]
        public string DllName
        {
            get
            {
                return (string)this[nameof(DllName)];
            }
            set
            {
                this[nameof(DllName)] = value;
            }
        }
        internal static readonly string Folder1Default = "Folder1";
        [ConfigurationProperty(nameof(Folder1), DefaultValue = "Folder1")]
        public string Folder1
        {
            get
            {
                return (string)this[nameof(Folder1)];
            }
            set
            {
                this[nameof(Folder1)] = value;
            }
        }
        internal static readonly string Folder2Default = "Folder2";
        [ConfigurationProperty(nameof(Folder2), DefaultValue = "Folder2")]
        public string Folder2
        {
            get
            {
                return (string)this[nameof(Folder2)];
            }
            set
            {
                this[nameof(Folder2)] = value;
            }
        }
        internal static readonly string StagingFolderDefault = "StagingFolder";
        [ConfigurationProperty(nameof(StagingFolder), DefaultValue = "StagingFolder")]
        public string StagingFolder
        {
            get
            {
                return (string)this[nameof(StagingFolder)];
            }
            set
            {
                this[nameof(StagingFolder)] = value;
            }
        }

        [ConfigurationProperty(nameof(InPlaceRecycling), DefaultValue = true)]
        public bool InPlaceRecycling
        {
            get
            {
                return (bool)this[nameof(InPlaceRecycling)];
            }
            set
            {
                this[nameof(InPlaceRecycling)] = value;
            }
        }

        [ConfigurationProperty(nameof(RefreshedSections), DefaultValue = "")]
        public string RefreshedSections
        {

            get
            {
                return (string)this[nameof(RefreshedSections)];
            }
            set
            {
                this[nameof(RefreshedSections)] = value;
            }
        }
    }
}
