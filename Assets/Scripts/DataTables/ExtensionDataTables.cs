using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

#pragma warning disable 414
		//=============================================================
		//||                   Generated by BansheeGz Code Generator ||
		//=============================================================

		public partial class TBLTalisman : BGEntity
		{

			//=============================================================
			//||                   Generated by BansheeGz Code Generator ||
			//=============================================================

			public class Factory : BGEntity.EntityFactory
			{
				public BGEntity NewEntity(BGMetaEntity meta)
				{
					return new TBLTalisman(meta);
				}
				public BGEntity NewEntity(BGMetaEntity meta, BGId id)
				{
					return new TBLTalisman(meta, id);
				}
			}
			private static BansheeGz.BGDatabase.BGMetaRow _metaDefault;
			public static BansheeGz.BGDatabase.BGMetaRow MetaDefault
			{
				get
				{
					if(_metaDefault==null || _metaDefault.IsDeleted) _metaDefault=BGRepo.I.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(5316074384582091142,3441128812979716503));
					return _metaDefault;
				}
			}
			public static BansheeGz.BGDatabase.BGRepoEvents Events
			{
				get
				{
					return BGRepo.I.Events;
				}
			}
			private static readonly List<BGEntity> _find_Entities_Result = new List<BGEntity>();
			public static int CountEntities
			{
				get
				{
					return MetaDefault.CountEntities;
				}
			}
			public System.String name
			{
				get
				{
					return _name[Index];
				}
				set
				{
					_name[Index] = value;
				}
			}
			public System.Single normalSpeed
			{
				get
				{
					return _normalSpeed[Index];
				}
				set
				{
					_normalSpeed[Index] = value;
				}
			}
			public System.Single chargedSpeed
			{
				get
				{
					return _chargedSpeed[Index];
				}
				set
				{
					_chargedSpeed[Index] = value;
				}
			}
			public System.Single normalDamage
			{
				get
				{
					return _normalDamage[Index];
				}
				set
				{
					_normalDamage[Index] = value;
				}
			}
			public System.Single chargedDamage
			{
				get
				{
					return _chargedDamage[Index];
				}
				set
				{
					_chargedDamage[Index] = value;
				}
			}
			private static BansheeGz.BGDatabase.BGFieldEntityName _ufle12jhs77_name;
			public static BansheeGz.BGDatabase.BGFieldEntityName _name
			{
				get
				{
					if(_ufle12jhs77_name==null || _ufle12jhs77_name.IsDeleted) _ufle12jhs77_name=(BansheeGz.BGDatabase.BGFieldEntityName) MetaDefault.GetField(new BGId(4621556217480020872,14253139749336950442));
					return _ufle12jhs77_name;
				}
			}
			private static BansheeGz.BGDatabase.BGFieldFloat _ufle12jhs77_normalSpeed;
			public static BansheeGz.BGDatabase.BGFieldFloat _normalSpeed
			{
				get
				{
					if(_ufle12jhs77_normalSpeed==null || _ufle12jhs77_normalSpeed.IsDeleted) _ufle12jhs77_normalSpeed=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(4882841501558246989,4917772421278098824));
					return _ufle12jhs77_normalSpeed;
				}
			}
			private static BansheeGz.BGDatabase.BGFieldFloat _ufle12jhs77_chargedSpeed;
			public static BansheeGz.BGDatabase.BGFieldFloat _chargedSpeed
			{
				get
				{
					if(_ufle12jhs77_chargedSpeed==null || _ufle12jhs77_chargedSpeed.IsDeleted) _ufle12jhs77_chargedSpeed=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(4631373867462870299,10743961546380310170));
					return _ufle12jhs77_chargedSpeed;
				}
			}
			private static BansheeGz.BGDatabase.BGFieldFloat _ufle12jhs77_normalDamage;
			public static BansheeGz.BGDatabase.BGFieldFloat _normalDamage
			{
				get
				{
					if(_ufle12jhs77_normalDamage==null || _ufle12jhs77_normalDamage.IsDeleted) _ufle12jhs77_normalDamage=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(4783143083141051539,9551043223263311019));
					return _ufle12jhs77_normalDamage;
				}
			}
			private static BansheeGz.BGDatabase.BGFieldFloat _ufle12jhs77_chargedDamage;
			public static BansheeGz.BGDatabase.BGFieldFloat _chargedDamage
			{
				get
				{
					if(_ufle12jhs77_chargedDamage==null || _ufle12jhs77_chargedDamage.IsDeleted) _ufle12jhs77_chargedDamage=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(4645322850908980589,11986489246174559647));
					return _ufle12jhs77_chargedDamage;
				}
			}
			private static readonly TBLTalisman.Factory _factory3_PFS = new TBLTalisman.Factory();
			public TBLTalisman() : base(MetaDefault)
			{
			}
			public TBLTalisman(BGId id) : base(MetaDefault, id)
			{
			}
			public TBLTalisman(BGMetaEntity meta) : base(meta)
			{
			}
			public TBLTalisman(BGMetaEntity meta, BGId id) : base(meta, id)
			{
			}
			public static TBLTalisman FindEntity(Predicate<TBLTalisman> filter)
			{
				return MetaDefault.FindEntity(entity => filter==null || filter((TBLTalisman) entity)) as TBLTalisman;
			}
			public static List<TBLTalisman> FindEntities(Predicate<TBLTalisman> filter, List<TBLTalisman> result=null, Comparison<TBLTalisman> sort=null)
			{
				result = result ?? new List<TBLTalisman>();
				_find_Entities_Result.Clear();
				MetaDefault.FindEntities(filter == null ? (Predicate<BGEntity>) null: e => filter((TBLTalisman) e), _find_Entities_Result, sort == null ? (Comparison<BGEntity>) null : (e1, e2) => sort((TBLTalisman) e1, (TBLTalisman) e2));
				if (_find_Entities_Result.Count != 0)
				{
					for (var i = 0; i < _find_Entities_Result.Count; i++) result.Add((TBLTalisman) _find_Entities_Result[i]);
					_find_Entities_Result.Clear();
				}
				return result;
			}
			public static void ForEachEntity(Action<TBLTalisman> action, Predicate<TBLTalisman> filter=null, Comparison<TBLTalisman> sort=null)
			{
				MetaDefault.ForEachEntity(entity => action((TBLTalisman) entity), filter == null ? null : (Predicate<BGEntity>) (entity => filter((TBLTalisman) entity)), sort==null?(Comparison<BGEntity>) null:(e1,e2) => sort((TBLTalisman)e1,(TBLTalisman)e2));
			}
			public static TBLTalisman GetEntity(BGId entityId)
			{
				return (TBLTalisman) MetaDefault.GetEntity(entityId);
			}
			public static TBLTalisman GetEntity(int index)
			{
				return (TBLTalisman) MetaDefault[index];
			}
			public static TBLTalisman GetEntity(string entityName)
			{
				return (TBLTalisman) MetaDefault.GetEntity(entityName);
			}
			public static TBLTalisman NewEntity()
			{
				return (TBLTalisman) MetaDefault.NewEntity();
			}
		}

#pragma warning restore 414
