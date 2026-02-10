using UniRx;
using System.Linq;
using System;

namespace CatQuest_Randomizer
{
    public class GoalHandler
    {
        CatQuestDataHelper _dataHelper = new();

        public void Init() 
        {
            Randomizer.Logger.LogInfo($"GoalHandler Init() Called");

            Observable.EveryUpdate()
                .Where(_ => _dataHelper.GetCurrentLevel() != null && _dataHelper.GetCompletedQuests() != null)
                .Take(1)
                .Subscribe(_ => 
                { 
                    Randomizer.Logger.LogInfo($"Setup Goal Subscriptions"); 
                    SetupGoalSubscription(); 
                }); 
        }

        public void SetupGoalSubscription()
        {
            if (Randomizer.SlotDataHandler.goal == Goal.max_level)
            {
                Randomizer.Logger.LogInfo("Setup goal subscription for max level");

                _dataHelper.GetCurrentLevel()
                    .AsObservable()
                    .Where(lvl => lvl >= 99)
                    .Subscribe(_ =>
                    {
                        Randomizer.Logger.LogInfo("Player reached level 99 and goaled!");
                        OnGoalConditionMet();
                    });
            }

            if (Randomizer.SlotDataHandler.goal == Goal.questsanity)
            {
                Randomizer.Logger.LogInfo("Setup goal subscription for questsanity");

                _dataHelper.GetCompletedQuests()
                    .ObserveCountChanged()
                    .Where(count => count == 79)
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        Randomizer.Logger.LogInfo("Player has completed 79 quests and goaled!");
                        OnGoalConditionMet();
                    });
            }

            if (Randomizer.SlotDataHandler.goal == Goal.max_level_and_main_quest)
            {
                Randomizer.Logger.LogInfo("Setup goal subscription for max level and main quest");

                _dataHelper.GetCurrentLevel()
                    .AsObservable()
                    .Where(lvl => lvl >= 99)
                    .Subscribe(_ =>
                    {
                        if (_dataHelper.GetCompletedQuests().Contains("MainQuest_012"))
                        {
                            Randomizer.Logger.LogInfo("Player reached level 99 and completed the main quest and goaled!");
                            OnGoalConditionMet();
                        }
                        else { Randomizer.Logger.LogInfo("Player has reached level 99, but still needs to complete the main quest to goal"); }
                    });
            }

            if (Randomizer.SlotDataHandler.goal == Goal.spellmastery)
            {
                Randomizer.Logger.LogInfo("Setup goal subscription for spellmastery");

                Game.gameStream
                .EventsOfType<SkillEvent>()
                .Where(e => e.type == SkillEvent.EventType.UPGRADED)
                .Where(e => e.skill.level == 10)
                .Subscribe(e =>
                {
                    Randomizer.Logger.LogInfo($"Skill {e.skill.data.name} reached level {e.skill.level}");

                    var obtainedSkills = _dataHelper.GetObtainedSkills();
                    if (obtainedSkills.Count == 7 && obtainedSkills.All(skill => skill.level >= 10))
                    {
                        Randomizer.Logger.LogInfo("Player achieved max level in all skills and goaled!");
                        OnGoalConditionMet();
                    }
                });
            }
        }

        private void DevLoggerObservers()
        {

            _dataHelper.GetCompletedQuests()
                .ObserveAdd()
                .Subscribe(e =>
                {
                    string questName = e.Value;
                    Randomizer.Logger.LogInfo($"Quest completed: {questName} (total: {_dataHelper.GetCompletedQuests().Count})");
                });

            _dataHelper.GetCompletedMainQuests()
                .AsObservable()
                .Subscribe(_ =>
                {
                    Randomizer.Logger.LogInfo($"Completed a Main Quest. (total: {_dataHelper.GetCompletedMainQuests()})");
                });

            _dataHelper.GetCompletedCaves()
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var caveName = e.Value;

                    var caves = _dataHelper.GetCompletedCaves();

                    string Ccontents = string.Join(
                        ", ",
                        caves.Select(cave =>
                            $"ID:{cave.id ?? "none"}"
                        ).ToArray()
                    );

                    Randomizer.Logger.LogInfo($"Cave completed: {caveName} (total: {_dataHelper.GetCompletedCaves().Count}) -  [{Ccontents}]");

                    var chests = _dataHelper.GetCollectedChests();

                    string contents = string.Join(
                        ", ",
                        chests.Select(chest =>
                            $"ID:{chest.id ?? "none"}"
                        ).ToArray()
                    );

                    Randomizer.Logger.LogInfo($"Will also print collected chests. (total:{_dataHelper.GetCollectedChests().Count}) -  [{contents}]");
                });

            _dataHelper.GetClearedDungeons()
                .AsObservable()
                .Subscribe(_ =>
                {
                    Randomizer.Logger.LogInfo($"Completed a dungeon. (total:{_dataHelper.GetClearedDungeons()})");

                    var chests = _dataHelper.GetCollectedChests();

                    string contents = string.Join(
                        ", ",
                        chests.Select(chest =>
                            $"ID:{chest.id ?? "none"}"
                        ).ToArray()
                    );

                    Randomizer.Logger.LogInfo($"Will also print collected chests. (total:{_dataHelper.GetCollectedChests().Count}) -  [{contents}]");
                });

            _dataHelper.GetVisitedMonuments()
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var monumentName = e.Value;

                    var mons = _dataHelper.GetVisitedMonuments();

                    string contents = string.Join(
                        ", ",
                        mons.Select(mon =>
                            $"ID:{mon.id ?? "none"}"
                        ).ToArray()
                    );

                    Randomizer.Logger.LogInfo($"Monument Visited {monumentName} (total: {_dataHelper.GetVisitedMonuments().Count} -  [{contents}])");
                });

            _dataHelper.GetCurrentInventory()
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var thisinv = e.Value;

                    var items = _dataHelper.GetCurrentInventory();

                    string contents = string.Join(
                        ", ",
                        items.Select(item =>
                            $"{item.category} | Type:{item.typeId} | ID:{item.id ?? "none"} | Amount:{item.amount.Value}"
                        ).ToArray()
                    );

                    Randomizer.Logger.LogInfo(
                        $"Inventory item added {thisinv} (total: {items.Count}) [{contents}]"
                    );
                });

            _dataHelper.GetCurrentEquipment()
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var thisequipment = e.Value;

                    var equipment = _dataHelper.GetCurrentEquipment();

                    string contents = string.Join(
                        ", ",
                        equipment.Select(kvp =>
                        {
                            var d = kvp.Value?.staticData;
                            return d == null
                                ? $"{kvp.Key}: <null>"
                                : $"{kvp.Key}: {d.partName} (ID: {d.partId}, Type: {d.partType})";
                        }).ToArray()
                    );

                    Randomizer.Logger.LogInfo(
                        $"Equipment equipped {thisequipment} (total: {equipment.Count}) [{contents}]"
                    );
                });

            _dataHelper.GetCurrentLevel()
                .AsObservable()
                .Subscribe(_ =>
                {
                    Randomizer.Logger.LogInfo($"Player reached level {_dataHelper.GetCurrentLevel()}!");
                });

            _dataHelper.GetCurrentExp()
                .AsObservable()
                .Subscribe(_ =>
                {
                    Randomizer.Logger.LogInfo($"Player reached {_dataHelper.GetCurrentExp()} exp!");
                });

            _dataHelper.GetCurrentGold()
                .AsObservable()
                .Subscribe(_ =>
                {
                    Randomizer.Logger.LogInfo($"Player reached {_dataHelper.GetCurrentGold()} gold!");
                });

            _dataHelper.GetObtainedSkills()
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var thisskill = e.Value;

                    var skills = _dataHelper.GetObtainedSkills();

                    string contents = string.Join(
                    ", ",
                        skills.Select(item =>
                            $"{item.level} | Name:{item.data.name} | ID:{item.data.skillId ?? "none"} | gold cost :{item.data.goldCost} | goldCostPerLevel: {item.data.goldCostPerLevel} | effectType: {item.data.effectType} | targetType: {item.data.targetType}"
                        ).ToArray()
                    );

                    Randomizer.Logger.LogInfo(
                        $"skill obtained {thisskill} (total: {skills.Count}) [{contents}]"
                    );
                });

            _dataHelper.GetEquippedSkills()
                .ObserveAdd()
                .Subscribe(e =>
                {
                    var thisskill = e.Value;

                    var skills = _dataHelper.GetEquippedSkills();

                    string contents = string.Join(
                    ", ",
                        skills.Select(item =>
                            $"{item.level} | Name:{item.data.name} | ID:{item.data.skillId ?? "none"} | gold cost :{item.data.goldCost} | goldCostPerLevel: {item.data.goldCostPerLevel} | effectType: {item.data.effectType} | targetType: {item.data.targetType}"
                        ).ToArray()
                    );

                    Randomizer.Logger.LogInfo(
                        $"skill equipped {thisskill} (total: {skills.Count}) [{contents}]"
                    );
                });
        }

        public void CheckIfLocationIsGoal(string locationId)
        {
            if (Randomizer.SlotDataHandler.goal == Goal.main_quest)
            {
                if (locationId == "MainQuest_012")
                {
                    Randomizer.Logger.LogInfo("Player has beaten the main quest and goaled!");
                    OnGoalConditionMet();
                }
            }

            if (Randomizer.SlotDataHandler.goal == Goal.max_level_and_main_quest)
            {
                if (locationId == "MainQuest_012")
                {
                    if (_dataHelper.GetCurrentLevel().Value >= 99)
                    {
                        Randomizer.Logger.LogInfo("Player reached level 99 and completed the main quest and goaled!");
                        OnGoalConditionMet();
                    }
                    else { Randomizer.Logger.LogInfo("Player has completed the main quest, but still needs lvl 99 to goal"); }
                }
            }
        }

        public void CheckIfSkillWasGoal()
        {
            var obtainedSkills = _dataHelper.GetObtainedSkills();
            if (obtainedSkills.Count == 7 && obtainedSkills.All(skill => skill.level >= 10))
            {
                Randomizer.Logger.LogInfo("Player achieved max level in all skills and goaled!");
                OnGoalConditionMet();
            }
        }

        private void OnGoalConditionMet()
        {
            Randomizer.ConnectionHandler.SendGoal();
        }
    }
}
