using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            
            if (optionsBuilder != null)
            {

                if (bst.Startup.devenv)
                {
                    optionsBuilder.UseSqlServer("server=.;database=bstusers;Integrated Security=SSPI;user=sa;password=asd45214", null);
                }
                else
                {
                    optionsBuilder.UseMySQL("server=localhost;database=bstusers;user=bst;password=asd45214", null);
                }

            }
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ParticipateProtocol> ParticipateProtocols { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<Protocol> Protocols { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<FunctionalFile> FunctionalFiles { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<TimeFreq> TimeFreqs { get; set; }
        public DbSet<Stat> Stats { get; set; }
        public DbSet<HeadModel> HeadModels { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Recording> Recordings { get; set; }
        public DbSet<Matrix> Matrices { get; set; }
        public DbSet<Dipole> Dipoles { get; set; }
        public DbSet<Covariance> Covariances { get; set; }
        public DbSet<Fiber> Fibers { get; set; }
        public DbSet<Volume> Volumes { get; set; }
        public DbSet<Surface> Surfaces { get; set; }
        public DbSet<AnatomicalFile> AnatomicalFiles { get; set; }

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

        public virtual ICollection<ParticipateProtocol> Protocols { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Protocol> Locks { get; set; }
    }

    public partial class ParticipateProtocol
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Protocol Protocol { get; set; }
        [Required]
        public int Privilege { get; set; }
    }

    public partial class Role
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Group Group { get; set; }
        /// <summary>
        /// 1 -> administrator
        /// 2 -> has write access
        /// 3 -> has read access
        /// </summary>
        [Required]
        public int Privilege { get; set; }
    }

    public partial class Group
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(100),Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Role> Users { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }

    }


    public partial class Invitation
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int Priviledge { get; set; }
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
    public class Protocol
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
        [Required]
        public virtual Group Group { get; set; }
        public virtual User LockedUser { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<Study> Studies { get; set; }
        public virtual ICollection<ParticipateProtocol> Participations { get; set; }
    }

    public class Subject
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

    public class Study
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Condition { get; set; }
        public DateTime DateOfStudy { get; set; }
        public int IChannel { get; set; }
        public int IHeadModel { get; set; }

        public virtual Protocol Protocol { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<FunctionalFile> FunctionalFiles { get; set; }
    }

    public class History
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }  //what does type mean here?
        public string HistoryEvent { get; set; }
        
        public virtual FunctionalFile FunctionalFile { get; set; }

        public virtual AnatomicalFile AnatomicalFile { get; set; }
    }

    #region Functional File and its subclasses 

    public enum FunctionalFileType
    {
        Channel,
        TimeFreq,
        Stat,
        HeadModel,
        Result,
        Recording,
        Matrix,
        Dipole,
        Covariance,
        Image,

    }

    public class FunctionalFile
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Comment { get; set; }
        public string FileName { get; set; }
        public FunctionalFileType FileType { get; set; }
        
        public virtual Study Study { get; set; }
        public virtual ICollection<History> Histories { get; set; }
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
        public virtual FunctionalFile Parent { get; set; }
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
        //summary info 
        public virtual FunctionalFile Parent { get; set; }
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
        public virtual FunctionalFile Parent { get; set; }
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
        //summary info
        public virtual FunctionalFile Parent { get; set; }
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
        public virtual FunctionalFile Parent { get; set; }
    }

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
        public int DbChannelFlag { get; set; }
        public virtual FunctionalFile Parent { get; set; }
    }

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
        public virtual FunctionalFile Parent { get; set; }
    }

    public class Dipole 
    {
        [Key]
        public Guid Id { get; set; }
        //summary info 
        public int DbDipole { get; set; }
        public virtual FunctionalFile Parent { get; set; }
    }

    public class Covariance 
    {
        [Key]
        public Guid Id { get; set; }
        public virtual FunctionalFile Parent { get; set; }
    }

    public class Image
    {
        [Key]
        public Guid Id { get; set; }
        public virtual FunctionalFile Parent { get; set; }
    }

    #endregion

    #region Anatomical File and its subclasses 

    public enum AnatomicalFileType
    {
        Fiber,
        Volume,
        Surface
    }

    public class AnatomicalFile
    {
        [Key]
        public Guid Id { get; set; }
        //metadata
        public string Comment { get; set; }
        public string FileName { get; set; }
        public AnatomicalFileType FileType { get; set; }
        
        public virtual Subject Subject { get; set; }
        public virtual ICollection<History> Histories { get; set; }
    }

    public class Fiber 
    {
        [Key]
        public Guid Id { get; set; }

        public virtual AnatomicalFile Parent { get; set; }
    }

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

        public virtual AnatomicalFile Parent { get; set; }
    }

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

        public virtual AnatomicalFile Parent { get; set; }
    }
    #endregion
}
