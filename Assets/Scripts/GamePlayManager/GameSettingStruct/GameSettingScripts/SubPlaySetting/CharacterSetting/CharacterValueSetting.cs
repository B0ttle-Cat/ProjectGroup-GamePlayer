using System;

using BC.OdccBase;

using UnityEngine;

namespace BC.GamePlayManager
{
	[Serializable]
	public partial class CharacterValueSetting : IUnitPlayValue
	{
		[SerializeField] private AbilityMath.AbilityValue<float> _�þ߰Ÿ�;
		[SerializeField] private AbilityMath.AbilityValue<float> _�����Ÿ�;
		[SerializeField] private AbilityMath.AbilityValue<float> _�����Ÿ�;
		[SerializeField] private AbilityMath.AbilityValue<float> _���ݷ���;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<int> _�ִ�ä��;
		[SerializeField] private AbilityMath.AbilityValue<int> _�ִ���;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _���ݷ�;
		[SerializeField] private AbilityMath.AbilityValue<float> _����;
		[SerializeField] private AbilityMath.AbilityValue<float> _ġ����;
		[SerializeField] private AbilityMath.AbilityValue<float> _���з�;
		[SerializeField] private AbilityMath.AbilityValue<float> _���׷�;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _���߼�ġ;
		[SerializeField] private AbilityMath.AbilityValue<float> _ȸ�Ǽ�ġ;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _��������;
		[SerializeField] private AbilityMath.AbilityValue<float> _��������;
		[Space]
		[SerializeField] private AbilityMath.AbilityValue<float> _ġ����ݼ�ġ;
		[SerializeField] private AbilityMath.AbilityValue<float> _ġ�����������;
		[SerializeField] private AbilityMath.AbilityValue<float> _ġ�����ġ;
		[SerializeField] private AbilityMath.AbilityValue<float> _ġ����������;

		public AbilityMath.AbilityValue<float> �þ߰Ÿ� { get => _�þ߰Ÿ�; set => _�þ߰Ÿ� = value; }
		public AbilityMath.AbilityValue<float> �����Ÿ� { get => _�����Ÿ�; set => _�����Ÿ� = value; }
		public AbilityMath.AbilityValue<float> �����Ÿ� { get => _�����Ÿ�; set => _�����Ÿ� = value; }
		public AbilityMath.AbilityValue<float> ���ݷ��� { get => _���ݷ���; set => _���ݷ��� = value; }
		public AbilityMath.AbilityValue<int> ä�� { get => _�ִ�ä��; set => _�ִ�ä�� = value; }
		public AbilityMath.AbilityValue<int> ��� { get => _�ִ���; set => _�ִ��� = value; }
		public AbilityMath.AbilityValue<float> ���ݷ� { get => _���ݷ�; set => _���ݷ� = value; }
		public AbilityMath.AbilityValue<float> ���� { get => _����; set => _���� = value; }
		public AbilityMath.AbilityValue<float> ġ���� { get => _ġ����; set => _ġ���� = value; }
		public AbilityMath.AbilityValue<float> ���з� { get => _���з�; set => _���з� = value; }
		public AbilityMath.AbilityValue<float> ���׷� { get => _���׷�; set => _���׷� = value; }
		public AbilityMath.AbilityValue<float> ���߼�ġ { get => _���߼�ġ; set => _���߼�ġ = value; }
		public AbilityMath.AbilityValue<float> ȸ�Ǽ�ġ { get => _ȸ�Ǽ�ġ; set => _ȸ�Ǽ�ġ = value; }
		public AbilityMath.AbilityValue<float> �������� { get => _��������; set => _�������� = value; }
		public AbilityMath.AbilityValue<float> �������� { get => _��������; set => _�������� = value; }
		public AbilityMath.AbilityValue<float> ġ����ݼ�ġ { get => _ġ����ݼ�ġ; set => _ġ����ݼ�ġ = value; }
		public AbilityMath.AbilityValue<float> ġ����������� { get => _ġ�����������; set => _ġ����������� = value; }
		public AbilityMath.AbilityValue<float> ġ�����ġ { get => _ġ�����ġ; set => _ġ�����ġ = value; }
		public AbilityMath.AbilityValue<float> ġ���������� { get => _ġ����������; set => _ġ���������� = value; }
		public void Dispose()
		{

		}
	}
}
