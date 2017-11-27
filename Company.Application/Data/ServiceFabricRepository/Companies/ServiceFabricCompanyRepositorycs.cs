
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Application.Entities;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading;

namespace Company.Data.ServiceFabricRepository.Companies
{
    /// <summary>
    /// Repositorio service fabric para empresa.
    /// </summary>
    public class ServiceFabricCompanyRepository : ICompanyRepository
    {
        private IReliableStateManager _stateManager;
        private object cancellationToken;

        public ServiceFabricCompanyRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        /// <summary>
        /// Cria uma empresa.
        /// </summary>
        /// <param name="company">entidade de uma empresa.</param>
        /// <returns>
        /// status 200
        /// </returns>
        public async Task AddCompany(CompanyEntity company)
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, CompanyEntity>>("companies");

            using (var transac = _stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(transac, company.CompanyId, company, (id, value) => company);
                await transac.CommitAsync();
            }
        }

        /// <summary>
        /// Retorna uma ou mais empresas pela razao social.
        /// </summary>
        /// <param name="socialReason">Nome da razão social de uma empresa.</param>
        /// <returns>
        /// Uma ou mais empresas.
        /// </returns>
        public async Task<IEnumerable<CompanyEntity>> GetCompany(string socialReason)
        {
            var companies = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, CompanyEntity>>("companies");
            var result = new List<CompanyEntity>();

            using (var transac = _stateManager.CreateTransaction())
            {
                var allCompanies = await companies.CreateEnumerableAsync(transac, EnumerationMode.Unordered);

                using (var enumerator = allCompanies.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, CompanyEntity> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result.Where(w => socialReason.ToLower().Contains(w.TradingName)).ToList();
        }


    }
}
