using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PayMvc.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/data/{action}")]
    public class DataController : ApiController
    {
    }
}
