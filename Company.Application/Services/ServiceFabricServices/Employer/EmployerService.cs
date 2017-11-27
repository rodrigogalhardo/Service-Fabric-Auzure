using Company.Application.Entities;
using Company.Data.ServiceFabricRepository.Companies;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using System;
using Company.Data.ServiceFabricRepository.Employer;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Threading;

namespace Company.Application.Services.ServiceFabricServices.Employer
{
    /// <summary>
    /// Instancia da classe employerService que é criada para cada replica usando o runtime do service fabric.
    /// </summary>
    public class EmployerService : StatefulService, IEmployerService
    {
        private IEmployerRepository _repositoryEmployer;
        private ICompanyRepository _repositoryCompany;

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="serviceContext"></param>
        public EmployerService(StatefulServiceContext serviceContext) : base(serviceContext)
        {
        }

        /// <summary>
        /// Cria um registro de um funcionario para uma empresa.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employer"></param>
        /// <returns>
        /// status 200:: sucesso para um registro criado.
        /// </returns>
        public async Task AddEmployer(EmployerEntity employer)
        {
            await _repositoryEmployer.AddEmployer(employer);
        }

        /// <summary>
        /// Retorna uma lista de funcionarios de uma empresa pelo nome.
        /// </summary>
        /// <param name="employerName">Nome do funcionario.</param>
        /// <returns>
        /// status 200.
        /// </returns>
        public async Task<IEnumerable<EmployerEntity>> GetAllEmployerByNameAsync(string employerName)
        {
            return await _repositoryEmployer.GetAllEmployerByName(employerName);
        }

        /// <summary>
        /// Remove um funcionario de uma empresa.
        /// </summary>
        /// <param name="employerId">identificador unico de uma empresa.</param>
        /// <returns>
        /// status 200::sucesso para um funcionario removido.
        /// </returns>
        public async Task RemoveEmployer(Guid employerId)
        {
            await _repositoryEmployer.RemoveEmployer(employerId);
        }

        /// <summary>
        /// Atualiza os dados de um funcionario.
        /// </summary>
        /// <param name="employerId">identificador unico de um funcionario.</param>
        /// <param name="employer">entidade de um funcionario.</param>
        /// <returns>
        /// Status 200::Atualizado com sucesso.
        /// </returns>
        public async Task UpdateEmployer(Guid employerId, EmployerEntity employer)
        {
            await _repositoryEmployer.UpdateEmployer(employerId, employer);
        }

        /// <summary>
        /// Override opcional para criar um listener (ex: HTTP,Service Remoting e etc) para que esta replica de serviço fique aguardando um request vindo do client.
        /// </summary>
        /// <remarks>
        /// Exemplo para mais informações: https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns></returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context=>this.CreateServiceRemotingListener(context))
            };
        }

        /// <summary>
        /// Ponto principal de entrada para o service replica.
        /// Este metodo é executado quando a replica de um servico começa a ser invocado.
        /// </summary>
        /// <param name="cancellationToken">Cancelado quando um service fabric precisa desligar este serviço de replica.</param>
        /// <returns></returns>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repositoryEmployer = new ServiceFabricEmployerRepository(this.StateManager);
            _repositoryCompany = new ServiceFabricCompanyRepository(this.StateManager);

            var company = _repositoryCompany.GetCompany("trends").GetAwaiter().GetResult().GetEnumerator();

            var employer1 = new EmployerEntity
            {
                EmployerId = Guid.NewGuid(),
                CompanyId = company.Current.CompanyId,
                Birthday = new DateTime(1988, 03, 30),
                Genre = "Masculino",
                Name = "Rodrigo",
                LastName = "Galhardo"
            };

            var employer2 = new EmployerEntity
            {
                EmployerId = Guid.NewGuid(),
                CompanyId = company.Current.CompanyId,
                Birthday = new DateTime(1988, 02, 15),
                Genre = "Masculino",
                Name = "Rodolpho",
                LastName = "Galhardo"
            };

            var employer3 = new EmployerEntity
            {
                EmployerId = Guid.NewGuid(),
                CompanyId = company.Current.CompanyId,
                Birthday = new DateTime(1988, 02, 15),
                Genre = "Masculino",
                Name = "Bruce Ro",
                LastName = "Wayne"
            };

            await _repositoryEmployer.AddEmployer(employer1);
            await _repositoryEmployer.AddEmployer(employer1);
            await _repositoryEmployer.AddEmployer(employer1);

            IEnumerable<EmployerEntity> All = await _repositoryEmployer.GetAllEmployerByName("ro");
        }

    }
}
