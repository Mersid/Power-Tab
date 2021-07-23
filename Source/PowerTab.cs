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
            

            _lastY = y;
            
            Widgets.EndScrollView();
            
        }
    }
}
