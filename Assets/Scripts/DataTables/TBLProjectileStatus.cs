using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/TBLProjectileStatus")]
public partial class TBLProjectileStatus : BGEntityGo
{
	public override BGMetaEntity MetaConstraint
	{
		get
		{
			return MetaDefault;
		}
	}
	private static BansheeGz.BGDatabase.BGMetaRow _metaDefault;
	public static BansheeGz.BGDatabase.BGMetaRow MetaDefault
	{
		get
		{
			if(_metaDefault==null || _metaDefault.IsDeleted) _metaDefault=BGRepo.I.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(5403286218538413865,15787621285960666264));
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
	public new System.String name
	{
		get
		{
			return _name[Entity.Index];
		}
		set
		{
			_name[Entity.Index] = value;
		}
	}
	public System.Int32 damage
	{
		get
		{
			return _damage[Entity.Index];
		}
		set
		{
			_damage[Entity.Index] = value;
		}
	}
	public System.Single speed
	{
		get
		{
			return _speed[Entity.Index];
		}
		set
		{
			_speed[Entity.Index] = value;
		}
	}
	public System.String collisionLayerName
	{
		get
		{
			return _collisionLayerName[Entity.Index];
		}
		set
		{
			_collisionLayerName[Entity.Index] = value;
		}
	}
	public UnityEngine.Vector2 hitboxOffset
	{
		get
		{
			return _hitboxOffset[Entity.Index];
		}
		set
		{
			_hitboxOffset[Entity.Index] = value;
		}
	}
	public UnityEngine.Vector2 hitboxSize
	{
		get
		{
			return _hitboxSize[Entity.Index];
		}
		set
		{
			_hitboxSize[Entity.Index] = value;
		}
	}
	public System.Single lifeTime
	{
		get
		{
			return _lifeTime[Entity.Index];
		}
		set
		{
			_lifeTime[Entity.Index] = value;
		}
	}
	public System.String moveType
	{
		get
		{
			return _moveType[Entity.Index];
		}
		set
		{
			_moveType[Entity.Index] = value;
		}
	}
	public System.String attackType
	{
		get
		{
			return _attackType[Entity.Index];
		}
		set
		{
			_attackType[Entity.Index] = value;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldEntityName __name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _name
	{
		get
		{
			if(__name==null || __name.IsDeleted) __name=(BansheeGz.BGDatabase.BGFieldEntityName) MetaDefault.GetField(new BGId(5349640413800106396,15246783913620657335));
			return __name;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldInt __damage;
	public static BansheeGz.BGDatabase.BGFieldInt _damage
	{
		get
		{
			if(__damage==null || __damage.IsDeleted) __damage=(BansheeGz.BGDatabase.BGFieldInt) MetaDefault.GetField(new BGId(5088558781905369320,11366558149220593559));
			return __damage;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldFloat __speed;
	public static BansheeGz.BGDatabase.BGFieldFloat _speed
	{
		get
		{
			if(__speed==null || __speed.IsDeleted) __speed=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(5291623440061312105,11407164497712171964));
			return __speed;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldString __collisionLayerName;
	public static BansheeGz.BGDatabase.BGFieldString _collisionLayerName
	{
		get
		{
			if(__collisionLayerName==null || __collisionLayerName.IsDeleted) __collisionLayerName=(BansheeGz.BGDatabase.BGFieldString) MetaDefault.GetField(new BGId(5640664736938531964,8315437914689234356));
			return __collisionLayerName;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldVector2 __hitboxOffset;
	public static BansheeGz.BGDatabase.BGFieldVector2 _hitboxOffset
	{
		get
		{
			if(__hitboxOffset==null || __hitboxOffset.IsDeleted) __hitboxOffset=(BansheeGz.BGDatabase.BGFieldVector2) MetaDefault.GetField(new BGId(5416599933110093201,12058101938230048440));
			return __hitboxOffset;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldVector2 __hitboxSize;
	public static BansheeGz.BGDatabase.BGFieldVector2 _hitboxSize
	{
		get
		{
			if(__hitboxSize==null || __hitboxSize.IsDeleted) __hitboxSize=(BansheeGz.BGDatabase.BGFieldVector2) MetaDefault.GetField(new BGId(5379345426512287729,1844351239110886825));
			return __hitboxSize;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldFloat __lifeTime;
	public static BansheeGz.BGDatabase.BGFieldFloat _lifeTime
	{
		get
		{
			if(__lifeTime==null || __lifeTime.IsDeleted) __lifeTime=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(5673530901536212325,8126910241556050820));
			return __lifeTime;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldString __moveType;
	public static BansheeGz.BGDatabase.BGFieldString _moveType
	{
		get
		{
			if(__moveType==null || __moveType.IsDeleted) __moveType=(BansheeGz.BGDatabase.BGFieldString) MetaDefault.GetField(new BGId(5278834531543241527,4928883701501858738));
			return __moveType;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldString __attackType;
	public static BansheeGz.BGDatabase.BGFieldString _attackType
	{
		get
		{
			if(__attackType==null || __attackType.IsDeleted) __attackType=(BansheeGz.BGDatabase.BGFieldString) MetaDefault.GetField(new BGId(4869025940877484089,4086206600187063970));
			return __attackType;
		}
	}
}
