using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.API.Contracts
{
    /// <summary>
    /// Contrato para Criar / Atualizar um funcionario de uma empresa
    /// </summary>
    public class EmployerPostResponse
    {

        /// <summary>
        /// Identificador unico de um funcionario.
        /// </summary>
        [JsonProperty("employerId")]
        public Guid EmployerId { get; set; }
        /// <summary>
        /// Identificador unico de uma empresa.
        /// </summary>
        [JsonProperty("companyid")]
        public Guid CompanyId { get; set; }
        /// <summary>
        /// Nome de um funcionario.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Sobrenome de um funcionario.
        /// </summary>
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        /// <summary>
        /// Data de nascimento de um funcionario.
        /// </summary>
        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }
        /// <summary>
        /// Sexo, Genero de um funcionario (Masculino, Feminino, outros).
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; }
    }
}
