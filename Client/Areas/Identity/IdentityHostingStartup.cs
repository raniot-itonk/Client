﻿//using System;
//using Client.Models;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//[assembly: HostingStartup(typeof(Client.Areas.Identity.IdentityHostingStartup))]
//namespace Client.Areas.Identity
//{
//    public class IdentityHostingStartup : IHostingStartup
//    {
//        public void Configure(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices((context, services) => {
//                services.AddDbContext<ClientContext>(options =>
//                    options.UseSqlServer(
//                        context.Configuration.GetConnectionString("ClientContextConnection")));

//                services.AddDefaultIdentity<IdentityUser>()
//                    .AddEntityFrameworkStores<ClientContext>();
//            });
//        }
//    }
//}