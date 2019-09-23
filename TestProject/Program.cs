using System;
using bst.Controllers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;


namespace TestProject
{
    class Program
    {
        #region variables
        static DateTime starttime;
        static DateTime lasttime;
        static StreamWriter fs = new StreamWriter("testdata.json");
        static HttpClient client;
        static HttpClient client2;

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
        #endregion
        static void Main(string[] args)
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost/");
            client2 = new HttpClient();
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
            var data = await client.PostAsJsonAsync("user/createuser", p);
            Assert.AreEqual(data.StatusCode, HttpStatusCode.OK);
            var userdata = await data.Content.ReadAsAsync<CreateUserOut>();
            sessionid = userdata.Sessionid.ToString();
            Assert.AreEqual(u.FirstName, userdata.Firstname);
            Assert.AreEqual(u.LastName, userdata.Lastname);
            Assert.AreEqual(u.Email, userdata.Email);

            fs.WriteLine("deviceid: " + deviceid);
        }
        public static async Task login()
        {
            var r1 = await client.PostAsJsonAsync("user/login", new LoginIn
            {
                Email = u.Email,
                Password = "fjoidasjfo",
                Deviceid = deviceid
            });
            Assert.AreEqual(r1.StatusCode, HttpStatusCode.Unauthorized);
            var r2 = await client.PostAsJsonAsync("user/login", new LoginIn
            {
                Email = u.Email,
                Password = u.Password,
                Deviceid = deviceid
            });
            sessionid = (await r2.Content.ReadAsAsync<LoginOut>()).Sessionid.ToString();
            fs.WriteLine("sessionid: " + sessionid);
        }
        public static async Task failauthentication()
        {
            var r = await client.PostAsJsonAsync("user/listgroups", new ListCount
            {
                Start = 0,
                Count = 10,
                Order = 0
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.Unauthorized);
            client.DefaultRequestHeaders.Add("sessionid", sessionid);
            client.DefaultRequestHeaders.Add("deviceid", deviceid);

        }
        public static async Task listprotocols()
        {
            var r = await client.PostAsJsonAsync("user/listprotocols", new ListCount
            {
                Start = 0,
                Count = 10,
                Order = 0
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            var protocols = await r.Content.ReadAsAsync<List<ProtocolData>>();
            if (protocols.Count == 0)
            {
                return;
            }
            Assert.Fail();
        }
        public static async Task listgroups()
        {
            var r = await client.PostAsJsonAsync("user/listgroups", new ListCount
            {
                Start = 0,
                Count = 10,
                Order = 0
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            var groups = await r.Content.ReadAsAsync<List<GroupPreview>>();
            if (groups.Count == 0)
            {
                return;
            }
            Assert.Fail();
        }
        public static async Task creategroup()
        {
            var r = await client.PostAsJsonAsync("group/create", new CreateGroupIn
            {
                Name = g.Name
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            var gp = await r.Content.ReadAsAsync<GroupPreview>();
            Assert.AreEqual(g.Name, gp.Name);
            Assert.AreEqual(u.Email, gp.Users.FirstOrDefault().Email);
            Assert.AreEqual(1, gp.Users.FirstOrDefault().Privilege);
            Assert.AreEqual(0, gp.Projects.Count());
        }
        public static async Task groupdetail()
        {
            var r = await client.PostAsJsonAsync("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            var gp = await r.Content.ReadAsAsync<GroupPreview>();
            Assert.AreEqual(g.Name, gp.Name);
            Assert.AreEqual(u.Email, gp.Users.FirstOrDefault().Email);
            Assert.AreEqual(1, gp.Users.FirstOrDefault().Privilege);
            Assert.AreEqual(0, gp.Projects.Count());
        }
        public static async Task createSecondUser()
        {
            var r = await client.PostAsJsonAsync("user/createuser", new CreateUserIn
            {
                Email = u2.Email,
                Password = u2.Password,
                FirstName = u2.FirstName,
                LastName = u2.LastName,
                Deviceid = deviceid2
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            sessionid2 = (await r.Content.ReadAsAsync<CreateUserOut>()).Sessionid.ToString();

            fs.WriteLine("deviceid2: " + deviceid2);
            fs.WriteLine("sessionid2: " + sessionid2);
        }
        public static async Task addSecondUserToGroup()
        {
            var r = await client.PostAsJsonAsync("group/adduser", new AddGroupUserIn
            {
                GroupName = g.Name,
                UserEmail = "c" + u2.Email,
                Privilege = 2
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.NotFound);
            var r2 = await client.PostAsJsonAsync("group/adduser", new AddGroupUserIn
            {
                GroupName = g.Name,
                UserEmail = u2.Email,
                Privilege = 2
            });
            Assert.AreEqual(r2.StatusCode, HttpStatusCode.OK);

            var r3 = await client.PostAsJsonAsync("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(r3.StatusCode, HttpStatusCode.OK);
            var gp = await r3.Content.ReadAsAsync<GroupPreview>();
            Assert.AreEqual(g.Name, gp.Name);
            Assert.AreEqual(u.Email, gp.Users.FirstOrDefault().Email);
            Assert.AreEqual(2, gp.Users.Where(x => x.Email.Equals(u2.Email)).FirstOrDefault().Privilege);
        }
        public static async Task changeGroupMemberPriviledge()
        {
            var r2 = await client2.PostAsJsonAsync("group/changerole", new AddGroupUserIn
            {
                GroupName = g.Name,
                UserEmail = u.Email,
                Privilege = 2
            });
            Assert.AreEqual(r2.StatusCode, HttpStatusCode.Unauthorized);
            var r = await client.PostAsJsonAsync("group/changerole", new EditGroupMemberIn
            {
                GroupName = g.Name,
                UserEmail = u2.Email,
                Role = 1
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            var r3 = await client.PostAsJsonAsync("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(r3.StatusCode, HttpStatusCode.OK);
            var gp = await r3.Content.ReadAsAsync<GroupPreview>();
            Assert.AreEqual(gp.Users.Where(x => x.Email.Equals(u2.Email)).First().Privilege, 1);

        }
        public static async Task removeUserFromGroup()
        {
            var r = await client.PostAsJsonAsync("group/removeuser", new RemoveGroupUserIn
            {
                GroupName = g.Name,
                UserEmail = u2.Email
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            r = await client.PostAsJsonAsync("group/detail", new GroupName
            {
                Name = g.Name
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            var gp = await r.Content.ReadAsAsync<GroupPreview>();
            Assert.AreEqual(gp.Users.Count(), 1);
        }
        public static async Task createProtocol()
        {
            var r = await client.PostAsJsonAsync("protocol/share", new CreateProtocol
            {
                Name = protocol.Name,
                Isprivate = protocol.Isprivate,
                Comment = protocol.Comment,
                Istudy = protocol.IStudy,
                Usedefaultanat = protocol.UseDefaultAnat,
                Usedefaultchannel = protocol.UseDefaultChannel
            });
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            protocol.Id = (await r.Content.ReadAsAsync<Protocolid>()).Id;

        }
        public static async Task getProtocol()
        {
            var r = await client.GetAsync($"protocol/get/{protocol.Id}");
            Assert.AreEqual(r.StatusCode, HttpStatusCode.OK);
            var data = (await r.Content.ReadAsAsync<ProtocolData>());
            Assert.AreEqual(data.Name, protocol.Name);
            Assert.AreEqual(data.UseDefaultAnat, protocol.UseDefaultAnat);
            Assert.AreEqual(data.UseDefaultChannel, protocol.UseDefaultChannel);
            Assert.AreEqual(data.Comment, protocol.Comment);
            r = await client.GetAsync($"protocol/get/{Guid.Empty}");
            Assert.AreEqual(r.StatusCode, HttpStatusCode.NotFound);
        }
        public static async Task checkProtocolGroup()
        {

        }

        public static string randomstr()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        }

        public static void call(MethodInfo f)
        {
            lasttime = System.DateTime.Now;
            f.Invoke(null, null);
            Console.WriteLine($"{f.Name} passed using {(System.DateTime.Now-lasttime).TotalMilliseconds}ms");
        }
    }
}
