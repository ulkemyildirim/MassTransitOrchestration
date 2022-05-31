using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaStateMachine.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(configure =>
                    {
                        configure.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
                              .EntityFrameworkRepository(options =>
                              {
                                  options.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                                  {
                                      builder.UseSqlServer(Shared.SqlServerConsts.SqlServerConn);
                                  });
                              });

                        configure.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            cfg.Host(Shared.RabbitMqConsts.RabbitMqUri, def =>
                            {
                                def.Username(Shared.RabbitMqConsts.UserName);
                                def.Password(Shared.RabbitMqConsts.Password);
                            }
                            );

                            cfg.ReceiveEndpoint(Shared.RabbitMQSettings.StateMachine, e =>
                                    e.ConfigureSaga<OrderStateInstance>(provider));
                        }));
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
