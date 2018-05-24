using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using PlateDetector.Detection;
using System.Drawing;
using DetectionAPI.Database;
using DetectionAPI.Database.Entities;
using DbAndLogging.Database;

namespace DbAndLogging
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("Creating context");
                var context = new ApiDbContext();
                Console.WriteLine("Created");


                var usersRepository = new UsersRepository(context);

                var res = usersRepository.FetchRecords();

                Console.WriteLine();
                Console.WriteLine("All users");
                foreach (var r in res)
                {
                    Console.WriteLine(r.ToString());
                }
                Console.WriteLine();


                var newUser = new User()
                {
                    AccessToken = "token1234",
                    CreationTime = DateTime.Now,
                    SessionId = 3,
                    Password = "12345qwerty",
                    Username = "oqewok@gmail.com",
                    UserType = 0
                };


                var insertResult = usersRepository.AddRecord(newUser);
                Console.WriteLine(insertResult);



                Console.WriteLine();
                Console.WriteLine();

                var certain_user = context
                    .Set<User>()
                    .Where(e => e.Username == "oqewok@gmail.com")
                    .First();

                if (certain_user != null)
                {


                    Console.WriteLine("Addind new session");

                    var newSession = new Session()
                    {
                        CreationTime = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddMonths(1),
                        ImageCount = 0,
                        IsLimitReached = false,
                        PlatesCount = 0,
                        SessionType = certain_user.UserType,
                        UserId = certain_user.Id,
                        User = certain_user
                    };

                    context.Sessions.Add(newSession);
                    context.SaveChanges();

                }


                Console.WriteLine();
                Console.WriteLine("Eager fetch?");
                Console.WriteLine();
                Console.WriteLine();

                var AllUsers = context.Set<User>().ToList();

                foreach(var u in AllUsers)
                {
                    Console.WriteLine(u.ToString());
                }


                Console.ReadKey();


                Console.WriteLine();
                Console.WriteLine("Show all sessions");
                Console.WriteLine();
                Console.WriteLine();


                var allSession = context.Set<Session>().ToList();

                foreach(var s in allSession)
                {
                    Console.WriteLine(s.ToString());
                }


                Console.WriteLine();
                Console.WriteLine($@"Show session with UserId = {certain_user.Id}");
                Console.WriteLine();
                Console.WriteLine();

                var certainSession = context.Set<Session>().Where(p => p.UserId == certain_user.Id).ToList();

                foreach(var a in certainSession)
                {
                    Console.WriteLine(a.ToString());
                }

                Console.ReadKey();

                Console.WriteLine();
                Console.WriteLine("Checking limit");

                var checkingUser = context
                    .Set<User>()
                    .Where(e => e.Username == "oqewok@gmail.com")
                    .First();

                long i = checkingUser.Id;

                var checkingSession = context.Set<Session>().Where(p => p.UserId == i).ToList().LastOrDefault();

                Console.WriteLine();
                if (checkingSession != null)
                {
                    var limit = checkingSession.IsLimitReached;
                    var limitCount = checkingSession.ImageCount;

                    checkingSession.IsLimitReached = true;

                    Console.WriteLine("Limit reached : true");
                    context.SaveChanges();
                }

                var newImage = new ImageInfo
                {
                    ImagePath = "ddsf",
                    MarkupPath = "dsfdf",

                    PlatesCount = 1,
                    SessionId = checkingSession.Id,
                    UploadDate = DateTime.Now,
                    UserId = checkingSession.UserId
                };

                context.Images.Add(newImage);
                context.SaveChanges();

            }

            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);

                Console.ReadKey();
            }
           

        }
    }
}
