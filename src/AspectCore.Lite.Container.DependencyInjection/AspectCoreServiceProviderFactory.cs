﻿using AspectCore.Lite.Abstractions;
using AspectCore.Lite.Abstractions.Resolution;
using AspectCore.Lite.Abstractions.Resolution.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace AspectCore.Lite.Container.DependencyInjection
{
    public class AspectCoreServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.TryAddAspectCoreLite().BuildServiceProvider();

            var aspectValidator = serviceProvider.GetRequiredService<IAspectValidator>();

            var dynamicProxyServices = new ServiceCollection();

            var generator = new TypeGeneratorWrapper();

            foreach (var descriptor in serviceCollection)
            {
                if (!Validate(descriptor, aspectValidator))
                {
                    dynamicProxyServices.Add(descriptor);
                    continue;
                }
                var proxyType = generator.CreateType(descriptor.ServiceType, descriptor.ImplementationType, aspectValidator);
                dynamicProxyServices.Add(ServiceDescriptor.Describe(descriptor.ServiceType, proxyType, descriptor.Lifetime));
            }

            dynamicProxyServices.AddScoped<ISupportOriginalService>(p =>
            {
                var scopedProvider = serviceProvider.GetService<IServiceScopeFactory>().CreateScope().ServiceProvider;
                return new SupportOriginalService(scopedProvider);
            });

            return dynamicProxyServices;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return CreateBuilder(containerBuilder).BuildServiceProvider();
        }

        private static bool Validate(ServiceDescriptor serviceDescriptor, IAspectValidator aspectValidator)
        {
            var implementationType = serviceDescriptor.ImplementationType;
            if (implementationType == null)
            {
                return false;
            }

            if (!serviceDescriptor.ServiceType.GetTypeInfo().ValidateAspect(aspectValidator))
            {
                return false;
            }

            if (!serviceDescriptor.ImplementationType.GetTypeInfo().CanInherited())
            {
                return false;
            }

            return true;
        }
    }
}