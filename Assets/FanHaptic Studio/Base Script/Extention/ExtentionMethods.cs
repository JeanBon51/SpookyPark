using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using BigInteger = System.Numerics.BigInteger;

static class ExtentionMethods {
	private static System.Random rng = new System.Random();

	public static void Shuffle<T>(this IList<T> list) {
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void ForEach<T>(this T[] arr, UnityAction<T> action) {
		foreach (T t in arr) { action.Invoke(t); }
	}

	public static void ForEach<T>(this T[,] arr, UnityAction<T> action) {
		foreach (T t in arr) { action.Invoke(t); }
	}

	public static void ForEach<T>(this List<T> list, UnityAction<T> action) {
		foreach (T t in list) { action.Invoke(t); }
	}

	public static void ForEach<K, V>(this Dictionary<K, V> list, UnityAction<KeyValuePair<K, V>> action) {
		foreach (KeyValuePair<K, V> pair in list) { action.Invoke(pair); }
	}

	public static T[] WhereArr<T>(this T[,] arr, Func<T,bool> condition) {
		Stack<T> stackResult = new Stack<T>();
		foreach (T t in arr) {
			if (condition.Invoke(t))
				stackResult.Push(t);
		}
		return stackResult.ToArray();
	}

	public static List<T> WhereList<T>(this T[,] arr, Func<T, bool> condition) {
		List<T> stackResult = new List<T>();
		foreach (T t in arr) {
			if (condition.Invoke(t))
				stackResult.Add(t);
		}
		return stackResult;
	}

	public static T[] ReconstructWithElementAtIndex<T>(this T[] arr, T elem, int index) {
		List<T> nl = new List<T>(arr);
		nl.Remove(elem);
		return ReconstructList(nl, elem, index);
	}

	public static T[] ReconstructWithElementAtIndex<T>(this T[] arr, int elementIndex, int index) {
		List<T> nl = new List<T>(arr);
		T elem = nl[elementIndex];
		nl.RemoveAt(elementIndex);
		return ReconstructList(nl, elem, index);
	}

	private static T[] ReconstructList<T>(List<T> list, T elem, int index) {
		T[] nl = new T[list.Count + 1];
		for (int i = 0; i < index; i++) {
			nl[i] = list[i];
		}
		nl[index] = elem;
		for (int i = index + 1; i < list.Count + 1; i++) {
			nl[i] = list[i - 1];
		}
		return nl;
	}

	public static Color Alpha(this Color color, float a) {
		if (a > 1.0f) a = 1.0f;
		else if (a < 0.0f) a = 0.0f;
		return new Color(color.r, color.g, color.b, a);
	}

	public static string ToStringWithNotation(this System.Numerics.BigInteger value, string[] notations = null)
	{
		if (notations == null)
		{
			notations = new []{ "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax","ay","az" };
		}
		
		BigInteger amount = value;
		BigInteger lastAmount = amount;
		int num = 0;
		while (amount >= 1000)
		{
			num++;
			lastAmount = amount;
			amount /= 1000;
		}

		string result = "";
        
		if(num == 0) result = $"{amount}";
		else
		{
			BigInteger x = lastAmount - amount * 1000;
			string s;
			if (x < 100)
			{
				if (x < 10) s = "00" + x;
				else s = "0" + x;
			}
			else s = x.ToString();
			result = $"{amount},{s}";
		}
        
		result = result.Length > 5 ? result.Substring(0, 6) : result;
        
		return result + " " + (num >= notations.Length ? notations[^1] : notations[num]);
	}

	public static System.Numerics.BigInteger GetBigInteger(this SetterBigInteger value)
	{
		int nb = value.notation.GetHashCode();
		BigInteger v = value.value;
		for (int i = 0; i < nb; i++)
		{
			v *= 1000;
		}

		return v;
	}
	
	public static int GetListenerNumber(this UnityEventBase unityEvent)
	{
		var field = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
		var invokeCallList = field.GetValue(unityEvent);
		var property = invokeCallList.GetType().GetProperty("Count");
		return (int)property.GetValue(invokeCallList);
	}
	
	public static Vector3 GetScrollToHorizontal(this ScrollRect instance, RectTransform child) => instance.content.localPosition.SetX(-(instance.viewport.localPosition.x + child.localPosition.x));
	public static ScrollRect ScrollToHorizontal(this ScrollRect instance, RectTransform child)
	{
		Canvas.ForceUpdateCanvases();
		instance.content.localPosition = GetScrollToHorizontal(instance, child);
		return instance;
	}
	public static Vector3 GetScrollToVertical(this ScrollRect instance, RectTransform child) => instance.content.localPosition.SetY(-(instance.viewport.localPosition.y + child.localPosition.y));
	public static ScrollRect ScrollToVertical(this ScrollRect instance, RectTransform child)
	{
		Canvas.ForceUpdateCanvases();
		instance.content.localPosition = GetScrollToVertical(instance, child);
		return instance;
	}
	public static Quaternion SetZ(this Quaternion v, float value) => new Quaternion(v.x, v.y, value, v.w);
	public static Quaternion SetX(this Quaternion v, float value) => new Quaternion(value, v.y, v.z, v.w);
	public static Quaternion SetY(this Quaternion v, float value) => new Quaternion(v.x, value, v.z, v.w);
	public static Vector3 SetX(this Vector3 v, float value)
	{
		v.x = value;
		return v;
	}
	public static Vector3 SetY(this Vector3 v, float value)
	{
		v.y = value;
		return v;
	}
	public static Vector3 SetZ(this Vector3 v, float value)
	{
		v.z = value;
		return v;
	}

	public static Vector2 SetX(this Vector2 v, float value)
	{
		v.x = value;
		return v;
	}
	public static Vector2 SetY(this Vector2 v, float value)
	{
		v.y = value;
		return v;
	}
	
	public static string Bold(this string str) => "<b>" + str + "</b>";
	public static string Color(this string str, string clr) => string.Format("<color={0}>{1}</color>", clr, str);
	public static string Italic(this string str) => "<i>" + str + "</i>";
	public static string Size(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
}

[System.Serializable]
public class SetterBigInteger
{
	public SetterBigInteger()
	{
	}
	public SetterBigInteger(SetterBigInteger setterBigInteger)
	{
		value = setterBigInteger.value;
		notation = setterBigInteger.notation;
	}
	
	public enum NotationType {
		normal = 0,
		K = 1,
		M = 2,
		B = 3,
		T = 4,
		aa = 5,
		ab = 6,
		ac = 7,
		ad = 8,
		ae = 9,
		af = 10,
		ag = 11,
		ah = 12,
		ai = 13,
		aj,
		ak,
		al,
		am,
		an,
		ao,
		ap,
		aq,
		ar,
		As,
		at,
		au,
		av,
		aw,
		ax,
		ay,
		az

	}
	public int value = 10;
	public NotationType notation = NotationType.normal;

	public string GetValueAsString()
	{
		if (this.notation == NotationType.normal) return value.ToString();
		else return $"{value} {this.notation}";
	}
}