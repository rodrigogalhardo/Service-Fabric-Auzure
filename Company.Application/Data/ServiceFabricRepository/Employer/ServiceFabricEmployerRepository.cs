
using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Application.Entities;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading;

namespace Company.Data.ServiceFabricRepository.Employer
{
    /// <summary>
    /// Repositorio service fablic para Funcionario.
    /// </summary>
    public class ServiceFabricEmployerRepository : IEmployerRepository
    {
        private IReliableStateManager _stateManager;
        private object cancelationToken;

        public ServiceFabricEmployerRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        /// <summary>
        /// Cria um novo funcionario.
        /// </summary>
        /// <param name="companyId">identificador unico da empresa.</param>
        /// <param name="employer">dados de um funcionario.</param>
        /// <returns></returns>
        public async Task AddEmployer(EmployerEntity employer)
        {
            var employers = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, EmployerEntity>>("companies");

            using (var transac = _stateManager.CreateTransaction())
            {
                await employers.AddOrUpdateAsync(transac, employer.EmployerId, employer, (id, value) => employer);
                await transac.CommitAsync();
            }
        }

        /// <summary>
        /// Atualiza os dados de um funcionario.
        /// </summary>
        /// <param name="companyId">Id da empresa.</param>
        /// <param name="employer">Entidade de funcionario para atualizar.</param>
        /// <returns></returns>
        public async Task UpdateEmployer(Guid employerId, EmployerEntity employer)
        {
            var employers = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, EmployerEntity>>("employers");
            var employee = await GetEmployerById(employerId);

            using (var transac = _stateManager.CreateTransaction())
            {
                ConditionalValue<EmployerEntity> employ = await employers.TryGetValueAsync(transac, employerId);
                var ret = employ.HasValue ? employ.Value : null;

                if (ret != null)
                {
                    await employers.TryUpdateAsync(transac, employerId, employer, employee);
                    await transac.CommitAsync();
                }
            }
        }

        /// <summary>
        /// Remove um funcionario de uma empresa.
        /// </summary>
        /// <param name="employerId">Id do funcionario</param>
        /// <returns></returns>
        public async Task RemoveEmployer(Guid employerId)
        {
            var employers = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, EmployerEntity>>("employers");
            var employee = await GetEmployerById(employerId);

            using (var transac = _stateManager.CreateTransaction())
            {
                if (employee.EmployerId != null)
                {
                    await employers.TryRemoveAsync(transac, employerId);
                    await transac.CommitAsync();
                }
            }
        }

        /// <summary>
        /// Retorna uma lista de funcionarios pelo nome.
        /// </summary>
        /// <param name="employerName">Nome do funcionario para pesquisa</param>
        /// <returns></returns>
        public async Task<IEnumerable<EmployerEntity>> GetAllEmployerByName(string employerName)
        {
            var employers = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, EmployerEntity>>("employer");
            var result = new List<EmployerEntity>();

            using (var transac = _stateManager.CreateTransaction())
            {
                var allEmployers = await employers.CreateEnumerableAsync(transac, EnumerationMode.Unordered);

                using (var enumerator = allEmployers.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, EmployerEntity> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result.Where(w => employerName.ToLower().Contains(w.Name) || employerName.ToLower().Contains(w.LastName)).ToList();
        }

        /// <summary>
        /// Metodo interno para retorna um funcionarios pelo Id.
        /// </summary>
        /// <param name="employerName">Nome do funcionario para pesquisa</param>
        /// <returns></returns>
        private async Task<EmployerEntity> GetEmployerById(Guid employerId)
        {
            var employers = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, EmployerEntity>>("employer");
            var result = new List<EmployerEntity>();

            using (var transac = _stateManager.CreateTransaction())
            {
                ConditionalValue<EmployerEntity> employer = await employers.TryGetValueAsync(transac, employerId);
                return employer.HasValue ? employer.Value : null;
            }
        }
    }
}
