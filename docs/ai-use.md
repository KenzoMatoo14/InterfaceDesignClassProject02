# AI Use Declaration

## 1. Purpose

This document records how AI tools were used during the development of the HCI Quest Checkout project. Each team member is responsible for documenting their own AI use, including the tasks where AI provided assistance, the content or code that was used, and how the results were reviewed and tested.

AI was used as a support tool. Team members remain responsible for understanding, reviewing, and verifying the work they submit.

## 2. Team Member Declarations

### Team Member 1

**Name:** Kenzo Matoo López
**Main contribution:** Implementation of the Quest Headset Manager interface in Unity (UI screens and interaction logic).

* **AI tool:** Claude (Claude Code, Opus 5.5 model, Claude desktop app)
* **Date used:** 2026-10-04 to 2026-10-10
* **Purpose:** Get help implementing my UI design in Unity and programming its interaction logic.
* **Prompt or request:** I asked step by step how to set up the Canvas at my design's dimensions, for scripts to manage the headset statuses, for a timeline to select reservation times generated from the current date and time, and for the flow between screens (reservation panel → confirmation ticket → send to email → back to the main screen). I also sent screenshots of visual bugs and asked for fixes.
* **AI response used:** Guidance for the Canvas Scaler, Game view resolution and UI styling. C# scripts: `QuestHeadsetManager.cs` and `HeadsetCard.cs` (headset statuses and counters), `ReservationTimeline.cs` (day × time grid generated at runtime, with selection of up to 3 consecutive 30-minute slots), `ReservationPanel.cs` (shows the selected headset's data) and `ConfirmationTicket.cs` (pick-up and return times, send-to-email panel, and updating the headset to Reserved). It also diagnosed layout problems by reading my scene file.
* **Changes made:** I built the scene hierarchy and prefabs in the Unity Editor and configured the components and values in the Inspector. I defined the behavior rules: always-pink buttons, no countdown timer, past hours in red, a maximum of 90 minutes per reservation.
* **Verification:** Claude could not run Unity, so I tested every change myself in Play mode. When something failed (the timeline overflowing the panel, the layout breaking when switching headsets, a booking marking too many hours as occupied), I reported it with screenshots and asked for corrections until the full flow worked as designed.
* **Related project files or work:** `Assets/Scripts/` (the 5 scripts above), `Assets/Prefabs/` (Cell, HourLabel, DayLabel), `Assets/Scenes/MainMenu.unity`, `Assets/Scenes/CaseA.unity`, `Assets/Scenes/CaseB.unity`

#### Additional AI Use

No additional AI use. All AI assistance for my part of the project is summarized in AI Use Record 1.

### Team Member 2

**Name:** Alberto Stephen Dubin Hernández
**Main contribution:** [PENDING – describe your main responsibility in the project]

#### AI Use Record 1

* **AI tool:** [PENDING – ChatGPT, GitHub Copilot, Claude, etc.]
* **Date used:** [PENDING – YYYY-MM-DD]
* **Purpose:** [PENDING – What were you trying to accomplish?]
* **Prompt or request:** [PENDING – Copy the prompt or summarize what you asked the tool]
* **AI response used:** [PENDING – Describe the code, explanation, suggestion, or other output you used]
* **Changes made:** [PENDING – Explain what you modified or adapted before using the result]
* **Verification:** [PENDING – Explain how you checked that the result worked correctly]
* **Related project files or work:** [PENDING – File names, scripts, scenes, documentation, or other relevant work]

#### Additional AI Use

[PENDING – Add another AI Use Record for each significant use of AI. If you did not use AI, explicitly state that.]


### Team Member 3

**Name:** Fátima Sofía Walls Fernández

**Main contribution:** Project documentation, including the README, design justification, testing documentation, video organization, AI use declaration, and coordination of the documentation deliverables.

#### AI Use Record 1

* **AI tool:** ChatGPT (OpenAI)
* **Date used:** October 11, 2026
* **Purpose:** To organize and prepare the project documentation required for the HCI Quest Checkout project.
* **Prompt or request:** Asked ChatGPT for help structuring and drafting the project documentation, including the design justification, testing report, README, and AI use declaration, based on the project requirements.
* **AI response used:** Used suggestions for organizing the documentation, structuring the required sections, identifying the information needed for the design and testing reports, and preparing documentation templates.
* **Changes made:** Adapted the suggested structure and content to our project's requirements, the team's responsibilities, and the actual implementation status. Project-specific details and test results were completed based on our team's work.
* **Verification:** Compared the suggested documentation structure with the project instructions and filled the content to ensure it matched the team's actual work. Implementation details and testing results are documented based on evidence from the project rather than assumed.
* **Related project files or work:** `README.md`, `docs/design.md`, `docs/testing.md`, and `docs/ai-use.md`.

#### Additional AI Use

No additional AI tools were used for my project contribution. My use of AI was limited to assistance with documentation and organizing the required deliverables (AI Use Records 1 and 2). I did not use AI to generate or modify the Unity implementation or to perform the project's functional tests.


## 3. General Notes

* AI assistance was reviewed by the team members responsible for the related work.
* Code and functionality were checked through implementation and testing where applicable.
* Any limitations, errors, or suggestions that were not used should be mentioned when relevant.
* Each team member is responsible for the accuracy of their own declaration.
