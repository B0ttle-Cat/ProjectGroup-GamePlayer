#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

using UnityEngine;

using Debug = UnityEngine.Debug;

public class TestScript : MonoBehaviour
{
	public int listCount = 1000;
	public bool usingCash = false;

	private List<Item> listA = new List<Item>();
	private List<Item> listB = new List<Item>();

	HashSet<int> cashHashSet;
	BigInteger cashBigInteger;
	BitArray BitArray;
	public struct Item
	{
		public int ID;

		public Item(int id)
		{
			ID = id;
		}
	}
	public void OnEnable()
	{
		listA = new List<Item>();
		listB = new List<Item>();
		for(int i = 0 ; i < listCount ; i++)
		{
			listA.Add(new Item(i));
			listB.Add(new Item(i));
		}

		ShuffleList(listA);
		ShuffleList(listB);

		if(usingCash)
		{
			cashHashSet = new HashSet<int>();
			foreach(var item in listB)
			{
				cashHashSet.Add(item.ID);
			}
			cashBigInteger = ConvertToBigInteger(listB);

			BitArray = new BitArray(listB.Count);

			// 리스트2의 ID를 비트 벡터에 설정
			foreach(var item in listB)
			{
				BitArray.Set(item.ID, true);
			}
		}
	}


	public void Update()
	{
		// 시간 측정
		Stopwatch stopwatch = new Stopwatch();

		// HashSet 방법
		stopwatch.Start();
		CompareListsHashSet(listA, listB);
		stopwatch.Stop();
		long hasSetTime = stopwatch.ElapsedTicks;

		//// BigInteger 방법
		//stopwatch.Reset();
		//stopwatch.Start();
		//CompareListsBigInteger(listA, listB);
		//stopwatch.AttackStop();
		//long bigIntegerTime = stopwatch.ElapsedTicks;

		stopwatch.Reset();
		stopwatch.Start();
		CompareLists(listA, listB);
		stopwatch.Stop();
		long bitVector = stopwatch.ElapsedTicks;

		Debug.Log($"HashSet VS bitVector == {hasSetTime} VS {bitVector} == {(hasSetTime > bitVector ? "bitVector 더 빠름" : "HashSet이 더 빠름")}");
		// HashSet VS bitVector == 9504 VS 3550 == bitVector 더 빠름
	}

	void CompareListsHashSet(List<Item> list1, List<Item> list2)
	{
		HashSet<int> set1 = new HashSet<int>();
		HashSet<int> set2 = usingCash ? cashHashSet : new HashSet<int>();

		foreach(var item in list1)
		{
			set1.Add(item.ID);
		}

		if(!usingCash)
		{
			foreach(var item in list2)
			{
				set2.Add(item.ID);
			}
		}

		set1.SymmetricExceptWith(set2);

	}

	void CompareListsBigInteger(List<Item> list1, List<Item> list2)
	{
		BigInteger bigInt1 = ConvertToBigInteger(list1);
		BigInteger bigInt2 = usingCash ? cashBigInteger : ConvertToBigInteger(list2);

		bool diff = bigInt1 != bigInt2;
	}

	BigInteger ConvertToBigInteger(List<Item> list)
	{
		BigInteger result = 0;
		foreach(var item in list)
		{
			result |= 1L << item.ID;
		}
		return result;
	}

	void CompareLists(List<Item> list1, List<Item> list2)
	{
		// 비트 벡터 생성
		BitArray bitArray1 = new BitArray(list1.Count);
		BitArray bitArray2 = usingCash ? this.BitArray : new BitArray(list2.Count);

		foreach(var item in list1)
		{
			bitArray1.Set(item.ID, true);
		}

		if(!usingCash)
		{
			foreach(var item in list2)
			{
				bitArray2.Set(item.ID, true);
			}
		}

		//int bitArray1Length = bitArray1.Length;
		//int bitArray2Length = bitArray2.Length;
		//if (bitArray1Length != bitArray2Length) return;

		//for (int i = 0; i < bitArray1Length; i++)
		//{
		//	if (bitArray1.Get(i) != bitArray2.Get(1))
		//	{
		//		return;
		//	}
		//}

		BitArray comparison = (BitArray)bitArray1.Clone();
		comparison.Xor(bitArray2);

		// 결과 확인
		bool areEqual = true;
		for(int i = 0 ; i < comparison.Length ; i++)
		{
			if(comparison[i])
			{
				areEqual = false;
				break;
			}
		}
	}


	void ShuffleList<T>(List<T> list)
	{
		System.Random random = new System.Random();
		int n = list.Count;
		for(int i = 0 ; i < n ; i++)
		{
			int j = random.Next(i, n); // i부터 n-1까지의 랜덤 인덱스를 선택
			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
	}
}
#endif