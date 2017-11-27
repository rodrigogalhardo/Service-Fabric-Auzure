﻿using Company.Application.Entities;
using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmployerService : IService
    {
        /// <summary>
        /// Retorna uma lista de funcionarios de uma empresa pelo nome.
        /// </summary>
        /// <param name="employerName"></param>
        /// <returns>
        /// Um ou mais funcionarios pela empresa.
        /// </returns>
        Task<IEnumerable<EmployerEntity>> GetAllEmployerByNameAsync(string employerName);

        /// <summary>
        /// Cria um registro de um funcionario para uma empresa.
        /// </summary>
        /// <param name="employer"></param>
        Task AddEmployer(EmployerEntity employer);

        /// <summary>
        /// Atualiza os dados de um funcionario.
        /// </summary>
        /// <param name="companyId">Id da empresa.</param>
        /// <param name="employer">Entidade de funcionario para atualizar.</param>
        /// <returns>
        /// Status 200::Atualizado com sucesso.
        /// </returns>
        Task UpdateEmployer(Guid employerId, EmployerEntity employer);


        /// <summary>
        /// Remove um funcionario de uma empresa.
        /// </summary>
        /// <param name="employerId">Id do funcionario</param>
        /// <returns>
        /// Status 200::Removido com sucesso.
        /// </returns>
        Task RemoveEmployer(Guid employerId);
    }
}
