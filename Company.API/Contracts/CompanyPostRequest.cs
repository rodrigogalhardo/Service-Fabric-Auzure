using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.API.Contracts
{
    /// <summary>
    /// Contrato para Response POST da api.
    /// </summary>
    public class CompanyPostRequest
    {
        /// <summary>
        /// Identificador unico de uma empresa.
        /// </summary>
        [JsonProperty("companyid")]
        public Guid CompanyId { get; set; }
        /// <summary>
        /// CNPJ de uma empresa.
        /// </summary>
        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }
        /// <summary>
        /// Razão social de uma empresa.
        /// </summary>
        [JsonProperty("socialreason")]
        public string SocialReason { get; set; }
        /// <summary>
        /// Nome, marca, nome fantasia de uma empresa.
        /// </summary>
        [JsonProperty("tradingname")]
        public string TradingName { get; set; }
    }
}
