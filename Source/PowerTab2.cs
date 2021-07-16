using System.Collections.Generic;
using System.Linq;
using PowerTab.UIElements;
using RimWorld;
using UnityEngine;
using Verse;

namespace PowerTab
{
    // ReSharper disable once UnusedType.Global
    // This class is used in Harmony
    public class PowerTab2 : ITab
    {
        private Vector2 _scrollPos;
        private float _lastY;

        private PowerNetElements _powerNetElements;

        private const float LeftMargin = 5;
        private const float RightMargin = 2;
        private const float TopMargin = 30;
        private const float BottomMargin = 5;

        private Vector2 innerSize; // Size of the scrollable portion of the tab display. It is the size, minus the margins.
        private readonly Dictionary<ThingDef, bool> _groupCollapsed; // PowerTab* are drawable elements and shouldn't contain state, so we'll put the collapsed tracker here.
                                                                      // Since PowerTab* elements are recreated each tick, store them as a dict with a CompPower as the key, since they persist.
                                                                      // In the context of groups, it is safe to reference them by type since all instances will be in the same group.
                                                                      
        
        public PowerTab2()
        {
            size = new Vector2(450f, 450f);
            innerSize = new Vector2(size.x - (LeftMargin + RightMargin), size.y - (TopMargin + BottomMargin));
            labelKey = "PowerSwitch_Power";
            _powerNetElements = new PowerNetElements();
            _groupCollapsed = new Dictionary<ThingDef, bool>();
        }

        public void UpdatePowerNetInfo(PowerNetElements powerNetElements)
        {
            _powerNetElements = powerNetElements;
        }
        
        protected override void FillTab()
        {
            Widgets.BeginScrollView(new Rect(
                    new Vector2(LeftMargin, TopMargin), 
                    new Vector2(innerSize.x, innerSize.y)).ContractedBy(GenUI.GapTiny), // Defines the outer (fixed) view - the viewport.
                                                                                            // Size vector is subtracted by margins so opposite end doesnt fall off screen.
                ref _scrollPos,
                new Rect(default, new Vector2(innerSize.x - GenUI.GapTiny * 2 - GenUI.ScrollBarWidth, _lastY)) // Defines the inner, scrollable, view. 
                                                                                                                                     // When bigger than outRect, scroll bars appear for navigation.
            );
            
            float y = 10;
            
            List<PowerTabGroup> powerTabGroups = new List<PowerTabGroup>();
            foreach (IGrouping<ThingDef, CompPowerPlant> powerPlantGroup in _powerNetElements.PowerPlants.GroupBy(t => t.parent.def))
            {
                // Iterating over groups. Each group consists of all instances of a single type of power plant (ex: every solar panel, every chemfuel generator, etc)
                // in the grid. To access the instances themselves, use a second loop.
                
                // If _groupCollapsed[powerPlantGroup.Key] key does not exist, the code will crash, so initialize all such keys in the dictionary
                if (!_groupCollapsed.ContainsKey(powerPlantGroup.Key))
                    _groupCollapsed[powerPlantGroup.Key] = false;
                
                
                //Iterate over each item in an item group
                List<PowerTabThing> powerTabThings = new List<PowerTabThing>();
                foreach (CompPowerPlant powerPlant in powerPlantGroup)
                {
                    powerTabThings.Add(new PowerTabThing(powerPlant.parent, powerPlant.PowerOutput,powerPlant.PowerOutput / -powerPlant.Props.basePowerConsumption, innerSize.x));
                }
                
                // This shouldn't happen, but we'll check anyway
                if (!powerPlantGroup.ToList().Any())
                    continue;

                float groupPower = powerPlantGroup.Sum(t => t.PowerOutput);
                float groupPowerMax = -powerPlantGroup.Sum(t => t.Props.basePowerConsumption);
                
                PowerTabGroup powerTabGroup = new PowerTabGroup(powerPlantGroup.First().parent.LabelCap, powerPlantGroup.ToList().Count(), groupPower, groupPower / groupPowerMax, powerTabThings, innerSize.x, _groupCollapsed[powerPlantGroup.Key], false, group => _groupCollapsed[powerPlantGroup.Key] = !_groupCollapsed[powerPlantGroup.Key]);
                powerTabGroups.Add(powerTabGroup);
            }
            PowerTabCategory powerTabCategory = new PowerTabCategory("Producers", 1000, 0.85f, powerTabGroups, innerSize.x);
            powerTabCategory.Draw(y);
            y += powerTabCategory.Height;

            _lastY = y;
            
            Widgets.EndScrollView();
            
        }
    }
}
