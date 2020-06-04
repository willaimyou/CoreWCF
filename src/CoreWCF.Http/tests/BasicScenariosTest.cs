﻿using CoreWCF.Configuration;
using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ServiceModel.Channels;
using Xunit;
using Xunit.Abstractions;

namespace CoreWCF.Http.Tests
{
    public class BasicScenariosTest
    {
        private ITestOutputHelper _output;

        public BasicScenariosTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BasicScenariosAndOps()
        {
            var host = ServiceHelper.CreateWebHostBuilder<Startup>(_output).Build();
            using (host)
            {
                host.Start();
                var httpBinding = ClientHelper.GetBufferedModeBinding();
                var factory = new System.ServiceModel.ChannelFactory<ClientContract.ITestBasicScenarios>(httpBinding,
                    new System.ServiceModel.EndpointAddress(new Uri("http://localhost:8080/BasicWcfService/ITestBasicScenariosService.svc")));
                var channel = factory.CreateChannel();

                var factory2 = new System.ServiceModel.ChannelFactory<ClientContract.ITestBasicScenariosClientService>(httpBinding,
                    new System.ServiceModel.EndpointAddress(new Uri("http://localhost:8080/BasicWcfService/ITestBasicScenariosService.svc")));
                var channel2 = factory2.CreateChannel();

                //Variation string TestMethodDefaults
                    int ID = 1;
                    string name = "Defaults";
                    string result = channel.TestMethodDefaults(ID, name);
                    Assert.NotNull(result);
                    Assert.Equal(result, name);

                //Variation_void TestMethodSetAction
                    ID = 1;
                    name = "Action";
                    channel.TestMethodSetAction(ID, name);

                //Variation_int TestMethodSetReplyAction               
                    ID = 1;
                    name = "ReplyAction";
                    int resultInt = channel.TestMethodSetReplyAction(ID, name);                    
                    Assert.Equal(resultInt, ID);
                
                //Variation_void TestMethodUntypedAction                
                    Message clientMessage = Message.CreateMessage(MessageVersion.Soap11, "myUntypedAction");
                    channel.TestMethodUntypedAction(clientMessage);

                //Variation_Message TestMethodUntypedreplyAction                
                    Message msg = channel.TestMethodUntypedReplyAction();
                    Assert.NotNull(msg);                

                //Variation_void TestMethodSetUntypedAction                                  
                    Message clientUntypedActionMessage = Message.CreateMessage(MessageVersion.Soap11, "mySetUntypedAction");
                    channel.TestMethodSetUntypedAction(clientUntypedActionMessage);                

                //Variation_sting TestMethodasync                
                    ID = 1;
                    name = "Async";
                    result = channel2.TestMethodAsync(ID, name).GetAwaiter().GetResult();
                    Assert.NotNull(result);
                    Assert.Equal(result, name);              
            }            
        }

        internal class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseServiceModel(builder =>
                {
                    builder.AddService<Services.TestBasicScenariosService>();
                    builder.AddServiceEndpoint<Services.TestBasicScenariosService, ServiceContract.ITestBasicScenarios>(new BasicHttpBinding(), "/BasicWcfService/ITestBasicScenariosService.svc");
                });
            }
        }
    }

   }
