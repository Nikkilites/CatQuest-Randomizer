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

        {
            if (locationId == "MainQuest_012")
                OnGoalConditionMet();
        }

        private void OnGoalConditionMet()
        {
            Randomizer.ConnectionHandler.SendGoal();
        }
    }
}
