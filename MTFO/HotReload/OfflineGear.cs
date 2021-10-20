using GameData;
using Gear;
using MTFO.Utilities;

namespace MTFO.HotReload
{
    public class OfflineGear
    {
        private OfflineGear(PlayerOfflineGearDataBlock block)
        {
            if (TryParseGearJson(block.GearJSON, out GearIDRange)
            &&  TryParseGearID(GearIDRange, out ItemData, out inventorySlot))
            {
                GearIDRange.PlayfabItemId = block.name;
                GearIDRange.PlayfabItemInstanceId = $"OfflineGear_ID_{block.persistentID}";
                GearIDRange.OfflineGearType = block.Type;
                persistentID = block.persistentID;
            }
            else
            {
                Log.Warn($"Unable to construct Offline Gear [{block.persistentID}] {block.name}");
            }
        }

        private bool TryParseGearJson(string gearJson, out GearIDRange gearIDRange)
        {
            if (string.IsNullOrEmpty(gearJson))
            {
                gearIDRange = null;
                Log.Warn("Unable to assign GearIDRange due to null or empty GearJson");
                return false;
            }
            gearIDRange = new GearIDRange(gearJson);
            if (gearIDRange == null) return false;
            return true;
        }

        private bool TryParseGearID(GearIDRange gearIDRange, out ItemDataBlock itemData, out int inventorySlot)
        {
            inventorySlot = 0;
            itemData = null;

            if (gearIDRange == null)
            {
                Log.Warn("Unable to parse GearIDRange due to it being null");
                return false;
            }

            uint compID = gearIDRange.GetCompID(eGearComponent.BaseItem);
            itemData = compID > 0U
                         ? GameDataBlockBase<ItemDataBlock>.GetBlock(compID)
                         : null;

            if (itemData == null)
            {
                Log.Warn($"Invalid ItemDataBlock for component in offlinear gear [c:{compID}]");
                return false;
            }

            inventorySlot = (int)itemData.inventorySlot;
            return true;
        }

        public static void Load(PlayerOfflineGearDataBlock block)
        {
            if (TryParse(block, out var gear))
            {
                if (TryStash(gear))
                {
                    Log.Verbose($"Loaded offline gear [{block.persistentID}] {block.name}");
                }
                else
                {
                    Log.Warn($"Unable to stash offline gear [{block.persistentID}] {block.name}");
                }
            }
            else
            {
                Log.Warn($"Unable to parse offline gear [{block.persistentID}] {block.name}");
            }
        }

        private static bool TryParse(PlayerOfflineGearDataBlock block, out OfflineGear result)
        {
            switch (block.Type)
            {
                case eOfflineGearType.StandardInventory:
                case eOfflineGearType.RundownSpecificInventory:
                    result = new OfflineGear(block);
                    return true;
                default:
                    result = null;
                    Log.Warn($"Unimplemented Offline Gear Type [{block.persistentID}] {block.Type}");
                    return false;
            }
        }

        private static bool TryStash(OfflineGear gear)
        {
            if (gear == null)
            {
                Log.Warn("Unable to stash due to null offline gear");
                return false;
            }
            if (gear.GearIDRange == null)
            {
                Log.Warn($"Unable to stash offline gear due to null GearIDRange [{gear.persistentID}]");
                return false;
            }
            if (gear.ItemData == null)
            {
                Log.Warn($"Unable to stash offline gear due to null ItemDataBlock [{gear.persistentID}]");
                return false;
            }
            GearManager.Current.m_gearPerSlot[gear.inventorySlot].Add(gear.GearIDRange);
            return true;
        }

        private GearIDRange GearIDRange;
        private ItemDataBlock ItemData;
        private int inventorySlot;
        private uint persistentID;
    }
}
