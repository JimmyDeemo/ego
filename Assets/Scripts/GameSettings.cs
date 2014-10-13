using UnityEngine;
using System.Collections;

public class GameSettings
{
	#region Player Settings
	public static float PLAYER_RATE_OF_FIRE = 0.1f; //In seconds.
	public static int PLAYER_BULLET_POOL_SIZE = 15;
	public static float PLAYER_SCALE_FACTOR = 1.005f;
	#endregion

	#region Enemy Settings
	public static int SHOTGUN_SPREAD = 2;

	public static int ENEMY_BULLET_POOL_SIZE = 100;
	public static float ENEMY_RATE_OF_SPAWN_MIN = 0.4f; //In seconds.
	public static float ENEMY_RATE_OF_SPAWN_MAX = 0.8f; //In seconds.
	public static float ENEMY_SPEED_MIN = 3.0f;
	public static float ENEMY_SPEED_MAX = 5.0f;
	#endregion
}
