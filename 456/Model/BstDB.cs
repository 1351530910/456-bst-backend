using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;



namespace bst.Model
{
    public class BstDB:DbContext
    {
        public BstDB()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (optionsBuilder != null)
            {
                optionsBuilder.UseMySQL("server=localhost;database=bstdata;user=bst;password=asd45214", null);
            }
        }

        public class Protocol
        {
            [Key]
            public int ProtocolID { get; set; }
            //metadata
            public string Comment { get; set; }
            public int IStudy { get; set; }
            public bool UseDefaultAnat { get; set; }
            public bool UseDefaultChannel { get; set; }
            public bool IsLocked { get; set; }
            //FK
            public int UserID { get; set; }
            public virtual User LockedUser { get; set; }
        }

        public class Study
        {
            [Key]
            public int StudyID { get; set; }
            //metadata
            public string Filename { get; set; }
            public string Name { get; set; }
            public string Condition { get; set; }
            public DateTime DateOfStudy { get; set; }
            public int IChannel { get; set; }
            public int IHeadModel { get; set; }
            //FK
            public int ProtocolID { get; set; }
            public virtual Protocol Protocol { get; set; }
            public int SubjectID { get; set; }
            public virtual Subject Subject { get; set; }
        }

        public class Subject
        {
            [Key]
            public int SubjectID { get; set; }
            //metadata
            public string Comment { get; set; }
            public string Filename { get; set; }
            public string Name { get; set; }
            public bool UseDefaultAnat { get; set; }
            public bool UseDefaultChannel { get; set; }
            public int IAnatomy { get; set; }
            public int IScalp { get; set; }
            public int ICortex { get; set; }
            public int IInnerSkull { get; set; }
            public int IOuterSkull { get; set; }
            public int IOther { get; set; }
            //FK 
            public int ProtocolID { get; set; }
            public virtual Protocol Protocol { get; set; }
        }

        //TODO change later 
        public enum FileType
        {
            channel, timefreq, stat, headmodel
        }

        public abstract class FunctionalFile
        {
            [Key]
            public int FunctionalFileID { get; set; }
            //metadata
            public string Comment { get; set; }
            public string FileName { get; set; }
            public FileType FileType { get; set; }
            //FK 
            public int StudyID { get; set; }
            public Study Study { get; set; }          
            //summary info
            public FunctionalFile DbDataFile { get; set; }
        }

        public class Channel: FunctionalFile
        {
            //public Guid ChannelID { get; set; }
            //metadata
            public int NbChannels { get; set; }
            public string TransfMegLabels { get; set; }
            public string TransfEegLabels { get; set; }
            //summary info
            public bool DbMegRefCoef { get; set; }
            public int DbProjector { get; set; }
            public int DbHeadPoints { get; set; }
            public int DbTransfMeg { get; set; }
            public int DbTransfEeg { get; set; }
            public int DbIntraElectrodes { get; set; }
        }

        public class TimeFreq: FunctionalFile
        {
            //metadata
            public string Measure { get; set; }
            public string Method { get; set; }
            public int NAvg { get; set; }
            public string ColormapType { get; set; }
            public string DisplayUnits { get; set; }
            //summary info 
            public virtual ICollection<FunctionalFile> Files { get; set; }

        }

        public class Stat: FunctionalFile
        {
            //metadata 
            public int Df { get; set; }
            public bool Correction { get; set; }
            public string Type { get; set; }
            //summary info
            public int DbChannelFlag { get; set; }
        }

        public class HeadModel: FunctionalFile
        {
            //metadata
            public string Type { get; set; }
            public string MEGMethod { get; set; }
            public string EEGMethod { get; set; }
            public string ECOGMethod { get; set; }
            public string SEEGMethod { get; set; }
            //summary info
            public virtual ICollection<FunctionalFile> Files { get; set; }
        }

        public class Result: FunctionalFile
        {
            //metadata 
            public bool IsLink { get; set; }
            public int NComponents { get; set; }
            public string Function { get; set; }
            public int NAvg { get; set; }
            public string ColormapType { get; set; }
            public string DisplayUnits { get; set; }

            //summary info
            public bool DbImagingKernel { get; set; }
            public int DbChannelFlag { get; set; }
            public int DbAtlas { get; set; }
            public virtual ICollection<FunctionalFile> Files { get; set; }


        }






    }
}
