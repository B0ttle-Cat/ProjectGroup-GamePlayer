using System;

using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayManager
{
	[Serializable]
	public partial class CharacterValueSetting : IUnitPlayValue
	{
		[SerializeField] private AbilityMath.AbilityValue<float> _시야거리;
		[SerializeField] private AbilityMath.AbilityValue<float> _반응거리;
		[SerializeField] private AbilityMath.AbilityValue<float> _추적거리;
		[SerializeField] private AbilityMath.AbilityValue<float> _공격러리;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<int> _최대채력;
		[SerializeField] private AbilityMath.AbilityValue<int> _최대기력;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _공격력;
		[SerializeField] private AbilityMath.AbilityValue<float> _방어력;
		[SerializeField] private AbilityMath.AbilityValue<float> _치유력;
		[SerializeField] private AbilityMath.AbilityValue<float> _제압력;
		[SerializeField] private AbilityMath.AbilityValue<float> _저항력;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _명중수치;
		[SerializeField] private AbilityMath.AbilityValue<float> _회피수치;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _조종지연;
		[SerializeField] private AbilityMath.AbilityValue<float> _공격지연;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _치명공격수치;
		[SerializeField] private AbilityMath.AbilityValue<float> _치명공격증가율;
		[SerializeField] private AbilityMath.AbilityValue<float> _치명방어수치;
		[SerializeField] private AbilityMath.AbilityValue<float> _치명방어증가율;

		public AbilityMath.AbilityValue<float> 시야거리 { get => _시야거리; set => _시야거리 = value; }
		public AbilityMath.AbilityValue<float> 반응거리 { get => _반응거리; set => _반응거리 = value; }
		public AbilityMath.AbilityValue<float> 추적거리 { get => _추적거리; set => _추적거리 = value; }
		public AbilityMath.AbilityValue<float> 공격러리 { get => _공격러리; set => _공격러리 = value; }
		public AbilityMath.AbilityValue<int> 채력 { get => _최대채력; set => _최대채력 = value; }
		public AbilityMath.AbilityValue<int> 기력 { get => _최대기력; set => _최대기력 = value; }
		public AbilityMath.AbilityValue<float> 공격력 { get => _공격력; set => _공격력 = value; }
		public AbilityMath.AbilityValue<float> 방어력 { get => _방어력; set => _방어력 = value; }
		public AbilityMath.AbilityValue<float> 치유력 { get => _치유력; set => _치유력 = value; }
		public AbilityMath.AbilityValue<float> 제압력 { get => _제압력; set => _제압력 = value; }
		public AbilityMath.AbilityValue<float> 저항력 { get => _저항력; set => _저항력 = value; }
		public AbilityMath.AbilityValue<float> 명중수치 { get => _명중수치; set => _명중수치 = value; }
		public AbilityMath.AbilityValue<float> 회피수치 { get => _회피수치; set => _회피수치 = value; }
		public AbilityMath.AbilityValue<float> 조종지연 { get => _조종지연; set => _조종지연 = value; }
		public AbilityMath.AbilityValue<float> 공격지연 { get => _공격지연; set => _공격지연 = value; }
		public AbilityMath.AbilityValue<float> 치명공격수치 { get => _치명공격수치; set => _치명공격수치 = value; }
		public AbilityMath.AbilityValue<float> 치명공격증가율 { get => _치명공격증가율; set => _치명공격증가율 = value; }
		public AbilityMath.AbilityValue<float> 치명방어수치 { get => _치명방어수치; set => _치명방어수치 = value; }
		public AbilityMath.AbilityValue<float> 치명방어증가율 { get => _치명방어증가율; set => _치명방어증가율 = value; }
		public void Dispose()
		{

		}
	}
}
