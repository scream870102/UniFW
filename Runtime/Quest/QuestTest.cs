namespace Scream.UniFW.Quest
{
	public class TestQuest : IQuest<TestQuestData>
	{
		public int Id { get; private set; }
		public bool IsComplete => Remain <= 0;

		public int Remain { get; private set; }
		public TestQuest(int remain)
		{
			Id = this.GetHashCode();
			Remain = remain;
		}
		public void Refresh(TestQuestData data)
		{
			Remain -= data.Count;	
		}
	}

	public class TestQuestData : IQuestData
	{
		public int Count { get; private set; }
		public TestQuestData(int count)
		{
			Count = count;
		}
	}

	public class TestQuest2 : IQuest<TestQuest2Data>
	{
		public int Id { get; private set; }
		public bool IsComplete => State == "Complete";
		public string State { get; private set; } = string.Empty;

		public TestQuest2(string initState)
		{
			Id = this.GetHashCode();
			State = initState;
		}
		public void Refresh(TestQuest2Data data)
		{
			State = data.State;
		}
	}

	public class TestQuest2Data : IQuestData
	{
		public string State { get; private set; }
		public TestQuest2Data(string state)
		{
			State = state;
		}
	}
}