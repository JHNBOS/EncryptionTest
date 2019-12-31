using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TicketingAPI.Helpers;
using TicketingAPI.Models;

namespace TicketingAPI.Controllers
{
	[RoutePrefix("api/tickets")]
    public class TicketController : Controller
    {
		[HttpGet]
		[Route(Name = "test")]
		public string Test()
		{
			var diffieHellman = new DiffieHellman();
			var cipher = diffieHellman.Init();

			return String.Format("{0}{1}&{2}", 3, 17263, cipher);
		}

		// GET api/tickets
		public IEnumerable<Activity> Get()
		{
			return GetActivities();
		}

		// GET api/tickets/5
		public Activity Get(int id)
		{
			return GetActivities().FirstOrDefault(q => q.ActivityID == id);
		}

		private List<Transaction> GetTransactions()
		{
			var entries = new List<Transaction>();
			entries.Add(new Transaction { 
				TransactionID = 2821139,
				ActivityID = 12929,
				Seat = 183,
				IsPaid = true
			});
			entries.Add(new Transaction
			{
				TransactionID = 123383,
				ActivityID = 8373,
				Seat = 122,
				IsPaid = false
			});
			entries.Add(new Transaction
			{
				TransactionID = 237473,
				ActivityID = 10038,
				Seat = 13,
				IsPaid = true
			});
			return entries;
		}

		private List<Activity> GetActivities()
		{
			var entries = new List<Activity>();
			entries.Add(new Activity
			{
				ActivityID = 12929,
				Title = "De kleine zeemeermin",
				Subtitle = "Voor jong en oud",
				HasBreak = true,
				Start = new DateTime(2019, 12, 31, 20, 0, 0),
				End = new DateTime(2019, 12, 31, 22, 45, 0, 0),
				DoorsOpen = new DateTime(2019, 12, 31, 20, 0, 0),
			});
			entries.Add(new Activity
			{
				ActivityID = 8373,
				Title = "Die Hard",
				Subtitle = "The Musical",
				HasBreak = false,
				Start = new DateTime(2019, 12, 31, 19, 15, 0),
				End = new DateTime(2019, 12, 31, 22, 30, 0, 0),
				DoorsOpen = new DateTime(2019, 12, 31, 19, 15, 0),
			});
			entries.Add(new Activity
			{
				ActivityID = 10038,
				Title = "The Grinch",
				Subtitle = "Who stole christmas",
				HasBreak = true,
				Start = new DateTime(2019, 12, 25, 18, 0, 0),
				End = new DateTime(2019, 12, 25, 20, 30, 0, 0),
				DoorsOpen = new DateTime(2019, 12, 25, 18, 0, 0),
			});
			return entries;
		}
	}
}