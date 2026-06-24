# ColdSlime 

**ColdSlime** is a simple 3D Top-down Survival project created as a practice project to learn and experiment with game development in Unity. The primary goal of this project is to explore and implement various core Unity features and mechanics.

## 🛠️ Key Learnings & Features

- **Enemy AI & Navigation:** Implementing pathfinding and chasing mechanics using `NavMesh` and `NavMeshAgent`.
- **Combat System:** Physics-based projectile shooting (`SphereCast`) with health and damage logic.
- **Level Management:** A wave progression system configured via `ScriptableObjects`.
- **Optimization:** Utilizing **Object Pooling** to efficiently spawn and manage a large number of monsters.
- **Data Persistence:** Basic save/load system using JSON to store player progression (Level & Score).
- **Input Handling:** Integrated Unity's New Input System for smooth character movement and camera rotation.

## 🚀 How to Run / Play

### 📱 Play on Android
The game supports Android devices. You can download the pre-built APK file directly from this repository:
- Navigate to the `Build/ColdSlime/` directory and download the `.apk` file to install and play on your Android device.

### 💻 Open in Unity (For Developers)
1. Clone this repository:
   ```bash
   git clone https://github.com/KanViii/BasicUnity.git
   ```
2. Open the project in Unity (configured with Universal Render Pipeline).
3. Navigate to `Assets/Scenes/` and open `MainMenu.unity` or `map_snow.unity`.
4. Hit the **Play** button to explore the project!
