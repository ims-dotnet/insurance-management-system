using InsureTrust.QueryService.Models;
using InsureTrust.QueryService.Services;
using InsureTrust.SupportService.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsureTrust.SupportService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportQueriesController : ControllerBase
    {
        private readonly ISupportService _service ;

        public SupportQueriesController(ISupportService service)
        {
            _service = service;
        }

        // GET: api/SupportQueries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportQuery>>> GetSupportQuery()
        {
            return Ok();
        }

        // GET: api/SupportQueries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupportQuery>> GetSupportQuery(int id)
        {

            return Ok();
        }

       
    }
}
