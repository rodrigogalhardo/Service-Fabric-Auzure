using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Company.Application.Entities;

namespace Company.Application.Services
{
    /// <summary>
    /// Interface para manipulação de uma empresa.
    /// </summary>
    public interface ICompanyService : IService
    {
        /// <summary>
        /// Retorna uma empresa.
        /// </summary>
        /// <param name="socialReason">parametro de busca pelo nome da empresa.</param>
        /// <returns>
        /// Uma ou mais lista de empresa pela razao social.
        /// </returns>
        Task<IEnumerable<CompanyEntity>> GetCompany(string socialReason);

        /// <summary>
        /// Cria um registro de uma empresa.
        /// </summary>
        /// <param name="company">Entidade de uma empresa.</param>
        Task AddCompany(CompanyEntity company);
    }
}
