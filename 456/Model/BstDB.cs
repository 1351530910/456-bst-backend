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

       public class History
        {
            [Key]
            public int HistoryID { get; set; }
            //metadata
            public DateTime TimeStamp { get; set; }
            public string Type { get; set; }  //what does type mean here?
            public string HistoryEvent { get; set; }
            //FK
            public Guid FunctioncalFileID { get; set; }
            public virtual FunctionalFile FunctionalFile { get; set; }
            public Guid AnatomicalFileID { get; set; }
            public virtual AnatomicalFile AnatomicalFile { get; set; }
        }

        #region Functional File and its subclasses 

        public enum FunctionalFileType
        {
            channel, timefreq, stat, headmodel, result, recording, matrix,
            dipole, covariance, image
        }

        public abstract class FunctionalFile
        {
            [Key]
            public Guid FunctionalFileID { get; set; }
            //metadata
            public string Comment { get; set; }
            public string FileName { get; set; }
            public FunctionalFileType FileType { get; set; }
            //FK 
            public int StudyID { get; set; }
            public Study Study { get; set; }
            //summary info
            public virtual ICollection<FunctionalFile> Files { get; set; }
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

        public class Recording: FunctionalFile
        {
            //metadata 
            public string Format { get; set; }
            public string Device { get; set; }
            public char Byteorder { get; set; }
            public string DataType { get; set; }
            public int NAvg { get; set; }
            public int SFreq { get; set; }
            public double TimeStart { get; set; }
            public double TimeEnd { get; set; }
            public int SamplesStart { get; set; }
            public int SamplesEnd { get; set; }
            public int CurrCrfComp { get; set; }
            public int DestCtfComp { get; set; }
            public DateTime Acq_Date { get; set; }
            public string ColormapType { get; set; }
            public string DisplayUnits { get; set; }
            public bool IsBids { get; set; }

            //summary info
            public int DbEvents { get; set; }
            public int DbEpoch { get; set; }
            public int DbChannelFlag { get; set; }
            
        }

        public class Matrix: FunctionalFile
        {
            //metadata
            public int NAvg { get; set; }
            public string DisplayUnits { get; set; }
            //summary info
            public int DbChannelFlag { get; set; }
            public int DbEvents { get; set; }
            public int DbAtlas { get; set; }
            public virtual ICollection<FunctionalFile> Files { get; set; }

        }

        public class Dipole : FunctionalFile
        {
            //summary info 
            public int DbDipole { get; set; }
    }

        public class Covariance: FunctionalFile
        {

        }

        public class Image: FunctionalFile
        {

        }

        #endregion

        #region Anatomical File and its subclasses 

        public enum AnatomicalFileType
        {
            Fiber, Volume, Surface
        }

        public abstract class AnatomicalFile
        {
            [Key]
            public Guid AnatomicalFileID { get; set; }
            //metadata
            public string Comment { get; set; }
            public string FileName { get; set; }
            public AnatomicalFileType FileType { get; set; }
            //FK
            public int SubjectID { get; set; }
            public virtual Subject Subject { get; set; }
        }

        public class Fiber: AnatomicalFile
        {

        }

        public class Volume: AnatomicalFile
        {
            //summary info
            public int DbCubeX { get; set; }
            public int DbCubeY { get; set; }
            public int DbCubeZ { get; set; }
            public double DbVoxsizeX { get; set; }
            public double DbVoxsizeY { get; set; }
            public double DbVoxsizeZ { get; set; }
            public bool DbSCS { get; set; }
            public bool DbNCS { get; set; }
        }

        public class Surface: AnatomicalFile
        {
            //metadata
            public int IAtlas { get; set; }
            //summary info
            public int DbVertices { get; set; }
            public int DbFaces { get; set; }
            public int DbAtlas { get; set; }
        }

        #endregion




    }
}
