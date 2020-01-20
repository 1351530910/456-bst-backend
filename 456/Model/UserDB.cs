using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace bst.Model
{
    public class UserDB : DbContext
    {
        public UserDB() : base()
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Group>().HasIndex(g => g.Name).IsUnique();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);

            switch (ConnectionString.servertype)
            {
                case "MYSQL":
                    optionsBuilder.UseMySQL(ConnectionString.connectionstring);
                    break;
                case "MSSQL":
                    optionsBuilder.UseSqlServer(ConnectionString.connectionstring);
                    break;
                default:
                    break;
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ProtocolUser> ProtocolUsers { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Protocol> Protocols { get; set; }
        public DbSet<ProtocolGroup> ProtocolGroups { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<FunctionalFile> FunctionalFiles { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<TimeFreq> TimeFreqs { get; set; }
        public DbSet<Stat> Stats { get; set; }
        public DbSet<HeadModel> HeadModels { get; set; }
        public DbSet<Result> Results { get; set; }
        //public DbSet<Recording> Recordings { get; set; }
        public DbSet<Matrix> Matrices { get; set; }
        public DbSet<Other> Others { get; set; }

        public DbSet<Dipole> Dipoles { get; set; }
        //public DbSet<Covariance> Covariances { get; set; }
        //public DbSet<Fiber> Fibers { get; set; }
        //public DbSet<Volume> Volumes { get; set; }
        public DbSet<Surface> Surfaces { get; set; }
        public DbSet<AnatomicalFile> AnatomicalFiles { get; set; }
    }

    public class Tracked
    {
        public DateTime LastUpdate { get; set; }
    }

    public class BoolToIntConverter : ValueConverter<bool, int>
    {
        public BoolToIntConverter(ConverterMappingHints mappingHints = null)
            : base(
                  v => Convert.ToInt32(v),
                  v => Convert.ToBoolean(v),
                  mappingHints)
        {
        }
        public static ValueConverterInfo DefaultInfo { get; }
            = new ValueConverterInfo(typeof(bool), typeof(int), i => new BoolToIntConverter(i.MappingHints));
    }

    #region user-group
    public partial class User
    {
        [Key]
        public Guid Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [MinLength(8), MaxLength(15), Required]
        public string Password { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }

        public virtual ICollection<ProtocolUser> ProtocolUsers { get; set; }
        public virtual ICollection<GroupUser> GroupUsers { get; set; }
        
    }

    public partial class Group
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100), Required]
        public string Name { get; set; }

        public virtual ICollection<GroupUser> Members { get; set; }
        public virtual ICollection<ProtocolGroup> GroupProtocols { get; set; }

    }

    public partial class GroupUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Group Group { get; set; }
        /// <summary>
        /// 1 -> group manager
        /// 2 -> member
        /// </summary>
        [Required]
        public int Role { get; set; }
    }

    public class Protocol:Tracked
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100), Required]
        public string Name { get; set; }
        public bool Isprivate { get; set; }
        //metadata
        public string Comment { get; set; }
        public int IStudy { get; set; }
        public bool UseDefaultAnat { get; set; }
        public bool UseDefaultChannel { get; set; }

        public virtual User LockedUser { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<Study> Studies { get; set; }
        public virtual ICollection<ProtocolUser> ProtocolUsers { get; set; }
        public virtual ICollection<ProtocolGroup> ProtocolGroups { get; set; }
    }

    public partial class ProtocolUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Protocol Protocol { get; set; }
        /// <summary>
        /// 1 -> protocol admin
        /// 2 -> has read-write access
        /// 3 -> has read access
        /// </summary>
        [Required]
        public int Privilege { get; set; }
    }

    public partial class ProtocolGroup
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual Protocol Protocol { get; set;}
        [Required]
        public virtual Group Group { get; set; }
        /// <summary>
        /// 1 -> has read-write access
        /// 2 -> has read access
        /// </summary>
        [Required]
        public int GroupPrivilege { get; set; }       
    }


    public partial class Invitation
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int Privilege { get; set; }
        public Guid GroupId { get; set; }
        public Guid ProtocolId { get; set; }
        [Required]
        public DateTime Expiration { get; set; }

        public virtual User SentFrom { get; set; }

        public string Message { get; set; }
    }

    #endregion

    public enum Privilege
    {
        Administrator,
        ReadWrite,
        Read
    }

    public class Subject:Tracked
    {
        [Key]
        public Guid Id { get; set; }
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
        
        public virtual Protocol Protocol { get; set; }
        public virtual ICollection<Study> Studies { get; set; }
        public virtual ICollection<AnatomicalFile> AnatomicalFiles { get; set; }
    }

    public class Study:Tracked
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Condition { get; set; }
        public string DateOfStudy { get; set; }
        public int IChannel { get; set; }
        public int IHeadModel { get; set; }

        public virtual Subject Subject { get; set; }
        public virtual Protocol Protocol { get; set; }

        public virtual ICollection<Channel> Channels { get; set; }
        public virtual ICollection<TimeFreq> TimeFreqs { get; set; }
        public virtual ICollection<Stat> Stats { get; set; }
        public virtual ICollection<HeadModel> HeadModels { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        //public virtual ICollection<Recording> Recordings { get; set; }
        public virtual ICollection<Matrix> Matrixs { get; set; }
        public virtual ICollection<Other> Others { get; set; }
        public virtual ICollection<Dipole> Dipoles { get; set; }
        //public virtual ICollection<Covariance> Covariances { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }

    public class History
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public DateTime TimeStamp { get; set; }
        public string HistoryEvent { get; set; }
        
        public virtual FunctionalFile FunctionalFile { get; set; }

        public virtual AnatomicalFile AnatomicalFile { get; set; }
        public virtual Protocol Protocol { get; set; }
    }

    #region Functional File and its subclasses 

    public enum FunctionalFileType
    {
        Channel = 1,
        TimeFreq = 2,
        Stat = 3,
        HeadModel = 4,
        Result = 5,
        //Recording,
        Matrix = 6,
        Other = 7
        //Dipole = 7,
        //Covariance,
        //Image = 8
    }

#warning raw files.


    public class FunctionalFile:Tracked
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Comment { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public FunctionalFileType FileType { get; set; }
        
        public virtual ICollection<History> Histories { get; set; }
        public FunctionalFile()
        {

        }
        public FunctionalFile(FunctionalFileData data)
        {
            Id = Guid.NewGuid();
            Comment = data.Comment;
            FileName = data.FileName;
            FileType = data.FileType;
        }
    }

    public class Channel
    {
        [Key]
        public Guid Id { get; set; }
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
        [Required]
        public virtual Study Study { get; set; }
        [Required]
        public virtual FunctionalFile Parent { get; set; }
        public Channel()
        {

        }
        public Channel(ChannelData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
            NbChannels = data.NbChannels;
            TransfEegLabels = data.TransfEegLabels;
            TransfMegLabels = data.TransfMegLabels;
        }
    }

    public class TimeFreq 
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Measure { get; set; }
        public string Method { get; set; }
        public int NAvg { get; set; }
        public string ColormapType { get; set; }
        public string DisplayUnits { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        //summary info 
        [Required]
        public virtual FunctionalFile Parent { get; set; }
        public TimeFreq()
        {

        }
        public TimeFreq(TimeFreqData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
            Measure = data.Measure;
            NAvg = data.NAvg;
            ColormapType = data.ColormapType;
            DisplayUnits = data.DisplayUnits;
        }
    }

    public class Stat
    {
        [Key]
        public Guid Id { get; set; }
        //metadata 
        public int Df { get; set; }
        public bool Correction { get; set; }
        public string Type { get; set; }
        //summary info
        public int DbChannelFlag { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        [Required]
        public virtual FunctionalFile Parent { get; set; }
        public Stat()
        {

        }
        public Stat(StatData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
            Df = data.Df;
            Correction = data.Correction;
            Type = data.Type;
        }
    }

    public class HeadModel
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Type { get; set; }
        public string MEGMethod { get; set; }
        public string EEGMethod { get; set; }
        public string ECOGMethod { get; set; }
        public string SEEGMethod { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        [Required]
        //summary info
        public virtual FunctionalFile Parent { get; set; }
        public HeadModel()
        {

        }
        public HeadModel(HeadModelData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
            Type = data.Type;
            MEGMethod = data.MEGMethod;
            EEGMethod = data.EEGMethod;
            ECOGMethod = data.ECOGMethod;
            SEEGMethod = data.SEEGMethod;
        }
    }

    public class Result
    {
        [Key]
        public Guid Id { get; set; }
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
        [Required]
        public virtual Study Study { get; set; }
        [Required]
        public virtual FunctionalFile Parent { get; set; }
        public Result()
        {

        }
        public Result(ResultData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
            IsLink = data.IsLink;
            NComponents = data.NComponents;
            Function = data.Function;
            NAvg = data.NAvg;
            ColormapType = data.ColormapType;
            DisplayUnits = data.DisplayUnits;

        }
    }

    /*
    public class Recording
    {
        [Key]
        public Guid Id { get; set; }
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
        [Required]
        public virtual Study Study { get; set; }
        public int DbChannelFlag { get; set; }
        [Required]
        public virtual FunctionalFile Parent { get; set; }
        public Recording()
        {

        }
        public Recording(RecordingData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
            Format = data.Format;
            Device = data.Device;
            Byteorder = data.Byteorder;
            DataType = data.DataType;
            NAvg = data.NAvg;
            SFreq = data.SFreq;
            TimeStart = data.TimeStart;
            TimeEnd = data.TimeEnd;
            SamplesStart = data.SamplesStart;
            SamplesEnd = data.SamplesEnd;
            CurrCrfComp = data.CurrCrfComp;
            DestCtfComp = data.DestCtfComp;
            Acq_Date = data.Acq_Date;
            ColormapType = data.ColormapType;
            DisplayUnits = data.DisplayUnits;
            IsBids = data.IsBids;

        }
    }
    */

    public class Matrix
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public int NAvg { get; set; }
        public string DisplayUnits { get; set; }
        //summary info
        public int DbChannelFlag { get; set; }
        public int DbEvents { get; set; }
        public int DbAtlas { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        [Required]
        public virtual FunctionalFile Parent { get; set; }
        public Matrix()
        {

        }
        public Matrix(MatrixData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
            NAvg = data.NAvg;
            DisplayUnits = data.DisplayUnits;
        }
    }


    public class Other
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        [Required]
        public virtual FunctionalFile Parent { get; set; }
        public Other()
        {

        }
        public Other(OtherData data)
        {
            Parent = new FunctionalFile(data);
            Id = Parent.Id;
        }
    }


    public class Dipole 
    {
        [Key]
        public Guid Id { get; set; }
        //summary info 
        public int DbDipole { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        public virtual FunctionalFile Parent { get; set; }
    }

    /*
    public class Covariance 
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        public virtual FunctionalFile Parent { get; set; }
    }
    */

    public class Image
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual Study Study { get; set; }
        public virtual FunctionalFile Parent { get; set; }
    }

    #endregion

    #region Anatomical File and its subclasses 

    public enum AnatomicalFileType
    {
        //Fiber,
        //Volume,
        Surface = 1
    }

    public class AnatomicalFile:Tracked
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Comment { get; set; }
        public string FileName { get; set; }
        public AnatomicalFileType FileType { get; set; }
        
        
        public virtual ICollection<History> Histories { get; set; }
    }

    //public class Fiber 
    //{
    //    [Key]
    //    public Guid Id { get; set; }
    //    public virtual Subject Subject { get; set; }

    //    public virtual AnatomicalFile Parent { get; set; }
    //}

    /*
    public class Volume
    {
        [Key]
        public Guid Id { get; set; }
        //summary info
        public int DbCubeX { get; set; }
        public int DbCubeY { get; set; }
        public int DbCubeZ { get; set; }
        public double DbVoxsizeX { get; set; }
        public double DbVoxsizeY { get; set; }
        public double DbVoxsizeZ { get; set; }
        public bool DbSCS { get; set; }
        public bool DbNCS { get; set; }
        public virtual Subject Subject { get; set; }

        public virtual AnatomicalFile Parent { get; set; }
    }
    */

    public class Surface
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public int IAtlas { get; set; }
        //summary info
        public int DbVertices { get; set; }
        public int DbFaces { get; set; }
        public int DbAtlas { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual AnatomicalFile Parent { get; set; }
    }
    #endregion
}
