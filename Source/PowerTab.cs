using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace PowerTab
{
    // ReSharper disable once UnusedType.Global
    // This class is used in Harmony
    public class PowerTab : ITab
    {
        public PowerTab()
        {
            size = new Vector2(450f, 450f);
            labelKey = "PowerSwitch_Power";
        }


        private Vector2 _scrollPos;
        private float _lastY;
        private readonly Dictionary<ThingDef, bool> _collapseTab = new Dictionary<ThingDef, bool>();

        protected override void FillTab()
        {
            CompPowerTracker compPower = SelThing.TryGetComp<CompPowerTracker>();
            PowerNet powerNet = compPower?.PowerNetwork;
            if (powerNet == null) return;

            Widgets.BeginScrollView(
                new Rect(default(Vector2), size).ContractedBy(GenUI.GapTiny),
                ref _scrollPos,
                new Rect(default(Vector2) - new Vector2(50, 0), new Vector2(size.x - GenUI.GapTiny * 2 - GenUI.ScrollBarWidth, _lastY))
            );

            float yref = 10;

            IEnumerable< IGrouping <CompPowerTracker.PowerType, IGrouping <ThingDef, CompPowerTracker>>> categories =
                powerNet.batteryComps.ConvertAll((c) => c.parent.GetComp<CompPowerTracker>())
                .Concat(powerNet.powerComps.ConvertAll((c) => c.parent.GetComp<CompPowerTracker>()))
                .GroupBy((c) => c.parent.def)
                .GroupBy((d) => CompPowerTracker.PowerTypeFor(d.Key));

            foreach (IGrouping<CompPowerTracker.PowerType, IGrouping<ThingDef, CompPowerTracker>> type in categories) {
                
                float p = type.Sum((g) => g.Sum((t) => t.PowerUsage));
                float m = type.Sum((g) => g.Sum((t) => t.MaxPowerUsage));

                Rect rect = new Rect(150, yref, size.x - 172, Text.SmallFontHeight);

                Widgets.FillableBarLabeled(rect, Math.Abs(p / m), 0, "");
                string label = "{0}W".Formatted(p.ToString("0"));
                float width = label.GetWidthCached();
                Widgets.DrawRectFast(new RectOffset(-4, -4, -4, -4).Add(rect).LeftPartPixels(width + 4), Color.black);
                Widgets.Label(new RectOffset(-4, 0, 0, 0).Add(rect), label);

                Widgets.ListSeparator(ref yref, rect.width, "{0}".Formatted(CompPowerTracker.PowerTypeString[(int)type.Key]));

                float maxPowerUsage = type.Max((g) => g.Sum((c) => Math.Abs(c.MaxPowerUsage)));

                yref += GenUI.GapTiny;

                foreach (IGrouping<ThingDef, CompPowerTracker> defs in type)
                {
                    // Begin group; All future GUI elements are relative to this group
                    GUI.BeginGroup(new Rect(0, yref, size.x, Text.SmallFontHeight + GenUI.GapTiny * 2));

                    // Make a rect that is the size of our group
                    rect = new Rect(0, 0, size.x - GenUI.GapTiny * 2 - GenUI.ScrollBarWidth, Text.SmallFontHeight + GenUI.GapTiny * 2);

                    // Draw a background behind our group
                    Widgets.DrawOptionSelected(rect);

                    if (!_collapseTab.ContainsKey(defs.Key)) _collapseTab.Add(defs.Key, false);
                    if (Widgets.ButtonText(rect.LeftPartPixels(rect.height).ContractedBy(GenUI.GapTiny), _collapseTab[defs.Key] ? "-" : "+")) _collapseTab[defs.Key] = !_collapseTab[defs.Key];

                    rect.xMin += rect.height;
                    Widgets.Label(rect.LeftPartPixels(150).ContractedBy(GenUI.GapTiny), "{0} {1}".Formatted(defs.Count(), defs.Key.LabelCap));
                    rect.xMin += 150;
                    Widgets.FillableBarLabeled(rect.ContractedBy(GenUI.GapTiny), defs.Sum((c) => Math.Abs(c.PowerUsage)) / maxPowerUsage, 50, "Power");
                    label = "{0}W".Formatted(defs.Sum((c) => c.PowerUsage).ToString("0"));
                    width = label.GetWidthCached();
                    rect = new RectOffset(-58, 0, -4, -4).Add(rect).LeftPartPixels(width + 4);
                    Widgets.DrawRectFast(new RectOffset(0, 0, -4, -4).Add(rect), Color.black);
                    Widgets.Label(rect, label);
                    yref += GenUI.ListSpacing + GenUI.GapTiny;

                    GUI.EndGroup();

                    if (!_collapseTab[defs.Key]) continue;
                    
                    foreach (CompPowerTracker comp in defs)
                    {
                        comp.DrawGUI(ref yref, size.x, defs.Max((c) => c.MaxPowerUsage));
                    }
                    
                }
            }

            Widgets.EndScrollView();

            _lastY = yref;
        }
    }
}
