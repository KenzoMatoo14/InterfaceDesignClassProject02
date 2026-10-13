# InterfaceDesignClassProject02

# Quest Checkout

## Project Overview

Quest Checkout is a Unity-based kiosk application for managing Meta Quest 3 kit loans. It allows users to check availability, select a kit, verify its contents, review their choices, and confirm a loan.

The app manages six Quest 3 kits (Quest3 01 – Quest3 06), blocks unavailable kits, saves the inventory and loans between restarts, and includes two test scenarios:

* **Scenario A (Case A):** at least one kit is available, so the user can borrow it.
* **Scenario B (Case B):** no kits are available, so the app explains the situation and offers the kits that will be free soonest as a working next step.

## Team Members

* Kenzo Matoo López
* Fátima Sofía Walls Fernández
* Alberto Stephen Dubin Hernández

## Team Contributions

* **Kenzo Matoo López:** Implementation of the Quest Headset Manager interface in Unity (UI screens, headset statuses, reservation timeline, confirmation ticket and interaction logic).
* **Fátima Sofía Walls Fernández:** Project documentation: README, design justification, testing documentation, video organization, AI use declaration and coordination of the documentation deliverables.
* **Alberto Stephen Dubin Hernández:** FALTANTE

## Figma Design

https://www.figma.com/design/5yRzIzVPcyZezECFmFmCy6/Project1_Wireframe?node-id=0-1&t=VEjo1whUymS7okcp-1

## Requirements

* Unity version: Unity 6.5 (6000.5.4f1)
* Operating system: Windows

## Getting Started

1. Clone this repository.
2. Open the project using the specified Unity version (6000.5.4f1) from Unity Hub.
3. Open the main scene: `Assets/Scenes/MainMenu.unity`.
4. Run the application from the Unity Editor (Play button).

The three scenes (`MainMenu`, `CaseA`, `CaseB`) are already registered in the Build Settings, in that order.

## Download and Run the Build

Download the runnable build from the [GitHub Release](https://github.com/KenzoMatoo14/InterfaceDesignClassProject02/releases/tag/Release).

## Running the Build on Windows
Download the Windows build from the project's GitHub Release.
Extract the downloaded ZIP file to a folder on your computer.
Open the extracted folder.
Double-click the game's .exe file to launch Quest Checkout.
Follow the on-screen instructions to interact with the application.
Use the available scenario controls to test Scenario A and Scenario B, and reset the application state when needed.

Note: If Windows displays a security warning, verify that the executable was downloaded from the team's official GitHub repository before deciding whether to run it.

Requirements: A Windows computer compatible with the build.

Troubleshooting: If the application does not launch, make sure the ZIP file was fully extracted and try running the executable again from the extracted folder.

## Controls and Usage

The application is operated with the mouse (or touch).

**Main Menu**

| Control | What it does |
|---|---|
| **CASE A** | Opens the kiosk with at least one available kit (Scenario A). |
| **CASE B** | Opens the kiosk with no available kits (Scenario B). |
| **Reset** | Deletes the saved loan and inventory data so both scenarios start from their original state. |

**Kiosk screen (Case A and Case B)**

* The top summary shows how many headsets are **Available**, **Reserved**, **Loaned** and **Charging**.
* Each of the six headset cards shows its name, status and either a **Start** button (available) or the time left until it is ready (unavailable).
* **Go to main menu** returns to the Main Menu.

**How to complete a loan (Scenario A)**

1. Select an available headset (the card or its **Start** button).
2. In the reservation panel, check the headset details and the kit contents listed under **Includes** (headset, left and right controllers, charger).
3. On the timeline, select the pickup time by clicking free cells. Each cell is 30 minutes; select **three consecutive cells to borrow the kit for 90 minutes**. Red cells are already occupied or in the past and cannot be selected.
4. Review the selection. You can click a cell at the edge of the selection to remove it, click an adjacent cell to extend it, or click another free cell to start over. The **X** button closes the panel without saving anything.
5. Press **Confirm**. The button is only enabled once a time is selected.
6. The confirmation ticket shows the **pick-up time**, the **return deadline**, the drop-off location and the kit contents. You can press **Send to email** (receipt screen) or the **X** to go back to the main screen, where the headset now appears as Reserved and the counters are updated.

## Testing the Scenarios

### Scenario A — Available Kit

1. In the Main Menu, press **Reset** to restore the original data.
2. Press **CASE A**. The summary should show 1 Available, 3 Reserved, 1 Loaned and 1 Charging. **Quest3 01** is the available kit.
3. Click **Start** on Quest3 01, select three consecutive 30-minute cells (90 minutes) and press **Confirm**.
4. Verify that the ticket shows the pick-up time, the return deadline 90 minutes later and the drop-off location, and that, after closing the ticket, the summary shows 0 Available and 4 Reserved.
5. Try clicking **Start** on any other headset: it is disabled because those kits are not available.

### Scenario B — No Kits Available

1. In the Main Menu, press **Reset**.
2. Press **CASE B**. The summary should show 0 Available, 4 Reserved, 1 Loaned and 1 Charging, and the subtitle reads "No headsets available right now."
3. Click any headset. A message explains that there are no more available Quest 3 headsets and shows the next available options with their status and the time left.
4. Select one of the suggested headsets, choose a time on the timeline and press **Confirm** to complete a working next step.

### Resetting the Application

Go to the Main Menu and press **Reset**. This deletes the saved data file, so the next time **CASE A** or **CASE B** is opened, every headset starts from its original state for that scenario. Use **Go to main menu** inside either scenario to return to the Reset button.

Loans and inventory are saved automatically when a loan is confirmed and are **kept when the application is closed and reopened**. The data is stored in a `headsets.json` file in Unity's persistent data folder (on Windows: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\InterfaceDesingProjectPartial2\`). Press **Reset** whenever you want to start a test from a clean state.

## Documentation

* [Design Justification](docs/design.md)
* [Testing Report](docs/testing.md)
* [AI Usage Log](docs/ai-use.md)

## Known Limitations

* The remaining time on each card is stored when the loan is made and **does not count down in real time**; a reserved kit does not become available again on its own. Use **Reset** to restore the inventory.
* A confirmed loan marks the kit as **Reserved** (with the time left until the return deadline); the app does not have a separate check-in/return flow.
* The loan length is chosen on the timeline in 30-minute slots, up to a maximum of 90 minutes. Selecting three consecutive slots is required for a full 90-minute loan.
* The kit contents are displayed for verification, but the app does not require ticking each item.
* **Send to email** is a prototype screen; no email is actually sent.
* The drop-off location shown on the ticket is a fixed text value set in the Unity project (`booth x`).
* Charging kits are stored with 0 minutes remaining, so in Scenario B they can appear as one of the "soonest available" options.
* The timeline skips weekends (except the current day) and shows five days at a time.
* Only a Windows build has been tested.

## Demo Video

[Link to the demo video]
