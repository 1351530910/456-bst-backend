using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bst.Model
{
    /*
    public class ProtocolData
    {
        //metadata
        public string Comment { get; set; }
        public int IStudy { get; set; }
        public bool UseDefaultAnat { get; set; }
        public bool UseDefaultChannel { get; set; }
        public bool IsLocked { get; set; }

        public Guid LockedUserId { get; set; }
    }
    */

    public class SubjectData
    {
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

        public Guid ProtocolId { get; set; }

        public SubjectData(Subject subject, Guid protocolid)
        {
            Comment = subject.Comment;
            Filename = subject.Filename;
            Name = subject.Name;
            UseDefaultAnat = subject.UseDefaultAnat;
            UseDefaultChannel = subject.UseDefaultChannel;
            IAnatomy = subject.IAnatomy;
            IScalp = subject.IScalp;
            ICortex = subject.ICortex;
            IInnerSkull = subject.IInnerSkull;
            IOuterSkull = subject.IOuterSkull;
            IOther = subject.IOther;
            ProtocolId = protocolid;
        }
    }

  

    public class StudyData
    {
        //metadata
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Condition { get; set; }
        public DateTime DateOfStudy { get; set; }
        public int IChannel { get; set; }
        public int IHeadModel { get; set; }

        public Guid ProtocolId { get; set; }
        public Guid SubjectId { get; set; }
    }


    public class HistoryData
    {
        //metadata
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }
        public string HistoryEvent { get; set; }

        public Guid FunctionalFileId { get; set; }
        public Guid AnatomicalFileId { get; set; }
    }

    #region Functional File and its subclasses 


    public class FunctionalFileData
    {
        //metadata
        public string Comment { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

        public Guid StudyId { get; set; }

    }

    public class ChannelData
    {
        //metadata
        public int NbChannels { get; set; }
        public string TransfMegLabels { get; set; }
        public string TransfEegLabels { get; set; }
        /*
        //summary info
        public bool DbMegRefCoef { get; set; }
        public int DbProjector { get; set; }
        public int DbHeadPoints { get; set; }
        public int DbTransfMeg { get; set; }
        public int DbTransfEeg { get; set; }
        public int DbIntraElectrodes { get; set; }
        */
        public Guid ParentId { get; set; }
    }

    public class TimeFreqData
    {
        //metadata
        public string Measure { get; set; }
        public string Method { get; set; }
        public int NAvg { get; set; }
        public string ColormapType { get; set; }
        public string DisplayUnits { get; set; }

        //summary info 
        public Guid ParentId { get; set; }

    }

    public class StatData
    {
        //metadata 
        public int Df { get; set; }
        public bool Correction { get; set; }
        public string Type { get; set; }
        /*
        //summary info
        public int DbChannelFlag { get; set; }  
        */
        public Guid ParentId { get; set; }
    }

    public class HeadModelData
    {
        //metadata
        public string Type { get; set; }
        public string MEGMethod { get; set; }
        public string EEGMethod { get; set; }
        public string ECOGMethod { get; set; }
        public string SEEGMethod { get; set; }

        //summary info
        public Guid ParentId { get; set; }
    }

    public class ResultData
    {
        //metadata 
        public bool IsLink { get; set; }
        public int NComponents { get; set; }
        public string Function { get; set; }
        public int NAvg { get; set; }
        public string ColormapType { get; set; }
        public string DisplayUnits { get; set; }

        /*
        //summary info
        public bool DbImagingKernel { get; set; }
        public int DbChannelFlag { get; set; }
        public int DbAtlas { get; set; }
        */

        public Guid ParentId { get; set; }
    }

    public class RecordingData
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

        /*
        //summary info
        public int DbEvents { get; set; }
        public int DbEpoch { get; set; }
        public int DbChannelFlag { get; set; }
        */
        public Guid ParentId { get; set; }
    }

    public class MatrixData
    {
        //metadata
        public int NAvg { get; set; }
        public string DisplayUnits { get; set; }
        /*
        //summary info
        public int DbChannelFlag { get; set; }
        public int DbEvents { get; set; }
        public int DbAtlas { get; set; }
        */
        public Guid ParentId { get; set; }
    }


    /*

    public class Dipole
    {
        [Key]
        public Guid id { get; set; }
        //summary info 
        public int DbDipole { get; set; }
        public virtual FunctionalFile parent { get; set; }
    }

    public class Covariance
    {
        [Key]
        public Guid id { get; set; }
        public virtual FunctionalFile parent { get; set; }
    }

    public class Image
    {
        [Key]
        public Guid id { get; set; }
        public virtual FunctionalFile parent { get; set; }
    }
    */
    #endregion

    #region Anatomical File and its subclasses 
    /*
    public enum AnatomicalFileType
    {
        Fiber, Volume, Surface
    }

    public class AnatomicalFile
    {
        [Key]
        public Guid id { get; set; }
        //metadata
        public string Comment { get; set; }
        public string FileName { get; set; }
        public int FileType { get; set; }

        public virtual Subject Subject { get; set; }
    }

    public class Fiber
    {
        [Key]
        public Guid id { get; set; }

        public virtual AnatomicalFile parent { get; set; }
    }

    public class Volume
    {
        [Key]
        public Guid id { get; set; }
        //summary info
        public int DbCubeX { get; set; }
        public int DbCubeY { get; set; }
        public int DbCubeZ { get; set; }
        public double DbVoxsizeX { get; set; }
        public double DbVoxsizeY { get; set; }
        public double DbVoxsizeZ { get; set; }
        public bool DbSCS { get; set; }
        public bool DbNCS { get; set; }

        public virtual AnatomicalFile parent { get; set; }
    }

    public class Surface
    {
        [Key]
        public Guid id { get; set; }
        //metadata
        public int IAtlas { get; set; }
        //summary info
        public int DbVertices { get; set; }
        public int DbFaces { get; set; }
        public int DbAtlas { get; set; }

        public virtual AnatomicalFile parent { get; set; }
    }
    */
    #endregion
}
