﻿using AspectCore.Lite.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspectCore.Lite.Container.DependencyInjection.Test
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddAspectConfiguration_Test()
        {
            var services = new ServiceCollection();
            services.AddAspectConfiguration(c => { });
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServices = aspectCoreServiceProviderFactory.CreateBuilder(services);
            var descriptor = Assert.Single(proxyServices, d => d.ServiceType == typeof(IAspectConfiguration));
            Assert.Null(descriptor.ImplementationType);
            Assert.NotNull(descriptor.ImplementationInstance);
        }
    }
}
