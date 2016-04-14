using System.Linq;
using ICities;
using ColossalFramework.Plugins;
using UnityEngine;

namespace PloppableRICO
{
    

    public class Loading : LoadingExtensionBase
	{

        private XMLManager xmlManager;
        private ConvertPrefabs convertPrefabs;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
            return;
           
            xmlManager = new XMLManager();
            xmlManager.Run();

            convertPrefabs = new ConvertPrefabs();
            convertPrefabs.run();

            PloppableTool.Initialize();
            RICOSettingsPanel.Initialize();

            Detour.BuildingToolDetour.Deploy();

        }

        public override void OnLevelUnloading ()
		{
			base.OnLevelUnloading ();

			//RICO ploppables need a non private item class assigned to pass though the game reload. 
			for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount (); i++) {
				
				var prefab = PrefabCollection<BuildingInfo>.GetLoaded (i);

				if (prefab.m_buildingAI is PloppableRICO.PloppableExtractor || prefab.m_buildingAI  is PloppableResidential
				    || prefab.m_buildingAI  is PloppableOffice ||   prefab.m_buildingAI is PloppableCommercial ||
				    prefab.m_buildingAI  is PloppableIndustrial) {

					// Just assign any RICO prefab a ploppable ItemClass so it will reload. It gets set back once the mod loads. 
					prefab.m_class = ItemClassCollection.FindClass ("Beautification Item"); 
					prefab.InitializePrefab ();
				}
			}
		}

		public override void OnReleased ()
		{
			base.OnReleased ();

            Detour.BuildingToolDetour.Revert ();

		}
	}
}
