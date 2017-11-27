using Company.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Data.ServiceFabricRepository.Companies
{
    /// <summary>
    /// Interface para manipulação de uma empresa, no repositorio.
    /// </summary>
    public interface ICompanyRepository
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
        /// <param name="company"></param>
        Task AddCompany(CompanyEntity company);
    }
}
