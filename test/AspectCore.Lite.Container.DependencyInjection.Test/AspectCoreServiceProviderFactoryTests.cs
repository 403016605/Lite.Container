﻿using AspectCore.Lite.Container.Test.Fakes;
using Microsoft.Extensions.DependencyInjection;
using AspectCore.Lite.Abstractions.Extensions;
using System;
using System.Reflection;
using Xunit;
using AspectCore.Lite.Abstractions;

namespace AspectCore.Lite.Container.DependencyInjection.Test
{
    public class AspectCoreServiceProviderFactoryTests
    {
        [Fact]
        public void CreateBuilderWithProxy_Test()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service>();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServices = aspectCoreServiceProviderFactory.CreateBuilder(services);
            var descriptor = Assert.Single(proxyServices, d => d.ServiceType == typeof(IService));
            Assert.NotNull(descriptor.ImplementationType);
            Assert.Equal(descriptor.Lifetime, ServiceLifetime.Transient);
            Assert.IsNotType<Service>(descriptor.ImplementationType);
            Assert.True(descriptor.ImplementationType.GetTypeInfo().BaseType == typeof(Service));
            Assert.True(descriptor.ImplementationType.GetTypeInfo().IsDynamically());
        }

        [Theory]
        [InlineData(typeof(IAspectActivator))]
        [InlineData(typeof(IAspectBuilder))]
        [InlineData(typeof(IAspectConfiguration))]
        [InlineData(typeof(IAspectValidator))]
        [InlineData(typeof(IInterceptorInjector))]
        [InlineData(typeof(IInterceptorMatcher))]
        [InlineData(typeof(IProxyGenerator))]
        public void CreateBuilderWithAspectCoreServices_Test(Type serviceType)
        {
            var services = new ServiceCollection();
            var aspectCoreServiceProviderFactory = new AspectCoreServiceProviderFactory();
            var proxyServices = aspectCoreServiceProviderFactory.CreateBuilder(services);
            var descriptor = Assert.Single(proxyServices, d => d.ServiceType == serviceType);
            Assert.False(descriptor.ImplementationType.GetTypeInfo().IsDynamically());
        }
    }
}
