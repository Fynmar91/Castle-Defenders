﻿using Sandbox;

public sealed class Peasant : BaseNPC
{
	public override string NPCName => "Peasant";
	public override float BaseHealth => 15;
	public override float BaseSpeed { get; set; } = 15;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override int[] MinMaxCashReward => new[] { 1, 15 };
	public override int[] MinMaxEXPReward => new[] { 1, 5 };
	public override float NPCScale => 0.45f;
	public override float Damage => 2.5f;
}

public sealed class Zombie : BaseNPC
{
	public override string NPCName => "Zombie";
	public override float BaseHealth => 65;
	public override float BaseSpeed { get; set; } = 12.5f;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override int[] MinMaxCashReward => new[] { 6, 24 };
	public override int[] MinMaxEXPReward => new[] { 4, 13 };
	public override float NPCScale => 0.45f;
	public override float Damage => 5.0f;

	public override void Spawn()
	{
		base.Spawn();

		ApplyTexture( "materials/npcs/zombie.vmat" );
		ApplyTextureClient( To.Everyone, "materials/npcs/zombie.vmat" );
	}
}
