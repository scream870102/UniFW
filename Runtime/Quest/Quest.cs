namespace Scream.UniFW.Quest
{
	public interface IQuest
	{
		int Id { get; }
		bool IsComplete { get; }
	}
	public interface IQuest<T> : IQuest where T : IQuestData
	{
		void Refresh(T data);
	}

	public interface IQuestData { }
}