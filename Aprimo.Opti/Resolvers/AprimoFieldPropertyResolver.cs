using Aprimo.Opti.Attributes;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aprimo.Opti.Resolvers
{
    [ServiceConfiguration(typeof(IAprimoFieldPropertyResolver), IncludeServiceAccessor = false, Lifecycle = ServiceInstanceScope.Singleton)]
    public class AprimoFieldPropertyResolver : IAprimoFieldPropertyResolver
    {
        private readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _descriptorPropertyInfos = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        public IEnumerable<PropertyInfo> Resolve(Type type) => _descriptorPropertyInfos.GetOrAdd(type, delegate
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (Attribute.GetCustomAttributes(propertyInfo, typeof(AprimoFieldNameAttribute), inherit: true).Any())
                {
                    list.Add(propertyInfo);
                }
            }
            return list;
        });
    }
}