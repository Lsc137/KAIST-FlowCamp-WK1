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

### Gameplay
<img width="392" height="685" alt="StageClear" src="https://github.com/user-attachments/assets/d5bd204f-36df-4e79-8c4b-a6ba9fb2dcc7" />
<img width="388" height="680" alt="SNS" src="https://github.com/user-attachments/assets/c44f5504-5f0b-46ad-b0f8-89dd6798bde2" />
<img width="389" height="680" alt="MainMeny" src="https://github.com/user-attachments/assets/adfb856e-0d20-41a0-b1cb-02a4dbf8e40c" />
<img width="389" height="678" alt="Calculator" src="https://github.com/user-attachments/assets/41e7017e-bef0-42ea-b1ea-72b5fda04c48" />
<img width="385" height="677" alt="AppGrid" src="https://github.com/user-attachments/assets/85471c19-2a4e-4aeb-87b1-745c31297932" />
<img width="388" height="683" alt="ToDoList" src="https://github.com/user-attachments/assets/993cca06-a7cd-47d5-b90b-ec9f5b363bd0" />


### Project Structure (Key Folders)

- `Assets/Scenes` – Contains `MainMenu` and main gameplay scenes.
- `Assets/Scripts/Bugs` – Bug behaviours, data, and spawn logic.
- `Assets/Scripts/Maps` – App‑specific logic such as Calculator and Todo List.
- `Assets/Scripts/Utils` – Global managers (OS, results, etc.).

### Credits

Created for KAIST FlowCamp Week 1 as a small game about finding and fixing bugs in playful, visual form.

