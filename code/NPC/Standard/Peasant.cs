﻿using Sandbox;
using System;
using System.Linq;

public partial class Peasant : BaseNPC
{
	public override string NPCName => "Peasant";
	public override int BaseHealth => 15;
	public override float BaseSpeed => 15;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override int[] MinMaxCashReward => new int[] { 1, 10 };
	public override int[] MinMaxEXPReward => new int[] { 1, 5 };
	public override float NPCScale => 0.45f;
	public override float Damage => 5;

	public override void Spawn()
	{
		base.Spawn();
	}
}
