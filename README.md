# 🎮 2D Tank Online Multiplayer Game

A real-time **2D top-down multiplayer tank battle game** built with **Unity** and **Netcode for GameObjects**. Players connect online, join lobbies, and compete in fast-paced tank combat — collecting coins, eliminating opponents, and climbing the leaderboard.

---

## 📖 About the Project

This project demonstrates a fully networked multiplayer game architecture using Unity's modern networking stack. It supports **three runtime modes**:

- **Host** — A player who also acts as the server (peer-hosted sessions via Relay).
- **Client** — A player who connects to an existing lobby or through matchmaking.
- **Dedicated Server** — A headless server instance for production-grade multiplayer (via Unity Multiplay & Matchmaker).

The game features a top-down 2D tank arena where players move, aim, and fire projectiles at each other while collecting coins scattered across the map.

---

## 🚀 Key Features

| Feature | Description |
|---|---|
| **Online Multiplayer** | Real-time gameplay for up to 20 players per session using Unity Netcode for GameObjects. |
| **Lobby System** | Create, browse, and join lobbies through Unity Lobby Service. Supports both public and private lobbies. |
| **Relay Networking** | Peer-hosted games use Unity Relay with DTLS encryption — no port forwarding required. |
| **Dedicated Server Support** | Full dedicated server mode with Unity Multiplay integration for scalable hosting. |
| **Matchmaking** | Automated matchmaking via Unity Matchmaker with Solo and Team queue support and backfill logic. |
| **Authentication** | Player authentication through Unity Authentication Service. |
| **Tank Combat** | Tank movement, turret aiming (mouse-based), multiple shell types (Anti-Tank, Piercing), and health/damage system. |
| **Coin Economy** | Collectible coins (standard, respawning, bounty) with a wallet system that feeds into the leaderboard. |
| **Leaderboard** | Live, networked leaderboard tracking player scores with team aggregate scoring in team mode. |
| **Healing Zones** | Map areas that restore player health over time. |
| **Respawn System** | Automatic player respawn at random spawn points after elimination. |
| **Minimap** | In-game minimap rendered via a secondary camera and render texture. |
| **Team System** | Team-based play with color-coded players and team leaderboards. |
| **Particle Effects** | Dust cloud particles on tank movement and projectile VFX. |
| **Custom Crosshair** | In-game custom cursor/crosshair for aiming. |

---

## 🛠️ Tech Stack

| Technology | Version / Details |
|---|---|
| **Unity** | 2022.3.50f1 (LTS) |
| **C#** | 9.0 |
| **Netcode for GameObjects** | 1.11.0 |
| **Unity Relay** | 1.0.5 |
| **Unity Lobby** | 1.2.2 |
| **Unity Matchmaker** | 1.1.2 |
| **Unity Multiplay** | 1.2.5 |
| **Cinemachine** | 2.10.1 |
| **Input System** | 1.11.0 (New Input System) |
| **TextMesh Pro** | 3.0.8 |
| **Render Pipeline** | Built-in |

---

### Networking Flow

1. **Startup** → `ApplicationController` detects if running as a dedicated server or client.
2. **Client Path** → Authenticates via Unity Services → enters Main Menu → can Host, Join (by code or lobby list), or Matchmake.
3. **Host Path** → Creates a Relay allocation → creates a Lobby → starts `NetworkServer` → loads the Game scene.
4. **Dedicated Server Path** → Listens for Multiplay allocation → receives matchmaker payload → backfills as needed → manages player lifecycle.

### Connection Approval

The server validates all incoming connections via a custom approval callback. Player identity (`UserData` including username and auth ID) is serialized as a JSON payload in the connection request, enabling server-authoritative player management.

---

## 🎮 How to Play

1. **Login** — Enter your player name.
2. **Main Menu** — Choose to:
   - **Host** a new lobby (public or private)
   - **Join** an existing lobby from the lobby browser or via a join code
   - **Matchmake** into a Solo or Team queue
3. **In-Game** —
   - **WASD** to move and rotate your tank
   - **Mouse** to aim the turret
   - **Left Click** to fire
   - Collect coins to climb the leaderboard
   - Eliminate opponents to earn bounty coins
   - Use healing zones to recover health

---

## 🏗️ Project Architecture

### Scenes Flow

| Scene | Purpose |
|-------|---------|
| **Login** | Player name entry |
| **Loading** | Authentication & Unity Services initialization |
| **MainMenu** | Lobby browser, host/join/matchmake options |
| **Game** | Main gameplay arena |

---

### Scripts

#### 🌐 Networking (`Scripts/Networking/`)

The networking layer is split by role — each mode has its own manager and persistent singleton:

| Folder | Key Scripts | Responsibility |
|--------|------------|----------------|
| **Shared** | `GameData`, `ApplicationData` | Data models shared across all modes (UserData, queues, game modes) |
| **Client** | `ClientGameManager`, `NetworkClient`, `AuthenticationWraper` | Authentication, relay join, matchmaking, disconnect handling |
| **Client/Services** | `MatchplayMatchmaker` | Matchmaker ticket polling & cancellation |
| **Host** | `HostGameManager`, `HostSingletone` | Relay allocation, lobby creation & heartbeat, host startup |
| **Server** | `NetworkServer`, `ServerGameManager` | Connection approval, player spawning, auth tracking, Multiplay integration |
| **Server/Services** | `MultiplayAllocationService`, `MatchplayBackfiller` | Dedicated server lifecycle & backfill for open slots |
| *(root)* | `ApplicationController` | Entry point — detects dedicated server vs client and bootstraps accordingly |

---

#### 🎮 Core Gameplay (`Scripts/Core/`)

| Folder | Key Scripts | Responsibility |
|--------|------------|----------------|
| **Player** | `TankPlayer`, `PlayerMovement`, `PlayerAiming`, `ShellSelection` | Main player NetworkBehaviour, tank movement/rotation, mouse-based turret aim, ammo switching |
| **Player** | `PlayerNameDisplay`, `PlayerColorDisplay`, `TeamColorLookup` | Overhead name labels, team-based coloring via ScriptableObject |
| **Combat** | `Health`, `HealthDisplay`, `DealDamageOnContact` | NetworkVariable-driven health system with death events |
| **Combat** | `HealingZone`, `RespawnHandler`, `ShellSelectionDisplay` | Area healing, death→respawn lifecycle, ammo type HUD |
| **Projectile** | `Projectile`, `ProjectileLauncher` | Projectile behaviour, firing logic & cooldowns |
| **Coins** | `Coin`, `BountyCoin`, `RespawningCoin` | Coin types — base, dropped on kill, auto-respawning |
| **Coins** | `CoinSpawner`, `CoinWallet`, `CoinDisplay` | Server-side spawning, per-player wallet (NetworkVariable), HUD element |
| *(root)* | `SpawnPoint`, `UIManager`, `ParticleAligner` | Random spawn positions, in-game UI orchestration, VFX helpers |

---

#### 🖥️ UI (`Scripts/UI/`)

| Folder | Key Scripts | Responsibility |
|--------|------------|----------------|
| *(root)* | `NameSelector`, `MainMenu`, `GameHUD` | Login name input, main menu actions, in-game HUD controller |
| **Lobby** | `LobbiesList`, `LobbyItem` | Lobby browser — query, display, and join |
| **Leaderboard** | `Leaderboard`, `LeaderboardEntityDisplay`, `LeaderboardEntityState` | Live networked leaderboard with solo & team scoring |

---

#### ⌨️ Input (`Scripts/Input/`)

| Script | Responsibility |
|--------|----------------|
| `InputReader` | New Input System wrapper — exposes move, aim, and fire events |

---

#### 🔧 Utility (`Scripts/Utility/`)

| Script | Responsibility |
|--------|----------------|
| `ClientNetworkTransform` | Client-authoritative NetworkTransform override |
| `DestroySelfOnContact` | Auto-destroy on collision |
| `SpawnOnDestroy` | Spawn VFX/object on destruction |
| `Lifetime` | Timed auto-destroy |

---

### Assets

| Folder | Contents |
|--------|----------|
| **Prefabs/Networking** | ClientManager, HostManager, ServerManager singletons |
| **Prefabs/Projectiles** | Server & client shells — AntiTank, Piercing, Standard |
| **Prefabs/Coins** | Coin, BountyCoin, RespawningCoin |
| **Prefabs/Terrain** | Rocks, Walls, Wall Patterns |
| **Prefabs/Effects** | DustCloud particle effect |
| **Prefabs/UI** | LeaderboardEntity, LobbyItem, OverheadCanvas |
| **Sprites** | Tank, Level, HUD, Currency, Consumables, UI, VFX art assets |
| **Settings** | Input Action maps, render pipeline config, network prefab list |
| **Minimap** | Minimap RenderTexture |
| **Player Colors** | TeamColorLookup ScriptableObject |

---

## 🧩 Setup & Running

### Prerequisites
- Unity **2022.3.50f1** (or compatible 2022.3 LTS)
- A Unity account with **Unity Gaming Services** enabled (Relay, Lobby, Authentication, Matchmaker)

### Steps
1. Clone the repository.
2. Open the project in Unity Hub.
3. In **Edit → Project Settings → Services**, link to your Unity project/organization.
4. Enable the required services in the [Unity Dashboard](https://dashboard.unity.com/): Authentication, Relay, Lobby, and (optionally) Matchmaker + Multiplay for dedicated servers.
5. Open the **Loading** scene and press Play.

> **Note:** Dedicated server mode requires building a Linux server build and deploying via Unity Multiplay. For local testing, use Host mode.

---

## 📌 Highlights for Recruiters

- **Production-grade networking architecture** with clean separation of Client, Host, and Dedicated Server responsibilities.
- **Unity Gaming Services integration** — demonstrates proficiency with Relay, Lobby, Matchmaker, Multiplay, and Authentication.
- **Server-authoritative design** — connection approval, server-side spawning, and `NetworkVariable`-driven state sync.
- **Scalable lobby system** with heartbeat pings, backfill logic, and graceful shutdown/cleanup.
- **Modular, maintainable codebase** — clear folder structure, singleton management for networking layers, and event-driven architecture.
- **Full game loop** — from authentication → matchmaking → gameplay → leaderboard → disconnect handling.

---

## 📄 License

This project is intended as a portfolio piece. Feel free to explore the code for learning purposes.