﻿using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpDemo.Configuration;
using AbpDemo.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Abp.AspNetCore.SignalR;

namespace AbpDemo.Web.Startup
{
    [DependsOn(
        typeof(AbpDemoApplicationModule), 
        typeof(AbpDemoEntityFrameworkCoreModule),
        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpAspNetCoreModule))]
    public class AbpDemoWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AbpDemoWebModule(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(AbpDemoConsts.ConnectionStringName);

            Configuration.Navigation.Providers.Add<AbpDemoNavigationProvider>();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AbpDemoApplicationModule).GetAssembly(),
                "demo",true);
            Configuration.Modules.AbpAspNetCore().UseMvcDateTimeFormatForAppServices = true;//使用MVC时间格式
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpDemoWebModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(AbpDemoWebModule).Assembly);
        }
    }
}