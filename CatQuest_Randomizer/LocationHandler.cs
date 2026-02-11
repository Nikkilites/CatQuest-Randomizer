using CatQuest_Randomizer.Model;
using System.Linq;
using UniRx;

namespace CatQuest_Randomizer
{
    public class LocationHandler
    {
        CatQuestDataHelper _dataHelper = new();

        public void Init()
        {
            Randomizer.Logger.LogInfo($"LocationHandler Init() Called");

            Observable.EveryUpdate()
                .Where(_ => _dataHelper.GetVisitedMonuments() != null)
                .Take(1)
                .Subscribe(_ =>
                {
                    Randomizer.Logger.LogInfo($"Setup Location Subscriptions");
                    SetupLocationSubscriptions();
                });
        }

        public void SetupLocationSubscriptions()
        {
            if (Randomizer.SlotDataHandler.includeMonuments)
            {
                Randomizer.Logger.LogInfo("Setup Monument subscription");

                _dataHelper.GetVisitedMonuments()
                    .ObserveAdd()
                    .Subscribe(e =>
                    {
                        Randomizer.Logger.LogInfo($"Monument {e.Value.id} was visited, send check from LocationHandler");
                        CheckLocation(e.Value.id);
                    });
            }
        }

        public void CheckLocation(string locationId)
        {
            Randomizer.Logger.LogInfo($"Will look for Location with id {locationId} on list");

            Location location = Randomizer.DataStorageHandler.locations.FirstOrDefault(location => location.Id == locationId);

            if (location != null)
            {
                Randomizer.Logger.LogInfo($"Location was found on list of Locations");

                Randomizer.ConnectionHandler.SendLocation(location);
                Randomizer.GoalHandler.CheckIfLocationIsGoal(locationId);
            }
            else
            {
                Randomizer.Logger.LogError($"Location was not found on list of Locations");
            }
        }
    }
}
