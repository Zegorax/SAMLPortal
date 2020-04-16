using System.Security.Principal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Configuration;
using SAMLPortal.Misc;

namespace SAMLPortal.Models
{
	public static class GlobalSettings
	{
		private static Dictionary<string, string> _appSettings = new Dictionary<string, string>();
		public static X509Certificate2 _signingCertificate { get; set; }

		/// <summary>
		/// All environment variables for this app MUST begin with SP_
		/// </summary>
		public static void InitSettingsFromEnvironment()
		{
			UpdateFromEnvironment();

			string configurationPath = GlobalSettings.Get("CONFIG_PATH");
			if (configurationPath != null)
			{
				if (Directory.Exists(configurationPath))
				{
					// Ensure trailing slash is present
					configurationPath = configurationPath.TrimEnd('/') + '/';

					var fileConfigPath = configurationPath + "config.env";
					if (!File.Exists(fileConfigPath))
					{
						FileStream fs = File.Create(fileConfigPath);
						fs.Close();
						Helpers.WriteEnvVariableToFile(fileConfigPath, "SP_CONFIG_SETUPASSISTANT_STEP", "0");
					}

					DotNetEnv.Env.Load(fileConfigPath);
					Environment.SetEnvironmentVariable("SP_CONFIG_FILE", fileConfigPath);
					UpdateFromEnvironment();

					if (Environment.GetEnvironmentVariable("SP_MYSQL_USER") != null)
					{
						UpdateFromDatabase();
					}
				}
				else
				{
					throw new NotSupportedException("The directory " + configurationPath + " does not exists. Please create it first.");
				}
			}
			else
			{
				throw new NotSupportedException("Environment variable SP_CONFIG_PATH undefined. Please set it to a correct directory.");
			}
		}

		public static void GenerateSigningCertificate()
		{
			if (GlobalSettings.Get("SAML_Signing_Certificate") == null)
			{
				_signingCertificate = Helpers.GenerateCertificate(GlobalSettings.Get("CONFIG_CompanySubject"), GlobalSettings.Get("CONFIG_CompanyName"), GlobalSettings.Get("CONFIG_CompanyCountryCode"));

				GlobalSettings.Store("SAML_Signing_Certificate", Helpers.X509Certificate2ToString(_signingCertificate));
			}
			else
			{
				_signingCertificate = Helpers.StringToX509Certificate2(GlobalSettings.Get("SAML_Signing_Certificate"));
				Console.WriteLine(_signingCertificate.Thumbprint);
			}
		}

		/// <summary>
		/// Store a configuration inside the database and global settings
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void Store(string key, string value)
		{
			_appSettings[key] = value;

			UpdateDatabaseSettings();
		}

		public static void UpdateDatabaseSettings()
		{
			SAMLPortalContext context = new SAMLPortalContext();

			foreach (var key in _appSettings.Keys)
			{
				var settingExists = context.KeyValue.Any(k => k.Key == key);

				if (settingExists)
				{
					KeyValue settingToFind = context.KeyValue.Single(k => k.Key == key);
					settingToFind.Value = _appSettings[key];
				}
				else
				{
					KeyValue newSetting = new KeyValue
					{
						Key = key,
						Value = _appSettings[key]
					};

					context.KeyValue.Add(newSetting);
				}
			}

			context.SaveChanges();
		}

		private static void UpdateFromEnvironment()
		{
			_appSettings.Clear();

			// Debug Only:
			_appSettings.Add("CONFIG_PATH", "/tmp/SAMLPortal");

			IDictionary environment = Environment.GetEnvironmentVariables();
			foreach (var key in environment.Keys)
			{
				if (key.ToString().StartsWith("SP_") && !key.ToString().StartsWith("SP_MYSQL"))
				{
					string shortenedKey = key.ToString().Remove(0, 3);
					_appSettings.Add(shortenedKey, environment[key].ToString());
				}
			}
		}

		private static void UpdateFromDatabase()
		{
			UpdateFromEnvironment();

			SAMLPortalContext context = new SAMLPortalContext();
			var settings = context.KeyValue.ToList();

			foreach (var setting in settings)
			{
				_appSettings[setting.Key] = setting.Value;
			}
		}

		public static string Get(string key)
		{
			try
			{
				return _appSettings[key];
			}
			catch (KeyNotFoundException ex)
			{
				return null;
			}

		}

		public static int? GetInt(string key)
		{
			try
			{
				return Convert.ToInt32(_appSettings[key]);
			}
			catch (Exception e) when (e is KeyNotFoundException)
			{
				return null;
			}
		}
	}
}