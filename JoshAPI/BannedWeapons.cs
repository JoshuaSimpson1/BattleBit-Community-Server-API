using System.Collections.ObjectModel;
using BattleBitAPI.Common;

namespace CommunityServerAPI; 

public static class BannedWeapons {
	private static Collection<Weapon> BannedGunCollection = new Collection<Weapon>();

	public static void BanWeapon(Weapon weapon) {
		// Make sure to not double add weapons
		if (BannedGunCollection.Contains(weapon)) {
			return;
		}
		BannedGunCollection.Add(weapon);
	}

	public static void ForgiveWeapon(Weapon weapon) {
		BannedGunCollection.Remove(weapon);
	}

	public static bool IsBanned(Weapon weapon) {
		return BannedGunCollection.Contains(weapon);
	}
}