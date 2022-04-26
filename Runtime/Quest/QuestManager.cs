using System;
using System.Collections.Generic;
namespace Scream.UniFW.Quest
{
	public static class QuestManager
	{
		public static event Action<IQuest> OnQuestCompleted;
		public static Dictionary<Type, Dictionary<int, IQuest>> Quests = new Dictionary<Type, Dictionary<int, IQuest>>();

		public static void AddQuest<QuestType>(QuestType quest) where QuestType : IQuest
		{
			var keyType = GetDataType<QuestType>();
			AddQuestImpl(quest, keyType);
		}

		public static void AddQuest<QuestType, DataType>(QuestType quest) where QuestType : IQuest<DataType> where DataType : IQuestData
		{
			var keyType = typeof(DataType);
			AddQuestImpl(quest, keyType);
		}

		public static void RemoveQuest(IQuest quest)
		{
			var type = GetDataType(quest.GetType());
			if (type == default)
			{
				return;
			}
			RemoveQuestImpl(quest.Id, type);
		}

		public static void RemoveQuest<QuestType, DataType>(QuestType quest) where QuestType : IQuest<DataType> where DataType : IQuestData
		{
			var type = typeof(DataType);
			RemoveQuestImpl(quest.Id, type);
		}

		public static void RemoveQuest<QuestType>(int id) where QuestType : IQuest
		{
			var type = GetDataType<QuestType>();
			if (type == default)
			{
				return;
			}
			RemoveQuestImpl(id, type);
		}

		public static void RemoveQuest<QuestType, DataType>(int id) where QuestType : IQuest<DataType> where DataType : IQuestData
		{
			var type = typeof(DataType);
			RemoveQuestImpl(id, type);
		}

		public static void RefreshQuest<DataType>(DataType data) where DataType : IQuestData
		{
			var type = typeof(DataType);
			var completeQuest = new List<IQuest>();
			if (Quests.ContainsKey(type))
			{
				foreach (var pair in Quests[type])
				{
					(pair.Value as IQuest<DataType>).Refresh(data);
					if (pair.Value.IsComplete)
					{
						completeQuest.Add(pair.Value);
					}
				}
			}
			for (int i = completeQuest.Count - 1; i >= 0; i--)
			{
				OnQuestCompleted?.Invoke(completeQuest[i]);
			}
		}

		public static bool TryGetQuest<QuestType, DataType>(int id, out QuestType result) where QuestType : IQuest<DataType> where DataType : IQuestData
		{
			var type = typeof(DataType);
			return TryGetQuestImpl(id, type, out result);
		}

		public static bool TryGetQuest<QuestType>(int id, out QuestType result) where QuestType : IQuest
		{
			var dataType = GetDataType(typeof(QuestType));
			if (dataType != default)
			{
				return TryGetQuestImpl(id, dataType, out result);
			}
			result = default;
			return false;
		}

		#region PRIVATE_METHOD
		private static void AddQuestImpl<QuestType>(QuestType quest, Type keyType) where QuestType : IQuest
		{
			if (keyType != default && Quests.ContainsKey(keyType))
			{
				if (Quests[keyType].ContainsKey(quest.Id))
				{
					return;
				}
				else
				{
					Quests[keyType].Add(quest.Id, quest);
				}
			}
			else
			{
				Quests.Add(keyType, new Dictionary<int, IQuest>());
				Quests[keyType].Add(quest.Id, quest);
			}
		}

		private static void RemoveQuestImpl(int id, Type type)
		{
			var isTypeExist = Quests.ContainsKey(type);
			if (isTypeExist && Quests[type].ContainsKey(id))
			{
				Quests[type].Remove(id);
			}
		}

		private static bool TryGetQuestImpl<QuestType>(int id, Type dataType, out QuestType result) where QuestType : IQuest
		{
			if (Quests.ContainsKey(dataType))
			{
				var internalQuests = Quests[dataType];
				if (internalQuests.ContainsKey(id))
				{
					if (internalQuests[id] is QuestType concrectQuest)
					{
						result = concrectQuest;
						return true;
					}
				}
			}
			result = default;
			return false;
		}

		private static Type GetDataType<QuestType>() where QuestType : IQuest
		{
			var questType = typeof(QuestType);
			var dataType = GetDataTypeImpl(questType);
			return dataType;

		}

		private static Type GetDataType(Type questType)
		{
			var dataType = GetDataTypeImpl(questType);
			return dataType;
		}

		private static Type GetDataTypeImpl(Type questType)
		{
			foreach (var questImplInter in questType.GetInterfaces())
			{
				if (questImplInter.IsAbstract && questImplInter.IsConstructedGenericType)
				{
					var questChildImplInters = questImplInter.GetInterfaces();
					foreach (var childImplType in questChildImplInters)
					{
						if (childImplType == typeof(IQuest))
						{
							foreach (var genericType in questImplInter.GenericTypeArguments)
							{
								var defaultDataInterType = typeof(IQuestData);
								foreach (var dataImplType in genericType.GetInterfaces())
								{
									if (dataImplType == defaultDataInterType)
									{
										return genericType;
									}
								}
							}
						}
					}
				}
			}
			return default;
		}
		#endregion PRIVATE_METHOD
	}
}