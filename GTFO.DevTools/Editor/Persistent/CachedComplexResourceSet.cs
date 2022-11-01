using Expedition;
using GameData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTFO.DevTools.Persistent
{
    public class CachedComplexResourceSet : PersistentObject<CachedComplexResourceSet>
    {
        public CachedResourceData GeomorphTiles_1x1 = new CachedResourceData();

        public CachedResourceData GeomorphTiles_2x1 = new CachedResourceData();

        public CachedResourceData GeomorphTiles_2x2 = new CachedResourceData();

        public CachedResourceData SmallWeakGates = new CachedResourceData();

        public CachedResourceData SmallSecurityGates = new CachedResourceData();

        public CachedResourceData SmallApexGates = new CachedResourceData();

        public CachedResourceData SmallBulkheadGates = new CachedResourceData();

        public CachedResourceData SmallMainPathBulkheadGates = new CachedResourceData();

        public CachedResourceData SmallWallCaps = new CachedResourceData();

        public CachedResourceData SmallDestroyedCaps = new CachedResourceData();

        public CachedResourceData SmallWallAndDestroyedCaps = new CachedResourceData();

        public CachedResourceData MediumWeakGates = new CachedResourceData();

        public CachedResourceData MediumSecurityGates = new CachedResourceData();

        public CachedResourceData MediumApexGates = new CachedResourceData();

        public CachedResourceData MediumBulkheadGates = new CachedResourceData();

        public CachedResourceData MediumMainPathBulkheadGates = new CachedResourceData();

        public CachedResourceData MediumWallCaps = new CachedResourceData();

        public CachedResourceData MediumDestroyedCaps = new CachedResourceData();

        public CachedResourceData MediumWallAndDestroyedCaps = new CachedResourceData();

        public CachedResourceData LargeWeakGates = new CachedResourceData();

        public CachedResourceData LargeSecurityGates = new CachedResourceData();

        public CachedResourceData LargeApexGates = new CachedResourceData();

        public CachedResourceData LargeBulkheadGates = new CachedResourceData();

        public CachedResourceData LargeMainPathBulkheadGates = new CachedResourceData();

        public CachedResourceData LargeWallCaps = new CachedResourceData();

        public CachedResourceData LargeDestroyedCaps = new CachedResourceData();

        public CachedResourceData LargeWallAndDestroyedCaps = new CachedResourceData();

        public CachedResourceData StraightPlugsNoGates = new CachedResourceData();

        public CachedResourceData StraightPlugsWithGates = new CachedResourceData();

        public CachedResourceData SingleDropPlugsNoGates = new CachedResourceData();

        public CachedResourceData SingleDropPlugsWithGates = new CachedResourceData();

        public CachedResourceData DoubleDropPlugsNoGates = new CachedResourceData();

        public CachedResourceData DoubleDropPlugsWithGates = new CachedResourceData();

        public CachedResourceData PlugCaps = new CachedResourceData();

        public CachedResourceData ElevatorShafts_1x1 = new CachedResourceData();

        public CachedResourceData CustomGeomorphs_Exit_1x1 = new CachedResourceData();

        public CachedResourceData CustomGeomorphs_Objective_1x1 = new CachedResourceData();

        public CachedResourceData CustomGeomorphs_Challenge_1x1 = new CachedResourceData();

        public CachedResourceData Ladders_4m = new CachedResourceData();

        public CachedResourceData Ladders_2m = new CachedResourceData();

        public CachedResourceData Ladders_1m = new CachedResourceData();

        public CachedResourceData Ladders_05m = new CachedResourceData();

        public CachedResourceData Ladders_Bottom = new CachedResourceData();

        public CachedResourceData Ladders_Top = new CachedResourceData();

        protected override void OnFirstCreate()
        {
            this.Recache();
        }

        public void Recache()
        {
            this.CustomGeomorphs_Challenge_1x1.Clear();
            this.CustomGeomorphs_Exit_1x1.Clear();
            this.CustomGeomorphs_Objective_1x1.Clear();
            this.DoubleDropPlugsNoGates.Clear();
            this.DoubleDropPlugsWithGates.Clear();
            this.ElevatorShafts_1x1.Clear();
            this.GeomorphTiles_1x1.Clear();
            this.GeomorphTiles_2x1.Clear();
            this.GeomorphTiles_2x2.Clear();
            this.Ladders_05m.Clear();
            this.Ladders_1m.Clear();
            this.Ladders_2m.Clear();
            this.Ladders_4m.Clear();
            this.Ladders_Bottom.Clear();
            this.Ladders_Top.Clear();
            this.LargeApexGates.Clear();
            this.LargeBulkheadGates.Clear();
            this.LargeDestroyedCaps.Clear();
            this.LargeMainPathBulkheadGates.Clear();
            this.LargeSecurityGates.Clear();
            this.LargeWallAndDestroyedCaps.Clear();
            this.LargeWallCaps.Clear();
            this.LargeWeakGates.Clear();
            this.MediumApexGates.Clear();
            this.MediumBulkheadGates.Clear();
            this.MediumDestroyedCaps.Clear();
            this.MediumMainPathBulkheadGates.Clear();
            this.MediumSecurityGates.Clear();
            this.MediumWallAndDestroyedCaps.Clear();
            this.MediumWallCaps.Clear();
            this.MediumWeakGates.Clear();
            this.PlugCaps.Clear();
            this.SingleDropPlugsNoGates.Clear();
            this.SingleDropPlugsWithGates.Clear();
            this.SmallApexGates.Clear();
            this.SmallBulkheadGates.Clear();
            this.SmallDestroyedCaps.Clear();
            this.SmallMainPathBulkheadGates.Clear();
            this.SmallSecurityGates.Clear();
            this.SmallWallAndDestroyedCaps.Clear();
            this.SmallWallCaps.Clear();
            this.SmallWeakGates.Clear();
            this.StraightPlugsNoGates.Clear();
            this.StraightPlugsWithGates.Clear();

            this.Rebase();
        }

        public void Rebase()
        {
            if (GTFOGameConfig.Rundown.DataBlocks?.ComplexResourceSet == null)
                return;

            foreach (var complexResourceSet in GTFOGameConfig.Rundown.DataBlocks.ComplexResourceSet.GetBlocks())
            {
                this.CacheComplexResourceSet(complexResourceSet);
            }
        }

        public void CacheComplexResourceSet(ComplexResourceSetDataBlock datablock)
        {
            this.CustomGeomorphs_Challenge_1x1.Cache(datablock.CustomGeomorphs_Challenge_1x1, datablock.ComplexType);
            this.CustomGeomorphs_Exit_1x1.Cache(datablock.CustomGeomorphs_Exit_1x1, datablock.ComplexType);
            this.CustomGeomorphs_Objective_1x1.Cache(datablock.CustomGeomorphs_Objective_1x1, datablock.ComplexType);
            this.DoubleDropPlugsNoGates.Cache(datablock.DoubleDropPlugsNoGates, datablock.ComplexType);
            this.DoubleDropPlugsWithGates.Cache(datablock.DoubleDropPlugsWithGates, datablock.ComplexType);
            this.ElevatorShafts_1x1.Cache(datablock.ElevatorShafts_1x1, datablock.ComplexType);
            this.GeomorphTiles_1x1.Cache(datablock.GeomorphTiles_1x1, datablock.ComplexType);
            this.GeomorphTiles_2x1.Cache(datablock.GeomorphTiles_2x1, datablock.ComplexType);
            this.GeomorphTiles_2x2.Cache(datablock.GeomorphTiles_2x2, datablock.ComplexType);
            this.Ladders_05m.Cache(datablock.Ladders_05m, datablock.ComplexType);
            this.Ladders_1m.Cache(datablock.Ladders_1m, datablock.ComplexType);
            this.Ladders_2m.Cache(datablock.Ladders_2m, datablock.ComplexType);
            this.Ladders_4m.Cache(datablock.Ladders_4m, datablock.ComplexType);
            this.Ladders_Bottom.Cache(datablock.Ladders_Bottom, datablock.ComplexType);
            this.Ladders_Top.Cache(datablock.Ladders_Top, datablock.ComplexType);
            this.LargeApexGates.Cache(datablock.LargeApexGates, datablock.ComplexType);
            this.LargeBulkheadGates.Cache(datablock.LargeBulkheadGates, datablock.ComplexType);
            this.LargeDestroyedCaps.Cache(datablock.LargeDestroyedCaps, datablock.ComplexType);
            this.LargeMainPathBulkheadGates.Cache(datablock.LargeMainPathBulkheadGates, datablock.ComplexType);
            this.LargeSecurityGates.Cache(datablock.LargeSecurityGates, datablock.ComplexType);
            this.LargeWallAndDestroyedCaps.Cache(datablock.LargeWallAndDestroyedCaps, datablock.ComplexType);
            this.LargeWallCaps.Cache(datablock.LargeWallCaps, datablock.ComplexType);
            this.LargeWeakGates.Cache(datablock.LargeWeakGates, datablock.ComplexType);
            this.MediumApexGates.Cache(datablock.MediumApexGates, datablock.ComplexType);
            this.MediumBulkheadGates.Cache(datablock.MediumBulkheadGates, datablock.ComplexType);
            this.MediumDestroyedCaps.Cache(datablock.MediumDestroyedCaps, datablock.ComplexType);
            this.MediumMainPathBulkheadGates.Cache(datablock.MediumMainPathBulkheadGates, datablock.ComplexType);
            this.MediumSecurityGates.Cache(datablock.MediumSecurityGates, datablock.ComplexType);
            this.MediumWallAndDestroyedCaps.Cache(datablock.MediumWallAndDestroyedCaps, datablock.ComplexType);
            this.MediumWallCaps.Cache(datablock.MediumWallCaps, datablock.ComplexType);
            this.MediumWeakGates.Cache(datablock.MediumWeakGates, datablock.ComplexType);
            this.PlugCaps.Cache(datablock.PlugCaps, datablock.ComplexType);
            this.SingleDropPlugsNoGates.Cache(datablock.SingleDropPlugsNoGates, datablock.ComplexType);
            this.SingleDropPlugsWithGates.Cache(datablock.SingleDropPlugsWithGates, datablock.ComplexType);
            this.SmallApexGates.Cache(datablock.SmallApexGates, datablock.ComplexType);
            this.SmallBulkheadGates.Cache(datablock.SmallBulkheadGates, datablock.ComplexType);
            this.SmallDestroyedCaps.Cache(datablock.SmallDestroyedCaps, datablock.ComplexType);
            this.SmallMainPathBulkheadGates.Cache(datablock.SmallMainPathBulkheadGates, datablock.ComplexType);
            this.SmallSecurityGates.Cache(datablock.SmallSecurityGates, datablock.ComplexType);
            this.SmallWallAndDestroyedCaps.Cache(datablock.SmallWallAndDestroyedCaps, datablock.ComplexType);
            this.SmallWallCaps.Cache(datablock.SmallWallCaps, datablock.ComplexType);
            this.SmallWeakGates.Cache(datablock.SmallWeakGates, datablock.ComplexType);
            this.StraightPlugsNoGates.Cache(datablock.StraightPlugsNoGates, datablock.ComplexType);
            this.StraightPlugsWithGates.Cache(datablock.StraightPlugsWithGates, datablock.ComplexType);
        }

        [Serializable]
        public class CachedResourceData
        {
            [SerializeField]
            private List<CachedResourceDataItem> items = new List<CachedResourceDataItem>();

            public void Cache(List<ResourceData> data, Complex complex)
            {
                foreach (var item in data)
                {
                    if (item.SubComplex == SubComplex.All)
                    {
                        switch (complex)
                        {
                            case Complex.Mining:
                                this.Add(SubComplex.DigSite, item.Prefab);
                                this.Add(SubComplex.Storage, item.Prefab);
                                this.Add(SubComplex.Refinery, item.Prefab);
                                break;
                            case Complex.Service:
                                this.Add(SubComplex.Floodways, item.Prefab);
                                break;
                            case Complex.Tech:
                                this.Add(SubComplex.Lab, item.Prefab);
                                this.Add(SubComplex.DataCenter, item.Prefab);
                                break;
                        }
                    }
                    else
                    {
                        this.Add(item.SubComplex, item.Prefab);
                    }
                }
            }
            
            public void Add(SubComplex subcomplex, params string[] prefabs)
            {
                var item = this[subcomplex];
                if (item == null)
                {
                    item = new CachedResourceDataItem(subcomplex);
                    this.items.Add(item);
                }

                item.AddPrefabs(prefabs);
            }

            public bool Contains(SubComplex subcomplex)
            {
                for (int i = 0; i < this.items.Count; i++)
                {
                    if (this.items[i].SubComplex == subcomplex)
                        return true;
                }
                return false;
            }

            public bool Remove(SubComplex subcomplex)
            {
                for (int i = 0; i < this.items.Count; i++)
                {
                    if (this.items[i].SubComplex == subcomplex)
                    {
                        this.items.RemoveAt(i);
                        return true;
                    }    
                }
                return false;
            }
            public void Clear()
            {
                this.items.Clear();
            }
            
            public string[] GetAllPrefabs()
            {
                List<string> prefabs = new List<string>();
                foreach (var item in this.items)
                {
                    prefabs.AddRange(item.GetPrefabs());
                }
                return prefabs.ToArray();
            }

            public string[] GetPrefabs(SubComplex subcomplex)
            {
                var item = this[subcomplex];
                if (item == null)
                    return Array.Empty<string>();
                return item.GetPrefabs();
            }

            public CachedResourceDataItem this[SubComplex subcomplex]
            {
                get
                {
                    for (int i = 0; i < this.items.Count; i++)
                    {
                        if (this.items[i].SubComplex == subcomplex)
                            return this.items[i];
                    }
                    return null;
                }
            }

        }

        [Serializable]
        public class CachedResourceDataItem
        {
            [SerializeField]
            private SubComplex subcomplex;
            [SerializeField]
            private List<string> prefabs = new List<string>();

            public CachedResourceDataItem()
            { }
            public CachedResourceDataItem(SubComplex subComplex)
            {
                this.subcomplex = subComplex;
            }

            public SubComplex SubComplex => this.subcomplex;
            public void AddPrefab(string prefab)
            {
                if (!this.Contains(prefab))
                    this.prefabs.Add(prefab);
            }
            public void AddPrefabs(params string[] prefabs)
            {
                foreach (var prefab in prefabs)
                {
                    this.AddPrefab(prefab);
                }
            }

            public bool Contains(string prefab)
                => this.prefabs.Contains(prefab);

            public bool Remove(string prefab)
                => this.prefabs.Remove(prefab);

            public void Clear()
            {
                this.prefabs.Clear();
            }

            public string[] GetPrefabs() => this.prefabs.ToArray();
        }

        static CachedComplexResourceSet()
        {
            s_path = "Assets/GTFO.DevTools/CachedComplexResourceSet.asset";
            
        }
    }
}
