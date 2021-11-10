using System;
using System.Collections.Generic;
using System.Reflection;

namespace Aprimo.Opti.Resolvers
{
    public interface IAprimoTransformDescriptorPropertyResolver
    {
        IEnumerable<PropertyInfo> Resolve(Type type);
    }
}