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

            using (StreamWriter navWriter = new StreamWriter("NavCells.cs"))
            {
                navWriter.WriteLine("using System;");
                navWriter.WriteLine("using System.Collections.Generic;");
                navWriter.WriteLine();
                navWriter.WriteLine("namespace libdiablo3.Api");
                navWriter.WriteLine("{");
                navWriter.WriteLine("    public static class NavCells");
                navWriter.WriteLine("    {");
                navWriter.WriteLine("        public static readonly Dictionary<int, NavCell[]> SceneNavCells = new Dictionary<int, NavCell[]>");
                navWriter.WriteLine("        {");

                var scenes = MPQStorage.Data.Assets[SNOGroup.Scene];

                foreach (Asset asset in scenes.Values)
                {
                    Scene scene = asset.Data as Scene;

                    navWriter.Write("            {{ {0}, new NavCell[] {{", asset.SNOId);

                    foreach (Scene.NavCell cell in scene.NavZone.NavCells)
                    {
                        navWriter.Write(" new NavCell({0:0.###}f, {1:0.###}f, {2:0.###}f, {3:0.###}f, {4:0.###}f, {5:0.###}f, {6}),",
                            cell.Min.X, cell.Min.Y, cell.Min.Z,
                            cell.Max.X, cell.Max.Y, cell.Max.Z,
                            (int)cell.Flags);
                    }

                    navWriter.WriteLine(" } },");
                }

                navWriter.WriteLine("        };");
                navWriter.WriteLine("    };");
                navWriter.WriteLine("};");
                navWriter.WriteLine();
            }

            #endregion Scenes

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
