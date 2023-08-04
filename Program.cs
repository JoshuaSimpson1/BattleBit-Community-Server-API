using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using BattleBitAPI.Storage;
using System.Numerics;
using CommunityServerAPI;

class Program
{
    public static DiskStorage DiskStorage;
    static void Main(string[] args)
    {
        DiskStorage = new DiskStorage("Players\\");
        var listener = new ServerListener<MyPlayer>();
        listener.OnGameServerTick += OnGameServerTick;
        listener.OnPlayerSpawning += OnPlayerSpawning;
        listener.OnAPlayerKilledAnotherPlayer += OnPlayerKill;
        listener.OnSavePlayerStats += OnSavePlayerStats;
        listener.OnGetPlayerStats += OnGetPlayerStats;
        listener.OnPlayerTypedMessage += OnPlayerTypedMessage;
        listener.Start(29294);//Port
        Thread.Sleep(-1);
    }

    private static async Task OnPlayerTypedMessage(MyPlayer player, ChatChannel channel, string msg) {
        if (msg.StartsWith("!")) {
            // Remove the first character
            string command = msg.Substring(1);
            command = command.ToLower();
            string[] args = command.Split(" ");

            if (DiskStorage == null) {
                player.Message("Failed to find player data!");
                return;
            }
            Commands.ProcessCommand(player, args[0], args);
        }
    }

    private static async Task<PlayerStats> OnGetPlayerStats(ulong steamID, PlayerStats officialStats) {
        PlayerStats playerStat = DiskStorage.GetPlayerStatsOf(steamID).Result;
        // If the player is not created yet, we will first pull off from the bb servers
        if (playerStat == null) {
            playerStat = officialStats;
        }
        // In the case the bb servers have a more updated version of a players stats this way you can
        // progress on the server if you wish too but you aren't limited to my own servers
        if ((playerStat.Progress.Rank < officialStats.Progress.Rank &&
             playerStat.Progress.Prestige <= officialStats.Progress.Prestige)
            || playerStat.Progress.Prestige < officialStats.Progress.Prestige) {
            playerStat = officialStats;
        }
        return playerStat;
    }

    private static async Task OnSavePlayerStats(ulong steamID, PlayerStats stats) 
    {
        await DiskStorage.SavePlayerStatsOf(steamID, stats);
    }

    private static async Task OnPlayerKill(MyPlayer killer, Vector3 killerPos, MyPlayer victim, Vector3 victimPos, string tool) {
        if(tool == Gadgets.Pickaxe.Name || tool == Gadgets.PickaxeIronPickaxe.Name) {
            killer.GameServer.SayToChat(killer.Name + " just pwned " + victim.Name + " with a pickaxe!");
        } else if(tool == Gadgets.SledgeHammer.Name || tool == Gadgets.SledgeHammerSkinA.Name || tool == Gadgets.SledgeHammerSkinB.Name 
            || tool == Gadgets.SledgeHammerSkinC.Name) {
            killer.GameServer.SayToChat(killer.Name + " just pwned " + victim.Name + " with a sledgehammer!");
        }
        // TODO: When the api is updated to also count headshots make this correct such that the xp given is actually
        // (200 + 400) * 2
        if (killer.DoubleXp) {
            DiskStorage.GetPlayerStatsOf(killer.SteamID).Result.Progress.EXP += 200;
        }
    }

    private static async Task<PlayerSpawnRequest> OnPlayerSpawning(MyPlayer player, PlayerSpawnRequest request) {
        PlayerStats playerStat = DiskStorage.GetPlayerStatsOf(player.SteamID).Result;
        if(BannedWeapons.IsBanned(request.Loadout.PrimaryWeapon.Tool)) {
            // Do not allow banned weapons
            player.Message("This server currently doesn't allow the " + request.Loadout.PrimaryWeapon.Tool.Name +
                           " you will just need to get good.");
            request.Loadout.PrimaryWeapon.Tool = Weapons.M9;
        }
        return request;
    }

    private static async Task OnGameServerTick(GameServer server)
    {
        //server.Settings.SpectatorEnabled = !server.Settings.SpectatorEnabled;
        //server.MapRotation.AddToRotation("DustyDew");
        //server.MapRotation.AddToRotation("District");
        //server.GamemodeRotation.AddToRotation("CONQ");
        //server.ForceEndGame();
    }
    
}
class MyPlayer : Player {
    public bool DoubleXp = false;
}
