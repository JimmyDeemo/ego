# Ego

Ego is a hobby game project, written in Unity, to act as a demonstration of my knowledge of Unity's APIs and development processes.

*Note: This project is a work in progress.*

# How to play
## Controls
| Action | Key |
|--------|-----|
| Start Game | 'R' Key |
| Movement| WASD or Arrow Keys|
| Fire | Space Bar |
| Super Shot | Release 'Space Bar'

## Gameplay

Negate enemy fire using your own bullets. As you score hits your craft will grow larger, earning points but making it difficult to manouver through the enemy fire. You have a shield that will absorb the impact of one bullet, but will then need time to recharge. If you let go of the 'Fire' button it will consume your shield and launch a 'Super Shot' to help clear a path. You cannot use 'Super Shot' if you shield is recharging.

## Notable Elements

### C# Usage
| Feature | Reference |
|---------|-----------|
| Generics | See `RequestBulletsFromPool` in `Assets/Scripts/BulletManager.cs`.|
| Interfaces | See `Assets/Scripts/Interfaces/IManagedBullet.cs` and it's usage.

### Unity Feature Usage
| Feature | Reference |
|---------|-----------|
| Coroutines | See the firing coroutines inside `PluseEnemy.cs` and `ShotgunEnemy.cs`. |

## Roadmap

Things that will be implemented soon, in no particular order:
- UI Revamp (UIToolkit)
- Enemies persist until destroyed.
- Asteroids (can be destroyed with focused fire, but otherwise difficult objects)
- Gameplay balance and enemy/obsacle waves.
- Venting (reduce the size of your craft when not firing)