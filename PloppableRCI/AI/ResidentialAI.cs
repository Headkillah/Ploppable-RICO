using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.Plugins;
using UnityEngine;
using System.Linq;


namespace PloppableRICO
{

	public class PloppableResidential : ResidentialBuildingAI
	{
		public int m_constructionCost = 1;
		public int m_homeCount = 1;
        public int m_pbhomeCount = 0;

        public override void GetWidthRange (out int minWidth, out int maxWidth)
		{
			minWidth = 1;
			maxWidth = 16;
		}

		public override void GetLengthRange (out int minLength, out int maxLength)
		{
			minLength = 1;
			maxLength = 16;
		}

		public override string GenerateName(ushort buildingID, InstanceID caller)
        { 
			return base.m_info.GetUncheckedLocalizedTitle();
		}
			
		public override bool ClearOccupiedZoning (){
			return false;
		}


        public override int GetConstructionCost()
		{
			int result = (m_constructionCost * 100);
			Singleton<EconomyManager>.instance.m_EconomyWrapper.OnGetConstructionCost(ref result, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
			return result;
		}

        public override int CalculateHomeCount(Randomizer r, int width, int length)
        {
            
            if (Util.IsModEnabled(426163185ul))
            {
                return base.CalculateHomeCount(r, width, length);
            }        

            var width2 = m_homeCount;
            var length2 = 1;
			int num = 100;
			return Mathf.Max(100, width2 * length2 * num + r.Int32(100u)) / 100;
            
		}

        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {

            Util.buildingFlags(ref buildingData);

			base.SimulationStep(buildingID, ref buildingData, ref frameData);

            Util.buildingFlags(ref buildingData);

        }

		protected override void SimulationStepActive(ushort buildingID, ref Building buildingData, ref Building.Frame frameData){

            Util.buildingFlags(ref buildingData);

            base.SimulationStepActive(buildingID, ref buildingData, ref frameData);

            Util.buildingFlags(ref buildingData);

        }

		public override BuildingInfo GetUpgradeInfo(ushort buildingID, ref Building data){
			
			return null; //this will cause a check to fail in CheckBuildingLevel, and prevent the building form leveling. 
		}
	}
}