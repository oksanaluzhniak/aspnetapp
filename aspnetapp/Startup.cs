using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetapp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/IsLoginFree", IsLoginFree);
            app.Map("/AllLogins", AllLogins);
            app.Map("/AddLogin", AddLogin);
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Docker in OS LINUX");
            });
        }

        public static bool IsFree(string login)
        {
            bool isFree = true;
            using (StreamReader fileObj = new StreamReader("/user/src/app/shared_folder/users.txt"))
            {
                string s = fileObj.ReadLine();
                while (s != null)
                {
                    if (s == login)
                    {
                        isFree = false;
                    }

                    s = fileObj.ReadLine();
                }
            }
            return isFree;
        }
        public static string NewLogin(string login)
        {
            string text = "";
            if (IsFree(login))
            {
                using (StreamWriter fileObj = new StreamWriter("/user/src/app/shared_folder/users.txt", true))
                {
                    fileObj.WriteLine(login);
                }
                text = string.Format("Login {0} is succesful added", login);
            }
            else
            {
                text = string.Format("Login {0} is alredy exist", login);
            }

            return text;
        }
        public static List<string> Users()
        {
            List<string> users = new List<string>();
            using (StreamReader fileObj = new StreamReader("/user/src/app/shared_folder/users.txt"))
            {
                string line = fileObj.ReadLine();
                
                while (line != null)
                {
                    users.Add(line);
                    line = fileObj.ReadLine();                    
                }
            }
            return users;
        }
        private static void IsLoginFree(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                string login = context.Request.Query["login"];
                await context.Response.WriteAsync(JsonConvert.SerializeObject(IsFree(login)));
            });
        }
        private static void AddLogin(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var request = context.Request.Body;
                string login;
                using (StreamReader reader = new StreamReader(request))
                {
                    string body = await reader.ReadToEndAsync();
                    login = JObject.Parse(body)["login"].ToString();
                }
                await context.Response.WriteAsync(JsonConvert.SerializeObject(NewLogin(login)));
            });
        }
        private static void AllLogins(IApplicationBuilder app)
        {
            app.Run(async context =>
            {

                await context.Response.WriteAsync(JsonConvert.SerializeObject(Users()));
            });
        }
    }
}
