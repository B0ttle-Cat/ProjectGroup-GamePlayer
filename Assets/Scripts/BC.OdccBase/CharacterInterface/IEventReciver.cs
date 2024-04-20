using BC.ODCC;

using UnityEngine;

namespace BC.OdccBase
{
	public interface IEventReciver : IOdccComponent
	{
		public interface IWeaponFire : IEventReciver
		{
			void DoFire();
		}
		public interface IWeaponReload : IEventReciver
		{
			void DoReload();
		}
		public interface IAgentMoveStop<T> : IEventReciver where T : class
		{
			void DoAgentMoveStop(T agent);
		}
		public interface IAgentMoveStart<T> : IEventReciver where T : class
		{
			void DoAgentMoveStart(T agent, Vector3 target);
		}
	}
	/// <summary>
	/// 적 발견됨
	/// </summary>
	/// <typeparam name="T"> 보고자  </typeparam>
	/// <typeparam name="TE"> 발견된 적 </typeparam>
	public interface IEnemyDetected<T, TE> : IEventReciver where T : class, IEventReciver where TE : class
	{
		void DoEnemyDetected(T agent, TE enemy);
	}
	/// <summary>
	/// 적이 사라짐
	/// </summary>
	/// <typeparam name="T"> 보고자 </typeparam>
	/// <typeparam name="TE"> 사리진 적 </typeparam>
	public interface IEnemyDisappear<T, TE> : IEventReciver where T : class, IEventReciver where TE : class
	{
		void DoEnemyDisappear(T agent, TE enemy);
	}
	/// <summary>
	/// 적을 무력화 함
	/// </summary>
	/// <typeparam name="T"> 보고자 </typeparam>
	/// <typeparam name="TE"> 처치한 적 </typeparam>
	public interface IEnemyNeutralized<T, TE> : IEventReciver where T : class, IEventReciver where TE : class
	{
		void DoEnemyNeutralized(T agent, TE enemy);
	}
	/// <summary>
	/// 아군이 쓰러짐
	/// </summary>
	/// <typeparam name="T"> 보고자 </typeparam>
	/// <typeparam name="TE"> 공격한 적 </typeparam>
	public interface IAllyTakeDown<T, TE> : IEventReciver where T : class, IEventReciver where TE : class
	{
		void DoAllyTakeDown(T agent, TE enemy);
	}
}
