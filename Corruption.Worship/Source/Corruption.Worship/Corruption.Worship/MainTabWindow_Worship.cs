using Corruption.Core.Gods;
using Corruption.Core.Soul;
using Corruption.Worship.Wonders;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Corruption.Worship
{
    public class MainTabWindow_Worship : MainTabWindow
    {
        private List<Pawn> Worshippers = new List<Pawn>();
        private List<BuildingAltar> Altars = new List<BuildingAltar>();
        private static List<TabRecord> tabsList = new List<TabRecord>();

        private MainTabWindow_Worship.Tab selectedTab;

        private enum Tab
        {
            Favour,
            Pawns
        }

        public MainTabWindow_Worship()
        {
            this.forcePause = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            CacheWorshippers();
            CacheAltars();

            tabsList.Clear();
            tabsList.Add(new TabRecord("FavourTab".Translate(), delegate
            {
                selectedTab = Tab.Favour;
            }, selectedTab == Tab.Favour));
            tabsList.Add(new TabRecord("WorshippersTab".Translate(), delegate
            {
                selectedTab = Tab.Pawns;
            }, selectedTab == Tab.Pawns));
        }

        private void CacheAltars()
        {
            this.Altars.Clear();
            this.Altars = Find.CurrentMap.listerBuildings.allBuildingsColonist.Where(x => x is BuildingAltar).Cast<BuildingAltar>().ToList();
        }

        private void CacheWorshippers()
        {
            this.Worshippers.Clear();

            List<Map> maps = Find.Maps;
            maps.SortBy((Map x) => !x.IsPlayerHome, (Map x) => x.uniqueID);
            foreach (var map in maps)
            {
                Worshippers.AddRange(map.mapPawns.FreeColonists);
                List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                for (int j = 0; j < list.Count; j++)
                {
                    if (!list[j].IsDessicated())
                    {
                        Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                        if (innerPawn != null && innerPawn.IsColonist)
                        {
                            Worshippers.Add(innerPawn);
                        }
                    }
                }
                List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
                for (int k = 0; k < allPawnsSpawned.Count; k++)
                {
                    Corpse corpse = allPawnsSpawned[k].carryTracker.CarriedThing as Corpse;
                    if (corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist)
                    {
                        Worshippers.Add(corpse.InnerPawn);
                    }
                }
            }
            foreach (var caravan in Find.WorldObjects.Caravans)
            {
                this.Worshippers.AddRange(caravan.pawns.InnerListForReading.FindAll(x => x.IsColonist));
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            inRect.yMin += 32f;

            Widgets.DrawMenuSection(inRect);
            TabDrawer.DrawTabs(inRect, tabsList);
            inRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            inRect.x = 0f;
            inRect.y = 0f;
            //inRect.yMax -= 59f;
            switch (selectedTab)
            {
                case Tab.Favour:
                    {
                        WorshipUIUtility.DrawColonyPantheon(inRect);
                        break;
                    }
                case Tab.Pawns:
                    {
                        WorshipUIUtility.DrawWorshippers(inRect, Worshippers, this.Altars);
                        break;
                    }
            }
            GUI.EndGroup();
        }

    }

    [StaticConstructorOnStartup]
    public static class WorshipUIUtility
    {
        private static Vector2 pantheonScrollPos = Vector2.zero;
        private static readonly Texture2D BackgroundTile = ContentFinder<Texture2D>.Get("UI/Background/SoulmeterTile", true);

        private static List<WonderDef> cachedWonders = DefDatabase<WonderDef>.AllDefsListForReading;

        public static void DrawColonyPantheon(Rect inRect)
        {
            Rect pantheonHeaderRect = new Rect(inRect);
            pantheonHeaderRect.height = 236f;
            pantheonHeaderRect.width -= 16f; 
            Widgets.DrawMenuSection(pantheonHeaderRect);
            Rect innerRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(innerRect);
            var pantheon = GlobalWorshipTracker.Current.PlayerPantheon;
            Rect pantheonRect = new Rect(0f, 0f, innerRect.width, 32f);
            Text.Font = GameFont.Medium;
            Widgets.Label(pantheonRect, "ColonyPantheon".Translate(new NamedArgument(pantheon.label, "RELIGION")));
            Text.Font = GameFont.Small;
            Rect debugRectAdd = new Rect(innerRect.width - 256f, 0f, 256f, 24f);
            if (Prefs.DevMode)
            {
                if (Widgets.ButtonText(debugRectAdd, "Debug: +1000 FavourAll"))
                {
                    foreach (var god in pantheon.members.Select(x => x.god))
                    {
                        GlobalWorshipTracker.Current.TryAddFavor(god, 1000f);
                    }
                }
            }

            Rect descriptionRect = new Rect(0f, pantheonRect.yMax, pantheonRect.width, Text.LineHeight * 3f);
            Widgets.TextAreaScrollable(descriptionRect, pantheon.description, ref pantheonScrollPos, true);
            Text.Font = GameFont.Medium;
            Rect progressDescRect = new Rect(0f, descriptionRect.yMax + 8f, innerRect.width, 32f);
            Widgets.Label(progressDescRect, "OverallFavour".Translate());
            Text.Font = GameFont.Small;
            Rect progressRect = new Rect(24f, progressDescRect.yMax + 8f, innerRect.width - 80f, 32f);
            GUI.DrawTexture(progressRect, BackgroundTile);
            float overallProgress = GlobalWorshipTracker.Current.PantheonFavourPercentage;
            TooltipHandler.TipRegion(progressRect, new TipSignal(string.Concat(overallProgress * 100f, "%")));
            Rect innerProgressRect = progressRect.ContractedBy(2f);
            Widgets.FillableBar(innerProgressRect, overallProgress, pantheon.FillTex, null, false);
            Rect leftNodeRect = new Rect(0f, progressRect.y - 8f, 48f, 48f);
            GUI.DrawTexture(leftNodeRect, pantheon.Icon);
            Rect rightNodeRect = new Rect(leftNodeRect);
            rightNodeRect.x = progressRect.xMax - 24f;
            GUI.DrawTexture(rightNodeRect, pantheon.Icon);


            Rect attributesRect = new Rect(0f, pantheonHeaderRect.yMax + 4f, innerRect.width * 0.33f, innerRect.height - pantheonHeaderRect.yMax - 8f);
            DrawAttributes(attributesRect, pantheon);

            Rect godsRect = new Rect(attributesRect.xMax + 4f, attributesRect.y, innerRect.width - attributesRect.xMax - 4f, attributesRect.height);
            DrawGods(pantheon, godsRect);
            GUI.EndGroup();
        }

        private static void DrawAttributes(Rect inRect, PantheonDef pantheon)
        {
            Widgets.DrawMenuSection(inRect);
            inRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            Rect titleRect = new Rect(0f, 0f, inRect.width, 48f);
            Text.Font = GameFont.Medium;
            Widgets.Label(titleRect, "PantheonAttributes".Translate());
            Text.Font = GameFont.Small;

            float curY = titleRect.yMax + 4f;

            foreach(var attribute in pantheon.pantheonAttributes)
            {
                Rect attributeRect = new Rect(0f, curY, inRect.width, 64f);
                Rect attributeIconRect = new Rect(0f, curY, 64f, 64f);
                GUI.DrawTexture(attributeIconRect, attribute.Icon);
                Rect labelRect = new Rect(attributeIconRect.xMax + 4f, curY, attributeRect.width - 64f, 64f);
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, attribute.label.CapitalizeFirst());
                if (Mouse.IsOver(attributeRect))
                {
                    GUI.DrawTexture(attributeRect, TexUI.HighlightSelectedTex);
                }
                TooltipHandler.TipRegion(attributeRect, new TipSignal(attribute.description));

                Text.Anchor = TextAnchor.UpperLeft;

                curY = attributeRect.yMax + 4f;
            }

            GUI.EndGroup();
        }


        private static Vector2 godScrollPos = Vector2.zero;
        private static void DrawGods(PantheonDef pantheon, Rect godsRect)
        {
            Widgets.DrawMenuSection(godsRect);
            godsRect = godsRect.ContractedBy(17f);
            GUI.BeginGroup(godsRect);
            Text.Font = GameFont.Medium;
            Rect godsDescrRect = new Rect(0f, 0f, godsRect.width, 32f);
            Widgets.Label(godsDescrRect, "PantheonMembers".Translate());
            Text.Font = GameFont.Small;
            Rect memberRect = new Rect(0f, godsDescrRect.yMax + 4f, godsRect.width, godsRect.height - godsDescrRect.yMax - 4f);
            Rect viewRect = new Rect(0f, 0f, godsRect.width - 24f, pantheon.members.Count * 226f);
            Widgets.BeginScrollView(memberRect, ref godScrollPos, viewRect);

            float curY = 0f;
            foreach (var member in pantheon.members)
            {
                Widgets.DrawLineHorizontal(0f, curY, godsRect.width);
                Rect godLabelRect = new Rect(0f, curY + 8f, godsRect.width, 32f);
                GUI.color = member.god.mainColor;
                Text.Font = GameFont.Medium;
                Widgets.Label(godLabelRect, member.god.label);
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
                Rect descriptionRect = new Rect(godLabelRect);
                descriptionRect.height += 48f;
                descriptionRect.y += godLabelRect.height + 2f;
                Widgets.Label(descriptionRect, member.god.description);

                float godsFavour = GlobalWorshipTracker.Current.GetFavourProgressFor(member.god).Favour;
                Rect favourLabelRect = new Rect(0f, descriptionRect.yMax + 4f, descriptionRect.width, Text.LineHeight);
                Widgets.Label(favourLabelRect, String.Concat("GodsFavour".Translate(),": ", ((int)godsFavour).ToString()));

                Rect borderRect = new Rect(48f, favourLabelRect.yMax + 6f, godsRect.width - 96f, 32f);
                GUI.DrawTexture(borderRect, BackgroundTile);
                Rect progressRect = new Rect(borderRect).ContractedBy(4f);
                Widgets.FillableBar(progressRect, GlobalWorshipTracker.Current.GetFavourProgressFor(member.god).FavourPercentage, member.god.worshipBarFillTexture, null, false);

                //TooltipHandler.TipRegion(progressRect, new TipSignal(string.Concat(godsFavour, " / ", FavourProgress.ProgressRange.max)));

                Rect nodesRect = new Rect(borderRect.x, borderRect.y - 8f, progressRect.width, 48f);
                DrawGodWonderNodes(nodesRect, member.god, godsFavour);
                curY = nodesRect.yMax + 4f;
            }
            Widgets.EndScrollView();

            GUI.EndGroup();
        }

        private static void DrawGodWonderNodes(Rect nodesRect, GodDef god, float godsFavour)
        {
            float totalWidth = nodesRect.width;
            foreach (var wonder in cachedWonders.Where(x => x.associatedGods.Contains(god)))
            {
                bool available = godsFavour >= wonder.favourCost;
                float xOffset = nodesRect.x + (wonder.favourCost / FavourProgress.ProgressRange.max * totalWidth) - 24f;
                Rect nodeRect = new Rect(xOffset, nodesRect.y, 48f, 48f);
                GUI.DrawTexture(nodeRect, wonder.wonderIcon);
                if (Mouse.IsOver(nodeRect))
                {
                    GUI.DrawTexture(nodeRect, TexUI.HighlightSelectedTex);
                    TooltipHandler.TipRegion(nodeRect, new TipSignal(wonder.ToolTip, wonder.description.GetHashCode() * 124758));
                }
                if (available && Widgets.ButtonInvisible(nodeRect, true))
                {
                    if (wonder.pointsScalable)
                    {
                        Func<int, string> textGetter;
                        textGetter = ((int x) => "SetWorshipPoints".Translate());
                        Dialog_Slider window = new Dialog_Slider(textGetter, wonder.favourCost, (int)godsFavour, delegate (int value)
                        {
                            wonder.Worker.TryExecuteWonder(value);
                            GlobalWorshipTracker.Current.ConsumeFavourFor(value, god);
                        }, wonder.favourCost);
                        Find.WindowStack.Add(window);
                    }
                    else
                    {
                        wonder.Worker.TryExecuteWonder(wonder.favourCost);
                        GlobalWorshipTracker.Current.ConsumeFavourFor(wonder.favourCost, god);
                    }
                }
            }
        }

        private static Vector2 followersScrollPos = Vector2.zero;
        private static Vector2 altarsScrollPos = Vector2.zero;

        internal static void DrawWorshippers(Rect inRect, List<Pawn> worshippers, List<BuildingAltar> altars)
        {
            inRect = inRect.ContractedBy(17f);
            GUI.BeginGroup(inRect);
            Rect pawnsRect = new Rect(0f, 0f, inRect.width * 256f, inRect.height);
            GUI.BeginGroup(pawnsRect);
            pawnsRect = pawnsRect.ContractedBy(5f);
            Text.Font = GameFont.Medium;
            Rect labelRect = new Rect(0f, 0f, pawnsRect.width, Text.LineHeight);
            Widgets.Label(labelRect, "Followers".Translate());
            Text.Font = GameFont.Small;
            Rect outerRect = new Rect(0f, labelRect.yMax + 4f, pawnsRect.width, pawnsRect.height - labelRect.yMax - 8f);
            Rect viewRect = new Rect(0f, 0f, outerRect.width - 8f, worshippers.Count * 128f);
            Widgets.BeginScrollView(outerRect, ref followersScrollPos, viewRect);
            float curY = 0f;
            foreach (var pawn in worshippers)
            {
                Rect rect = new Rect(0f, curY, 64f, 64f);
                GUI.BeginGroup(rect);
                pawn.Drawer.renderer.RenderPortrait();
                GUI.EndGroup();

                Rect nameRect = new Rect(68f, curY, 196f, Text.LineHeight);
                Widgets.Label(nameRect, pawn.NameShortColored);
                curY = rect.yMax + 4f;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();

            Rect altarsRect = new Rect(pawnsRect.xMax + 8f, 0f, inRect.width - pawnsRect.xMax - 8f, pawnsRect.height);
            Widgets.DrawMenuSection(altarsRect);
            altarsRect = altarsRect.ContractedBy(17f);
            GUI.BeginGroup(altarsRect);
            altarsRect = new Rect(0f, 0f, altarsRect.width, altarsRect.height);
            Text.Font = GameFont.Medium;
            Rect altarTitleRect = new Rect(0f, 0f, altarsRect.width, Text.LineHeight);
            Widgets.Label(altarTitleRect, "PlacesOfWorship".Translate());
            Text.Font = GameFont.Small;
            Rect outerAltarRect = new Rect(0f, altarTitleRect.yMax + 4f, altarsRect.width, altarsRect.height - altarTitleRect.yMax - 4f);
            Rect altarViewRect = new Rect(4f, 2f, altarsRect.width - 8f, altars.Count * 128f);
            Widgets.BeginScrollView(outerAltarRect, ref altarsScrollPos, altarViewRect);
            curY = 0f;
            foreach (var altar in altars)
            {
                Widgets.DrawLineHorizontal(4f, curY, altarViewRect.width - 8f);
                Rect iconRect = new Rect(0f, curY + 32f, 64f, 64f);
                GUI.DrawTexture(iconRect, altar.def.uiIcon);
                Rect titleRect = new Rect(68f, curY + 2f, altarViewRect.width, Text.LineHeight);
                Widgets.Label(titleRect, altar.RoomName);

                Rect rect3 = new Rect(68f, curY + 2f, 200f, 25f);
                string label2 = TempleCardUtility.PreacherLabel(altar);
                if (Widgets.ButtonText(rect3, label2, true, false, true))
                {
                    TempleCardUtility.OpenPreacherSelectMenu(altar);
                }

            }
            Widgets.EndScrollView();
            GUI.EndGroup();


            GUI.EndGroup();
        }
    }
}
