﻿using InventorySystem.Disarming;
using InventorySystem;

namespace Qurre.API.Classification.Player
{
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.Attachments;
    using MapGeneration;
    using PlayerRoles;
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.API.Objects;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GamePlay
    {
        private readonly Player _player;
        internal GamePlay(Player pl) => _player = pl;

        public Inventory Inventory => _player.ReferenceHub.inventory;

        public bool Cuffed => _player.ReferenceHub.inventory.IsDisarmed();
        public bool Overwatch
        {
            get => _player.ReferenceHub.serverRoles.IsInOverwatch;
            set => _player.ReferenceHub.serverRoles.IsInOverwatch = value;
        }

        public bool GodMode
        {
            get => _player.ClassManager.GodMode;
            set => _player.ClassManager.GodMode = value;
        }

        public ZoneType CurrentZone => Room?.Zone ?? ZoneType.Unknown;

        public Room Room
        {
            get => RoomIdUtils.RoomAtPosition(_player.MovementState.Position).GetRoom() ??
                Map.Rooms.OrderBy(x => Vector3.Distance(x.Position, _player.MovementState.Position)).FirstOrDefault();
            set => _player.MovementState.Position = value.Position + Vector3.up * 2;
        }

        public Item AddItem(ItemBase itemBase)
        {
            Item item = Item.Get(itemBase);
            Inventory.UserInventory.Items[itemBase.PickupDropModel.NetworkInfo.Serial] = itemBase;

            itemBase.OnAdded(itemBase.PickupDropModel);
            if (itemBase is InventorySystem.Items.Firearms.Firearm)
                AttachmentsServerHandler.SetupProvidedWeapon(_player.ReferenceHub, itemBase);

            Inventory.SendItemsNextFrame = true;
            return item;
        }
        public void AddItem(Item item, int amount)
        {
            if (0 >= amount) return;
            for (int i = 0; i < amount; i++)
                AddItem(item.Base);
        }
        public void AddItem(List<Item> items)
        {
            if (0 >= items.Count) return;
            for (int i = 0; i < items.Count; i++)
                AddItem(items[i].Base);
        }
        public void DropItem(Item item) => Inventory.ServerDropItem(item.Serial);
        public bool HasItem(ItemType item) => Inventory.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);
     
    }
}