using Corruption.Core.Soul;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Corruption.Worship
{
    [StaticConstructorOnStartup]
    public class TempleCardUtility
    {
        public static readonly Texture2D Rename = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");
        public static Vector2 TempleCardSize = new Vector2(570f, 400f);

        public static void DrawTempleCard(Rect rect, BuildingAltar altar)
        {
            GUI.BeginGroup(rect);
            Rect rect2 = new Rect(rect.x, rect.y + 20f, 250f, 55f);
            Text.Font = GameFont.Medium;
            Widgets.Label(rect2, altar.RoomName);
            Text.Font = GameFont.Small;

            Rect rect3 = rect2;
            rect3.y = rect2.yMax + 30f;
            rect3.width = 200f;
            rect3.height = 25f;
            Widgets.Label(rect3, "Preacher".Translate() + ": ");
            rect3.xMin = rect3.center.x + 10f;
            string label2 = PreacherLabel(altar);
            if (Widgets.ButtonText(rect3, label2, true, false, true))
            {
                TempleCardUtility.OpenPreacherSelectMenu(altar);
            }

            if (altar.Faction == Faction.OfPlayer)
            {
                Rect rect8 = new Rect(rect.width - 36f, 0f, 30f, 30f);
                TooltipHandler.TipRegion(rect8, () => "RenameTemple".Translate(), altar.thingIDNumber);
                if (Widgets.ButtonImage(rect8, Rename))
                {
                    Find.WindowStack.Add(new Dialog_RenameTemple(altar));
                }
            }

            Rect morningRect = new Rect(0f, rect3.yMax + 4f, rect.width, Text.LineHeight * 2f + 8f);

            foreach (var template in altar.Templates)
            {
                float curY = DrawSermonTemplate(morningRect, template);
            }
            GUI.EndGroup();
        }

        private static float DrawSermonTemplate(Rect morningRect, SermonTemplate template)
        {
            Rect valRect = new Rect(morningRect.x, morningRect.y, morningRect.width / 2f, morningRect.height);
            Widgets.HorizontalSlider(valRect, template.PreferredStartTime, template.AvailableRange.min, template.AvailableRange.max, true, "SermonStartingTime".Translate(),null, null, 1f);

            Rect checkRect = valRect;
            checkRect.x += valRect.width + 4f;

            Widgets.CheckboxLabeled(checkRect, "Active".Translate(), ref template.Active, false);

            return checkRect.yMax + 4f;
        }

        public static string PreacherLabel(BuildingAltar altar)
        {
            if (altar.preacher == null)
            {
                return "None";
            }
            else
            {
                return altar.preacher.NameShortColored;
            }
        }

        private static void ForceListenersTest(BuildingAltar altar)
        {

            IntVec3 result;
            Building chair;
            foreach (Pawn p in altar.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.RaceProps.intelligence == Intelligence.Humanlike))
            {
                if (!SermonUtility.IsPreacher(p))
                {
                    if (!WatchBuildingUtility.TryFindBestWatchCell(altar, p, true, out result, out chair))
                    {

                        if (!WatchBuildingUtility.TryFindBestWatchCell(altar as Thing, p, false, out result, out chair))
                        {
                            return;
                        }
                    }
                    if (chair != null)
                    {
                        Job J = new Job(JobDefOf.AttendSermon, altar.preacher, altar, chair);
                        p.jobs.jobQueue.EnqueueLast(J);
                        p.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                    else
                    {
                        Job J = new Job(JobDefOf.AttendSermon, altar.preacher, altar, result);
                        p.jobs.jobQueue.EnqueueLast(J);
                        p.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }
            }
        }

        public static void OpenPreacherSelectMenu(BuildingAltar altar)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("(" + "NoneLower".Translate() + ")", delegate
            {
                altar.preacher = null;
            }, MenuOptionPriority.Default, null, null, 0f, null));

            foreach (Pawn current in altar.Map.mapPawns.FreeColonistsSpawned)
            {
                if (!SermonUtility.IsPreacher(current))
                {
                    CompSoul nsoul = current.Soul();
                    string text1 = current.NameShortColored;

                    Action action;
                    Pawn localCol = current;
                    action = delegate
                    {
                        altar.preacher = localCol;
                    };
                    list.Add(new FloatMenuOption(text1, action, MenuOptionPriority.Default, null, null, 0f, null));
                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
