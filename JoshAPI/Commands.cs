using System.Numerics;
using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using BattleBitAPI.Storage;

namespace CommunityServerAPI; 

public static class Commands {

	private static readonly Dictionary<string, Func<Player, string[], bool>> CommandDictionary = new Dictionary<string, Func<Player, string[], bool>>() {
		{"kill", KillCommand},
		{"fling", FlingCommand},
		{"doublexp", DoubleXpCommand},
		{"bangun", BanWeaponCommand},
		{"help", HelpCommand},
		{"bannedguns", TellBannedGunsCommand}
	};

	
	private static bool KillCommand(Player callingPlayer, string[] args) {
		// Admin or Mod is needed for this command
		if (!PlayerIsMod(callingPlayer)) {
			return false;
		}

		if (args.Length == 2) {
			Player victim = FindPlayerFromName(args[1],callingPlayer.GameServer);
			if (victim == null) {
				callingPlayer.Message("Cannot find player.");
			}
			else {
				victim.Kill();
				victim.Message("You have been killed by an admin.");
				return true;
			}
		}
		else {
			callingPlayer.Message("Usage: kill (player name)");
		}
		return false;
	}

	private static bool FlingCommand(Player callingPlayer, string[] args) {
		if (!PlayerIsMod(callingPlayer)) {
			return false;
		}
		if (args.Length == 2) {
			Player victim = FindPlayerFromName(args[1],callingPlayer.GameServer);
			if (victim == null) {
				callingPlayer.Message("Cannot find player.");
			}
			else {
				victim.Teleport(new Vector3(0,0,100));
				victim.Message("You have been flung by an admin.");
				return true;
			}
		}
		else {
			callingPlayer.Message("Usage: fling (player name)");
		}
		return false;
	}
	
	private static bool DoubleXpCommand(Player callingPlayer, string[] args) {
		throw new NotImplementedException();
	}

	private static bool BanWeaponCommand(Player callingPlayer, string[] args) {
		throw new NotImplementedException();
	}

	private static bool HelpCommand(Player callingPlayer, string[] args) {
		throw new NotImplementedException();
	}

	private static bool TellBannedGunsCommand(Player callingPlayer, string[] args) {
		throw new NotImplementedException();
	}

	private static bool PlayerIsAdmin(Player callingPlayer) {
		if (Program.DiskStorage.GetPlayerStatsOf(callingPlayer.SteamID).Result.Roles.HasFlag(Roles.Admin)) {
			return true;
		}
		return false;
	}
	private static bool PlayerIsMod(Player callingPlayer) {
		if (Program.DiskStorage.GetPlayerStatsOf(callingPlayer.SteamID).Result.Roles.HasFlag(Roles.Moderator | Roles.Admin)) {
			return true;
		}
		return false;
	}
	public static bool ProcessCommand(Player callingPlayer, string command, string[] args) {
		Func<Player, string[], bool> method;

		if (!CommandDictionary.TryGetValue(command, out method)) {
			// The command wasn't found
			callingPlayer.Message("Command not found!");
			return false;
		}

		bool result = method(callingPlayer, args);
		return result;
	}
	
	private static Player FindPlayerFromName(string name, GameServer gameServer) {
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