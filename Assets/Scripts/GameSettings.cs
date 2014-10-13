using UnityEngine;
using System.Collections;

public class GameSettings
{
	#region Player Settings
	public static float PLAYER_RATE_OF_FIRE = 0.1f; //In seconds.
	public static int PLAYER_BULLET_POOL_SIZE = 15;
	#endregion

	#region Enemy Settings
	public static int ENEMY_BULLET_POOL_SIZE = 100;
	public static float ENEMY_RATE_OF_SPAWN = 0.2f; //In seconds.
	public static float ENEMY_SPEED = 5.0f;
	#endregion
}
