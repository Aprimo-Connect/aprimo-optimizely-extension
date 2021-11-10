using Aprimo.Opti.AprimoPersistance;
using EPiServer.Core;
using System;
using System.Collections.Generic;

namespace Aprimo.Opti.Services
{
    public interface IAprimoAssetPersistantService
    {
        void CheckDBConsistancy(string version);

        void Delete(int id);

        void DeleteAll();

        IEnumerable<AprimoPersistantAsset> GetAll();

        IEnumerable<AprimoPersistantAsset> GetAssets(Guid assetId);

        IEnumerable<AprimoPersistantAsset> Search(string query);

        AprimoPersistantAsset GetAsset(int id);

        AprimoPersistantAsset GetAsset(ContentReference contentReference);

        AprimoPersistantAsset GetAsset(Guid assetId, string renditionId);

        AprimoPersistantAsset GetAsset(string renditionId);

        AprimoPersistantAsset Save(AprimoPersistantAsset asset);

        AprimoPersistantAsset Update(AprimoPersistantAsset asset);
    }
}