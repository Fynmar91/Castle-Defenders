﻿using Sandbox;
using System;
using System.Linq;


public partial class CDPawn : Player
{
	public ClothingContainer Clothing = new();

	TimeSince timeLastTowerPlace;

	bool isoView = false;

	Sound curMusic;

	public CDPawn()
	{

	}

	public CDPawn( Client cl ) : this()
	{
		Clothing.LoadFromClient( cl );
	}

	public void SpawnAtLocation()
	{
		var randomSpawnPoint = All.OfType<SpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			Transform = tx;
		}
	}

	public void SwitchCameraView()
	{
		isoView = !isoView;

		if( isoView )
		{
			//TODO: Isometric view
		} 
		else if (!isoView )
		{
			CameraMode = new FirstPersonCamera();
		}
	}

	[ClientRpc]
	public void PlayMusic(string music)
	{
		curMusic = Sound.FromScreen( music );
	}

	[ClientRpc]
	public void EndMusic( string musicEnd )
	{
		curMusic.Stop();
		curMusic = Sound.FromScreen( musicEnd );
	}

	[ClientRpc]
	public void PlaySoundOnClient(string sndPath)
	{
		PlaySound( sndPath );
	}

	public override void Spawn()
	{
		EnableLagCompensation = true;

		CreateHull();
		Tags.Add( "cdplayer" );

		SpawnAtLocation();

		SetModel( "models/citizen/citizen.vmdl_c" );
		Clothing.DressEntity( this );

		CameraMode = new FirstPersonCamera();
		Animator = new StandardPlayerAnimator();
		Controller = new WalkController();

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( InputButton.View ) )
			SwitchCameraView();

		//Check if debug is false and we're in an active game
		if ( CDGame.Instance.Debug == false )
		{
			if ( CDGame.Instance.GameStatus == CDGame.GameEnum.Active )
				DoTDInputs();
		}
		//Otherwise if debug is on, just do the TDInputs
		else
			DoTDInputs();

		DoTowerOverview();
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
	}
}
