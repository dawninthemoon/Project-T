using System;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGDatabase;

//=============================================================
//||                   Generated by BansheeGz Code Generator ||
//=============================================================

[AddComponentMenu("BansheeGz/Generated/TBLPlayerStatus")]
public partial class TBLPlayerStatus : BGEntityGo
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
			if(_metaDefault==null || _metaDefault.IsDeleted) _metaDefault=BGRepo.I.GetMeta<BansheeGz.BGDatabase.BGMetaRow>(new BGId(5019724043690752934,8165130890644791169));
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
	public System.Single moveSpeed
	{
		get
		{
			return _moveSpeed[Entity.Index];
		}
		set
		{
			_moveSpeed[Entity.Index] = value;
		}
	}
	public System.Single minJumpHeight
	{
		get
		{
			return _minJumpHeight[Entity.Index];
		}
		set
		{
			_minJumpHeight[Entity.Index] = value;
		}
	}
	public System.Single maxJumpHeight
	{
		get
		{
			return _maxJumpHeight[Entity.Index];
		}
		set
		{
			_maxJumpHeight[Entity.Index] = value;
		}
	}
	public UnityEngine.Vector2 meleeAttackOffset
	{
		get
		{
			return _meleeAttackOffset[Entity.Index];
		}
		set
		{
			_meleeAttackOffset[Entity.Index] = value;
		}
	}
	public UnityEngine.Vector2 shootOffset
	{
		get
		{
			return _shootOffset[Entity.Index];
		}
		set
		{
			_shootOffset[Entity.Index] = value;
		}
	}
	public UnityEngine.Vector2 meleeAttackSize
	{
		get
		{
			return _meleeAttackSize[Entity.Index];
		}
		set
		{
			_meleeAttackSize[Entity.Index] = value;
		}
	}
	public System.Int32 meleeAttackDamage
	{
		get
		{
			return _meleeAttackDamage[Entity.Index];
		}
		set
		{
			_meleeAttackDamage[Entity.Index] = value;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldEntityName __name;
	public static BansheeGz.BGDatabase.BGFieldEntityName _name
	{
		get
		{
			if(__name==null || __name.IsDeleted) __name=(BansheeGz.BGDatabase.BGFieldEntityName) MetaDefault.GetField(new BGId(5530482193986285011,8438961672885908913));
			return __name;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldFloat __moveSpeed;
	public static BansheeGz.BGDatabase.BGFieldFloat _moveSpeed
	{
		get
		{
			if(__moveSpeed==null || __moveSpeed.IsDeleted) __moveSpeed=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(5674120293527248723,9898524774998421931));
			return __moveSpeed;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldFloat __minJumpHeight;
	public static BansheeGz.BGDatabase.BGFieldFloat _minJumpHeight
	{
		get
		{
			if(__minJumpHeight==null || __minJumpHeight.IsDeleted) __minJumpHeight=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(5377716173471244235,10377407478378447783));
			return __minJumpHeight;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldFloat __maxJumpHeight;
	public static BansheeGz.BGDatabase.BGFieldFloat _maxJumpHeight
	{
		get
		{
			if(__maxJumpHeight==null || __maxJumpHeight.IsDeleted) __maxJumpHeight=(BansheeGz.BGDatabase.BGFieldFloat) MetaDefault.GetField(new BGId(5089643509535846038,5951098283929571765));
			return __maxJumpHeight;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldVector2 __meleeAttackOffset;
	public static BansheeGz.BGDatabase.BGFieldVector2 _meleeAttackOffset
	{
		get
		{
			if(__meleeAttackOffset==null || __meleeAttackOffset.IsDeleted) __meleeAttackOffset=(BansheeGz.BGDatabase.BGFieldVector2) MetaDefault.GetField(new BGId(5383172222021905109,17991773874149183902));
			return __meleeAttackOffset;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldVector2 __shootOffset;
	public static BansheeGz.BGDatabase.BGFieldVector2 _shootOffset
	{
		get
		{
			if(__shootOffset==null || __shootOffset.IsDeleted) __shootOffset=(BansheeGz.BGDatabase.BGFieldVector2) MetaDefault.GetField(new BGId(5718047970036351789,8567812798406467996));
			return __shootOffset;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldVector2 __meleeAttackSize;
	public static BansheeGz.BGDatabase.BGFieldVector2 _meleeAttackSize
	{
		get
		{
			if(__meleeAttackSize==null || __meleeAttackSize.IsDeleted) __meleeAttackSize=(BansheeGz.BGDatabase.BGFieldVector2) MetaDefault.GetField(new BGId(4904729729814094198,12318528735908965526));
			return __meleeAttackSize;
		}
	}
	private static BansheeGz.BGDatabase.BGFieldInt __meleeAttackDamage;
	public static BansheeGz.BGDatabase.BGFieldInt _meleeAttackDamage
	{
		get
		{
			if(__meleeAttackDamage==null || __meleeAttackDamage.IsDeleted) __meleeAttackDamage=(BansheeGz.BGDatabase.BGFieldInt) MetaDefault.GetField(new BGId(5456382862615549585,14492920877162419134));
			return __meleeAttackDamage;
		}
	}
}
