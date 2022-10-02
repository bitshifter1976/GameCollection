using System;
using System.Configuration;
using System.Globalization;

namespace Framework
{
	public class ConfigBase
	{
        /// <summary>
		/// Get a value from config file. If key does not exist, return default value.
		/// </summary>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="name">Config entry key.</param>
		/// <param name="defaultValue">Default value.</param>
		/// <returns>Config entry value or default value.</returns>
		public static T GetOrDefault<T>(string name, T defaultValue)
		{
			try
			{
				// get value from config file
				var value = ConfigurationManager.AppSettings.Get(name);
				// if value null or empty then return default
				if (string.IsNullOrEmpty(value))
					return defaultValue;
				// enum
				if (defaultValue.GetType().IsEnum)
					return (T)Enum.Parse(typeof(T), value);
				// double
				if (typeof(T) == typeof(double))
					return (T)Convert.ChangeType(double.Parse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture), typeof(double));
				// other types
				return (T)Convert.ChangeType(value, typeof(T));
			}
			catch (Exception ex)
			{
				Log.Out(LogLevel.Error, $"Config.GetOrDefault Exception: {ex}");
				return defaultValue;
			}
		}
    }
}
