using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bst.Model
{
    
    public class ProtocolData
    {
        //metadata
        public string Comment { get; set; }
        public int IStudy { get; set; }
        public bool UseDefaultAnat { get; set; }
        public bool UseDefaultChannel { get; set; }
        public bool IsLocked { get; set; }

        public IEnumerable<SubjectData> Subjects { get; set; }
        public ProtocolData()
        {

        }
        public ProtocolData(Protocol protocol)
        {
            Comment = protocol.Comment;
            IStudy = protocol.IStudy;
            UseDefaultAnat = protocol.UseDefaultAnat;
            UseDefaultChannel = protocol.UseDefaultChannel;
#warning lock
            IsLocked = false;

            Subjects = protocol.Subjects.Select(x => new SubjectData(x));
        }
    }
    

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


        public IEnumerable<StudyData> Studies { get; set; }

        public SubjectData()
        {

        }

        public SubjectData(Subject subject)
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
            Studies = subject.Studies.Select(x => new StudyData(x));
            ProtocolId = subject.Protocol.Id;
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

        public IEnumerable<ChannelData> Channels { get; set; }
        public IEnumerable<TimeFreqData> TimeFreqs { get; set; }
        public IEnumerable<StatData> Stats { get; set; }
        public IEnumerable<HeadModelData> HeadModels { get; set; }
        public IEnumerable<ResultData> Results { get; set; }
        public IEnumerable<RecordingData> Recordings { get; set; }
        public IEnumerable<MatrixData> Matrixs { get; set; }
        //public IEnumerable<DipoleData> Dipoles { get; set; }
        //public IEnumerable<CovarianceData> Covariances { get; set; }
        //public IEnumerable<ImageData> Images { get; set; }

        public StudyData()
        {

        }
        public StudyData(Study study)
        {
            Filename = study.Filename;
            Name = study.Name;
            DateOfStudy = study.DateOfStudy;
            IChannel = study.IChannel;
            IHeadModel = study.IHeadModel;
            ProtocolId = study.Subject.Protocol.Id;
            Condition = study.Condition;
            SubjectId = study.Subject.Id;

            Channels = study.Channels.Select(x => new ChannelData(x));
            TimeFreqs = study.TimeFreqs.Select(x => new TimeFreqData(x));
            Stats = study.Stats.Select(x => new StatData(x));
            HeadModels = study.HeadModels.Select(x => new HeadModelData(x));
            Results = study.Results.Select(x => new ResultData(x));
            Recordings = study.Recordings.Select(x => new RecordingData(x));
            Matrixs = study.Matrixs.Select(x => new MatrixData(x));

        }

    }


    public class HistoryData
    {
        //metadata
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }
        public string HistoryEvent { get; set; }
        public HistoryData(History history)
        {
            TimeStamp = history.TimeStamp;
            Type = history.Type;
            HistoryEvent = history.HistoryEvent;
        }
    }

    #region Functional File and its subclasses 


    public abstract class FunctionalFileData
    {
        //metadata
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public string FileName { get; set; }
        public FunctionalFileType type { get; set; }
        public IEnumerable<HistoryData> Histories { get; set; }
        public FunctionalFileData()
        {

        }
        public FunctionalFileData(FunctionalFile f)
        {
            Id = f.Id;
            Comment = f.Comment;
            FileName = f.FileName;
            type = f.FileType;
            Histories = f.Histories.Select(x => new HistoryData(x));
        }
        
    }

    public class ChannelData : FunctionalFileData
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
        public ChannelData()
        {

        }
        public ChannelData(Channel f) : base(f.Parent)
        {
            NbChannels = f.NbChannels;
            TransfMegLabels = f.TransfMegLabels;
            TransfEegLabels = f.TransfEegLabels;
        }
    }


    public class TimeFreqData : FunctionalFileData
    {
        //metadata
        public string Measure { get; set; }
        public string Method { get; set; }
        public int NAvg { get; set; }
        public string ColormapType { get; set; }
        public string DisplayUnits { get; set; }

        public TimeFreqData()
        {

        }
        public TimeFreqData(TimeFreq f) : base(f.Parent)
        {
            Measure = f.Measure;
            Method = f.Method;
            NAvg = f.NAvg;
            ColormapType = f.ColormapType;
            DisplayUnits = f.DisplayUnits;
        }

    }

    public class StatData : FunctionalFileData
    {
        //metadata 
        public int Df { get; set; }
        public bool Correction { get; set; }
        public string Type { get; set; }
        /*
        //summary info
        public int DbChannelFlag { get; set; }  
        */
        public StatData()
        {

        }
        public StatData(Stat f) : base(f.Parent)
        {
            Df = f.Df;
            Correction = f.Correction;
            Type = f.Type;
        }
    }

    public class HeadModelData : FunctionalFileData
    {
        //metadata
        public string Type { get; set; }
        public string MEGMethod { get; set; }
        public string EEGMethod { get; set; }
        public string ECOGMethod { get; set; }
        public string SEEGMethod { get; set; }
        public HeadModelData()
        {

        }
        public HeadModelData(HeadModel f) : base(f.Parent)
        {
            Type = f.Type;
            MEGMethod = f.MEGMethod;
            EEGMethod = f.EEGMethod;
            ECOGMethod = f.ECOGMethod;
            SEEGMethod = f.SEEGMethod;
        }
    }

    public class ResultData : FunctionalFileData
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
        public ResultData()
        {

        }
        public ResultData(Result f) : base(f.Parent)
        {
            IsLink = f.IsLink;
            NComponents = f.NComponents;
            Function = f.Function;
            NAvg = f.NAvg;
            ColormapType = f.ColormapType;
            DisplayUnits = f.DisplayUnits;
        }
    }

    public class RecordingData : FunctionalFileData
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
        public RecordingData()
        {

        }
        public RecordingData(Recording f) : base(f.Parent)
        {
            Format = f.Format;
            Device = f.Device;
            Byteorder = f.Byteorder;
            DataType = f.DataType;
            NAvg = f.NAvg;
            SFreq = f.SFreq;
            TimeStart = f.TimeStart;
            TimeEnd = f.TimeEnd;
            SamplesStart = f.SamplesStart;
            SamplesEnd = f.SamplesEnd;
            CurrCrfComp = f.CurrCrfComp;
            DestCtfComp = f.DestCtfComp;
            Acq_Date = f.Acq_Date;
            ColormapType = f.ColormapType;
            DisplayUnits = f.DisplayUnits;
            IsBids = f.IsBids;
        }
    }

    public class MatrixData : FunctionalFileData
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
        public MatrixData()
        {

        }
        public MatrixData(Matrix f) : base(f.Parent)
        {
            NAvg = f.NAvg;
            DisplayUnits = f.DisplayUnits;
        }
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
        Fiber; Volume; Surface
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
