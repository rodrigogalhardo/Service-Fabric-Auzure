using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Entities
{
    /// <summary>
    /// Dado de uma empresa.
    /// </summary>
    public class CompanyEntity
    {
        /// <summary>
        /// Identificador unico de uma empresa.
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// CNPJ de uma empresa.
        /// </summary>
        public string Cnpj { get; set; }
        /// <summary>
        /// Razão social de uma empresa.
        /// </summary>
        public string SocialReason { get; set; }
        /// <summary>
        /// Nome, marca, nome fantasia de uma empresa.
        /// </summary>
        public string TradingName { get; set; }
        /// <summary>
        /// Relacionamento entre uma empresa e uma lista / collections de funcionarios.
        /// </summary>
        public virtual ICollection<EmployerEntity> Employers { get; set; }
    }
}
