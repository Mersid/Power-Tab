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
    public class PowerTab : ITab
    {
        private Vector2 _scrollPos;
        private float _lastY;

        private PowerNetElements _powerNetElements;

        private const float LeftMargin = 5;
        private const float RightMargin = 2;
        private const float TopMargin = 30;
        private const float BottomMargin = 5;

        private readonly Vector2 _innerSize; // Size of the scrollable portion of the tab display. It is the size, minus the margins.
        private readonly Dictionary<ThingDef, bool> _groupCollapsed; // PowerTab* are drawable elements and shouldn't contain state, so we'll put the collapsed tracker here.
                                                                      // Since PowerTab* elements are recreated each tick, store them as a dict with a CompPower as the key, since they persist.
                                                                      // In the context of groups, it is safe to reference them by type since all instances will be in the same group.
                                                                      
        
        public PowerTab()
        {
            size = new Vector2(450f, 450f);
            _innerSize = new Vector2(size.x - (LeftMargin + RightMargin), size.y - (TopMargin + BottomMargin));
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
            IEnumerable<IGrouping<ThingDef, CompPower>> batteryGroups = _powerNetElements.Batteries.GroupBy(t => t.parent.def);
            IEnumerable<IGrouping<ThingDef, CompPower>> powerConsumerGroups = _powerNetElements.Consumers.GroupBy(t => t.parent.def);
            IEnumerable<IGrouping<ThingDef, CompPower>> powerPlantsGroups = _powerNetElements.PowerPlants.GroupBy(t => t.parent.def);
            IEnumerable<IGrouping<ThingDef, CompPower>> groupings= batteryGroups.Concat(powerConsumerGroups).Concat(powerPlantsGroups);

            Widgets.BeginScrollView(new Rect(
                    new Vector2(LeftMargin, TopMargin), 
                    new Vector2(_innerSize.x, _innerSize.y)).ContractedBy(GenUI.GapTiny), // Defines the outer (fixed) view - the viewport.
                                                                                            // Size vector is subtracted by margins so opposite end doesnt fall off screen.
                ref _scrollPos,
                new Rect(default, new Vector2(_innerSize.x - GenUI.GapTiny * 2 - GenUI.ScrollBarWidth, _lastY)) // Defines the inner, scrollable, view. 
                                                                                                                                     // When bigger than outRect, scroll bars appear for navigation.
            );
            
            float y = 10;

            // These are the total category-level power production/consumption and the max it can produce/consume
            float batteryPower = 0;
            float batteryPowerMax = 0;
            float consumerPower = 0;
            float consumerPowerMax = 0;
            float producerPower = 0;
            float producerPowerMax = 0;
            
            // Each category consists of groups, which consist of things. The top level is not programatically generated, since there's only ever the same three
            List<PowerTabGroup> powerTabGroupsBatteries = new List<PowerTabGroup>();
            List<PowerTabGroup> powerTabGroupsConsumers = new List<PowerTabGroup>();
            List<PowerTabGroup> powerTabGroupsProducers = new List<PowerTabGroup>();
            foreach (IGrouping<ThingDef, CompPower> grouping in groupings)
            {
                // Iterating over groups. Each group consists of all instances of a single type of power building (ex: every solar panel, every chemfuel generator, battery, electric oven, etc)
                // in the grid. To access the instances themselves, use a second loop.
                
                // This shouldn't happen, but we'll check anyway
                if (!grouping.ToList().Any())
                    continue;
                
                // If _groupCollapsed[grouping.Key] key does not exist, the code will crash, so initialize all such keys in the dictionary
                if (!_groupCollapsed.ContainsKey(grouping.Key))
                    _groupCollapsed[grouping.Key] = false;

                float groupPower = 0;
                float groupPowerMax = 0;
                
                //Iterate over each item in an item group
                List<PowerTabThing> powerTabThings = new List<PowerTabThing>(); // Contains every instance of a specific item type
                foreach (CompPower compPower in grouping)
                {
                    // thingPower and thingPowerMax should always be positive, regardless of whether an item group is consuming or producing.
                    float thingPower = 0;
                    float thingPowerMax = 1; // So that if something goes wrong, we won't end up dividing by zero
                    switch (compPower)
                    {
                        case CompPowerBattery battery:
                            thingPower = battery.StoredEnergy;
                            thingPowerMax = battery.Props.storedEnergyMax;
                            break;
                        case CompPowerPlant powerPlant:
                            thingPower = powerPlant.PowerOutput;
                            thingPowerMax = -powerPlant.Props.basePowerConsumption;
                            break;
                        case CompPowerTrader consumer:
                            thingPower = -consumer.PowerOutput;
                            thingPowerMax = consumer.Props.basePowerConsumption;
                            break;
                        default:
                            Log.Error($"Could not determine if {compPower.parent.LabelCap} is a battery, producer, or consumer!");
                            break;
                    }

                    groupPower += thingPower;
                    groupPowerMax += thingPowerMax;

                    powerTabThings.Add(new PowerTabThing(compPower.parent, thingPower,thingPower / thingPowerMax, _innerSize.x));
                }
                
                
                PowerTabGroup powerTabGroup = new PowerTabGroup(grouping.First().parent.LabelCap, grouping.ToList().Count(), groupPower, groupPower / groupPowerMax, powerTabThings, _innerSize.x, _groupCollapsed[grouping.Key], false, _ => _groupCollapsed[grouping.Key] = !_groupCollapsed[grouping.Key]);
                switch (grouping.First())
                {
                    case CompPowerBattery:
                        powerTabGroupsBatteries.Add(powerTabGroup);
                        batteryPower += groupPower;
                        batteryPowerMax += groupPowerMax;
                        break;
                    case CompPowerPlant:
                        powerTabGroupsProducers.Add(powerTabGroup);
                        producerPower += groupPower;
                        producerPowerMax += groupPowerMax;
                        break;
                    case CompPowerTrader:
                        powerTabGroupsConsumers.Add(powerTabGroup);
                        consumerPower += groupPower;
                        consumerPowerMax += groupPowerMax;
                        break;
                    default:
                        Log.Error($"Could not determine if {grouping.First().parent.LabelCap} is a battery, producer, or consumer!");
                        break;
                }

            }
            PowerTabCategory batteryCategory = new PowerTabCategory("Batteries", batteryPower, batteryPower / batteryPowerMax, powerTabGroupsBatteries, _innerSize.x);
            batteryCategory.Draw(y);
            y += batteryCategory.Height;
            
            PowerTabCategory producerCategory = new PowerTabCategory("Producers", producerPower, producerPower / producerPowerMax, powerTabGroupsProducers, _innerSize.x);
            producerCategory.Draw(y);
            y += producerCategory.Height;
            
            PowerTabCategory consumerCategory = new PowerTabCategory("Consumers", consumerPower, consumerPower / consumerPowerMax, powerTabGroupsConsumers, _innerSize.x);
            consumerCategory.Draw(y);
            y += consumerCategory.Height;

            _lastY = y;
            
            Widgets.EndScrollView();
            
        }
    }
}
