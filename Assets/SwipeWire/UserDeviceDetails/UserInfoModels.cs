using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP
{

	/// <summary>
	/// The Geo data for a user.
	/// You are free to use ip-api.com for non-commercial use. We do not allow commercial use without prior approval.
	/// So if you want to use it in commercial use -> https://signup.ip-api.com/
	/// 
	///Check http://ip-api.com/docs/api:json for commercial and usage limits
	/// 
	/// <code>
	/// {
	/// 	"status": "success",
	/// 	"country": "COUNTRY",
	/// 	"countryCode": "COUNTRY CODE",
	/// 	"region": "REGION CODE",
	/// 	"regionName": "REGION NAME",
	/// 	"city": "CITY",
	/// 	"zip": "ZIP CODE",
	/// 	"lat": LATITUDE,
	/// 	"lon": LONGITUDE,
	/// 	"timezone": "TIME ZONE",
	/// 	"isp": "ISP NAME",
	/// 	"org": "ORGANIZATION NAME",
	/// 	"as": "AS NUMBER / NAME",
	/// 	"query": "IP ADDRESS USED FOR QUERY"
	/// }
	/// </code>
	/// 
	/// </summary>
	public class LocationData
	{
		/// <summary>
		/// The status that is returned if the response was successful.
		/// </summary>
		public string status;
		public string country;
		public string countryCode;
		public string region;
		public string regionName;
		public string city;
		public string zip;
		public string lat;
		public string lon;
		public string timezone;
		public string isp;
		public string org;
		public string query;
	}
}
