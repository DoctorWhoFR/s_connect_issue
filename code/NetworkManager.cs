using System.Threading.Tasks;
using Sandbox;
using Sandbox.Network;

public sealed class NetworkManager : Component, Component.INetworkListener
{
	[Property] public GameObject PlayerPrefab { get; set; }

	void CreatePlayer(Connection connection )
	{
		GameObject player = PlayerPrefab.Clone();
		Log.Info( player );
		player.Name = connection.DisplayName;
		player.MakeNameUnique();
		player.NetworkSpawn(connection);
	}

	async protected override void OnStart()
	{
		if ( Networking.IsActive )
		{
			Log.Info( "we are currently connecting to someone else." );
			return;
		}

		var lobbys = await Networking.QueryLobbies();
		Log.Info( lobbys.Count );

		if ( lobbys.Count == 0 )
		{
			Log.Info( "leaving" );
			return;
		}
		LobbyInformation lobby = lobbys.First();
		Log.Info( lobby.LobbyId );
		

	}

	async protected override void OnUpdate()
	{

		if ( Input.Pressed( "Slot1" ) )
		{
			Log.Info( "test" );
			Networking.CreateLobby( new LobbyConfig()
			{
				MaxPlayers = 8,
				Privacy = LobbyPrivacy.Public,
				Name = "My Lobby Name"
			} );

			// because if this an host now we need to do as we are going to do for an basic player creation
			Log.Info( "est" );
			Log.Info( Connection.Local );
		}

		if ( Input.Pressed( "Slot2" )){
			Networking.Disconnect();
		
		}

		if ( Input.Pressed("Slot3") )
		{
			var lobbys = await Networking.QueryLobbies();
			Log.Info( lobbys.Count );

			if ( lobbys.Count == 0 )
			{
				Log.Info( "lobbys count is 0" );
				return;
			}
			LobbyInformation lobby = lobbys.First();
			Log.Info( lobby.LobbyId );
			Networking.Connect( lobby.LobbyId );

		}
	}

	void INetworkListener.OnConnected(Sandbox.Connection channel)
	{
		Log.Info( "test" );
	}

	void INetworkListener.OnActive(Sandbox.Connection channel)
	{

		Log.Info( Connection.Host.PartyId );
		Log.Info( "is connecting" );
		if( channel.IsHost )
		{
			Log.Info( "is self" );
		}
		CreatePlayer(channel);
	} 

}
