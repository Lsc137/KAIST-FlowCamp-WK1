## BugFinder

BugFinder is a short puzzle–action game built in Unity where you fix software “bugs” inside a smartphone-style OS. Each app (like a Calculator or Todo List) starts malfunctioning, and your job is to track down and eliminate the literal bug creatures causing the problems.

### Core Concept

You interact with a fake phone UI, open different apps, notice how they are broken, then long‑press specific UI elements to spawn a bug. Once the bug appears, you tap it to damage and eventually destroy it. Clearing all bugs in a stage restores the app to normal.

### Gameplay Flow

- **Main Menu**: Start the game and enter the phone OS.
- **Home Screen**: Choose an app (stage), such as Calculator or Todo List.
- **Bugged Apps**: Each app has intentional glitches (wrong calculations, duplicated items, broken counters, etc.).
- **Spawn Bugs**: Long‑press the bugged button or area for about a second to spawn a bug.
- **Catch Bugs**: Bugs move around the screen, bounce off edges, and may have special traits (high HP, splitting, invisibility). Tap them repeatedly to defeat them.
- **Stage Clear**: When all bugs in the stage are fixed, a clear popup appears and you can move on to other apps.

### Requirements

- **Engine**: Unity (URP project; tested with recent 2022+ versions).
- **Platform**: Designed primarily for portrait mobile resolution, but also runs in the Unity editor Game view.

### How to Run

1. Open Unity Hub.
2. Add the `BugFinder` folder as an existing project.
3. Open the project in a compatible Unity version (2022 LTS or newer is recommended).
4. Open the `MainMenu` scene under `Assets/Scenes`.
5. Press **Play** to start and test the game.

### Project Structure (Key Folders)

- `Assets/Scenes` – Contains `MainMenu` and main gameplay scenes.
- `Assets/Scripts/Bugs` – Bug behaviours, data, and spawn logic.
- `Assets/Scripts/Maps` – App‑specific logic such as Calculator and Todo List.
- `Assets/Scripts/Utils` – Global managers (OS, results, etc.).

### Credits

Created for KAIST FlowCamp Week 1 as a small game about finding and fixing bugs in playful, visual form.

