using Aprimo.Opti.Models;
using Aprimo.Opti.Models.RestImpl;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aprimo.Opti
{
    public static class AprimoAssetExtensions
    {
        private static Injected<IContentLoader> ContentLoader;

        public static string GetAprimoUrl(this ContentReference contentReference)
        {
            if (!ContentReference.IsNullOrEmpty(contentReference))
            {
                if (contentReference.ProviderName != null && contentReference.ProviderName.Equals(AprimoConstants.ProviderKey))
                {
                    var type = ContentLoader.Service.Get<AprimoAssetData>(contentReference);
                    return type.Url;
                }
            }
            return UrlResolver.Current.GetUrl(contentReference);
        }

        public static string GetAprimoUrl(this Url url)
        {
            if (url != null)
            {
                var content = UrlResolver.Current.Route(new UrlBuilder(url), ContextMode.Default);
                if (content != null)
                {
                    if (content is AprimoAssetData assetData)
                    {
                        return assetData.Url;
                    }
                    return UrlResolver.Current.GetUrl(content.ContentLink);
                }
            }
            return string.Empty;
        }

        public static string GetAprimoUrl(this ContentReference contentReference, string propertyName)
        {
            if (!ContentReference.IsNullOrEmpty(contentReference))
            {
                if (contentReference.ProviderName != null && contentReference.ProviderName.Equals(AprimoConstants.ProviderKey))
                {
                    var type = ContentLoader.Service.Get<AprimoAssetData>(contentReference);
                    return type.GetPropertyValue(propertyName);
                }
            }
            return UrlResolver.Current.GetUrl(contentReference);
        }

        public static string GetFieldValue(this Asset asset, string fieldName)
        {
            if (asset.Embedded.Fields.Items.Any())
            {
                var field = asset.Embedded.Fields.Items.FirstOrDefault(x => x.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (field != null && field.LocalizedValues.Any())
                {
                    var localizedValue = field.LocalizedValues.FirstOrDefault()?.Value;
                    if (!string.IsNullOrWhiteSpace(localizedValue))
                        return localizedValue;
                }
            }
            return string.Empty;
        }

        public static List<string> GetFieldValues(this Asset asset, string fieldName)
        {
            var list = new List<string>();
            if (asset.Embedded.Fields.Items.Any())
            {
                var field = asset.Embedded.Fields.Items.FirstOrDefault(x => x.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                if (field != null && field.LocalizedValues.Any())
                {
                    var localizedValues = field.LocalizedValues.FirstOrDefault()?.Values;
                    if (localizedValues.Any())
                        list = localizedValues;
                }
            }
            return list;
        }

        public static TValue GetAttributValue<TAttribute, TValue>(this PropertyInfo prop, Func<TAttribute, TValue> value) where TAttribute : Attribute
        {
            if (prop.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att)
            {
                return value(att);
            }
            return default;
        }
    }
}