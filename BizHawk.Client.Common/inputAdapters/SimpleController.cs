using System.Collections.Generic;
using System.Linq;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	/// <summary>
	/// A basic implementation of IController
	/// </summary>
	public class SimpleController : IController
	{
		public ControllerDefinition Definition { get; set; }

		protected WorkingDictionary<string, bool> Buttons { get; private set; } = new WorkingDictionary<string, bool>();
		protected WorkingDictionary<string, float> Axes { get; private set; } = new WorkingDictionary<string, float>();
		protected WorkingDictionary<string, int> HapticFeedback { get; private set; } = new WorkingDictionary<string, int>();

		public void Clear()
		{
			Buttons = new WorkingDictionary<string, bool>();
			Axes = new WorkingDictionary<string, float>();
		}

		public bool this[string button]
		{
			get => Buttons[button];
			set => Buttons[button] = value;
		}

		public virtual bool IsPressed(string button)
		{
			return this[button];
		}

		public float AxisValue(string name)
		{
			return Axes[name];
		}

		public IReadOnlyCollection<(string Name, int Strength)> GetHapticsSnapshot() => HapticFeedback.Select(kvp => (kvp.Key, kvp.Value)).ToArray();

		public void SetHapticChannelStrength(string name, int strength) => HapticFeedback[name] = strength;

		public IEnumerable<KeyValuePair<string, bool>> BoolButtons()
		{
			return Buttons;
		}

		public void AcceptNewAxes((string AxisID, float Value) newValue)
		{
			Axes[newValue.AxisID] = newValue.Value;
		}

		public void AcceptNewAxes(IEnumerable<(string AxisID, float Value)> newValues)
		{
			foreach (var (axisID, value) in newValues)
			{
				Axes[axisID] = value;
			}
		}
	}
}
