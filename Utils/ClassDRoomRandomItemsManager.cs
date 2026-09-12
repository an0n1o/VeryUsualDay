using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;

namespace VeryUsualDay.Utils
{
    public static class ClassDRoomRandomItemsManager
    {
        private const string TestSubjectKey = "isTestSubject";
        private const string LastUseKey = "classDGetItemLastUse";
        private const string UsedRoomsKey = "classDGetItemUsedRooms";

        private static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(2);

        private static readonly List<ItemType> Items = new List<ItemType>()
        {
            ItemType.GunFSP9,
            ItemType.Coin,
            ItemType.Painkillers,
            ItemType.Radio,
            ItemType.Medkit,
            ItemType.KeycardGuard,
            ItemType.GunCOM15,
            ItemType.KeycardZoneManager,
            ItemType.Flashlight,
            ItemType.SurfaceAccessPass,
            ItemType.GrenadeFlash,
            ItemType.KeycardResearchCoordinator
        };

        private static readonly HashSet<RoomType> AllowedRooms = new HashSet<RoomType>()
        {
       
            RoomType.LczClassDSpawn,   
            RoomType.LczToilets,      
            RoomType.LczCafe,          
            RoomType.LczGlassBox,      
            RoomType.Lcz173,          

            RoomType.EzUpstairsPcs,    
            RoomType.EzPcs,           
            RoomType.EzDownstairsPcs,  
            RoomType.EzIntercom,       
            RoomType.EzVent            
        };

        public static bool TryGiveRandomItems(Player player, out string response)
        {

            if (!player.SessionVariables.TryGetValue(TestSubjectKey, out var value) ||
                !(value is bool isTestSubject) ||
                !isTestSubject)
            {
                response = "Эта команда доступна только Испытуемым.";
                return false;
            }

            if (player.Role.Type != RoleTypeId.ClassD)
            {
                response = "Эта команда доступна только Испытуемым.";
                return false;
            }

            Room room = player.CurrentRoom;

            if (room == null || !IsAllowedRoom(room))
            {
                response = "В этом помещении невозможно найти предметы.";
                return false;
            }

            HashSet<Room> usedRooms = GetUsedRooms(player);

            if (usedRooms.Contains(room))
            {
                response = "Вы уже искали предметы в этом помещении.";
                return false;
            }

            DateTime now = DateTime.UtcNow;

            if (player.SessionVariables.TryGetValue(LastUseKey, out var lastUseValue) &&
                lastUseValue is DateTime lastUse)
            {
                TimeSpan remaining = Cooldown - (now - lastUse);

                if (remaining > TimeSpan.Zero)
                {
                    int seconds = Math.Max(
                        1,
                        (int)Math.Ceiling(remaining.TotalSeconds));

                    int minutes = seconds / 60;
                    int remainingSeconds = seconds % 60;

                    response = $"Подождите ещё {minutes}:{remainingSeconds:00} перед следующим использованием.";
                    return false;
                }
            }

            HashSet<ItemType> selectedItems = GetRandomItems();

            foreach (ItemType item in selectedItems)
            {
                player.AddItem(item);
            }

            usedRooms.Add(room);
            player.SessionVariables[LastUseKey] = now;

            response = $"Вы нашли предметы. Получено: {selectedItems.Count}.";
            return true;
        }

        private static bool IsAllowedRoom(Room room)
        {
            if (room.Zone == ZoneType.HeavyContainment)
                return true;

            return AllowedRooms.Contains(room.Type);
        }

        private static HashSet<Room> GetUsedRooms(Player player)
        {
            if (player.SessionVariables.TryGetValue(UsedRoomsKey, out var value) &&
                value is HashSet<Room> usedRooms)
            {
                return usedRooms;
            }

            HashSet<Room> newUsedRooms = new HashSet<Room>();
            player.SessionVariables[UsedRoomsKey] = newUsedRooms;

            return newUsedRooms;
        }

        private static int GetRandomItemCount()
        {
            return UnityEngine.Random.Range(1, 4);
        }

        private static ItemType RandomItem()
        {
            int randomIndex = UnityEngine.Random.Range(0, Items.Count);
            return Items[randomIndex];
        }

        private static HashSet<ItemType> GetRandomItems()
        {
            HashSet<ItemType> selectedItems = new HashSet<ItemType>();

            int count = GetRandomItemCount();

            while (selectedItems.Count < count)
            {
                ItemType item = RandomItem();
                selectedItems.Add(item);
            }

            return selectedItems;
        }
    }
}
