using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
	public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dict, K key, V defaultValue) {
		return dict.ContainsKey (key) ? dict [key] : defaultValue;
	}
}
