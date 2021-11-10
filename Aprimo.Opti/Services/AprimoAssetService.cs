using Aprimo.Opti.Models.RestImpl;
using Aprimo.Opti.Models.RestImpl.Search;
using Aprimo.Opti.RestImpl;
using Aprimo.Opti.RestImpl.Orders;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Aprimo.Opti.Services
{
    [ServiceConfiguration(typeof(IAprimoAssetService), Lifecycle = ServiceInstanceScope.Transient)]
    public class AprimoAssetService : IAprimoAssetService
    {
        private readonly ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache;

        public AprimoAssetService(ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache)
        {
            this.synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
        }

        /// <summary>
        /// Gets a single asset
        /// </summary>
        /// <param name="id">The id of the asset you are attempting to retrieve</param>
        /// <param name="selectFields">The fields you expect returned</param>
        /// <returns>Asset</returns>
        public virtual Asset GetAsset(string id)
        {
            var request = new RestRequest("api/core/record/{recordId}", Method.GET)
                .AddParameter("recordId", id, ParameterType.UrlSegment)
                .AddHeader("select-record", AprimoConstants.SelectAssetFields)
                .AddHeader("select-fileversion", "additionalfiles")
                .AddHeader("select-additionalfile", "uri,metadata");

            var asset = this.GetFromRestCached<Asset>(request, $"arpimo-asset-{id}", 5);
            return asset;
        }

        /// <summary>
        /// Search assets by providing a search expression
        /// </summary>
        /// <returns></returns>
        public virtual AssetList SearchAsset(SearchRequest searchRequest)
        {
            var request = new RestRequest("api/core/search/records", Method.POST)
               .AddHeader("select-record", AprimoConstants.SelectAssetFields)
               .AddHeader("page", searchRequest.PageIndex.ToString())
               .AddHeader("pageSize", searchRequest.PageSize.ToString())
               .AddJsonBody(searchRequest);

            var assets = this.GetFromRest<AssetList>(request);
            return assets;
        }

        /// <summary>
        /// Add or update the aprimo record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="record"></param>
        public virtual void AddOrUpdate(string id, AddUpdateRecordRequest record)
        {
            var request = new RestRequest("api/core/record/{recordId}", Method.PUT)
                .AddParameter("recordId", id, ParameterType.UrlSegment)
                .AddJsonBody(record);

            var response = this.GetFromRest<Order>(request);
        }

        /// <summary>
        /// Create a url in aprimo to notify aprimo the permalink url in Optimizely as well as update referenced uri's.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentReference"></param>
        /// <param name="referenceContentLink"></param>
        public void CreatePermalinkForAprimo(string id, ContentReference aprimoAssetContentReference, ContentReference referencedContentLink)
        {
            try
            {
                var fields = new Fields();
                var fileAsset = this.GetAsset(id);
                var permalinkField = fileAsset.Embedded.Fields.Items.FirstOrDefault(x => x.FieldName.Equals(AprimoConstants.AprimoEPiPermalinkFieldName));
                if (permalinkField != null)
                {
                    fields.Add(permalinkField.Id, $"{VirtualPathUtilityEx.RemoveTrailingSlash(SiteDefinition.Current.SiteUrl.OriginalString)}{PageEditing.GetEditUrl(aprimoAssetContentReference.ToReferenceWithoutVersion())}");
                }
                if (!ContentReference.IsNullOrEmpty(referencedContentLink))
                {
                    var referencedUriField = fileAsset.Embedded.Fields.Items.FirstOrDefault(x => x.FieldName.Equals(AprimoConstants.AprimoEPiServerReferencedUris));
                    if (referencedUriField != null)
                    {
                        var uris = fileAsset.GetFieldValues(AprimoConstants.AprimoEPiServerReferencedUris);
                        var pageUrl = UrlResolver.Current.GetUrl(referencedContentLink);
                        uris.Add(pageUrl);
                        fields.Add(referencedUriField.Id, uris);
                    }
                }
                if (fields.AddOrUpdate.Any())
                {
                    this.AddOrUpdate(id, new AddUpdateRecordRequest(fields));
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// A Generic Rest method for making calls to the Aprimo DAM api.
        /// </summary>
        /// <typeparam name="T">A class object that is returned</typeparam>
        /// <param name="request">RestSharp IRequest object</param>
        /// <returns>T which is a class</returns>
        private T GetFromRest<T>(IRestRequest request) where T : class
        {
            var auth = this.GetOrCreateToken();
            var client = new RestClient($"https://{AprimoAPISettings.AprimoTenantId}.dam.aprimo.com");

            request
                .AddHeader("Content-Type", "application/json;charset=utf-8")
                .AddHeader("Authorization", $"Bearer {auth.AccessToken}")
                .AddHeader("API-VERSION", "1")
                .AddHeader("Accept", "application/hal+json");

            var response = client.Execute(request);
            if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created) && !string.IsNullOrWhiteSpace(response.Content))
            {
                var responseItem = JsonConvert.DeserializeObject<T>(response.Content);
                return responseItem;
            }
            return null;
        }

        private T GetFromRestCached<T>(IRestRequest request, string cacheKey, int cachedMinutes) where T : class
        {
            var cache = this.synchronizedObjectInstanceCache.Get<T>(cacheKey, ReadStrategy.Immediate);
            if (cache == null)
            {
                var auth = this.GetOrCreateToken();
                var client = new RestClient($"https://{AprimoAPISettings.AprimoTenantId}.dam.aprimo.com");

                request
                    .AddHeader("Content-Type", "application/json;charset=utf-8")
                    .AddHeader("Authorization", $"Bearer {auth.AccessToken}")
                    .AddHeader("API-VERSION", "1")
                    .AddHeader("Accept", "application/hal+json");

                var response = client.Execute(request);
                if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created) && !string.IsNullOrWhiteSpace(response.Content))
                {
                    var responseItem = JsonConvert.DeserializeObject<T>(response.Content);
                    cache = responseItem;
                    this.synchronizedObjectInstanceCache.Insert(cacheKey, cache, new CacheEvictionPolicy(TimeSpan.FromMinutes(cachedMinutes), CacheTimeoutType.Absolute));
                }
            }
            return cache;
        }

        /// <summary>
        /// Creates or gets an Arimo Access token for making api calls.
        /// </summary>
        /// <returns>APrimoAuthorization Object</returns>
        private TokenAuthorization GetOrCreateToken()
        {
            var aprimoAuthorization = this.synchronizedObjectInstanceCache.Get<TokenAuthorization>(AprimoAPISettings.ClientId, ReadStrategy.Immediate);
            if (aprimoAuthorization != null)
                return aprimoAuthorization;

            Dictionary<string, string> entries = new Dictionary<string, string>(){
                {"grant_type","client_credentials"},
                {"scope","api"},
                {"client_id",AprimoAPISettings.ClientId},
                {"client_secret",AprimoAPISettings.ClientSecret}
            };

            var client = new RestClient($"https://{AprimoAPISettings.AprimoTenantId}.aprimo.com");
            var request = new RestRequest("/login/connect/token", Method.POST);
            request.AddParameter(new Parameter(string.Empty,
                string.Join("&", entries.Select(x => $"{x.Key}={x.Value}")),
                "application/x-www-form-urlencoded",
                ParameterType.RequestBody,
                true));

            var response = client.Execute<TokenAuthorization>(request);
            if (response.IsSuccessful)
            {
                this.synchronizedObjectInstanceCache.Insert(AprimoAPISettings.ClientId, response.Data, new CacheEvictionPolicy(TimeSpan.FromMinutes(9), CacheTimeoutType.Absolute));
                return response.Data;
            }

            return null;
        }
    }
}