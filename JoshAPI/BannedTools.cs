using System.Collections.ObjectModel;
using BattleBitAPI;
using BattleBitAPI.Common;

namespace CommunityServerAPI; 

public static class BannedTools {
	private static Collection<Weapon> BannedGuns = new Collection<Weapon>();
	// This will keep track of how many unique votes each gun has.
	private static Dictionary<Weapon, int> BanGunVote = new Dictionary<Weapon, int>();
	// This will keep track of what each player has already voted on. <steamID, wepaons voted for>
	private static Dictionary<ulong, Collection<Weapon>> BanGunVoteCount = new Dictionary<ulong, Collection<Weapon>>();
	
	public static void BanWeapon(Weapon weapon) {
		// Make sure to not double add weapons
		if (BannedGuns.Contains(weapon)) {
			return;
		}
		BannedGuns.Add(weapon);
	}

	public static void ForgiveWeapon(Weapon weapon) {
		BannedGuns.Remove(weapon);
		// Reset the voting for the given weapon
        BanGunVote.Remove(weapon);
		// Reset each players vote on that specific weapon.
		foreach (Collection<Weapon> weapons in BanGunVoteCount.Values) {
			weapons.Remove(weapon);
		}
	}

	public static bool IsBanned(Weapon weapon) {
		return BannedGuns.Contains(weapon);
	}

	public static void VoteBanGun(Weapon weapon) {
		if (BanGunVote.ContainsKey(weapon)) {
		}
	}
	public static void ClearBans() {
		BannedGuns.Clear();
		BanGunVote.Clear();
		BanGunVoteCount.Clear();
	}
}