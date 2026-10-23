using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;

namespace EmployeeManagement.Tests;

public static class ControllerTestExtensions
{
	public static T WithTempData<T>(this T controller) where T : Controller
	{
		controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new NullTempDataProvider());
		return controller;
	}

	private class NullTempDataProvider : ITempDataProvider
	{
		public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
		public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
	}
}