using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;

namespace CommunityServerAPI; 

public static class Commands {
	public static void ProcessCommand(Player callingPlayer, string command) {
		if (Program.diskStorage == null) {
			Console.WriteLine("Unable to get the player stats!");
			return;
		}
		// Split command and its arguments and get player stats.
		string[] args = command.Split(" ");
		PlayerStats callingPlayerStats = Program.diskStorage.GetPlayerStatsOf(callingPlayer.SteamID).Result;
		
		// Admin commands only
		if (callingPlayerStats.Roles.HasFlag(Roles.Admin)) {
			// KILL COMMAND:
			if (args[0] == "kill" && args.Length == 2) {
				Player victim = FindPlayerFromName(args[1], callingPlayer.GameServer);
				if (victim == null) {
					callingPlayer.Message("Cannot Find the Player Specified.");
				}
				else {
					victim.Kill();
					victim.Message("An Admin decided to kill you. Yes, you deserved it.");
				}
				return;
			} else if (args[0] == "kill") {
				callingPlayer.Message("Improper use. Usage: kill (player name)");
				return;
			}
			// FLING COMMAND
			if (args[0] == "fling" && args.Length == 2) {
				Player victim = FindPlayerFromName(args[1], callingPlayer.GameServer);
				// TODO FLING COMMAND
			}
			else {
				callingPlayer.Message("Improper use. Usage: fling (player name)");
			}
		}
		
		//Command Not Found, tell the caller
		callingPlayer.Message("Command Not Found.");
		return;
	}

	public static Player FindPlayerFromName(string name, GameServer gameServer) {
		IEnumerator<Player> allPlayers = gameServer.GetAllPlayers().GetEnumerator();
		while (allPlayers.MoveNext()) {
			if (allPlayers.Current == null) {
				break;
			}

			if (String.CompareOrdinal(allPlayers.Current.Name.ToLower(), name.ToLower()) == 0) {
				// Found our player
				return allPlayers.Current;
			}
		}

		return null;
	}
}