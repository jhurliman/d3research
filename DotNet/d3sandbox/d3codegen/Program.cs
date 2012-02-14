using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using Mooege.Common.Logging;
using Mooege.Common.MPQ;
using Mooege.Core.GS.Common.Types.SNO;
using Mooege.Common.MPQ.FileFormats;
using Mooege.Core.GS.Common.Types.TagMap;

namespace d3codegen
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Enabled = true;
            LogManager.AttachLogTarget(new ConsoleTarget(Logger.Level.Trace, Logger.Level.Fatal, false));

            #region Scenes

            using (FileStream navStream = new FileStream("NavCells.bin", FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter navWriter = new BinaryWriter(navStream))
                {
                    var scenes = MPQStorage.Data.Assets[SNOGroup.Scene];

                    foreach (Asset asset in scenes.Values)
                    {
                        Scene scene = asset.Data as Scene;

                        navWriter.Write(asset.SNOId);
                        navWriter.Write(scene.NavZone.NavCells.Count);

                        foreach (Scene.NavCell cell in scene.NavZone.NavCells)
                        {
                            navWriter.Write(cell.Min.X);
                            navWriter.Write(cell.Min.Y);
                            navWriter.Write(cell.Min.Z);
                            navWriter.Write(cell.Max.X);
                            navWriter.Write(cell.Max.Y);
                            navWriter.Write(cell.Max.Z);
                            navWriter.Write((int)cell.Flags);
                        }
                    }
                }
            }

            #endregion Scenes

            #region Experience

            List<ExperienceTable> xpTable = null;
            foreach (Asset asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
            {
                if (((GameBalance)asset.Data).Experience.Count > 0)
                {
                    xpTable = ((GameBalance)asset.Data).Experience;
                    break;
                }
            }

            using (StreamWriter xpWriter = new StreamWriter("Experience.cs"))
            {
                xpWriter.WriteLine("using System;");
                xpWriter.WriteLine();
                xpWriter.WriteLine("namespace libdiablo3.Api");
                xpWriter.WriteLine("{");
                xpWriter.WriteLine("    public static class Experience");
                xpWriter.WriteLine("    {");
                xpWriter.WriteLine("        public static int[] Levels =");
                xpWriter.WriteLine("        {");

                int prevXP = 0;
                for (int i = 0; i < xpTable.Count; i++)
                {
                    int xp = xpTable[i].Exp - prevXP;
                    prevXP = xpTable[i].Exp;
                    xpWriter.WriteLine("            " + xp + ", // " + i);
                }

                xpWriter.WriteLine("        };");
                xpWriter.WriteLine("    }");
                xpWriter.WriteLine("}");
                xpWriter.WriteLine();
            }

            #endregion Experience

            #region Item Types

            Dictionary<int, ItemTable> items = new Dictionary<int, ItemTable>();
            Dictionary<int, ItemTypeTable> itemTypes = new Dictionary<int, ItemTypeTable>();

            foreach (Asset asset in MPQStorage.Data.Assets[SNOGroup.GameBalance].Values)
            {
                GameBalance gb = asset.Data as GameBalance;
                foreach (ItemTable item in gb.Item)
                    items.Add(item.Hash, item);
                foreach (ItemTypeTable itemType in gb.ItemType)
                    itemTypes.Add(itemType.Hash, itemType);
            }

            using (StreamWriter itemWriter = new StreamWriter("ItemDefinitions.cs"))
            {
                itemWriter.WriteLine("using System;");
                itemWriter.WriteLine("using System.Collections.Generic;");
                itemWriter.WriteLine();
                itemWriter.WriteLine("namespace libdiablo3.Api");
                itemWriter.WriteLine("{");
                itemWriter.WriteLine("    public static class ItemDefinitions");
                itemWriter.WriteLine("    {");
                itemWriter.WriteLine("        public static readonly Dictionary<int, ItemDefinition> Definitions = new Dictionary<int, ItemDefinition>");
                itemWriter.WriteLine("        {");

                foreach (KeyValuePair<int, ItemTable> kvp in items)
                {
                    ItemTable item = kvp.Value;
                    itemWriter.WriteLine("            {{ {0}, new ItemDefinition({1}, {2}, {3}, {4}, {5}, {6}, {7}) }},", kvp.Key,
                        (int)item.Quality, item.ItemLevel, item.RequiredLevel, item.BaseGoldValue, item.MaxSockets, item.MaxStackAmount, item.ItemType1);
                }

                itemWriter.WriteLine("        };");
                itemWriter.WriteLine("    }");
                itemWriter.WriteLine("}");
                itemWriter.WriteLine();
            }

            using (StreamWriter itemWriter = new StreamWriter("ItemTypes.cs"))
            {
                itemWriter.WriteLine("using System;");
                itemWriter.WriteLine("using System.Collections.Generic;");
                itemWriter.WriteLine();
                itemWriter.WriteLine("namespace libdiablo3.Api");
                itemWriter.WriteLine("{");
                itemWriter.WriteLine("    public static class ItemTypes");
                itemWriter.WriteLine("    {");
                itemWriter.WriteLine("        public static readonly Dictionary<int, ItemType> Types = new Dictionary<int, ItemType>");
                itemWriter.WriteLine("        {");

                foreach (KeyValuePair<int, ItemTypeTable> kvp in itemTypes)
                {
                    ItemTypeTable type = kvp.Value;
                    itemWriter.WriteLine("            {{ {0}, new ItemType(\"{1}\", {2}, {3}, {4}) }},",
                        kvp.Key, type.Name, type.Hash, type.ParentType, (int)type.Flags);
                }

                itemWriter.WriteLine("        };");
                itemWriter.WriteLine("    }");
                itemWriter.WriteLine("}");
                itemWriter.WriteLine();
            }

            #endregion Item Types

            using (StreamWriter enumWriter = new StreamWriter("ActorName.cs"))
            {
                enumWriter.WriteLine("using System;");
                enumWriter.WriteLine();
                enumWriter.WriteLine("namespace libdiablo3.Api");
                enumWriter.WriteLine("{");
                enumWriter.WriteLine("    public enum ActorName");
                enumWriter.WriteLine("    {");

                using (StreamWriter templateWriter = new StreamWriter("ActorTemplates.cs"))
                {
                    templateWriter.WriteLine("using System;");
                    templateWriter.WriteLine("using System.Collections.Generic;");
                    templateWriter.WriteLine();
                    templateWriter.WriteLine("namespace libdiablo3.Api");
                    templateWriter.WriteLine("{");
                    templateWriter.WriteLine("    public static class ActorTemplates");
                    templateWriter.WriteLine("    {");
                    templateWriter.WriteLine("        public static readonly Dictionary<int, Actor> Actors = new Dictionary<int, Actor>");
                    templateWriter.WriteLine("        {");

                    var actors = MPQStorage.Data.Assets[SNOGroup.Actor];
                    var monsters = MPQStorage.Data.Assets[SNOGroup.Monster];

                    foreach (Asset asset in actors.Values)
                    {
                        Actor actor = asset.Data as Actor;

                        string sanitizedName = asset.Name.Replace('\'', '_').Replace(' ', '_').Replace('-', '_').Replace('(', '_').Replace(')', '_');
                        enumWriter.WriteLine("        {0} = {1},", sanitizedName, actor.Header.SNOId);

                        switch (actor.Type)
                        {
                            case ActorType.Gizmo:
                                WriteGizmoActor(templateWriter, actor);
                                break;
                            case ActorType.Item:
                                WriteItemActor(templateWriter, actor);
                                break;
                            case ActorType.Monster:
                                WriteMonsterActor(templateWriter, actor, monsters);
                                break;
                            case ActorType.Player:
                                WritePlayerActor(templateWriter, actor);
                                break;
                            case ActorType.AxeSymbol:
                            case ActorType.ClientEffect:
                            case ActorType.Critter:
                            case ActorType.CustomBrain:
                            case ActorType.Enviroment:
                            case ActorType.Invalid:
                            case ActorType.Projectile:
                            case ActorType.ServerProp:
                            default:
                                WriteDefaultActor(templateWriter, actor);
                                break;
                        }
                    }

                    templateWriter.WriteLine("        };");
                    templateWriter.WriteLine("    }");
                    templateWriter.WriteLine("}");
                    templateWriter.WriteLine();
                }

                enumWriter.WriteLine("    }");
                enumWriter.WriteLine("}");
                enumWriter.WriteLine();
            }

            Console.WriteLine("Done writing .cs files");
        }

        static void WriteDefaultActor(StreamWriter writer, Actor actor)
        {
            int teamID = 0;
            if (actor.TagMap.ContainsKey(ActorKeys.TeamID))
                teamID = actor.TagMap[ActorKeys.TeamID];

            writer.WriteLine("            {{ {0}, new Actor({0}, {1}, {2}) }},",
                actor.Header.SNOId, (int)actor.Type, teamID);
        }

        static void WriteGizmoActor(StreamWriter writer, Actor actor)
        {
            int teamID = 0;
            if (actor.TagMap.ContainsKey(ActorKeys.TeamID))
                teamID = actor.TagMap[ActorKeys.TeamID];
            GizmoGroup group = actor.TagMap[ActorKeys.GizmoGroup];

            writer.WriteLine("            {{ {0}, new Gizmo({0}, {1}, {2}) }},",
                actor.Header.SNOId, (int)group, teamID);
        }

        static void WriteItemActor(StreamWriter writer, Actor actor)
        {
            int teamID = 0;
            if (actor.TagMap.ContainsKey(ActorKeys.TeamID))
                teamID = actor.TagMap[ActorKeys.TeamID];

            writer.WriteLine("            {{ {0}, new Item({0}, {1}) }},",
                actor.Header.SNOId, teamID);
        }

        static void WriteNPCActor(StreamWriter writer, Actor actor)
        {
            int teamID = 0;
            if (actor.TagMap.ContainsKey(ActorKeys.TeamID))
                teamID = actor.TagMap[ActorKeys.TeamID];

            writer.WriteLine("            {{ {0}, new NPC({0}, {1}, {2}) }},",
                actor.Header.SNOId, (int)actor.Type, teamID);
        }

        static void WriteMonsterActor(StreamWriter writer, Actor actor, ConcurrentDictionary<int, Asset> monsters)
        {
            Monster monster = (Monster)monsters[actor.MonsterSNO].Data;

            // Write Ally/Helper "monsters" as NPCs
            if (monster.Type == Monster.MonsterType.Ally ||
                monster.Type == Monster.MonsterType.Helper)
            {
                WriteNPCActor(writer, actor);
                return;
            }

            // Write other non-combatant "monsters" as normal actors
            if (actor.TagMap[ActorKeys.TeamID] != 10)
            {
                WriteDefaultActor(writer, actor);
                return;
            }

            writer.WriteLine("            {{ {0}, new Monster({0}, {1}, {2}, {3}, {4}, {5}) }},",
                actor.Header.SNOId, (int)monster.Size, (int)monster.Race, (int)monster.Type, (int)monster.PowerType,
                (int)monster.Resists);
        }

        static void WritePlayerActor(StreamWriter writer, Actor actor)
        {
            writer.WriteLine("            {{ {0}, new Hero({0}) }},",
                actor.Header.SNOId);
        }
    }
}
