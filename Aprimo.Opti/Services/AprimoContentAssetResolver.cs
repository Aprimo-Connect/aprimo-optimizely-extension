using Aprimo.Opti.Attributes;
using Aprimo.Opti.Models;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Opti.Services
{
    [ServiceConfiguration(typeof(AprimoContentAssetResolver), Lifecycle = ServiceInstanceScope.Singleton)]
    public class AprimoContentAssetResolver
    {
        private const string WildcardKey = "*";

        private Dictionary<string, IList<Type>> _extensionMappings = new Dictionary<string, IList<Type>>();

        public virtual void Initialize(ContentTypeModelRepository modelRepository)
        {
            GetMappings(modelRepository);
        }

        public virtual Type GetFirstMatching(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("extension must be given");
            }
            if (_extensionMappings.TryGetValue(EnsureNoDot(extension).ToUpperInvariant(), out var value))
            {
                return value.FirstOrDefault();
            }
            if (_extensionMappings.TryGetValue("*", out value))
            {
                return value.FirstOrDefault();
            }
            return null;
        }

        public virtual IEnumerable<Type> ListAllMatching(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("extension must be given");
            }
            List<Type> list = new List<Type>();
            if (_extensionMappings.TryGetValue(EnsureNoDot(extension).ToUpperInvariant(), out var value))
            {
                list.AddRange(value);
            }
            if (_extensionMappings.TryGetValue("*", out value))
            {
                list.AddRange(value);
            }
            return list;
        }

        private void GetMappings(ContentTypeModelRepository modelRepository)
        {
            Dictionary<string, IList<Type>> dictionary = new Dictionary<string, IList<Type>>();
            foreach (ContentTypeModel item in modelRepository.List().Where(x => typeof(AprimoAssetData).IsAssignableFrom(x.ModelType)))
            {
                var allAttributes = item.Attributes.GetAllAttributes<AprimoAssetDescriptorAttribute>();
                if (allAttributes == null || allAttributes.Count() == 0)
                {
                    if (!dictionary.TryGetValue("*", out var value))
                    {
                        value = (dictionary["*"] = new List<Type>());
                    }
                    value.Add(item.ModelType);
                    continue;
                }
                foreach (AprimoAssetDescriptorAttribute item2 in allAttributes)
                {
                    foreach (string item3 in item2.Extensions.Select((string s) => EnsureNoDot(s)))
                    {
                        string key = item3.ToUpperInvariant();
                        if (!dictionary.TryGetValue(key, out var value2))
                        {
                            value2 = dictionary[key] = new List<Type>();
                        }
                        value2.Add(item.ModelType);
                    }
                }
            }
            _extensionMappings = dictionary;
        }

        private static string EnsureNoDot(string extension)
        {
            if (extension[0] == '.')
            {
                extension = extension.Substring(1);
            }
            return extension;
        }
    }
}