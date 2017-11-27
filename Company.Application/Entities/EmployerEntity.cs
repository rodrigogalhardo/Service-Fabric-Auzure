using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Entities
{
    /// <summary>
    /// Dados de um funcionario de uma empresa.
    /// </summary>
    public class EmployerEntity
    {
        /// <summary>
        /// Identificador unico de um funcionario.
        /// </summary>
        public Guid EmployerId { get; set; }
        /// <summary>
        /// Identificador unico de uma empresa.
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// Nome de um funcionario.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Sobrenome de um funcionario.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Data de nascimento de um funcionario.
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// Sexo, Genero de um funcionario (Masculino, Feminino, outros).
        /// </summary>
        public string Genre { get; set; }
        /// <summary>
        /// Relacionamento entre um fucionario e uma empresa.
        /// </summary>
        public virtual CompanyEntity Company { get; set; }
    }
}
