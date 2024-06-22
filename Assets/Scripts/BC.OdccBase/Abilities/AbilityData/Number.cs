namespace BC.OdccBase
{
	public interface INumber
	{
	}
	public interface INumber<T> : INumber
	{
		T Value { get; set; }

		/// <summary>
		/// +
		/// </summary>
		public INumber<T> Plus(INumber<T> v);
		/// <summary>
		/// -
		/// </summary>
		public INumber<T> Minus(INumber<T> v);
		/// <summary>
		/// *
		/// </summary>
		public INumber<T> Multiply(INumber<T> v);
		/// <summary>
		/// /
		/// </summary>
		public INumber<T> Division(INumber<T> v);
	}
	public abstract class Number<T> : INumber<T>
	{
		public T Value { get; set; }

		public abstract INumber<T> Plus(INumber<T> v);
		public abstract INumber<T> Minus(INumber<T> v);
		public abstract INumber<T> Multiply(INumber<T> v);
		public abstract INumber<T> Division(INumber<T> v);
	}

	public struct Int_Number : INumber<int>
	{
		public int Value { get; set; }

		public INumber<int> Plus(INumber<int> v)
		{
			return new Int_Number { Value = Value + ((Int_Number)v).Value };
		}
		public INumber<int> Minus(INumber<int> v)
		{
			return new Int_Number { Value = Value - ((Int_Number)v).Value };
		}
		public INumber<int> Multiply(INumber<int> v)
		{
			return new Int_Number { Value = Value * ((Int_Number)v).Value };
		}
		public INumber<int> Division(INumber<int> v)
		{
			return new Int_Number { Value = Value / ((Int_Number)v).Value };
		}
	}
}
