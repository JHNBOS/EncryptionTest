using System.Security.Cryptography;

namespace TicketingAPI.Helpers
{
	public class ECC
	{
		private ECDiffieHellmanCng ECDH;
		private byte[] SharedKey;
		private ECDiffieHellmanPublicKey PublicKey;

		public ECC()
		{
			ECDH = new ECDiffieHellmanCng
			{
				KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
				HashAlgorithm = CngAlgorithm.Sha256,
				KeySize = 521
			};
		}

		private byte[] GeneratePublicKey()
		{
			var publicKey = ECDH.PublicKey;
			PublicKey = publicKey;

			return ECDH.PublicKey.ToByteArray();
		}

		public byte[] DeriveKey()
		{
			SharedKey = ECDH.DeriveKeyMaterial(CngKey.Import(GeneratePublicKey(), CngKeyBlobFormat.EccPublicBlob));
			return SharedKey;
		}
	}
}