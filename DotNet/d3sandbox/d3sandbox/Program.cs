using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Magic;

namespace d3sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            BlackMagic d3 = new BlackMagic();
            if (d3.OpenProcessAndThread(SProcess.GetProcessFromProcessName("Diablo III")))
            {
                // TODO: Check md5sum of the executable
                string exePath = d3.GetModuleFilePath();
                Console.WriteLine(exePath);

                // Enable a debug flag in the client. Valid values seem to be 0-23, although 8 crashes v8101
                d3.WriteInt(0x1405978, 20);

                MemoryReader memReader = new MemoryReader(d3);

                //string acct = memReader.GetTextboxValue("Root.NormalLayer.BattleNetLogin_main.LayoutRoot.LoginContainer.AccountInput");
                //string passwd = memReader.GetTextboxValue("Root.NormalLayer.BattleNetLogin_main.LayoutRoot.LoginContainer.PasswordInput");

                // Fetch the current player
                Player player = memReader.GetPlayer();
                Console.WriteLine("Player: " + player);

                uint[] skills = memReader.GetActiveSkills();
                Console.WriteLine("Active Skills:");
                for (int i = 0; i < skills.Length; i++)
                    Console.WriteLine(" {0:X}", skills[i]);

                // Fetch all entities
                List<Entity> entities = memReader.GetEntities();
                Console.WriteLine();
                Console.WriteLine("Entities ({0} total):", entities.Count);
                foreach (Entity entity in entities)
                    Console.WriteLine(entity);

                // Fetch all scenes
                List<Scene> scenes = memReader.GetScenes();
                Console.WriteLine();
                Console.WriteLine("Scenes ({0} total):", scenes.Count);
                foreach (Scene scene in scenes)
                    Console.WriteLine(scene);

                // Inject our custom code
                Injector injector = new Injector(d3, memReader.EndSceneAddr);

                // Punch the nearest monster
                Entity nearestMonster = GetNearestMonster(player, entities);
                if (nearestMonster != null)
                {
                    Console.WriteLine("Walking to monster {0:X}", nearestMonster._rActor.ActorID);
                    injector.UsePower(player, new PowerInfo(0x777C, nearestMonster.Position, nearestMonster._rActor.WorldID));

                    System.Threading.Thread.Sleep(2000);

                    Console.WriteLine("Punching monster {0:X}", nearestMonster._rActor.ActorID);
                    injector.UsePower(player, new PowerInfo(0x76B7, nearestMonster._acd.ACDID));
                }
            }
        }

        private static Entity GetNearestMonster(Player player, List<Entity> entities)
        {
            Entity nearestMonster = null;

            float nearestDist = Single.MaxValue;
            foreach (Entity entity in entities)
            {
                if (entity.Type == EntityType.Monster)
                {
                    float dist = player.Position.DistanceSquared(ref entity.Position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestMonster = entity;
                    }
                }
            }

            return nearestMonster;
        }
    }
}
