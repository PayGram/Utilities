using System;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Utilities.Http.Extentions
{
	public static class HttpExt
	{
		/// <summary>
		/// sets the ip endpoint to use for the connections estabilished by this HttpClientHandler
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="biep"></param>
		public static void SetServicePointOptions(this HttpClientHandler handler, BindIPEndPoint biep)
		{
			var field = handler.GetType().GetField("_startRequest", BindingFlags.NonPublic | BindingFlags.Instance); // Fieldname has a _ due to being private

			var startRequest = (Action<object>)field.GetValue(handler);
			Action<object> newStartRequest = obj =>
			{
				var webReqField = obj.GetType().GetField("webRequest", BindingFlags.NonPublic | BindingFlags.Instance);
				var webRequest = webReqField.GetValue(obj) as HttpWebRequest;
				webRequest.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(biep);
				startRequest(obj); //call original action
			};

			field.SetValue(handler, newStartRequest); //replace original 'startRequest' with the one above

		}
	}
}
