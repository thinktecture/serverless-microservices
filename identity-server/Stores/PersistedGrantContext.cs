using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using SInnovations.Azure.TableStorageRepository;
using SInnovations.Azure.TableStorageRepository.TableRepositories;

namespace Serverless
{
    public class PersistedGrantContext : TableStorageContext
    {
        public const string TABLENAME = "IdentityServer4Store";

        public PersistedGrantContext(
            ILoggerFactory loggerFactory,
            IEntityTypeConfigurationsContainer container,
            CloudStorageAccount account) :
            base(loggerFactory, container, account)
        {
            this.InsertionMode = InsertionMode.AddOrReplace;
        }

        protected override void OnModelCreating(TableStorageModelBuilder modelbuilder)
        {
            modelbuilder.Entity<PersistedGrant>()
               .HasKeys(k => new { k.SubjectId, k.ClientId }, r => new { r.Type, r.Key })
               .WithIndex(k => k.Key, true, TABLENAME)
               .WithKeyTransformation(k => k.Key, FixPartitionKey)
               .ToTable(TABLENAME);

            base.OnModelCreating(modelbuilder);
        }

        public static string FixPartitionKey(string p)
        {
            return $"idx__{p.Replace('/', '_')}";
        }

        public ITableRepository<PersistedGrant> PersistedGrants { get; set; }
    }
}