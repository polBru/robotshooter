﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shokai.Events.CustomEvents;

namespace Shokai.Items
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Items/Inventory")]
    public class InventoryController : ScriptableObject
    {
        [SerializeField] private VoidEvent onInventoryItemsUpdated = null;
        [SerializeField] private ItemSlot testItemSlot = new ItemSlot();
        [Header("Items")]
        [SerializeField] public ItemSlot jetpack = new ItemSlot();
        [SerializeField] public ItemSlot grenade = new ItemSlot();
        [SerializeField] public ItemSlot stickyGrenade = new ItemSlot();
        [SerializeField] public ItemSlot emp = new ItemSlot();
        [SerializeField] public ItemSlot laser = new ItemSlot();
        [SerializeField] public ItemSlot health = new ItemSlot();
        [SerializeField] public ItemSlot tTurret = new ItemSlot();
        [SerializeField] public ItemSlot aTurret = new ItemSlot();
        [SerializeField] public ItemSlot mine = new ItemSlot();

        public ItemContainer ItemContainer { get; } = new ItemContainer(3);

        public void OnEnable()
        {
            ItemContainer.OnItemsUpdated += onInventoryItemsUpdated.Raise;
        }

        public void OnDisable()
        {
            ItemContainer.OnItemsUpdated -= onInventoryItemsUpdated.Raise;
        }

        [ContextMenu("Test Add")]
        public void TestAdd()
        {
            ItemContainer.AddItem(testItemSlot);
        }

        public void AddConsumItem(string name)
        {
            switch (name)
            {
                case "Jetpack":
                    ItemContainer.AddItem(jetpack);
                    break;
                case "Grenade":
                    ItemContainer.AddItem(grenade);
                    break;
                case "Laser":
                    ItemContainer.AddItem(laser);
                    break;
                case "Health":
                    ItemContainer.AddItem(health);
                    break;
                case "StickyGrenade":
                    ItemContainer.AddItem(stickyGrenade);
                    break;
                case "EMP":
                    ItemContainer.AddItem(emp);
                    break;
                case "Mine":
                    ItemContainer.AddItem(mine);
                    break;
                case "TerrainTurret":
                    ItemContainer.AddItem(tTurret);
                    break;
                case "AirTurret":
                    ItemContainer.AddItem(aTurret);
                    break;

            }            
        }
    }
}
