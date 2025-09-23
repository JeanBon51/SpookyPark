using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;

public class NewtonSoftConverterInitializer : MonoBehaviour {

	private static bool isConverterSet = false;

	public static void SetUpJsonConverterSettings() {
		if (isConverterSet) return;

		JsonConvert.DefaultSettings = () => new JsonSerializerSettings() {
			Converters = new List<JsonConverter>() {
					new StringEnumConverter(),
				},
			Formatting = Formatting.Indented,
		};

		isConverterSet = true;
	}
}
