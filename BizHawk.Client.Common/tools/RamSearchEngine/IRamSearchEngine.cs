using System.Collections.Generic;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common.RamSearchEngine
{
	public interface IRamSearchEngine
	{
		IEnumerable<long> OutOfRangeAddress { get; }
		int Count { get; }
		Watch this[int index] { get; }
		Compare CompareTo { get; set; }
		long? CompareValue { get; set; }
		ComparisonOperator Operator { get; set; }
		int? DifferentBy { get; set; }
		MemoryDomain Domain { get; }
		bool UndoEnabled { get; set; }
		bool CanUndo { get; }
		bool CanRedo { get; }

		bool Preview(long address);
		void Update();
		void Start();
		int DoSearch();
		void SetType(DisplayType type);
		void SetPreviousType(PreviousType type);
		void SetEndian(bool bigEndian);
		void RemoveRange(IEnumerable<int> indices);
		void RemoveSmallWatchRange(IEnumerable<Watch> watches);
		void AddRange(IEnumerable<long> addresses, bool append);
		void ClearHistory();
		int Undo();
		int Redo();
		void SetPreviousToCurrent();
		void ClearChangeCounts();
		void Sort(string column, bool reverse);
	}

	public static class RamSearchEngineFactory
	{
		public static IRamSearchEngine Create(SearchEngineSettings settings, IMemoryDomains memoryDomains)
		{
			return new RamSearchEngine(settings, memoryDomains);
		}

		public static IRamSearchEngine Create(SearchEngineSettings settings, IMemoryDomains memoryDomains, Compare compareTo, long? compareValue, int? differentBy)
		{
			return new RamSearchEngine(settings, memoryDomains, compareTo, compareValue, differentBy);
		}
	}
}
