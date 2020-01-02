using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using TicketingAPI.Helpers;
using TicketingAPI.Models;

namespace TicketingAPI.Controllers
{
	[RoutePrefix("api/tickets")]
	public class TicketController : Controller
	{
		private EncryptionUtils _encryptionUtils;

		public TicketController()
		{
			_encryptionUtils = new EncryptionUtils();
		}

		[HttpGet]
		[Route(Name = "test")]
		public string Test()
		{
			//var sharedKey = _encryptionUtils.GenerateSharedKey();
			//var cipher = _encryptionUtils.EncryptAES(@"{transaction:38547494,seat:271,timestamp:1577788104}", sharedKey);

			//var plain = _encryptionUtils.DecryptAES(Convert.ToBase64String(cipher.Item1), sharedKey, cipher.Item2);

			//return String.Format("{0}{1}&{2}", 3, 17263, Convert.ToBase64String(cipher.Item1));

			var AESKey = _encryptionUtils.GenerateSecretKey();
			var RSAKey = _encryptionUtils.GeneratePublicKeyPair();
			var ECCKey = _encryptionUtils.GenerateSharedKey();

			//var stringMaxRSAKB = generateStringSize(((2048 - 384) / 8) + 7);
			var stringMaxRSAKB = generateStringSize(((2048 - 384) / 8) + 37);
			var string1KB = generateStringSize(1);
			var string10KB = generateStringSize(10);
			var string100KB = generateStringSize(100);
			var string1MB = generateStringSize(1000);
			var string10MB = generateStringSize(10000);
			var string100MB = generateStringSize(100000);
			var string1000MB = generateStringSize(1000000);
			var string1500MB = generateStringSize(1500000);
			var string2000MB = generateStringSize(2000000);

			var size = ((decimal)Encoding.UTF8.GetByteCount(string1KB) / 1048576);
			var list = new string[10] { string1KB, string10KB, string100KB, stringMaxRSAKB, string1MB, string10MB, string100MB, string1000MB, string1500MB, string2000MB };

			var stopwatch = Stopwatch.StartNew();
			string results = "";
			foreach (var item in list)
			{
				stopwatch.Reset();

				// AES
				stopwatch.Start();
				_encryptionUtils.EncryptAES(item, AESKey.Item1, AESKey.Item2);
				stopwatch.Stop();
				var resultAES = stopwatch.Elapsed;

				stopwatch.Reset();

				// RSA
				//if (item.Length <= ((2048 - 384) / 8) + 7)
				if (item.Length <= ((2048 - 384) / 8) + 37)
				{
					stopwatch.Start();
					_encryptionUtils.EncryptRSA(item, RSAKey.Item1);
					stopwatch.Stop();
				}
				var resultRSA = stopwatch.Elapsed;

				stopwatch.Reset();

				// ECC
				stopwatch.Start();
				_encryptionUtils.EncryptAES(item, ECCKey, null);
				stopwatch.Stop();
				var resultECC = stopwatch.Elapsed;

				results += "<br>" + String.Format("Encryption Test - {0:N5}MB<br><br>AES: {1}<br>RSA: {2}<br>ECC: {3}<br>------------------------------------------------<br><br>", 
					(item.Length / 1024f), resultAES, resultRSA, resultECC);
			}
			return results;
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
			entries.Add(new Transaction
			{
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

		private string generateStringSize(int times)
		{
			return new string('a', times);
		}
	}
}