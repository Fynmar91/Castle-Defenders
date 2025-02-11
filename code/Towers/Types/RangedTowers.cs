﻿using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

public partial class Pistol : BaseTower
{
	public override string TowerName => "Pistol";
	public override string TowerDesc => "A very simple pistol tower";

	public Vector3 TargetDirection { get; private set; }
	public override string TowerModel => "models/towers/pistol_tower.vmdl";
	public override int UnlockLevel => 0;
	public override BaseTower RequiredTowers => null;
	public override string[] TowerLevelDesc => new[]
	{
		"",
		"A pistol with an increased range, damage and fire-rate",
		"A pistol with improved range, damage and fire-rate than previously",
		"An enhanced pistol with even better range, damage and fire-rate",
		"A heavily enhanced, top of the line pistol with best range, damage and fire-rate"
	};

	public override List<(float AttTime, float AttDMG, int NewRange)> Upgrades => new()
	{
		new(-0.5f, 2.25f, 0),
		new(-0.5f, 2.50f, 25),
		new(-0.75f, 2.75f, 25),
		new(-0.75f, 3.25f, 25),
		new(-1.0f, 5.0f, 50)
	};

	public override int TowerMaxLevel => 5;
	public override int TowerCost => 20;
	public override int[] TowerLevelCosts => new[]
	{
		25,
		45,
		90,
		145,
		-1,
	};

	public override float DeploymentTime => 3.25f;
	public override float AttackTime { get; set; } = 3.5f;
	public override float AttackDamage { get; set; } = 8.0f;
	public override int RangeDistance { get; set; } = 125;
	public override string AttackSound => "pistol_fire";

	public override void SimulateTower()
	{
		base.SimulateTower();

		if ( IsPreviewing ) return;

		if ( Target != null && Target.IsValid() ) {
			TargetDirection = TargetDirection.LerpTo( ((Target.Position + Vector3.Up * ZEyeScale) - GetAttachment( "forward" ).Value.Position).Normal, Time.Delta * 10f );
			SetAnimParameter( "v_forward", TargetDirection );
		}

		SetAnimParameter( "b_attack", (TimeLastAttack + 0.1f) >= AttackTime && Target != null );
	}

	[ClientRpc]
	public override void FireEffects()
	{
		base.FireEffects();

		Particles.Create( "particles/bullet_muzzleflash.vpcf", this, "muzzle" );
	}
}

public partial class SMG : BaseTower
{
	public override string TowerName => "SMG";
	public override string TowerDesc => "A fast shooting submachine tower";
	public override string TowerModel => "models/towers/smgtower.vmdl";
	public override int UnlockLevel => 0;
	public override BaseTower RequiredTowers => null;
	public override string[] TowerLevelDesc => new[]
	{
		"",
		"A two barrel SMG for more damage along with better firing",
		"A more improved version with a spinning barrel",
		"A very fast spinning smg, don't mess with it"
	};

	public Vector3 TargetDirection { get; private set; }

	public override List<(float AttTime, float AttDMG, int NewRange)> Upgrades => new()
	{
		new(-0.15f, 1.5f, 0),
		new(-0.20f, 2.5f, 25),
		new(-0.25f, 3.25f, 25),
		new(-0.25f, 5.75f, 25)
	};

	public override int TowerMaxLevel => 4;
	public override int TowerCost => 45;
	public override int[] TowerLevelCosts => new[]
	{
		90,
		125,
		220,
		-1,
	};

	public override float DeploymentTime => 3.25f;
	public override float AttackTime { get; set; } = 1.10f;
	public override float AttackDamage { get; set; } = 6.5f;
	public override int RangeDistance { get; set; } = 110;
	public override string AttackSound => "smg_fire";

	[Event.Tick.Server]
	public override void SimulateTower()
	{
		base.SimulateTower();

		if ( Target != null && Target.IsValid() )
		{
			TargetDirection = TargetDirection.LerpTo( ((Target.Position + Vector3.Up * ZEyeScale) - GetAttachment( "forward" ).Value.Position).Normal, Time.Delta * 10f );
			SetAnimParameter( "v_forward", TargetDirection );
		}

		SetAnimParameter( "b_attack", (TimeLastAttack + 0.1f) >= AttackTime && Target != null );
	}

	[ClientRpc]
	public override void FireEffects()
	{
		base.FireEffects();

		Particles.Create( "particles/bullet_muzzleflash.vpcf", this, "muzzle_l" );
		Particles.Create( "particles/bullet_muzzleflash.vpcf", this, "muzzle_r" );
	}
}

public  partial class Sniper : BaseTower
{
	public override string TowerName => "Sniper";
	public override string TowerDesc => "A sniper tower that can fire from a large distance";

	//Temporary until we get a pistol model
	public override string TowerModel => "models/towers/snipertower.vmdl";
	public override int UnlockLevel => 3;
	public override BaseTower RequiredTowers => null;
	public override int[] TowerLevelCosts => new[]
	{
		165,
		230,
		290,
		375,
		465,
		-1
	};

	public override string[] TowerLevelDesc => new[]
	{
		"",
		"Upgraded sniper tower with better sniper power",
		"Modified sniper tower with more powerful sniping",
		"Enhanced sniper tower with better sniping power and equipped with a infra-red scope",
		"This tower can snipe incredibly fast and large distances",
		"This tower is dangerously lethal even from a far distance"
	};
	public override List<(float AttTime, float AttDMG, int NewRange)> Upgrades => new()
	{
		new(-0.25f, 2.5f, 50),
		new(-0.30f, 2.5f, 50),
		new(-0.45f, 5.0f, 75),
		new(-0.50f, 5.0f, 100),
		new(-0.50f, 7.5f, 100),
		new(-0.50f, 10.0f, 125)
	};

	public override int TowerMaxLevel => 6;
	public override int TowerCost => 150;
	public override float DeploymentTime => 3.25f;
	public override float AttackTime { get; set; } = 11.5f;
	public override float AttackDamage { get; set; } = 35.0f;
	public override int RangeDistance { get; set; } = 250;
	public override string AttackSound => "sniper_fire";
	public override float ZEyeScale => 35.0f;

	private bool _lockedOnTarget;
	private Particles _laserSight;

	public override void UpgradeTower()
	{
		base.UpgradeTower();

		if ( TowerLevel == 4 )
			CounterStealth = true;
	}

	protected override void OnDestroy()
	{
		LaserOff(To.Everyone);
		base.OnDestroy();
	}

	public override void SellTower()
	{
		LaserOff();
		base.SellTower();
	}

	[ClientRpc]
	public override void FireEffects()
	{
		base.FireEffects();
	}

	[Event.Tick.Server]
	public override void SimulateTower()
	{
		base.SimulateTower();

		if ( Target == null )
		{
			_lockedOnTarget = false;
			LaserOff(To.Everyone);

			return;
		}

		if ( TimeLastAttack * 4 >= AttackTime && !_lockedOnTarget )
		{
			LaserOn( To.Everyone, Target );
			_lockedOnTarget = true;
		}

		UpdateLaser( To.Everyone, Target );
	}

	[ClientRpc]
	private void LaserOn( BaseNPC target )
	{
		Game.AssertClient();

		if ( target == null )
			return;

		_laserSight = Particles.Create( "particles/sniper_beam.vpcf" );
		_laserSight.SetEntityAttachment( 1, this, "muzzle" );
		_laserSight.SetEntity( 0, target, Vector3.Up * 25 );
	}

	[ClientRpc]
	public void UpdateLaser(BaseNPC newTarget)
	{
		if ( _laserSight == null )
			return;

		_laserSight.SetEntity( 0, newTarget, Vector3.Up * 25 );
	}

	[ClientRpc]
	private void LaserOff()
	{
		Game.AssertClient();

		_laserSight?.Destroy( true );
		_laserSight = null;
	}

	public override void Attack( BaseNPC target )
	{
		_lockedOnTarget = false;
		LaserOff(To.Everyone);

		base.Attack( target );
	}
}

public partial class Shotgun : BaseTower
{
	public override string TowerName => "Shotgun";
	public override string TowerDesc => "A close ranged effective shotgun tower";

	//Temporary until we get a pistol model
	public override string TowerModel => "models/towers/shotguntower.vmdl";
	public override int UnlockLevel => 0;
	public override BaseTower RequiredTowers => null;
	public override string[] TowerLevelDesc => new[]
	{
		"",
		"A shotgun tower that fire extra pellets",
		"A shotgun tower that is more powerful at close range",
		"This shotgun will blast someone to bits within close range",
		"Seriously, don't get too close to this shotgun"
	};

	public override List<(float AttTime, float AttDMG, int NewRange)> Upgrades => new()
	{
		new(-0.10f, 9.25f, 5),
		new(-0.25f, 11.25f, 5),
		new(-0.25f, 13.0f, 10),
		new(-0.50f, 14.0f, 15),
		new(-0.75f, 15.0f, 25)
	};

	public override int TowerMaxLevel => 5;
	public override int TowerCost => 75;
	public override int[] TowerLevelCosts => new[]
	{
		125,
		200,
		365,
		575,
		-1,
	};

	public override float DeploymentTime => 3.25f;
	public override float AttackTime { get; set; } = 4.75f;
	public override float AttackDamage { get; set; } = 25.0f;
	public override int RangeDistance { get; set; } = 85;
	public override string AttackSound => "shotgun_fire";

	[Event.Tick.Server]
	public override void SimulateTower()
	{
		base.SimulateTower();

		//if ( Target != null && Target.IsValid() )
		//{
		//	TargetDirection = TargetDirection.LerpTo( ((Target.Position + Vector3.Up * ZEyeScale) - GetAttachment( "forward" ).Value.Position).Normal, Time.Delta * 10f );
		//	SetAnimParameter( "v_forward", TargetDirection );
		//}

		SetAnimParameter( "b_attack", (TimeLastAttack + 0.1f) >= AttackTime && Target != null );
	}

	[ClientRpc]
	public override void FireEffects()
	{
		base.FireEffects();

		Particles.Create( "particles/bullet_muzzleflash.vpcf", this, "muzzle" );
	}
}
