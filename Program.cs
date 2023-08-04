using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using BattleBitAPI.Storage;
using System.Numerics;

class Program
{
    public static DiskStorage diskStorage;
    static void Main(string[] args)
    {
        diskStorage = new DiskStorage("Players\\");
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
            string command = msg.Substring(1);
            // Process the command here.
        }
    }

    private static async Task<PlayerStats> OnGetPlayerStats(ulong steamID, PlayerStats officialStats) {
        PlayerStats playerStat = diskStorage.GetPlayerStatsOf(steamID).Result;
        // If the player is not created yet, we will first pull off from the bb servers
        if (playerStat == null) {
            return officialStats;
        }
        // In the case the bb servers have a more updated version of a players stats this way you can
        // progress on the server if you wish too but you aren't limited to my own servers
        if ((playerStat.Progress.Rank < officialStats.Progress.Rank &&
             playerStat.Progress.Prestige <= officialStats.Progress.Prestige)
            || playerStat.Progress.Prestige < officialStats.Progress.Prestige) {
            return officialStats;
        }
        return playerStat;
    }

    private static async Task OnSavePlayerStats(ulong steamID, PlayerStats stats) 
    {
        await diskStorage.SavePlayerStatsOf(steamID, stats);
    }

    private static async Task OnPlayerKill(MyPlayer killer, Vector3 killerPos, MyPlayer victim, Vector3 victimPos, string tool) {
        if(tool == Gadgets.Pickaxe.Name || tool == Gadgets.PickaxeIronPickaxe.Name) {
            killer.GameServer.SayToChat(killer.Name + " just pwned " + victim.Name + " with a pickaxe!");
        } else if(tool == Gadgets.SledgeHammer.Name || tool == Gadgets.SledgeHammerSkinA.Name || tool == Gadgets.SledgeHammerSkinB.Name 
            || tool == Gadgets.SledgeHammerSkinC.Name) {
            killer.GameServer.SayToChat(killer.Name + " just pwned " + victim.Name + " with a sledgehammer!");
        }

        if (killer.DoubleXP) {
            diskStorage.GetPlayerStatsOf(killer.SteamID).Result.Progress.EXP += 200;
        }
    }

    private static async Task<PlayerSpawnRequest> OnPlayerSpawning(MyPlayer player, PlayerSpawnRequest request) {
        if(request.Loadout.PrimaryWeapon.Tool == Weapons.KrissVector) {
            // Do not allow the vector
            player.Message("This server doesn't allow the vector, sorry you will have to get good.");
            request.Loadout.PrimaryWeapon.Tool = Weapons.HoneyBadger;
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
    public bool DoubleXP = false;
}
