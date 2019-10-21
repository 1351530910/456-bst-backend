using System;
using bst.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Reflection;


namespace Tests
{
    class Program
    {
        #region variables
        static DateTime starttime;
        static DateTime lasttime;
        static StreamWriter fs = new StreamWriter("testdata.json");
        static JsonHttpClient client;
        static JsonHttpClient client2;

        static bst.Model.User u = new bst.Model.User
        {
            Email = randomstr() + "@test.com",
            Password = randomstr(),
            FirstName = randomstr(),
            LastName = randomstr()
        };
        static bst.Model.User u2 = new bst.Model.User
        {
            Email = randomstr() + "@test.com",
            Password = randomstr(),
            FirstName = randomstr(),
            LastName = randomstr()
        };
        static bst.Model.Group g = new bst.Model.Group
        {
            Name = randomstr()
        };
        static string sessionid;
        static string deviceid = randomstr();
        static string sessionid2;
        static string deviceid2 = randomstr();
        static bst.Model.Protocol protocol = new bst.Model.Protocol
        {
            Name = randomstr(),
            Isprivate = true,
            Comment = randomstr(),
            IStudy = 5,
            UseDefaultAnat = true,
            UseDefaultChannel = true
        };
        static bst.Model.Study study = new bst.Model.Study
        {
            Filename = randomstr(),
            Name = randomstr(),
            Condition = randomstr(),
            DateOfStudy = System.DateTime.Now,
            IChannel = 5,
            IHeadModel = 1,
            Protocol = protocol
        };
        static bst.Model.Channel channel = new bst.Model.Channel
        {
            NbChannels = 5,
            TransfMegLabels = randomstr(),
            TransfEegLabels = randomstr()
        };
        static bst.Model.FunctionalFile ff = new bst.Model.FunctionalFile
        {
            Comment = randomstr(),
            Study = study,
            FileType = bst.Model.FunctionalFileType.Channel,
            FileName = "test.bin"
        };
        #endregion
        static void Main(string[] args)
        {
            client = new JsonHttpClient();
            client.BaseAddress = new System.Uri("http://localhost/");
            client2 = new JsonHttpClient();
            client2.BaseAddress = new System.Uri("http://localhost/");
            starttime = System.DateTime.Now;

            Console.WriteLine("test started");
            foreach (var f in typeof(Program).GetMethods().Where(x => x.IsStatic&&x.ReturnType.Equals(typeof(Task))).ToArray())
            {
                call(f);
            }
            Console.WriteLine($"total time is {(System.DateTime.Now - starttime).TotalMilliseconds}ms");
        }

        public static async Task createuser()
        {
            var p = new CreateUserIn
            {
                Email = u.Email,
                Password = u.Password,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Deviceid = deviceid
            };
            var userdata = await client.PostAsJsonAsync<CreateUserOut>("user/createuser", p);
            sessionid = userdata.Sessionid.ToString();

            Assert.AreEqual(u.FirstName, userdata.Firstname);
            Assert.AreEqual(u.LastName, userdata.Lastname);
            Assert.AreEqual(u.Email, userdata.Email);

            fs.WriteLine("deviceid: " + deviceid);
        }
        public static async Task login()
        {
            var r = await client.PostAsJsonAsync<LoginOut>("user/login", new LoginIn
            {
                Email = u.Email,
                Password = u.Password,
                Deviceid = deviceid
            });
            sessionid = r.Sessionid.ToString();
            fs.WriteLine("sessionid: " + sessionid);
            client.DefaultRequestHeaders.Add("sessionid", sessionid);
            client.DefaultRequestHeaders.Add("deviceid", deviceid);
        }
        public static async Task listprotocols()
        {
            var r = await client.PostAsJsonAsync<List<ProtocolData>>("user/listprotocols", new ListCount
            {
                Start = 0,
                Count = 10,
                Order = 0
            });
            if (r.Count == 0)
            {
                return;
            }
            Assert.Fail();
        }
        public static async Task listgroups()
        {
            var groups = await client.PostAsJsonAsync<List<GroupPreview>>("user/listgroups", new ListCount
            {
                Start = 0,
                Count = 10,
                Order = 0
            });
            if (groups.Count == 0)
            {
                return;
            }
            Assert.Fail();
        }
        public static async Task creategroup()
        {
            var gp = await client.PostAsJsonAsync<GroupPreview>("group/create", new CreateGroupIn
            {
                Name = g.Name
            });
            Assert.AreEqual(g.Name, gp.Name);
            Assert.AreEqual(u.Email, gp.Users.FirstOrDefault().Email);
            Assert.AreEqual(1, gp.Users.FirstOrDefault().Privilege);
            Assert.AreEqual(0, gp.Projects.Count());
        }
        public static async Task groupdetail()
        {
            var gp = await client.PostAsJsonAsync<GroupPreview>("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(g.Name, gp.Name);
            Assert.AreEqual(u.Email, gp.Users.FirstOrDefault().Email);
            Assert.AreEqual(1, gp.Users.FirstOrDefault().Privilege);
            Assert.AreEqual(0, gp.Projects.Count());
        }
        public static async Task createSecondUser()
        {
            await client.PostAsJsonAsync<CreateUserOut>("user/createuser", new CreateUserIn
            {
                Email = u2.Email,
                Password = u2.Password,
                FirstName = u2.FirstName,
                LastName = u2.LastName,
                Deviceid = deviceid2
            });

            fs.WriteLine("deviceid2: " + deviceid2);
            fs.WriteLine("sessionid2: " + sessionid2);
        }
        public static async Task addSecondUserToGroup()
        {
            await client.PostAsJsonAsync("group/adduser", new AddGroupUserIn
            {
                GroupName = g.Name,
                UserEmail = u2.Email,
                Privilege = 2
            });

            var gp = await client.PostAsJsonAsync<GroupPreview>("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(g.Name, gp.Name);
            Assert.AreEqual(u.Email, gp.Users.FirstOrDefault().Email);
            Assert.AreEqual(2, gp.Users.Where(x => x.Email.Equals(u2.Email)).FirstOrDefault().Privilege);
        }
        public static async Task changeGroupMemberPriviledge()
        {
            await client.PostAsJsonAsync("group/changerole", new EditGroupMemberIn
            {
                GroupName = g.Name,
                UserEmail = u2.Email,
                Role = 1
            });
            var gp = await client.PostAsJsonAsync<GroupPreview>("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(gp.Users.Where(x => x.Email.Equals(u2.Email)).First().Privilege, 1);

        }
        public static async Task removeUserFromGroup()
        {
            await client.PostAsJsonAsync("group/removeuser", new RemoveGroupUserIn
            {
                GroupName = g.Name,
                UserEmail = u2.Email
            });
            var gp = await client.PostAsJsonAsync<GroupPreview>("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(gp.Users.Count(), 1);
        }
        public static async Task createProtocol()
        {
            var r = await client.PostAsJsonAsync<Protocolid>("protocol/share", new CreateProtocol
            {
                Name = protocol.Name,
                Isprivate = protocol.Isprivate,
                Comment = protocol.Comment,
                Istudy = protocol.IStudy,
                Usedefaultanat = protocol.UseDefaultAnat,
                Usedefaultchannel = protocol.UseDefaultChannel
            });
            protocol.Id = r.Id;
            client.DefaultRequestHeaders.Add("protocolid", protocol.Id.ToString());
        }
        public static async Task getProtocol()
        {
            var data = await client.GetAsJsonAsync<ProtocolData>($"protocol/get/{protocol.Id}");
            Assert.AreEqual(data.Name, protocol.Name);
            Assert.AreEqual(data.UseDefaultAnat, protocol.UseDefaultAnat);
            Assert.AreEqual(data.UseDefaultChannel, protocol.UseDefaultChannel);
            Assert.AreEqual(data.Comment, protocol.Comment);
        }
        public static async Task createStudy()
        {
            var studyid= await client.PostAsJsonAsync<string>("study/create", new bst.Model.StudyData
            {
                Filename = study.Filename,
                Name = study.Name,
                Condition = study.Condition,
                DateOfStudy = study.DateOfStudy,
                IChannel = study.IChannel,
                IHeadModel = study.IHeadModel,
                ProtocolId = study.Protocol.Id,
                
            });
            study.Id = Guid.Parse(studyid);
        }
        public static async Task createChannel()
        {
            var uploadid = await client.PostAsJsonAsync<string>($"functionalfile/createchannel", new bst.Model.ChannelData
            {
                Comment = ff.Comment,
                studyID = ff.Study.Id,
                type = ff.FileType,
                NbChannels = channel.NbChannels,
                TransfEegLabels = channel.TransfEegLabels,
                TransfMegLabels = channel.TransfMegLabels,
                FileName = ff.FileName
            });
            string t = "test1234\n";
            var r = await client.PostAsync($"file/upload/{uploadid}/false", new ByteArrayContent(System.Text.Encoding.ASCII.GetBytes(t)));
            var r2 = await client.PostAsync($"file/upload/{uploadid}/true", new ByteArrayContent(System.Text.Encoding.ASCII.GetBytes(t)));
            //Console.WriteLine($"\n\tto be tested upload file using id {uploadid}\n\t device id is {deviceid} \n\t sessionid is {sessionid}");
        }
        public static string randomstr()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        }

        public static void call(MethodInfo f)
        {
            lasttime = System.DateTime.Now;
            try
            {
                Task t = (Task)f.Invoke(null, null);
                t.Wait();
                Console.WriteLine($"{f.Name} \tpassed using {(System.DateTime.Now - lasttime).TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine(f.Name + "\t"+ex.Message);
            }
            
        }
    }
}
