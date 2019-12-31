using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;

using RouteAttribute = System.Web.Mvc.RouteAttribute;

namespace TicketingAPI.Controllers
{
	[Route("registration")]
    public class RegistrationController : Controller
    {
		// GET: registration
		public ActionResult Index()
		{
			return View();
		}

		// GET api/registration
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/registration/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/registration
		public void Post([FromBody]string value)
		{
		}

		// PUT api/registration/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/registration/5
		public void Delete(int id)
		{
		}
	}
}