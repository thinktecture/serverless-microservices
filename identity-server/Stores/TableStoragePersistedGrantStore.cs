using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using SInnovations.Azure.TableStorageRepository.Queryable;

namespace Serverless
{
    public class TableStoragePersistedGrantStore : IPersistedGrantStore
    {
        private readonly PersistedGrantContext _context;

        public TableStoragePersistedGrantStore(PersistedGrantContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            return await _context.PersistedGrants.Where(k => k.SubjectId == subjectId).ToListAsync().ConfigureAwait(false);
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            return await _context.PersistedGrants.FindByIndexAsync(key).ConfigureAwait(false);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            this._context.PersistedGrants.Add(grant);

            await this._context.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            return Task.CompletedTask;
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            return Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            this._context.PersistedGrants.Delete(await _context.PersistedGrants.FindByIndexAsync(key).ConfigureAwait(false));
         
            await Task.WhenAll(
                this._context.PersistedGrants.DeleteByKey(PersistedGrantContext.FixPartitionKey(key), ""),
                this._context.SaveChangesAsync()
            );
        }
    }
}