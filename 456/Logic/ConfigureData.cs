using bst.Controllers;
using bst.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bst.Logic
{
    public class ConfigureData
    {
        public static StudyData ToStudyData(Study study)
        {
            return new StudyData
            {
                Filename = study.Filename,
                Name = study.Name,
                Condition = study.Condition,
                DateOfStudy = study.DateOfStudy,
                IChannel = study.IChannel,
                IHeadModel = study.IHeadModel,
                ProtocolId = study.Subject.Protocol.Id,
                SubjectId = study.Subject.Id
            };

        }

        public static SubjectData ToSubjectData(Subject subject)
        {
            return new SubjectData
            {
                Comment = subject.Comment,
                Filename = subject.Filename,
                Name = subject.Name,
                UseDefaultAnat = subject.UseDefaultAnat,
                UseDefaultChannel = subject.UseDefaultChannel,
                IAnatomy = subject.IAnatomy,
                IScalp = subject.IScalp,
                ICortex = subject.ICortex,
                IInnerSkull = subject.IInnerSkull,
                IOuterSkull = subject.IOuterSkull,
                IOther = subject.IOther
            };

        }



        public static FunctionalFile ToFunctionalFile(FunctionalFileData data, FunctionalFileType type, Study study)
        {
            return new FunctionalFile
            {
                Id = Guid.NewGuid(),
                Comment = data.Comment,
                FileName = data.FileName,
                FileType = type,
                Study = study
            };
        }

        public static Channel ToChannel(ChannelData data, FunctionalFile parent)
        {
            return new Channel
            {
                Id = Guid.NewGuid(),
                NbChannels = data.NbChannels,
                TransfMegLabels = data.TransfMegLabels,
                TransfEegLabels = data.TransfEegLabels,
                Parent = parent
            };
        }

        public static GroupManagement ToGroupManagement(Group group, Guid protocolid)
        {
            return new GroupManagement
            {
                GroupId = group.Id,
                GroupName = group.Name,
                Members = group.Members.Select(role => ToProtocolMember(role.User, protocolid)).ToList()
            };
        }

        public static ProtocolMember ToProtocolMember(User user, Guid protocolid)
        {
            return new ProtocolMember
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProtocolPrivilege = user.ProtocolUsers.First(x => x.Protocol.Id.Equals(protocolid)).Privilege
            };
        }
    }
}
