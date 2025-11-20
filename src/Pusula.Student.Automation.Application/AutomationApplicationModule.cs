using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pusula.Student.Automation.Workers;
using StackExchange.Redis;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Pusula.Student.Automation;

[DependsOn(
    typeof(AutomationDomainModule),
    typeof(AutomationApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpBackgroundJobsRabbitMqModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpBackgroundWorkersModule)
    )]
public class AutomationApplicationModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<PeriodicSetCourseStatusWorker>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "PTH:";
        });

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AutomationApplicationModule>();
        });

        context.Services.Replace(ServiceDescriptor.Transient<IBackgroundJobManager, DefaultBackgroundJobManager>());

        Configure<AbpBackgroundJobWorkerOptions>(options =>
        {
            options.DefaultFirstWaitDuration = 10;
            options.DefaultTimeout = 86400;
        });

        var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);

        context.Services
        .AddDataProtection()
        .SetApplicationName("PTH")
            .PersistKeysToStackExchangeRedis(redis, "PTH-Protection-Keys");

        context.Services.AddSingleton<IDistributedLockProvider>(_ => new RedisDistributedSynchronizationProvider(redis.GetDatabase()));
    }
}
