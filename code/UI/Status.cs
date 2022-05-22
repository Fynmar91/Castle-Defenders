﻿using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class Status : Panel
{
	public Panel CashPnl;
	public Label CurCashLbl;

	public Status()
	{
		StyleSheet.Load( "UI/Status.scss" );

		CashPnl = Add.Panel();
		CurCashLbl = Add.Label("???", "text");
	}


	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as TD2Pawn;

		if ( player == null )
			return;

		int plyCash = player.GetCash();

		if ( plyCash >= 10000)
		{
			string formatCash = plyCash.ToString();
			CurCashLbl.SetText( $"${formatCash[0]}{formatCash[1]}.{formatCash[2]}k");
		} 
		else if ( plyCash >= 1000 && plyCash < 10000)
		{
			string formatCash = plyCash.ToString();
			CurCashLbl.SetText( $"${formatCash[0]}.{formatCash[1]}k" );
		}
		else
		{
			CurCashLbl.SetText( $"${plyCash}" );
		}

	}
}
