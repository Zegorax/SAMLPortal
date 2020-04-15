using System.Runtime.CompilerServices;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System.Globalization;

namespace SAMLPortal.Misc
{
	public static class Helpers
	{
		public static string GenerateRandomPassword()
		{
			PasswordOptions opts = new PasswordOptions
			{
				RequiredLength = 40,
				RequiredUniqueChars = 4,
				RequireDigit = true,
				RequireLowercase = true,
				RequireNonAlphanumeric = true,
				RequireUppercase = true
			};

			return GenerateRandomPassword(opts);
		}

		public static string GenerateRandomPassword(PasswordOptions opts)
		{
			string[] randomChars = new[]
			{
				"ABCDEFGHJKLMNOPQRSTUVWXYZ", // uppercase 
				"abcdefghijkmnopqrstuvwxyz", // lowercase
				"0123456789", // digits
				"!@$?_-" // non-alphanumeric
			};
			Random rand = new Random(Environment.TickCount);
			List<char> chars = new List<char>();

			if (opts.RequireUppercase)
			{
				chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
			}

			if (opts.RequireLowercase)
			{
				chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
			}

			if (opts.RequireDigit)
			{
				chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);
			}

			if (opts.RequireNonAlphanumeric)
			{
				chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);
			}

			for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
			{
				string rcs = randomChars[rand.Next(0, randomChars.Length)];
				chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
			}

			return new string(chars.ToArray());
		}

		public static X509Certificate2 GenerateCertificate(string subject, string companyName, string countryCode)
		{
			// https://gist.github.com/svrooij/ec6f664cd93cd09e84414112d23f6a42

			var random = new SecureRandom();
			var certificateGenerator = new X509V3CertificateGenerator();

			var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
			certificateGenerator.SetSerialNumber(serialNumber);

			certificateGenerator.SetIssuerDN(new X509Name($"C={countryCode}, O={companyName}, CN={subject}"));
			certificateGenerator.SetSubjectDN(new X509Name($"C={countryCode}, O={companyName}, CN={subject}"));
			certificateGenerator.SetNotBefore(DateTime.UtcNow.Date);
			certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(999));

			const int strength = 2048;
			var keyGenerationParameters = new KeyGenerationParameters(random, strength);
			var keyPairGenerator = new RsaKeyPairGenerator();
			keyPairGenerator.Init(keyGenerationParameters);

			var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
			certificateGenerator.SetPublicKey(subjectKeyPair.Public);

			var issuerKeyPair = subjectKeyPair;
			const string signatureAlgorithm = "SHA256WithRSA";
			var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, issuerKeyPair.Private);
			var bouncyCert = certificateGenerator.Generate(signatureFactory);

			// Lets convert it to X509Certificate2
			X509Certificate2 certificate;

			Pkcs12Store store = new Pkcs12StoreBuilder().Build();
			store.SetKeyEntry($"{subject}_key", new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { new X509CertificateEntry(bouncyCert) });
			string exportpw = Guid.NewGuid().ToString("x");

			using (var ms = new System.IO.MemoryStream())
			{
				store.Save(ms, exportpw.ToCharArray(), random);
				certificate = new X509Certificate2(ms.ToArray(), exportpw, X509KeyStorageFlags.Exportable);
			}

#if DEBUG
			Console.WriteLine($"Generated cert with thumbprint {certificate.Thumbprint}");
#endif
			return certificate;
		}

		/// <summary>
		/// Convert a X509 certificate to a base64 encoded string export
		/// </summary>
		/// <param name="certificate">X509Certificate2 object</param>
		/// <returns>Base64 encoded string export of the certificate</returns>
		public static string X509Certificate2ToString(X509Certificate2 certificate)
		{
			var exportedCert = certificate.Export(X509ContentType.Pfx);
			var b64Cert = Convert.ToBase64String(exportedCert);

			return b64Cert;
		}

		/// <summary>
		/// Convert a base64 encoded X509 certificate to a X509Certificate 2 object
		/// </summary>
		/// <param name="b64Cert">base64 encoded string of the certificate</param>
		/// <returns>X509Certificate2 object</returns>
		public static X509Certificate2 StringToX509Certificate2(string b64Cert)
		{
			var decodedCert = Convert.FromBase64String(b64Cert);
			return new X509Certificate2(decodedCert);
		}

		/// <summary>
		/// Write environment variable to a file in the format VARIABLE=VALUE
		/// </summary>
		/// <param name="filePath">Path of the file to write to</param>
		/// <param name="key">Name of the environment variable</param>
		/// <param name="value">Value of the environment variable</param>
		public static void WriteEnvVariableToFile(string filePath, string key, string value)
		{
			if (!key.Contains(" ") && !value.Contains(" "))
			{
				using (StreamWriter sw = System.IO.File.AppendText(filePath))
				{
					sw.WriteLine(key + "=" + value);
				}
			}
		}

		/// <summary>
		/// Search and replace text inside a file
		/// </summary>
		/// <param name="filePath">Path of the file to edit</param>
		/// <param name="oldText">Old text to search for</param>
		/// <param name="newText">New text replacing the old text</param>
		public static void ReplaceTextInFile(string filePath, string oldText, string newText)
		{
			string fileContent = System.IO.File.ReadAllText(filePath);
			fileContent = fileContent.Replace(oldText, newText);
			System.IO.File.WriteAllText(filePath, fileContent);
		}

		/// <summary>
		/// Search for an environment variable in a file and replace it if found. If not found, the variable will be added to the file.
		/// </summary>
		/// <param name="filePath">Path of the file to edit</param>
		/// <param name="variable">Environment variable to search for</param>
		/// <param name="value">Value to set to the envrionment variable</param>
		public static void ReplaceEnvVariableInFile(string filePath, string variable, string value)
		{
			bool hasFoundVariable = false;
			Dictionary<string, string> valuesToReplace = new Dictionary<string, string>();

			using (StreamReader sr = new StreamReader(filePath))
			{
				while (sr.Peek() >= 0)
				{
					var line = sr.ReadLine();
					if (line.StartsWith(variable + "="))
					{
						hasFoundVariable = true;
						var newVariable = variable + "=" + value;
						valuesToReplace.Add(line, newVariable);
					}
				}
			}

			foreach (var (line, newVariable) in valuesToReplace)
			{
				ReplaceTextInFile(filePath, line, newVariable);
			}

			if (!hasFoundVariable)
			{
				WriteEnvVariableToFile(filePath, variable, value);
			}
		}
	}
}