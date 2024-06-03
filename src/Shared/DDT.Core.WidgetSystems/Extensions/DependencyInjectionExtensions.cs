using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Extensions;

public static class DependencyInjectionExtensions
{
    private static Type? constantCallSiteType;
    private static Type? serviceIdentifierType;
    private static ConstructorInfo? constantCallSiteConstructor;
    private static MethodInfo? fromServiceTypeMethod;

    static DependencyInjectionExtensions()
    {
        constantCallSiteType = typeof(ServiceProvider).Assembly.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.ConstantCallSite");
        serviceIdentifierType = typeof(ServiceProvider).Assembly.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.ServiceIdentifier");

        constantCallSiteConstructor = constantCallSiteType.GetConstructor([typeof(Type), typeof(object)]);
        fromServiceTypeMethod = serviceIdentifierType.GetMethod("FromServiceType");
    }

    public static void AddScopedPostBuild<TService>(this ServiceProvider serviceProvider, Func<IServiceProvider, TService> implementationFactory) where TService : class
    {
        // TODO:
        //var serviceType = typeof(TService);
        //var callSiteFactory = serviceProvider.GetPrivatePropertyValue<object>("CallSiteFactory");
        //var serviceIdentifier = fromServiceTypeMethod.Invoke(null, [(object)typeof(TService)]);
        //var objImplementation = implementationFactory(serviceProvider);
        //var callSite = constantCallSiteConstructor.Invoke([typeof(TService), objImplementation]);

        //callSiteFactory.CallMethod("Add", serviceIdentifier, callSite);
    }
}
