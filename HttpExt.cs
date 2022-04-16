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
			if (handler == null) throw new ArgumentNullException("handler cannot be null");
			var field = handler.GetType().GetField("_startRequest", BindingFlags.NonPublic | BindingFlags.Instance); // Fieldname has a _ due to being private
			if (field == null)
				throw new ArgumentNullException($"Field _startRequest not found in handler type {handler.GetType()}");
			var startRequest = field.GetValue(handler) as Action<object>;
			if (startRequest == null)
				throw new ArgumentNullException("startRequest value is null");

			Action<object> newStartRequest = obj =>
			{
				var webReqField = obj.GetType().GetField("webRequest", BindingFlags.NonPublic | BindingFlags.Instance);
				if (webReqField == null)
					throw new ArgumentNullException($"webRequest is not set on type {obj.GetType()}");
				var webRequest = webReqField.GetValue(obj) as HttpWebRequest;
				if (webRequest == null)
					throw new ArgumentNullException($"webRequest is null");
				webRequest.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(biep);
				startRequest(obj); //call original action
			};
			field.SetValue(handler, newStartRequest); //replace original 'startRequest' with the one above

		}
	}
}
