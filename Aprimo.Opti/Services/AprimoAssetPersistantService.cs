using Aprimo.Opti.AprimoPersistance;
using Dapper;
using EPiServer.Core;
using EPiServer.Framework.Configuration;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aprimo.Opti.Services
{
    [ServiceConfiguration(typeof(IAprimoAssetPersistantService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class AprimoAssetPersistantService : IAprimoAssetPersistantService
    {
        private readonly string connectionString;

        private IDbConnection PersistantSqlConnection => new SqlConnection(connectionString);

        public AprimoAssetPersistantService()
        {
            this.connectionString = GlobalConfigurationManager.Load().ConnectionStrings.ConnectionStrings["EPiServerDB"].ConnectionString;
        }

        public IEnumerable<AprimoPersistantAsset> GetAll()
        {
            using (var connection = PersistantSqlConnection)
                return connection.Query<AprimoPersistantAsset>(AprimoDBConstants.GetAllAssets, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<AprimoPersistantAsset> GetAssets(Guid assetId)
        {
            using (var connection = PersistantSqlConnection)
                return connection.Query<AprimoPersistantAsset>(AprimoDBConstants.GetAssetsByIds, new { assetId }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<AprimoPersistantAsset> Search(string query)
        {
            using (var connection = PersistantSqlConnection)
                return connection.Query<AprimoPersistantAsset>(AprimoDBConstants.SearchAssets, new { searchTerm = query }, commandType: CommandType.StoredProcedure);
        }

        public AprimoPersistantAsset GetAsset(int id) =>
            this.GetAsset(new ContentReference(id));

        public AprimoPersistantAsset GetAsset(ContentReference contentReference)
        {
            using (var connection = PersistantSqlConnection)
                return connection.QueryFirstOrDefault<AprimoPersistantAsset>(AprimoDBConstants.GetAsset, new { id = contentReference.ToReferenceWithoutVersion().ID }, commandType: CommandType.StoredProcedure);
        }

        public AprimoPersistantAsset GetAsset(Guid assetId, string renditionId)
        {
            using (var connection = PersistantSqlConnection)
                return connection.QueryFirstOrDefault<AprimoPersistantAsset>(AprimoDBConstants.GetAsset, new { AssetId = assetId, RenditionId = renditionId }, commandType: CommandType.StoredProcedure);
        }

        public AprimoPersistantAsset GetAsset(string renditionId)
        {
            using (var connection = PersistantSqlConnection)
                return connection.QueryFirstOrDefault<AprimoPersistantAsset>(AprimoDBConstants.GetAsset, new { renditionId }, commandType: CommandType.StoredProcedure);
        }

        public AprimoPersistantAsset Save(AprimoPersistantAsset asset)
        {
            using (var connection = PersistantSqlConnection)
            {
                connection.Execute(AprimoDBConstants.InsertAsset, new
                {
                    asset.Id,
                    asset.AssetId,
                    asset.RenditionId,
                    asset.Title,
                    asset.CDNUrl,
                    asset.ThumbnailUrl,
                    asset.MetaInformation,
                    asset.Extension,
                    asset.ModifiedDate
                }, commandType: CommandType.StoredProcedure);
            }

            return GetAsset(asset.Id);
        }

        public AprimoPersistantAsset Update(AprimoPersistantAsset asset)
        {
            using (var connection = PersistantSqlConnection)
            {
                connection.Execute(AprimoDBConstants.UpdateAsset, new
                {
                    asset.Id,
                    asset.AssetId,
                    asset.RenditionId,
                    asset.Title,
                    asset.CDNUrl,
                    asset.ThumbnailUrl,
                    asset.MetaInformation,
                    asset.Extension,
                    asset.ModifiedDate
                }, commandType: CommandType.StoredProcedure);
            }

            return GetAsset(asset.Id);
        }

        public void Delete(int id)
        {
            using (var connection = PersistantSqlConnection)
                connection.Execute(AprimoDBConstants.DeleteAsset, new { id }, commandType: CommandType.StoredProcedure);
        }

        public void DeleteAll()
        {
            using (var connection = PersistantSqlConnection)
                connection.Execute(AprimoDBConstants.DeleteAllAssets, commandType: CommandType.StoredProcedure);
        }

        public void CheckDBConsistancy(string version)
        {
            var aprimoDB = AprimoPersistantDBVersion.Get();

            if (aprimoDB == null)
            {
                aprimoDB = new AprimoPersistantDBVersion()
                {
                    DBVersion = AprimoConstants.DBVersion
                };
                this.RunUpgradeScript();
                aprimoDB.Save();
            }
            else if (!aprimoDB.DBVersion.Equals(AprimoConstants.DBVersion))
            {
                this.RunUpgradeScript();
                aprimoDB.DBVersion = AprimoConstants.DBVersion;
                aprimoDB.Save();
            }
        }

        private void RunUpgradeScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string folder = $"{assembly.GetName().Name}.DBScripts";
            var sqlFiles = assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(folder) && x.EndsWith(".sql"))
                .OrderBy(x => x);

            foreach (var file in sqlFiles)
            {
                using (Stream stream = assembly.GetManifestResourceStream(file))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string sqlScript = reader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(sqlScript))
                    {
                        var batchScript = CreateBatchCommands(sqlScript);
                        using (var connection = PersistantSqlConnection)
                        {
                            foreach (var batchedScript in batchScript)
                            {
                                connection.Execute(batchedScript);
                            }
                        }
                    }
                }
            }
        }

        private List<string> CreateBatchCommands(string sqlScript)
        {
            var list = new List<string>();
            StringBuilder sqlBatch = new StringBuilder();
            foreach (string line in sqlScript.Split(new string[2] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.ToUpperInvariant().Trim() == "GO")
                {
                    list.Add(sqlBatch.ToString());
                    sqlBatch = new StringBuilder();
                }
                else
                {
                    sqlBatch.AppendLine(line);
                }
            }
            return list;
        }
    }
}