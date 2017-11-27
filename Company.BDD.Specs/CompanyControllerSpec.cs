using Company.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Company.BDD.Specs
{
    [Binding]
    public class CompanyControllerSpec
    {
        Task<IActionResult> result;
        CompanyController controller;

        [When(@"Chamo o controller api/company/ [GET] method")]
        public void GiveCallControllerApiCompany()
        {
            controller = new CompanyController();
            var result = controller.GetEmployers("Bruce").GetAwaiter().GetResult();
        }

        [Then(@"Devo vizualizar as informações de um ou mais funcionarios.")]
        public void ThenIVizualizeEmployerData()
        {
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsNotNull(result.Result);
        }
    }
}
