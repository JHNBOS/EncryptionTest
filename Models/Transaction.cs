namespace TicketingAPI.Models
{
	public class Transaction
	{
		public int TransactionID { get; set; }
		public int ActivityID { get; set; }
		public int Seat { get; set; }
		public bool IsPaid { get; set; }
	}
}