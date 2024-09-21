using BC.Base;

using Sirenix.OdinInspector; // Odin Inspector 사용 시 필요

using UnityEngine;

public class TestRandom : MonoBehaviour
{
	// 샘플 수 설정
	public int sampleCount = 100;

	public float parameterA;
	public float parameterB;
	[ReadOnly]
	public int viewCount = 100;
	// 각 구간의 빈도를 나타내는 변수들 (0.1 단위)
	[ProgressBar(0, "viewCount"), LabelText("[-1.0, -0.9)")] public int value_0_9;
	[ProgressBar(0, "viewCount"), LabelText("[-0.9, -0.8)")] public int value_9_8;
	[ProgressBar(0, "viewCount"), LabelText("[-0.8, -0.7)")] public int value_8_7;
	[ProgressBar(0, "viewCount"), LabelText("[-0.7, -0.6)")] public int value_7_6;
	[ProgressBar(0, "viewCount"), LabelText("[-0.6, -0.5)")] public int value_6_5;
	[ProgressBar(0, "viewCount"), LabelText("[-0.5, -0.4)")] public int value_5_4;
	[ProgressBar(0, "viewCount"), LabelText("[-0.4, -0.3)")] public int value_4_3;
	[ProgressBar(0, "viewCount"), LabelText("[-0.3, -0.2)")] public int value_3_2;
	[ProgressBar(0, "viewCount"), LabelText("[-0.2, -0.1)")] public int value_2_1;
	[ProgressBar(0, "viewCount"), LabelText("[-0.1, -0.0)")] public int value_1_0;
	[ProgressBar(0, "viewCount"), LabelText("[0.0, 0.1)")] public int value_0_1;
	[ProgressBar(0, "viewCount"), LabelText("[0.1, 0.2)")] public int value_1_2;
	[ProgressBar(0, "viewCount"), LabelText("[0.2, 0.3)")] public int value_2_3;
	[ProgressBar(0, "viewCount"), LabelText("[0.3, 0.4)")] public int value_3_4;
	[ProgressBar(0, "viewCount"), LabelText("[0.4, 0.5)")] public int value_4_5;
	[ProgressBar(0, "viewCount"), LabelText("[0.5, 0.6)")] public int value_5_6;
	[ProgressBar(0, "viewCount"), LabelText("[0.6, 0.7)")] public int value_6_7;
	[ProgressBar(0, "viewCount"), LabelText("[0.7, 0.8)")] public int value_7_8;
	[ProgressBar(0, "viewCount"), LabelText("[0.8, 0.9)")] public int value_8_9;
	[ProgressBar(0, "viewCount"), LabelText("[0.9, 1.0)")] public int value_9_0;

	// 버튼 클릭 시 실행되는 메서드
	[Button]
	public void Compute()
	{
		// 최소 샘플 수 보장
		if(sampleCount < 20) sampleCount = 20;

		// 모든 bin 초기화
		value_0_9 = 0;
		value_9_8 = 0;
		value_8_7 = 0;
		value_7_6 = 0;
		value_6_5 = 0;
		value_5_4 = 0;
		value_4_3 = 0;
		value_3_2 = 0;
		value_2_1 = 0;
		value_1_0 = 0;
		value_0_1 = 0;
		value_1_2 = 0;
		value_2_3 = 0;
		value_3_4 = 0;
		value_4_5 = 0;
		value_5_6 = 0;
		value_6_7 = 0;
		value_7_8 = 0;
		value_8_9 = 0;
		value_9_0 = 0;

		// 샘플 생성 
		for(int i = 0 ; i < sampleCount ; i++)
		{
			// -1부터 1까지의 랜덤 값 생성
			//float randomValue = Random.Range(-1f, 1f);

			float randomValue = Utils.RandomDistribution.NextBeta(parameterA, parameterB);
			randomValue = randomValue * 2f - 1f;

			// bin 인덱스 계산 (0 ~ 19)
			int binIndex = Mathf.FloorToInt((randomValue + 1f) / 0.1f);
			if(binIndex < 0)
				binIndex = 0;
			if(binIndex >= 20)
				binIndex = 19;

			// 해당 bin에 빈도 증가
			switch(binIndex)
			{
				case 0: value_0_9++; break;
				case 1: value_9_8++; break;
				case 2: value_8_7++; break;
				case 3: value_7_6++; break;
				case 4: value_6_5++; break;
				case 5: value_5_4++; break;
				case 6: value_4_3++; break;
				case 7: value_3_2++; break;
				case 8: value_2_1++; break;
				case 9: value_1_0++; break;
				case 10: value_0_1++; break;
				case 11: value_1_2++; break;
				case 12: value_2_3++; break;
				case 13: value_3_4++; break;
				case 14: value_4_5++; break;
				case 15: value_5_6++; break;
				case 16: value_6_7++; break;
				case 17: value_7_8++; break;
				case 18: value_8_9++; break;
				case 19: value_9_0++; break;
				default: break;
			}
		}
		viewCount = Mathf.Max(
			value_0_9, value_9_8, value_8_7, value_7_6, value_6_5,
			value_5_4, value_4_3, value_3_2, value_2_1, value_1_0,
			value_0_1, value_1_2, value_2_3, value_3_4, value_4_5,
			value_5_6, value_6_7, value_7_8, value_8_9, value_9_0
		);


	}
}
