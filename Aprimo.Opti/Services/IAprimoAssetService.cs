using Aprimo.Opti.Models.RestImpl;
using Aprimo.Opti.Models.RestImpl.Search;
using Aprimo.Opti.RestImpl;
using EPiServer.Core;

namespace Aprimo.Opti.Services
{
    public interface IAprimoAssetService
    {
        Asset GetAsset(string id);

        AssetList SearchAsset(SearchRequest searchRequest);

        void AddOrUpdate(string id, AddUpdateRecordRequest record);

        void CreatePermalinkForAprimo(string id, ContentReference contentReference, ContentReference referenceContentLink);
    }
}