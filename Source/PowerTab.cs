using System.Collections.Generic;
using System.Diagnostics;
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
        public List<TestComp> TestComps { get; set; }

        private const float LeftMargin = 5;
        private const float RightMargin = 2;
        private const float TopMargin = 30;
        private const float BottomMargin = 5;

        public Vector2 InnerSize { get; } // Size of the scrollable portion of the tab display. It is the size, minus the margins.
        private readonly Dictionary<ThingDef, bool> _groupCollapsed; // PowerTab* are drawable elements and shouldn't contain state, so we'll put the collapsed tracker here.
                                                                      // Since PowerTab* elements are recreated each tick, store them as a dict with a CompPower as the key, since they persist.
                                                                      // In the context of groups, it is safe to reference them by type since all instances will be in the same group.
                                                                      
        
        public PowerTab()
        {
            size = new Vector2(450f, 450f);
            InnerSize = new Vector2(size.x - (LeftMargin + RightMargin), size.y - (TopMargin + BottomMargin));
            labelKey = "PowerSwitch_Power";
            _powerNetElements = new PowerNetElements();
            TestComps = new List<TestComp>();
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
                    new Vector2(InnerSize.x, InnerSize.y)).ContractedBy(GenUI.GapTiny), // Defines the outer (fixed) view - the viewport.
                                                                                            // Size vector is subtracted by margins so opposite end doesnt fall off screen.
                ref _scrollPos,
                new Rect(default, new Vector2(InnerSize.x - GenUI.GapTiny * 2 - GenUI.ScrollBarWidth, _lastY)) // Defines the inner, scrollable, view. 
                                                                                                                                     // When bigger than outRect, scroll bars appear for navigation.
            );
            
            float y = 10;
            
            IEnumerable<IGrouping<ThingDef, TestComp>> groups = TestComps.GroupBy(t => t.parent.def);

            // Create a list of PowerTabGroups. Do this instead of using immediately in loop to reduce nesting from categories.
            // Note that this is a list of every group, with no regard to category.
            List<PowerTabGroup> powerTabGroups = new List<PowerTabGroup>();
            foreach (IGrouping<ThingDef, TestComp> group in groups)
            {
                // Add dictionary entry to _groupCollapsed if necessary so code doesn't crash from trying to potentially access key that does not exist.
                if (!_groupCollapsed.ContainsKey(group.Key))
                    _groupCollapsed[group.Key] = false;
                
                // Create a PowerTabThing from every TestComp in a group. Recall that a group consists of every specific thing (ex: every solar panel, every machining table, etc)
                List<PowerTabThing> things = group.Select(
                    testComp => new PowerTabThing(
                        testComp.parent, 
                        testComp.CurrentPowerOutput, 
                        testComp.CurrentPowerOutput / testComp.DesiredPowerOutput, 
                        InnerSize.x, 
                        testComp.PowerType == PowerType.Battery))
                    .ToList();
                
                // Sort things in a group
                things.SortByDescending(t => Mathf.Abs(t.Power));

                // Some notes:
                // We could probably cache the group.Sum() method call to save time; may want to do it later if time permits.
                // Groups are guaranteed to contain at least one child element. If it doesn't and we're here, something's gone very wrong.
                PowerTabGroup powerTabGroup = new PowerTabGroup(
                    group.First().parent.def.LabelCap, // Def labels do not have modifiers like damage appended to it.
                    group.Count(),
                    group.Sum(t => t.CurrentPowerOutput),
                    group.Sum(t => t.CurrentPowerOutput) / group.Sum(t => t.DesiredPowerOutput),
                    things,
                    InnerSize.x,
                    _groupCollapsed[group.Key],
                    group.First().PowerType == PowerType.Battery,
                    _ => _groupCollapsed[group.Key] = !_groupCollapsed[group.Key]);
                powerTabGroups.Add(powerTabGroup);
            }
            
            // Sort groups in a category
            powerTabGroups.SortByDescending(t => Mathf.Abs(t.Power));

            // Creates categories. There should theoretically be no more than the three categories defined in PowerType.cs
            // This code essentially attempts to obtain the group's PowerType, which is, by its very nature, the same as its component children's PowerType,
            // so just get the group's first child's powertype.
            List<IGrouping<PowerType, PowerTabGroup>> categories =
                powerTabGroups.GroupBy(t => t.Children.First()._thing.TryGetComp<TestComp>().PowerType).ToList(); // :| That looks... fun.
            
            categories.SortBy(t => t.Key.ToString());

            foreach (IGrouping<PowerType, PowerTabGroup> category in categories)
            {
                List<TestComp> testCompsInPowerType = TestComps.Where(t => t.PowerType == category.Key).ToList();
                PowerTabCategory powerTabCategory = new PowerTabCategory(
                    category.Key.ToString(),
                    testCompsInPowerType.Sum(t => t.CurrentPowerOutput),
                    testCompsInPowerType.Sum(t => t.CurrentPowerOutput) /
                    testCompsInPowerType.Sum(t => t.DesiredPowerOutput),
                    powerTabGroups.Where(t => t.Children.First()._thing.TryGetComp<TestComp>().PowerType == category.Key), // Otherwise, each category will render every single group
                    InnerSize.x,
                    category.Key == PowerType.Battery);
                
                powerTabCategory.Draw(y);
                y += powerTabCategory.Height;
            }
            
            _lastY = y;
            
            Widgets.EndScrollView();
            
        }
    }
}
