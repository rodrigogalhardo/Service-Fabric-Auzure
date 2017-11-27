using Company.Application.Entities;
using Company.Data.ServiceFabricRepository.Companies;
using Company.Data.ServiceFabricRepository.Employer;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace Company.Application.Services.ServiceFabricServices.Company
{
    /// <summary>
    /// Instancia da classe companyservice que é criada para cada replica usando o runtime do service fabric.
    /// </summary>
    public class CompanyService : StatefulService, ICompanyService
    {
        private ICompanyRepository _repositoryCompany;

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="serviceContext">Service context.</param>
        public CompanyService(StatefulServiceContext serviceContext) : base(serviceContext)
        {
        }

        /// <summary>
        /// Cria um registro de uma empresa.
        /// </summary>
        /// <param name="company">entidade de uma nova empresa.</param>
        /// <returns>
        /// status 200::sucesso para a empresa criada.
        /// </returns>
        public async Task AddCompany(CompanyEntity company)
        {
            await _repositoryCompany.AddCompany(company);
        }

        /// <summary>
        /// Retorna uma empresa pela razão social.
        /// </summary>
        /// <param name="socialReason">nome da razão social de uma empresa.</param>
        /// <returns>
        /// Retorna uma ou mais empresa que correspondem a razão social informada.
        /// </returns>
        public async Task<IEnumerable<CompanyEntity>> GetCompany(string socialReason)
        {
            return await _repositoryCompany.GetCompany(socialReason);
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
            _repositoryCompany = new ServiceFabricCompanyRepository(this.StateManager);

            var company1 = new CompanyEntity
            {
                CompanyId = Guid.NewGuid(),
                Cnpj = "18720131000109",
                SocialReason = "Acme produtos de limpeza do curinga.",
                TradingName = "Acme S.A"
            };

            await _repositoryCompany.AddCompany(company1);
            
        }

    }
}
