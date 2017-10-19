using System.Linq;
using ICities;
using ColossalFramework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using PloppableRICO.Redirection;
using GrowableOverhaul;

namespace PloppableRICO
{
    public class Loading : LoadingExtensionBase
	{

        private readonly Dictionary<MethodInfo, Redirector> redirectsOnLoaded = new Dictionary<MethodInfo, Redirector>();
        private readonly Dictionary<MethodInfo, Redirector> redirectsOnCreated = new Dictionary<MethodInfo, Redirector>();

        private bool created;
        private bool loaded;

        public GameObject RICODataManager;
        public RICOPrefabManager xmlManager;

        private ConvertPrefabs convertPrefabs;

        public override void OnCreated(ILoading loading)
        {
            if (created) return;
            //Redirect(true);
            created = true;  
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            
            //if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
            //return;

            //Load xml only from main menu. 
            if (xmlManager == null)
            {
                xmlManager = new RICOPrefabManager();
                xmlManager.Run();
            }

            //Assign xml settings to prefabs.
            convertPrefabs = new ConvertPrefabs();
            convertPrefabs.run();


            BuildingManagerDetour.ApplyExtendedRefreshBuildings();
            //refresh Building Manager after prefabs converted
            try
            {
                //MethodInfo RefreshAreaBuildings_field = typeof(BuildingManager).GetMethod("RefreshAreaBuildings", BindingFlags.NonPublic | BindingFlags.Instance);
                //RefreshAreaBuildings_field.Invoke(BuildingManager.instance, null);
            }
            catch (Exception e)
            {

                //Debug.LogException(e);
            }

            //Init GUI
            PloppableTool.Initialize();
            RICOSettingsPanel.Initialize();

            //Deploy Detour
            Detour.BuildingToolDetour.Deploy();
            Debug.Log("Detour Deployed");

            //Redirect(true);

            //if (!created || loaded) return;

            

            loaded = true;

        }

        public override void OnLevelUnloading()
        {
            Detour.BuildingToolDetour.Revert();
            Util.AssignServiceClass();
            base.OnLevelUnloading ();
            //RICO ploppables need a non private item class assigned to pass though the game reload. 
            //if (!created || !loaded) return;

            //RevertRedirect(false);
            //loaded = false;
        }

		public override void OnReleased ()
		{

            //Util.AssignServiceClass();
           
            //RevertRedirect(false);

            base.OnReleased ();

            //if (!created) return;

            //if (loaded) OnLevelUnloading();
            //RevertRedirect(false);
            //created = false;
        }


        private void Redirect(bool onCreated)
        {
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            redirects.Clear();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                try
                {
                    var r = RedirectionUtil.RedirectType(type, onCreated);
                    if (r == null) continue;
                    foreach (var pair in r) redirects.Add(pair.Key, pair.Value);
                }
                catch (Exception e)
                {
                    //Debug.LogError($"An error occured while applying {type.Name} redirects!");
                    //Debug.LogException(e);
                }
            }
        }

        private void RevertRedirect(bool onCreated)
        {
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            foreach (var kvp in redirects)
            {
                try
                {
                    kvp.Value.Revert();
                }
                catch (Exception e)
                {
                    //Debug.LogError($"An error occured while reverting {kvp.Key.Name} redirect!");
                    //Debug.LogException(e);
                }
            }
            redirects.Clear();
        }
    }
}
