using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameData;
using Localization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace GTFO.DevTools
{
    public static class GTFOGameConfig
    {
        public static GTFORundownInfo Rundown { get; } = new GTFORundownInfo();
    }

    public class GTFORundownInfo
    {
        private string m_folder;
        private bool m_valid;

        public string Folder
        {
            get => this.m_folder;
            set
            {
                this.m_valid = false;
                this.m_folder = value;
            }
        }
        public bool Valid => this.m_valid;
        public GTFORundownDataBlocks DataBlocks { get; } = new GTFORundownDataBlocks();
        public bool ValidAndLoaded => this.Valid && this.DataBlocks.GameSetup.Loaded;

        public GTFODataBlockLoadable[] GetLoadableDataBlocks()
        {
            List<GTFODataBlockLoadable> datablocks = new List<GTFODataBlockLoadable>();

            var type = typeof(GTFORundownDataBlocks);
            var properties = type.GetProperties();
            int propertiesCount = properties.Length;
            for (int index = 0; index < propertiesCount; index++)
            {
                var property = properties[index];
                var value = property.GetValue(this.DataBlocks);
                if (value is GTFODataBlockLoadable datablock)
                {
                    datablocks.Add(datablock);
                }
            }

            return datablocks.ToArray();
        }

        public void Validate()
        {
            if (this.m_valid) return;

            bool valid = true;
            foreach (var value in this.GetLoadableDataBlocks())
            {
                if (!value.ExistsAtPath(this.Folder))
                {
                    Debug.LogError($"Failed to find data block '{value.BlockName}'\nat path <color=gray>{this.Folder}</color>");
                    valid = false;
                }
            }

            this.m_valid = valid;
        }
        
        public void LoadBlocks()
        {
            var blocks = this.GetLoadableDataBlocks();

            int blockCount = blocks.Length;
            for (int index = 0; index < blockCount; index++)
            {
                var block = blocks[index];
                EditorUtility.DisplayProgressBar("Load Data Blocks", block.BlockName, (index + 1f) / blockCount);
                try
                {
                    block.LoadAtPath(this.Folder);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to load data block '" + block.BlockName + "'!");
                    Debug.LogException(ex);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        public void SaveBlocks()
        {
            var blocks = this.GetLoadableDataBlocks();

            int blockCount = blocks.Length;
            for (int index = 0; index < blockCount; index++)
            {
                var block = blocks[index];
                EditorUtility.DisplayProgressBar("Save Data Blocks", block.BlockName, (index + 1f) / blockCount);
                try
                {
                    block.SaveAtPath(this.Folder);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to save data block '" + block.BlockName + "'!");
                    Debug.LogException(ex);
                }
            }
            EditorUtility.ClearProgressBar();
        }
    }

    public class GTFORundownDataBlocks
    {
        #region Cope
        public GTFODataBlockInfo<ItemPartDataBlock> ItemPart { get; } = new GTFODataBlockInfo<ItemPartDataBlock>();
        public GTFODataBlockInfo<ItemDataBlock> Item { get; } = new GTFODataBlockInfo<ItemDataBlock>();
        public GTFODataBlockInfo<WeaponDataBlock> Weapon { get; } = new GTFODataBlockInfo<WeaponDataBlock>();
        public GTFODataBlockInfo<PlayerDataBlock> Player { get; } = new GTFODataBlockInfo<PlayerDataBlock>();
        public GTFODataBlockInfo<FeedbackDataBlock> Feedback { get; } = new GTFODataBlockInfo<FeedbackDataBlock>();
        public GTFODataBlockInfo<EnvironmentFeedbackDataBlock> EnvironmentFeedback { get; } = new GTFODataBlockInfo<EnvironmentFeedbackDataBlock>();
        public GTFODataBlockInfo<EnemyDataBlock> Enemy { get; } = new GTFODataBlockInfo<EnemyDataBlock>();
        public GTFODataBlockInfo<EnemyDetectionDataBlock> EnemyDetection { get; } = new GTFODataBlockInfo<EnemyDetectionDataBlock>();
        public GTFODataBlockInfo<EnemyBehaviorDataBlock> EnemyBehavior { get; } = new GTFODataBlockInfo<EnemyBehaviorDataBlock>();
        public GTFODataBlockInfo<EnemyMovementDataBlock> EnemyMovement { get; } = new GTFODataBlockInfo<EnemyMovementDataBlock>();
        public GTFODataBlockInfo<EnemyBalancingDataBlock> EnemyBalancing { get; } = new GTFODataBlockInfo<EnemyBalancingDataBlock>();
        public GTFODataBlockInfo<EnemySFXDataBlock> EnemySFX { get; } = new GTFODataBlockInfo<EnemySFXDataBlock>();
        public GTFODataBlockInfo<SurvivalWavePopulationDataBlock> SurvivalWavePopulation { get; } = new GTFODataBlockInfo<SurvivalWavePopulationDataBlock>();
        public GTFODataBlockInfo<EnemyGroupDataBlock> EnemyGroup { get; } = new GTFODataBlockInfo<EnemyGroupDataBlock>();
        public GTFODataBlockInfo<PlayerDialogDataBlock> PlayerDialog { get; } = new GTFODataBlockInfo<PlayerDialogDataBlock>();
        public GTFODataBlockInfo<GearDataBlock> Gear { get; } = new GTFODataBlockInfo<GearDataBlock>();
        public GTFODataBlockInfo<ArchetypeDataBlock> Archetype { get; } = new GTFODataBlockInfo<ArchetypeDataBlock>();
        public GTFODataBlockInfo<RecoilDataBlock> Recoil { get; } = new GTFODataBlockInfo<RecoilDataBlock>();
        public GTFODataBlockInfo<ItemMovementAnimationDataBlock> ItemMovementAnimation { get; } = new GTFODataBlockInfo<ItemMovementAnimationDataBlock>();
        public GTFODataBlockInfo<MusicStateDataBlock> MusicState { get; } = new GTFODataBlockInfo<MusicStateDataBlock>();
        public GTFODataBlockInfo<LightSettingsDataBlock> LightSettings { get; } = new GTFODataBlockInfo<LightSettingsDataBlock>();
        public GTFODataBlockInfo<FogSettingsDataBlock> FogSettings { get; } = new GTFODataBlockInfo<FogSettingsDataBlock>();
        public GTFODataBlockInfo<WardenObjectiveDataBlock> WardenObjective { get; } = new GTFODataBlockInfo<WardenObjectiveDataBlock>();
        public GTFODataBlockInfo<LevelGenSettingsDataBlock> LevelGenSettings { get; } = new GTFODataBlockInfo<LevelGenSettingsDataBlock>();
        public GTFODataBlockInfo<StaticSpawnDataBlock> StaticSpawn { get; } = new GTFODataBlockInfo<StaticSpawnDataBlock>();
        public GTFODataBlockInfo<ExpeditionBalanceDataBlock> ExpeditionBalance { get; } = new GTFODataBlockInfo<ExpeditionBalanceDataBlock>();
        public GTFODataBlockInfo<MiningMarkerDataBlock> MiningMarker { get; } = new GTFODataBlockInfo<MiningMarkerDataBlock>();
        public GTFODataBlockInfo<ServiceMarkerDataBlock> ServiceMarker { get; } = new GTFODataBlockInfo<ServiceMarkerDataBlock>();
        public GTFODataBlockInfo<TechMarkerDataBlock> TechMarker { get; } = new GTFODataBlockInfo<TechMarkerDataBlock>();
        public GTFODataBlockInfo<MarkerGroupDataBlock> MarkerGroup { get; } = new GTFODataBlockInfo<MarkerGroupDataBlock>();
        public GTFODataBlockInfo<LootDataBlock> Loot { get; } = new GTFODataBlockInfo<LootDataBlock>();
        public GTFODataBlockInfo<ComplexResourceSetDataBlock> ComplexResourceSet { get; } = new GTFODataBlockInfo<ComplexResourceSetDataBlock>();
        public GTFODataBlockInfo<RundownDataBlock> Rundown { get; } = new GTFODataBlockInfo<RundownDataBlock>();
        public GTFODataBlockInfo<LevelLayoutDataBlock> LevelLayout { get; } = new GTFODataBlockInfo<LevelLayoutDataBlock>();
        public GTFODataBlockInfo<ConsumableDistributionDataBlock> ConsumableDistribution { get; } = new GTFODataBlockInfo<ConsumableDistributionDataBlock>();
        public GTFODataBlockInfo<BigPickupDistributionDataBlock> BigPickupDistribution { get; } = new GTFODataBlockInfo<BigPickupDistributionDataBlock>();
        public GTFODataBlockInfo<WeaponAudioDataBlock> WeaponAudio { get; } = new GTFODataBlockInfo<WeaponAudioDataBlock>();
        public GTFODataBlockInfo<WeaponMuzzleFlashDataBlock> WeaponMuzzleFlash { get; } = new GTFODataBlockInfo<WeaponMuzzleFlashDataBlock>();
        public GTFODataBlockInfo<WeaponShellCasingDataBlock> WeaponShellCasing { get; } = new GTFODataBlockInfo<WeaponShellCasingDataBlock>();
        public GTFODataBlockInfo<ItemFPSSettingsDataBlock> ItemFPSSettings { get; } = new GTFODataBlockInfo<ItemFPSSettingsDataBlock>();
        public GTFODataBlockInfo<GearCategoryDataBlock> GearCategory { get; } = new GTFODataBlockInfo<GearCategoryDataBlock>();
        public GTFODataBlockInfo<GearCategoryFilterDataBlock> GearCategoryFilter { get; } = new GTFODataBlockInfo<GearCategoryFilterDataBlock>();
        public GTFODataBlockInfo<GearFrontPartDataBlock> GearFrontPart { get; } = new GTFODataBlockInfo<GearFrontPartDataBlock>();
        public GTFODataBlockInfo<GearReceiverPartDataBlock> GearReceiverPart { get; } = new GTFODataBlockInfo<GearReceiverPartDataBlock>();
        public GTFODataBlockInfo<GearStockPartDataBlock> GearStockPart { get; } = new GTFODataBlockInfo<GearStockPartDataBlock>();
        public GTFODataBlockInfo<GearSightPartDataBlock> GearSightPart { get; } = new GTFODataBlockInfo<GearSightPartDataBlock>();
        public GTFODataBlockInfo<GearMagPartDataBlock> GearMagPart { get; } = new GTFODataBlockInfo<GearMagPartDataBlock>();
        public GTFODataBlockInfo<GearFlashlightPartDataBlock> GearFlashlightPart { get; } = new GTFODataBlockInfo<GearFlashlightPartDataBlock>();
        public GTFODataBlockInfo<GearPartAttachmentDataBlock> GearPartAttachment { get; } = new GTFODataBlockInfo<GearPartAttachmentDataBlock>();
        public GTFODataBlockInfo<GearPerkDataBlock> GearPerk { get; } = new GTFODataBlockInfo<GearPerkDataBlock>();
        public GTFODataBlockInfo<GearPatternDataBlock> GearPattern { get; } = new GTFODataBlockInfo<GearPatternDataBlock>();
        public GTFODataBlockInfo<GearPaletteDataBlock> GearPalette { get; } = new GTFODataBlockInfo<GearPaletteDataBlock>();
        public GTFODataBlockInfo<GearDecalDataBlock> GearDecal { get; } = new GTFODataBlockInfo<GearDecalDataBlock>();
        public GTFODataBlockInfo<PlayerOfflineGearDataBlock> PlayerOfflineGear { get; } = new GTFODataBlockInfo<PlayerOfflineGearDataBlock>();
        public GTFODataBlockInfo<ExtractionEventDataBlock> ExtractionEvent { get; } = new GTFODataBlockInfo<ExtractionEventDataBlock>();
        public GTFODataBlockInfo<ChainedPuzzleDataBlock> ChainedPuzzle { get; } = new GTFODataBlockInfo<ChainedPuzzleDataBlock>();
        public GTFODataBlockInfo<ChainedPuzzleTypeDataBlock> ChainedPuzzleType { get; } = new GTFODataBlockInfo<ChainedPuzzleTypeDataBlock>();
        public GTFODataBlockInfo<EventSequenceActionDataBlock> EventSequenceAction { get; } = new GTFODataBlockInfo<EventSequenceActionDataBlock>();
        public GTFODataBlockInfo<EventSequenceDataBlock> EventSequence { get; } = new GTFODataBlockInfo<EventSequenceDataBlock>();
        public GTFODataBlockInfo<EnemyPopulationDataBlock> EnemyPopulation { get; } = new GTFODataBlockInfo<EnemyPopulationDataBlock>();
        public GTFODataBlockInfo<GearMeleeHeadPartDataBlock> GearMeleeHeadPart { get; } = new GTFODataBlockInfo<GearMeleeHeadPartDataBlock>();
        public GTFODataBlockInfo<GearMeleeNeckPartDataBlock> GearMeleeNeckPart { get; } = new GTFODataBlockInfo<GearMeleeNeckPartDataBlock>();
        public GTFODataBlockInfo<GearMeleeHandlePartDataBlock> GearMeleeHandlePart { get; } = new GTFODataBlockInfo<GearMeleeHandlePartDataBlock>();
        public GTFODataBlockInfo<GearMeleePommelPartDataBlock> GearMeleePommelPart { get; } = new GTFODataBlockInfo<GearMeleePommelPartDataBlock>();
        public GTFODataBlockInfo<GearToolMainPartDataBlock> GearToolMainPart { get; } = new GTFODataBlockInfo<GearToolMainPartDataBlock>();
        public GTFODataBlockInfo<GearToolGripPartDataBlock> GearToolGripPart { get; } = new GTFODataBlockInfo<GearToolGripPartDataBlock>();
        public GTFODataBlockInfo<GearToolDeliveryPartDataBlock> GearToolDeliveryPart { get; } = new GTFODataBlockInfo<GearToolDeliveryPartDataBlock>();
        public GTFODataBlockInfo<GearToolPayloadPartDataBlock> GearToolPayloadPart { get; } = new GTFODataBlockInfo<GearToolPayloadPartDataBlock>();
        public GTFODataBlockInfo<GearToolTargetingPartDataBlock> GearToolTargetingPart { get; } = new GTFODataBlockInfo<GearToolTargetingPartDataBlock>();
        public GTFODataBlockInfo<GearToolScreenPartDataBlock> GearToolScreenPart { get; } = new GTFODataBlockInfo<GearToolScreenPartDataBlock>();
        public GTFODataBlockInfo<FlashlightSettingsDataBlock> FlashlightSettings { get; } = new GTFODataBlockInfo<FlashlightSettingsDataBlock>();
        public GTFODataBlockInfo<CustomAssetShardDataBlock> CustomAssetShard { get; } = new GTFODataBlockInfo<CustomAssetShardDataBlock>();
        public GTFODataBlockInfo<CommodityDataBlock> Commodity { get; } = new GTFODataBlockInfo<CommodityDataBlock>();
        public GTFODataBlockInfo<SurvivalWaveSettingsDataBlock> SurvivalWaveSettings { get; } = new GTFODataBlockInfo<SurvivalWaveSettingsDataBlock>();
        public GTFODataBlockInfo<GameSetupDataBlock> GameSetup { get; } = new GTFODataBlockInfo<GameSetupDataBlock>();
        public GTFODataBlockInfo<ArtifactDistributionDataBlock> ArtifactDistribution { get; } = new GTFODataBlockInfo<ArtifactDistributionDataBlock>();
        public GTFODataBlockInfo<ArtifactDataBlock> Artifact { get; } = new GTFODataBlockInfo<ArtifactDataBlock>();
        public GTFODataBlockInfo<ArtifactTagDataBlock> ArtifactTag { get; } = new GTFODataBlockInfo<ArtifactTagDataBlock>();
        public GTFODataBlockInfo<BoosterImplantEffectDataBlock> BoosterImplantEffect { get; } = new GTFODataBlockInfo<BoosterImplantEffectDataBlock>();
        public GTFODataBlockInfo<BoosterImplantConditionDataBlock> BoosterImplantCondition { get; } = new GTFODataBlockInfo<BoosterImplantConditionDataBlock>();
        public GTFODataBlockInfo<BoosterImplantTemplateDataBlock> BoosterImplantTemplate { get; } = new GTFODataBlockInfo<BoosterImplantTemplateDataBlock>();
        public GTFODataBlockInfo<TextDataBlock> Text { get; } = new GTFODataBlockInfo<TextDataBlock>();
        public GTFODataBlockInfo<TextCharacterMetaDataBlock> TextCharacterMeta { get; } = new GTFODataBlockInfo<TextCharacterMetaDataBlock>();
        public GTFODataBlockInfo<DimensionDataBlock> Dimension { get; } = new GTFODataBlockInfo<DimensionDataBlock>();
        public GTFODataBlockInfo<AtmosphereDataBlock> Atmosphere { get; } = new GTFODataBlockInfo<AtmosphereDataBlock>();
        public GTFODataBlockInfo<CloudsDataBlock> Clouds { get; } = new GTFODataBlockInfo<CloudsDataBlock>();
        public GTFODataBlockInfo<MeleeArchetypeDataBlock> MeleeArchetype { get; } = new GTFODataBlockInfo<MeleeArchetypeDataBlock>();
        public GTFODataBlockInfo<MeleeAnimationSetDataBlock> MeleeAnimationSet { get; } = new GTFODataBlockInfo<MeleeAnimationSetDataBlock>();
        public GTFODataBlockInfo<MeleeSFXDataBlock> MeleeSFX { get; } = new GTFODataBlockInfo<MeleeSFXDataBlock>();
        public GTFODataBlockInfo<VanityItemsTemplateDataBlock> VanityItemsTemplate { get; } = new GTFODataBlockInfo<VanityItemsTemplateDataBlock>();
        public GTFODataBlockInfo<VanityItemsGroupDataBlock> VanityItemsGroup { get; } = new GTFODataBlockInfo<VanityItemsGroupDataBlock>();
        #endregion

        public GTFODataBlockInfo<T> GetBlock<T>()
            where T : GameDataBlockBase<T>
        {
            return (GTFODataBlockInfo<T>)this.GetBlock(typeof(T));
        }
        public GTFODataBlockLoadable GetBlock(Type type)
        {
            string typeName = type.Name.Replace("DataBlock", "");
            var prop = this.GetType().GetProperty(typeName);
            if (prop != null)
            {
                return (GTFODataBlockLoadable)prop.GetValue(this);
            }
            return null;
        }

        public RundownDataBlock GetRundown()
        {
            if (!this.GameSetup.Loaded || !this.Rundown.Loaded)
                return null;

            var block = this.Rundown.GetBlockByID(this.GameSetup.GetBlockByID(1U)?.RundownIdToLoad ?? 0U);
            return block;
        }
    }

    public abstract class GTFODataBlockLoadable
    {
        private bool m_loaded;
        public bool Loaded => this.m_loaded;
        public abstract string BlockName { get; }
        

        protected abstract void DoLoadAtPath(string path);
        protected abstract void DoSaveAtPath(string path);
        protected abstract void DoUnload();

        public abstract BasicBlockInfo[] GetBasicBlocks();

        public string GetBlockPath(string folderPath)
        {
            return Path.Combine(folderPath, "GameData_" + this.BlockName + "_bin.json");
        }

        public bool ExistsAtPath(string folderPath)
        {
            return File.Exists(this.GetBlockPath(folderPath));
        }

        public string GetBlockContentsAtPath(string folderPath)
        {
            return File.ReadAllText(this.GetBlockPath(folderPath));
        }

        public void WriteBlockContentsAtPath(string folderPath, string contents)
        {
            File.WriteAllText(this.GetBlockPath(folderPath), contents);
        }

        public void Unload()
        {
            this.m_loaded = false;
            this.DoUnload();
        }


        public void SaveAtPath(string path)
        {
            if (!this.m_loaded) return;

            this.DoSaveAtPath(path);
        }


        public void LoadAtPath(string path)
        {
            if (this.m_loaded) this.Unload();

            this.DoLoadAtPath(path);
            this.m_loaded = true;
        }
    }

    public struct BasicBlockInfo
    {
        public readonly string blockTypeName;
        public readonly string name;
        public readonly uint persistentID;
        public readonly bool internalEnabled;

        public BasicBlockInfo(string blockTypeName, string name, uint persistentID, bool internalEnabled)
        {
            this.name = name;
            this.persistentID = persistentID;
            this.internalEnabled = internalEnabled;
            this.blockTypeName = blockTypeName;
        }

        public static BasicBlockInfo From<TBlock>(TBlock block)
            where TBlock : GameDataBlockBase<TBlock>
        {
            return new BasicBlockInfo(typeof(TBlock).Name, block.name, block.persistentID, block.internalEnabled);
        }
    }

    public class GTFODataBlockInfo<TBlock> : GTFODataBlockLoadable
        where TBlock : GameDataBlockBase<TBlock>
    {
        private GameDataBlockWrapper<TBlock> m_wrapper;
        private readonly Dictionary<uint, TBlock> m_blocksByID;
        private readonly Dictionary<string, uint> m_blockIDByName;

        public GTFODataBlockInfo()
        {
            this.m_blocksByID = new Dictionary<uint, TBlock>();
            this.m_blockIDByName = new Dictionary<string, uint>();
        }

        public override string BlockName => typeof(TBlock).Name;

        public TBlock[] GetBlocks() => this.m_blocksByID.Values.OrderBy((b) => b.persistentID).ToArray();
        public override BasicBlockInfo[] GetBasicBlocks()
            => this.GetBlocks().Select((block) => BasicBlockInfo.From<TBlock>(block)).ToArray();

        public GameDataBlockWrapper<TBlock> Data => this.Loaded ? this.m_wrapper : null;
        public TBlock GetBlockByID(uint id) => this.m_blocksByID.TryGetValue(id, out TBlock block) ? block : default;
        public TBlock GetBlockByName(string name) => this.m_blockIDByName.TryGetValue(name, out uint id) ? this.GetBlockByID(id) : default;

        protected override void DoUnload()
        {
            this.m_wrapper = null;
            this.m_blocksByID.Clear();
            this.m_blockIDByName.Clear();
        }

        protected override void DoSaveAtPath(string path)
        {
            this.WriteBlockContentsAtPath(path, JsonConvert.SerializeObject(this.Data, CellJSON.JSONSettings_GameData));
        }

        protected override void DoLoadAtPath(string path)
        {
            this.m_wrapper = JsonConvert.DeserializeObject<GameDataBlockWrapper<TBlock>>(this.GetBlockContentsAtPath(path), CellJSON.JSONSettings_GameData);

            this.VerifyLatestPersistentID();

            var data = this.m_wrapper;
            for (int index = 0; index < data.Blocks.Count; index++)
            {
                var block = data.Blocks[index];
                if (block.persistentID <= 0)
                {
                    Debug.Log($"[DataBlock <color=cyan>{this.BlockName}</color>] Updating data block (name: {block.name}, id: {block.persistentID}) to latest persistent ID: {data.LastPersistentID + 1}.");
                    
                    data.LastPersistentID++;
                    block.persistentID = data.LastPersistentID;
                }

                if (this.m_blocksByID.ContainsKey(block.persistentID))
                {
                    Debug.LogWarning($"<color=white>[DataBlock <color=cyan>{this.BlockName}</color>]</color> Tried to add block (name: {block.name}, id: {block.persistentID}), but that persistentID already exists. Updating to { data.LastPersistentID + 1}");
                    data.LastPersistentID++;
                    block.persistentID = data.LastPersistentID;
                }

                if (block.name == null)
                {
                    Debug.LogWarning($"<color=white>[DataBlock <color=cyan>{this.BlockName}</color>]</color> Block (id: {block.persistentID}), Has a null name. Setting to 'data block'");
                    block.name = "data block";
                }

                if (this.m_blockIDByName.ContainsKey(block.name))
                {
                    int increment = 0;
                    string nameBase = block.name + "_";
                    while (this.m_blockIDByName.ContainsKey(nameBase + increment))
                    {
                        increment++;
                    }

                    string newName = nameBase + increment;

                    Debug.LogWarning($"<color=white>[DataBlock <color=cyan>{this.BlockName}</color>]</color> Tried to add block (name: {block.name}, id: {block.persistentID}), but that name already exists. Changing name to <color=orange>{newName}</color>.");

                    block.name = newName;
                }

                this.m_blockIDByName.Add(block.name, block.persistentID);
                this.m_blocksByID.Add(block.persistentID, block);
            }
        }

        private bool VerifyLatestPersistentID()
        {
            uint newestPersistentID = 0U;
            var wrapper = this.m_wrapper;
            for (int i = 0; i < wrapper.Blocks.Count; i++)
            {
                newestPersistentID = Math.Max(wrapper.Blocks[i].persistentID, newestPersistentID);
            }

            if (newestPersistentID > wrapper.LastPersistentID)
            {
                Debug.Log($"<color=white>[DataBlock <color=cyan>{this.BlockName}</color>]</color> Found datablock with newer persistent ID, updating latest persistent id to: " + newestPersistentID);
                wrapper.LastPersistentID = newestPersistentID;
                return true;
            }

            return false;
        }
    }
}
