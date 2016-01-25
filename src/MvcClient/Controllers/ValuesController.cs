using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Chess.MvcClient.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

		public class ValuePutParams
		{
			public string val1 { get; set; }
			public string val2 { get; set; }
		}

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]ValuePutParams value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
