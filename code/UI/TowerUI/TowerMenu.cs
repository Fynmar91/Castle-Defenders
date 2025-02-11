﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class TowerMenu : Panel
{
	public Panel TowerPnl;
	public Label TowerName;
	public Label TowerDesc;
	public Label TowerCost;
	public Label TowerOwnerAndPriority;
	public Label TowerStats;
	public Label Priority;
	public Label NextUpgrade;

	public Panel InputGlyphPnl;
	public Label PrimMouseLabel;
	public Image MousePrim;

	public Label SecondMouseLabel;
	public Image MouseSecond; 
	
	public Label UseLabel;
	public Image UseImage;

	public Panel mousePrimary;
	public Panel mouseSecondary;

	Entity curTowerHover;

	public TowerMenu()
	{
		StyleSheet.Load( "ui/towerui/towermenu.scss" );

		TowerPnl = Add.Panel("mainPnl");
		TowerName = TowerPnl.Add.Label( "", "towerName" );
		TowerDesc = TowerPnl.Add.Label( "", "towerDesc" );
		TowerCost = TowerPnl.Add.Label( "", "towerCost" );
		NextUpgrade = TowerPnl.Add.Label( "Next Upgrade: ???", "towerNextUpg" );
		TowerStats = TowerPnl.Add.Label( "Statistics: ???", "towerStats" );
		TowerOwnerAndPriority = TowerPnl.Add.Label( "", "towerOwner basetext" );
		Priority = TowerPnl.Add.Label( "", "towerPriority basetext" );

		InputGlyphPnl = TowerPnl.Add.Panel( "inputPnl" );
		Panel InputsPNL = InputGlyphPnl.Add.Panel( "inputsPnl" );

		mousePrimary = InputsPNL.Add.Panel( "mousePnl" );
		MousePrim = mousePrimary.Add.Image(null, "primMouse" );
		MousePrim.SetTexture( Input.GetGlyph( InputButton.PrimaryAttack ).ResourcePath );

		PrimMouseLabel = mousePrimary.Add.Label( "???", "text" );

		mouseSecondary = InputsPNL.Add.Panel( "mousePnl" );
		MouseSecond = mouseSecondary.Add.Image( null, "secondMouse" );
		MouseSecond.SetTexture( Input.GetGlyph( InputButton.SecondaryAttack ).ResourcePath );

		SecondMouseLabel = mouseSecondary.Add.Label( "???", "text" );

		//UseImage.SetTexture( Input.GetGlyph( InputButton.Use ).ResourcePath );
		UseLabel = InputGlyphPnl.Add.Label( "???", "text" );

	}

	[Event.Client.Frame]
	public void FrameScan()
	{
		var player = Game.LocalPawn as CDPawn;

		if ( player == null )
			return;

		var clTr = Trace.Ray( player.AimRay.Position, player.AimRay.Position + player.AimRay.Forward * 350 )
			.UseHitboxes( true )
			.WithTag( "tower" )
			.Run();

		if ( player.SelectedTower.IsValid() )
		{
			curTowerHover = player.SelectedTower;
			return;
		}

		curTowerHover = clTr.Entity;
	}

	public override void Tick()
	{
		base.Tick();

		var player = Game.LocalPawn as CDPawn;

		if ( player == null )
			return;

		TowerPnl.SetClass( "showMenu", curTowerHover is BaseTower || curTowerHover is BaseSuperTower );
		InputGlyphPnl.SetClass( "showInputs", curTowerHover is BaseTower || curTowerHover is BaseSuperTower );

		if ( curTowerHover is BaseSuperTower superTower )
		{
			TowerOwnerAndPriority.SetText( $"Owner: {superTower.Owner.Client.Name}" );
			TowerName.SetText( superTower.NetName );
			TowerDesc.SetText( superTower.NetDesc );
			TowerCost.SetText( $"Build Cost: ${superTower.NetCost}" );

			PrimMouseLabel.SetText( "Use Active Ability, Press again to use" );
			SecondMouseLabel.SetText( "Cancel ability while using" );
			UseLabel.SetText( "" );

			NextUpgrade.SetText( "" );

			TowerStats.SetText(superTower.NetAbility);

			return;
		}

		if ( curTowerHover is BaseTower tower )
		{
			if(tower.IsPreviewing)
			{
				TowerName.SetText( tower.NetName );
				TowerDesc.SetText( tower.NetDesc );
				TowerCost.SetText( $"Build Cost: ${tower.NetCost}" );
				TowerStats.SetText( tower.NetStats );
				PrimMouseLabel.SetText( "Place Tower" );

				TowerOwnerAndPriority.SetText( "" );
				NextUpgrade.SetText( "" );
				SecondMouseLabel.SetText( "" );
				UseLabel.SetText( "" );

				mousePrimary.Style.Set( "display: flex;" );
				mouseSecondary.Style.Set( "display: none;" );

				return;
			}

			TowerOwnerAndPriority.SetText( $"Owner: {tower.Owner.Client.Name}" );
			Priority.SetText( $"Priority: {tower.TargetPriority}" );

			player.Circle( tower.Position + Vector3.Up * 5, Rotation.FromPitch( 90 ), tower.NetRange, Color.Green.WithAlpha( 0.45f ), false );

			TowerName.SetText( tower.NetName );
			TowerDesc.SetText( tower.NetDesc );

			if ( tower.NetCost != -1 )
			{
				TowerCost.SetText( $"Upgrade Cost: ${tower.NetCost}" );
				NextUpgrade.SetText( $"Next Upgrade: {tower.NetUpgradeDesc}" );
				mousePrimary.Style.Set( "display: flex;" );
			}
			else
			{
				TowerCost.SetText( "Max Level" );
				NextUpgrade.SetText( "" );
				mousePrimary.Style.Set( "display: none;" );
			}

			TowerStats.SetText( tower.NetStats );
			mouseSecondary.Style.Set( "display: flex;" );

			PrimMouseLabel.SetText( "Upgrade" );
			SecondMouseLabel.SetText( "Sell" );
			UseLabel.SetText( "\nUSE KEY: Change Priority" );
		}
	}
}
