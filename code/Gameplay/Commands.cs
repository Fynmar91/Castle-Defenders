﻿using System.Linq;
using Sandbox;

public  partial class CDGame
{
	public enum DebugEnum
	{
		Default,
		Tower,
		Gameplay,
		Path,
		All,
	}

	public DebugEnum DebugMode;

	[ConVar.Replicated( "cd.debug" )]
	public static bool Debug { get; set; }

	[ConCmd.Admin( "cd.debug.mode" )]
	public static void DebugModeSet(int mode)
	{
		switch(mode)
		{
			case 1:
				Instance.DebugMode = DebugEnum.Default;
				break;
			case 2:
				Instance.DebugMode = DebugEnum.Tower;
				break;
			case 3:
				Instance.DebugMode = DebugEnum.Gameplay;
				break;
			case 4:
				Instance.DebugMode = DebugEnum.Path;
				break;
			case 5:
				Instance.DebugMode = DebugEnum.All;
				break;
		}

		Log.Info( "Debug Mode: " + Instance.DebugMode );
	}

	[ConCmd.Server("cd.money.give")]
	public static void GiveMoney(int amount, string targetName = "")
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		var player = ConsoleSystem.Caller.Pawn as CDPawn;

		if ( player == null )
			return;

		if( !string.IsNullOrEmpty(targetName) )
		{
			All.OfType<CDPawn>().ToList().FirstOrDefault( x => x.Client.Name.ToLower().Contains(targetName.ToLower()) )
				.AddCash( amount );
		}
		else
		{
			player.AddCash( amount );
		}
	}

	[ConCmd.Server( "cd.exp.give" )]
	public static void GiveEXP( int amount, string targetName = "" )
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		var player = ConsoleSystem.Caller.Pawn as CDPawn;

		if ( player == null )
			return;

		if ( !string.IsNullOrEmpty( targetName ) )
		{
			All.OfType<CDPawn>().ToList().FirstOrDefault( x => x.Client.Name.ToLower().Contains( targetName.ToLower() ) )
				.AddEXP( amount );
		}
		else
		{
			player.AddEXP( amount );
		}
	}

	[ConCmd.Admin( "cd.diff.set" )]
	public static void SetDifficulty( int diffInt )
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		switch( diffInt )
		{
			case 1:
				Instance.Difficulty = DiffEnum.Easy;
				break;
			case 2:
				Instance.Difficulty = DiffEnum.Medium;
				break;
			case 3:
				Instance.Difficulty = DiffEnum.Hard;
				break;
			case 4:
				Instance.Difficulty = DiffEnum.Extreme;
				break;
			default:
				Log.Error( "Invalid setter for difficulty" );
				return;
		}

		Log.Info( "Updated difficulty to " + Instance.Difficulty );
	}

	[ConCmd.Server( "cd.npc.create" )]
	public static void SpawnNPC( string npcName, bool spawnOpposite = false)
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		var npc = new BaseNPC();

		if ( ResourceLibrary.TryGet( $"npcs/{npcName}.npc", out BaseNPCAsset asset ) )
			npc.UseAssetAndSpawn( asset );

		var spawnerPoint = All.OfType<NPCSpawner>().ToList();

		var blueSide = spawnerPoint.FirstOrDefault( x => x.AttackTeamSide == NPCSpawner.TeamEnum.Blue );
		var redSide = spawnerPoint.FirstOrDefault( x => x.AttackTeamSide == NPCSpawner.TeamEnum.Red );

		if ( !spawnOpposite && Instance.Competitive )
		{
			npc.Position = blueSide.Position;
			npc.Steer.Target = All.OfType<NPCPath>().FirstOrDefault( x => x.StartNode ).Position;
			npc.PathToFollow = BaseNPC.PathTeam.Blue;
			npc.CastleTarget = blueSide.FindCastle();
		} 
		else if ( spawnOpposite && Instance.Competitive )
		{
			npc.Position = redSide.Position;
			npc.Steer.Target = All.OfType<NPCPath>().FirstOrDefault( x => x.StartOpposingNode ).Position;
			npc.PathToFollow = BaseNPC.PathTeam.Red;
			npc.CastleTarget = redSide.FindCastle();
		} 
		else
		{
			npc.Position = blueSide.Position;
			npc.Steer.Target = All.OfType<NPCPath>().FirstOrDefault( x => x.StartNode ).Position;
			npc.PathToFollow = BaseNPC.PathTeam.Blue;
			npc.CastleTarget = blueSide.FindCastle();
		}
	}

	[ConCmd.Admin( "cd.game.start" )]
	public static void ForceStartGame()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		if ( Instance.GameStatus != GameEnum.Idle )
		{
			Log.Error( "Game is not in Idle state" );
			return;
		}

		Instance.StartGame();
	}

	[ConCmd.Admin( "cd.game.stop" )]
	public static void ForceStopGame()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		if ( Instance.GameStatus == GameEnum.Idle )
		{
			Log.Error( "Game is not in Idle state" );
			return;
		}

		Instance.GameStatus = GameEnum.Idle;
		Instance.WaveStatus = WaveEnum.Pre;
	}


	[ConCmd.Admin( "cd.game.restart" )]
	public static void ForceRestartGame()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		if ( Instance.GameStatus == GameEnum.Idle )
		{
			Log.Error( "Game is in Idle state" );
			return;
		}

		Instance.StartGame();
	}

	[ConCmd.Admin( "cd.game.mapvote" )]
	public static void ForceMapVote()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		Instance.StartMapVote();
	}

	[ConCmd.Admin( "cd.data.reset" )]
	public static void ResetData()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}


		var player = ConsoleSystem.Caller.Pawn as CDPawn;

		if ( player == null )
			return;

		player.NewPlayerStats();
	}

	[ConCmd.Admin( "cd.data.save" )]
	public static void SaveData()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		var player = ConsoleSystem.Caller.Pawn as CDPawn;

		if ( player == null )
			return;

		Instance.SaveData( player );
	}

	[ConCmd.Admin( "cd.data.save.all" )]
	public static void SaveAllData()
	{
		if ( !Debug )
			return;

		foreach ( var cl in Game.Clients )
		{
			if ( cl.Pawn is CDPawn player )
				Instance.SaveData( player );
		}
	}

	[ConCmd.Admin( "cd.data.load" )]
	public static void LoadDataCMD()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		Instance.LoadSave( ConsoleSystem.Caller );
	}

	[ConCmd.Server( "cd.set.towerslot" )]
	public static void SetTowerSlot(int slot, string name)
	{
		var player = ConsoleSystem.Caller.Pawn as CDPawn;
		
		if ( player == null )
			return;

		if ( name.ToLower().Contains( "hands" ) )
		{
			player.ChangeSlot( "Hands", player.TowerSlots.Count );
			return;
		}

		if ( TypeLibrary.GetType<Entity>( name ) == null )
		{
			Log.Error( "Invalid replacement slot name" );
			return;
		}

		slot -= 1;

		if ( player.TowerSlots[slot] == null )
		{
			Log.Error( "Invalid replacement slot index" );
			return;
		}

		player.TowerSlots[slot] = name;
		player.ChangeSlot( name, slot );
	}

	[ConCmd.Server( "cd.update.slots" )]
	public static void UpdateTowerSlotsCMD()
	{
		var player = ConsoleSystem.Caller.Pawn as CDPawn;
		if ( player == null )
			return;

		var slotNum = 1;
		player.ClearTowerSlots();

		foreach ( var item in player.TowerSlots )
		{
			player.UpdateSlots( To.Single( player ), item.Value, item.Key + 1);
			slotNum++;
		}

		player.UpdateSlots( To.Single( player ), "Hands", 0 );
	}

	[ConCmd.Server("cd.wave.set")]
	public static void SetWave(int wave)
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		Instance.ClearNPCs();
		Instance.PostWave();
		Instance.CurWave = Instance.CurWave.Clamp(wave - 1, Instance.MaxWaves);
	}

	[ConCmd.Server( "cd.wave.restart" )]
	public static void RestartWave()
	{
		if ( !Debug )
		{
			Log.Error( "Debug is turned off" );
			return;
		}

		Instance.ClearNPCs();
		Instance.PostWave();
		Instance.CurWave -= 1;
	}

}
