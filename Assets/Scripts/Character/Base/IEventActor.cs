using BC.ODCC;

using UnityEngine;

namespace BC.Base
{
	public interface IEventActor : IOdccComponent
	{
	}
	public interface IWeaponFire : IEventActor
	{
		void DoFire();
	}
	public interface IWeaponReload : IEventActor
	{
		void DoReload();
	}
	public interface IAgentMoveStop<T> : IEventActor where T : class
	{
		void DoAgentMoveStop(T agent);
	}
	public interface IAgentMoveStart<T> : IEventActor where T : class
	{
		void DoAgentMoveStart(T agent, Vector3 target);
	}

	/// <summary>
	/// �� �߰ߵ�
	/// </summary>
	/// <typeparam name="T"> ������  </typeparam>
	/// <typeparam name="TE"> �߰ߵ� �� </typeparam>
	public interface IEnemyDetected<T, TE> : IEventActor where T : class, IEventActor where TE : class
	{
		void DoEnemyDetected(T agent, TE enemy);
	}
	/// <summary>
	/// ���� �����
	/// </summary>
	/// <typeparam name="T"> ������ </typeparam>
	/// <typeparam name="TE"> �縮�� �� </typeparam>
	public interface IEnemyDisappear<T, TE> : IEventActor where T : class, IEventActor where TE : class
	{
		void DoEnemyDisappear(T agent, TE enemy);
	}
	/// <summary>
	/// ���� ����ȭ ��
	/// </summary>
	/// <typeparam name="T"> ������ </typeparam>
	/// <typeparam name="TE"> óġ�� �� </typeparam>
	public interface IEnemyNeutralized<T, TE> : IEventActor where T : class, IEventActor where TE : class
	{
		void DoEnemyNeutralized(T agent, TE enemy);
	}
	/// <summary>
	/// �Ʊ��� ������
	/// </summary>
	/// <typeparam name="T"> ������ </typeparam>
	/// <typeparam name="TE"> ������ �� </typeparam>
	public interface IAllyTakeDown<T, TE> : IEventActor where T : class, IEventActor where TE : class
	{
		void DoAllyTakeDown(T agent, TE enemy);
	}
}
