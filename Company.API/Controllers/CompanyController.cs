using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Company.Application.Services;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Collections;
using Company.API.Contracts;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace Company.API.Controllers
{
    /// <summary>
    /// Controller para manipulação de dados da empresa.
    /// </summary>
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private ICompanyService _companyService;
        private IEmployerService _employerService;

        /// <summary>
        /// Construtor.
        /// </summary>
        public CompanyController()
        {
            _companyService = ServiceProxy.Create<ICompanyService>(
                new Uri("fabric:/RodrigoGalhardoFabricApp/Company/"),
                new ServicePartitionKey(0));

            _employerService = ServiceProxy.Create<IEmployerService>(
                new Uri("fabric:/RodrigoGalhardoFabricApp/Employer/"),
                new ServicePartitionKey(0));
        }

        /// <summary>
        /// A partir do identificador razao social, retorna o uma ou mais empresas.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição para obter o primeiro nome do usuário.
        /// 
        ///     Request:
        ///     GET /api/company?socialReason=Acme S.A
        ///     GET /api/company/Acme S.A
        ///     
        ///     Response:
        ///     {
        ///         "cnpj": "14383462000199",
        ///         "companyid": "80f3a63f-3b8d-4b91-a08f-efd19ef379de",
        ///         "socialreason":"Acme Produtos Alimentos Ltda-SA",
        ///         "tradingname":"Acme S.A"
        ///     }
        /// 
        /// </remarks>
        /// <param name="socialReason">Identificador da empresa (Razão Social).</param>
        /// <returns>Lista de uma ou mais empresas.</returns>
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("{socialReason}")]
        public async Task<IActionResult> Get(string socialReason)
        {
            try
            {
                var company = await _companyService.GetCompany(socialReason);

                if (company.Count() == 0)
                    return NotFound();

                var response = company.Select(s => new CompanyGetResponse()
                {
                    Cnpj = s.Cnpj,
                    CompanyId = s.CompanyId,
                    SocialReason = s.SocialReason,
                    TradingName = s.TradingName
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Retorna o uma ou mais funcionarios de uma empresa.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição para obter o primeiro nome do usuário.
        /// 
        ///     Request:
        ///     GET /api/company/allemployers
        ///     
        ///     Response:
        ///     {
        ///         "companyid": "80f3a63f-3b8d-4b91-a08f-efd19ef379de",
        ///         "employers":[
        ///          {
        ///             "name":"Bruce",
        ///             "lastName":"Wayne",
        ///             "genre:"Masculino",
        ///             "birthday:"25/05/1976",
        ///             "companyId:"80f3a63f-3b8d-4b91-a08f-efd19ef379de",
        ///             "employerId:""
        ///             }
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="socialReason">Identificador da empresa (Razão Social).</param>
        /// <returns>Lista de uma ou mais empresas.</returns>
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [HttpGet("allemployers")]
        public async Task<IActionResult> GetEmployers(string socialReason)
        {
            try
            {
                var company = await _companyService.GetCompany(socialReason);

                if (company.Count() == 0)
                    return NotFound();

                var response = company.Select(e => new CompanyGetResponse()
                {
                    CompanyId = e.CompanyId,
                    Employers = e.Employers.Select(s => new EmployerGetResponse()
                    {
                        Name = s.Name,
                        LastName = s.LastName,
                        Genre = s.Genre,
                        Birthday = s.Birthday,
                        CompanyId = s.CompanyId,
                        EmployerId = s.EmployerId
                    }).ToList()
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Requisição para criar uma nova empresa.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição para criar uma nova empresa:
        /// 
        ///     Request:
        ///     POST api/company
        ///     {
        ///         "cnpj": "14383462000199",
        ///         "companyid": "80f3a63f-3b8d-4b91-a08f-efd19ef379de",
        ///         "socialreason":"Acme Produtos Alimentos Ltda-SA",
        ///         "tradingname":"Acme S.A"
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">Request contract para criação de uma nova empresa.</param>
        /// <returns>Se a empresa foi criada com sucesso.</returns>
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(CompanyPostRequest))]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CompanyPostRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(request);

                await _companyService.AddCompany(new Application.Entities.CompanyEntity()
                {
                    Cnpj = request.Cnpj,
                    SocialReason = request.SocialReason,
                    TradingName = request.TradingName
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }
        }

        /// <summary>
        /// Requisição criar um funcionario da empresa.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição para atualizar um funcionario da empresa:
        /// 
        ///     Request:
        ///     POST api/company/80f3a63f-3b8d-4b91-a08f-efd19ef379de/employee
        ///     {
        ///         "companyid": "80f3a63f-3b8d-4b91-a08f-efd19ef379de",
        ///         "Name":"Acme Produtos Alimentos Ltda-SA",
        ///         "lastname":"Acme S.A",
        ///         "employerid":"67f3a63f-5b8d-4b81-a01f-efd25ef391ae"
        ///         "birthday":"25/05/1988",
        ///         "genre:"masculino"
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">Request contract para criação de um novo funcionario.</param>
        /// <returns>Se o funcionario foi criado com sucesso.</returns>
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(EmployerPostResponse))]
        [HttpPost("{companyId}/employee", Name = "PostEmployer")]
        public async Task<IActionResult> PostEmployer(Guid companyId, [FromBody]EmployerPostRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(request);

                await _employerService.UpdateEmployer(request.EmployerId, new Application.Entities.EmployerEntity()
                {
                    CompanyId = request.CompanyId,
                    Name = request.Name,
                    LastName = request.LastName,
                    EmployerId = request.EmployerId,
                    Birthday = request.Birthday,
                    Genre = request.Genre
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }
        }

        /// <summary>
        /// Requisição atualizar um funcionario da empresa.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição para atualizar um funcionario da empresa:
        /// 
        ///     Request:
        ///     PUT api/company/80f3a63f-3b8d-4b91-a08f-efd19ef379de/employee
        ///     {
        ///         "companyid": "80f3a63f-3b8d-4b91-a08f-efd19ef379de",
        ///         "Name":"Acme Produtos Alimentos Ltda-SA",
        ///         "lastname":"Acme S.A",
        ///         "employerid":"67f3a63f-5b8d-4b81-a01f-efd25ef391ae"
        ///         "birthday":"25/05/1988",
        ///         "genre:"masculino"
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">Request contract para criação de um novo funcionario.</param>
        /// <returns>Se o funcionario foi criada com sucesso.</returns>
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(EmployerPostResponse))]
        [HttpPut("{companyId}/employee")]
        public async Task<IActionResult> Put(Guid companyId, [FromBody]EmployerPostResponse request)
        {
            try
            {
                if (request == null)
                    return BadRequest(request);

                await _employerService.UpdateEmployer(request.EmployerId, new Application.Entities.EmployerEntity()
                {
                    CompanyId = request.CompanyId,
                    Name = request.Name,
                    LastName = request.LastName,
                    EmployerId = request.EmployerId,
                    Birthday = request.Birthday,
                    Genre = request.Genre
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }
        }

        /// <summary>
        /// Requisição remover um funcionario da empresa.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição para remover um funcionario da empresa:
        /// 
        ///     Request:
        ///     DELETE api/company/80f3a63f-3b8d-4b91-a08f-efd19ef379de/employer
        /// 
        /// </remarks>
        /// <param name="id">identificador unico para um funcionario.</param>
        /// <returns>Se a empresa foi criada com sucesso.</returns>
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [HttpDelete("{id}/employer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id != null)
                    return BadRequest(id);

                await _employerService.RemoveEmployer(id);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}
