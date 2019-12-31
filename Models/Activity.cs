using System;

namespace TicketingAPI.Models
{
	public class Activity
	{
		public int ActivityID { get; set; }
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public DateTime DoorsOpen { get; set; }
		public bool HasBreak { get; set; }
	}
}