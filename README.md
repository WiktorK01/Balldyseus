# Balldyseus

## Game Description
This is a solo-project I've been working on for a few months, I'm quite proud of how it's turning out.
It's a Turn-Based game that's a bit of a mix of Billiards and Fire Emblem.

Enemies are placed throughout a level and have a set amount of health and movement-per-turn. 
Every turn they are moving a set amount of spaces along a grid towards a designated objective.

The player is trying to defeat all enemies before they reach an objective tile.
The player is ball, that can be aimed and controlled like a billiard ball. The player is not confined to moving along the grid like Enemies are.
On the player's turn, they aim and fire. Upon hitting an enemy, the enemy loses 1 health. if an enemy loses all health, they die.

The player can Left-Click to enter Attack Mode or Right-Click to enter Bounce Mode.
Attack Mode is the only way of directly dealing damage to an enemy, it has no other special properties.

Bounce Mode does not directly deal damage, but allows you to bounce off of objects and push enemies back.
You have a limited amount of bounces per-turn.
Bouncing off a wall or enemy will increase the player's velocity.
Bouncing off an enemy will push the enemy in the opposite direction by one-tile.
If the enemy is pushed into a wall, it will instead deal damage.
If the enemy is pushed into another Enemy, both enemies take damage.
This can also be used to push Enemies into obstacles like Fire-Tiles, lighting them on fire.

The player can switch between being in Attack Mode or Bounce Mode at any time, including after you've aimed and fired.
The player is encouraged to stay engaged and time their mode-switches mid-movement.

The player can also enter a "High Speed Mode" if the Player is moving fast enough.
While in this mode, the player deals double-damage while in attack-mode.
The idea here is to use Bounce-Mode to increase you velocity and reach High Speeds, then switch to attack-mode when the time is right to deal greater damage.

Playable [Here](https://wiktork01.itch.io/balldyseus)

## Features
- Turn Manager
- A* Pathfinding for Enemies
- Extensive usage of the "Feel" game asset to add juice to everything
- Basic animations and particles
- Observers, Singletons, Factories
