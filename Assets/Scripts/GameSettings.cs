using UnityEngine;
using System.Collections;

public class GameSettings
{
	#region Player Settings
	public static float PLAYER_RATE_OF_FIRE = 0.1f; //In seconds.
	public static int PLAYER_BULLET_POOL_SIZE = 40;
	public static float PLAYER_SCALE_FACTOR = 1.0025f;
	public static float GUN_SEPARATION = 0.3f;
	#endregion

	#region Enemy Settings
	public static float SHOTGUN_SPREAD = 3.0f;

	public static int ENEMY_BULLET_POOL_SIZE = 100;
	public static float ENEMY_RATE_OF_SPAWN_MIN = 0.5f; //In seconds.
	public static float ENEMY_RATE_OF_SPAWN_MAX = 1.0f; //In seconds.
	public static float ENEMY_SPEED_MIN = 2.0f;
	public static float ENEMY_SPEED_MAX = 3.0f;
	#endregion
}
