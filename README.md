# endless-plane-unity-game

This project is a Unity prototype demonstrating an **endless plane game**.  
The game is a single player 3D game where player travel endless tunnel in an aircraft and scores when successfully passed the enemies. Beware some enemies are smarter.

The focus of this assignment is on:
- Gameplay programming
- Code architecture
- Unity Editor scripting
- Performance awareness
- UX and polish Editor tooling

---

## Unity Version Used

```
Unity 2022.3.57f1 (LTS)
```

---

## Setup Instructions

1. Clone the repository or unzip the project.
2. Open the project in **Unity Hub** using the Unity version mentioned above.
3. Wait for the project to compile and import all assets.
4. Open **Game** scene at **Assets/Scenes/** to run the prototype.

No additional packages or setup steps are required.

---

## Controls

```
Use on-screen joystick to move the player (Supports Editor and Android build). Avoid coming obstacles to score.
```

---

## Wave Data Editor Tool (How to Use)

This project includes a **custom Unity Editor tool** for generating wave data in JSON format.

### Opening the Tool

In the Unity Editor menu:
```
Tools → Wave Data Generator
```

This opens the **Wave Data Generator** editor window.

---

### Tool Features

- Uses **Obstacle and SmartObstacle prefabs** as input
- Allows designers to:
  - Add or remove any number of waves
  - Configure each wave using simple input fields
- Generates a deterministic wave order
- Exports wave data to:
  ```
  Assets/Data/WaveData.json
  ```

---

### Configuring Obstacles

1. Assign **Obstacle prefabs** in the *Obstacle Prefabs* section.
2. Assign **SmartObstacle prefabs** in the *Smart Obstacle Prefabs* section.
3. Prefabs provide:
   - Obstacle ID
   - Spawn bounds (min/max X & Y)
4. Obstacle prefabs are in Assets/Prefabs/ folder.

---

### Configuring Waves

For each wave, the following fields are available:

- **Obstacle Holder IDs**  
  Comma-separated list of holder indices (e.g. `2,4,6`). There are 6 obstacle holders in the game but usage limits to 3 as 3 obstacles are currently there.

- **Total Sub Waves**  
  Number of sub-waves for the wave (minimum 1)

- **Mandatory Obstacle Included**  
  When enabled, ensures a mandatory obstacle appears first in each sub-wave

- **Mandatory Obstacle Type**  
  Select which obstacle type is mandatory (Obstacle / SmartObstacle)

Waves can be added or removed dynamically using the UI.

---

### Generating Wave Data

1. Ensure at least one wave is configured.
2. Click **Generate WaveData.json**.
3. The file is created (or overwritten) at:
   ```
   Assets/Data/WaveData.json
   ```
4. This JSON is consumed at runtime by the wave parsing system.

> Note:  
> The editor tool does **not** persist data between Unity sessions by design.

---

## Wholesome (Thinking) Test

### 1. What trade-offs did you make and why?

```
I heavily used Singletons for rapid development and in some places I skipped the validation assuming data availability as far as prototype is concerned.
Used basic and placeholder UI to save time. Used direct floating value comparison than putting a threshold at some places for quick results.
In wave data generator editor tool, the data doesn't persist - the reason is wave data will not be generated too frequently so keep it that way.
```

---

### 2. What would you improve if given one more week?

```
I will implement a dependency injection system (third party or custom specific to this game) to reduce singleton usage.
I will profile the game and check where optimizations needed. I will integrate a decent UI than basic placeholder one.
I will add data persistent system in wave data generator editor tool.
Currently, there are box colliders on player and enemies so I will add low poly mesh colliders to achieve more accurate collisions.
```

---

### 3. How would you scale this prototype into a full game?

```
There are multiple features that can be added to this prototype so that it can turn into full game.
- Collectibles can be added along with obstacles.
- Extra score can be awarded to near miss with an obstacle.
- Gun shooting can be added to destroy the enemies.
- More obstacles and advanced enemies can be added.
- Lives system can be added so that player gets more survival chances.
- Visuals can be improved.
- Thoughtful rewards can be provided so that player retains.
- Player customization can be added.
```

---

### 4. Which part of the project are you most satisfied with?

```
I am most satisfied with the endless environment which feels like tunnel is never ending and it is optimized as well.
The way environment frames move and carries the obstacles is working really well.
```

---

### 5. Which part would you refactor first?

```
I will refactor UI and integrate dependency injection first.
```

---

## Thank You
