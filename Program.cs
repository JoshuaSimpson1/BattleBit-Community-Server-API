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
        listener.Start(29294);//Port
        Thread.Sleep(-1);
    }

    private static async Task<PlayerStats> OnGetPlayerStats(ulong arg1, PlayerStats officalStats) {
        throw new NotImplementedException();
    }

    private static async Task OnSavePlayerStats(ulong steamID, PlayerStats stats) {
        diskStorage.SavePlayerStatsOf(steamID, stats);
    }

    private static async Task OnPlayerKill(MyPlayer killer, Vector3 killerPos, MyPlayer victim, Vector3 victimPos, string tool) {
        if(tool == Gadgets.Pickaxe.Name || tool == Gadgets.PickaxeIronPickaxe.Name) {
            killer.GameServer.SayToChat(killer.Name + " just pwned " + victim.Name + " with a pickaxe!");
        } else if(tool == Gadgets.SledgeHammer.Name || tool == Gadgets.SledgeHammerSkinA.Name || tool == Gadgets.SledgeHammerSkinB.Name 
            || tool == Gadgets.SledgeHammerSkinC.Name) {
            killer.GameServer.SayToChat(killer.Name + " just pwned " + victim.Name + " with a sledgehammer!");
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
class MyPlayer : Player
{
    
}
