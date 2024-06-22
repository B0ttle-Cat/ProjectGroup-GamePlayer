using System;

using BC.ODCC;

using Sirenix.OdinInspector;

namespace BC.OdccBase
{
	[Serializable]
	public abstract class NumberValue : DataObject, IOdccData
	{
		public abstract dynamic Value { get; set; }
	}
	[Serializable]
	public abstract class NumberValue<T> : NumberValue where T : unmanaged
	{
		public NumberValue() { value = default; }
		public NumberValue(T value) => this.value = value;
		public T value;
		public override dynamic Value { get => value; set => this.value = value; }
		[ResponsiveButtonGroup]
		public void Default()
		{
			value = default;
		}
	}
	public class IntNumber : NumberValue<int>
	{
		[ResponsiveButtonGroup]
		public void Min()
		{
			value = int.MinValue;
		}
		[ResponsiveButtonGroup]
		public void Max()
		{
			value = int.MaxValue;
		}
	}
	public class LongNumber : NumberValue<long>
	{
		[ResponsiveButtonGroup]
		public void Min()
		{
			value = long.MinValue;
		}
		[ResponsiveButtonGroup]
		public void Max()
		{
			value = long.MaxValue;
		}
	}
	public class FloatNumber : NumberValue<float>
	{
		[ResponsiveButtonGroup]
		public void Min()
		{
			value = float.MinValue;
		}
		[ResponsiveButtonGroup]
		public void Max()
		{
			value = float.MaxValue;
		}
		[ResponsiveButtonGroup]
		public void Epsilon()
		{
			value = float.Epsilon;
		}
		[ResponsiveButtonGroup]
		public void NaN()
		{
			value = float.NaN;
		}
		[ResponsiveButtonGroup]
		public void PInfinity()
		{
			value = float.PositiveInfinity;
		}
		[ResponsiveButtonGroup]
		public void NInfinity()
		{
			value = float.NegativeInfinity;
		}
	}
	public class DoubleNumber : NumberValue<double>
	{
		[ResponsiveButtonGroup]
		public void Min()
		{
			value = double.MinValue;
		}
		[ResponsiveButtonGroup]
		public void Max()
		{
			value = double.MaxValue;
		}
		[ResponsiveButtonGroup]
		public void Epsilon()
		{
			value = double.Epsilon;
		}
		[ResponsiveButtonGroup]
		public void NaN()
		{
			value = double.NaN;
		}
		[ResponsiveButtonGroup]
		public void PInfinity()
		{
			value = double.PositiveInfinity;
		}
		[ResponsiveButtonGroup]
		public void NInfinity()
		{
			value = double.NegativeInfinity;
		}
	}
}