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
**Main contribution:** I was in charge of the case B, I created a new canvas panel to add the parts that are specific to that case. I implemented the system of comparison between all the cards and pulling the main 2 with the shortest availability time, we implemented that because of the question, what if I need it as soon as possible? and I implemented the format of time on hours and minutes instead of just minutes.

#### AI Use Record 1

* **AI tool:** Claude Sonnet 5.5
* **Date used:** 2026-10-12
* **Purpose:** Documentation and teammate code understanding.
* **Prompt or request:** I asked the AI to not give me the code because I wanted to code, but if I asked a question give me a documentation style answer, and if I asked something with my code to give me a answer with an example that has no relation to my code. Only thing that I was allowed to ask the AI for code was when I had errors on syntax and I couldn't found why.
* **AI response used:**  This is literally a response I used to upgrade the code. 
Here’s the approach, using your own code as the map.

1. Detect the scene number

Unity’s SceneManager (namespace UnityEngine.SceneManagement, the same one SceneChange.cs already imports) gives you the active scene. From it you read buildIndex (the number) or name.

Generic example, unrelated to your project:

csharp
int index = SceneManager.GetActiveScene().buildIndex;
if (index == 2) { /* shop behavior */ } else { /* default behavior */ }

Two things to check:

The index comes from File > Build Settings > Scenes In Build. Your SceneChange already uses LoadScene(1) and LoadScene(2), so make sure the new scene is actually listed there at the position you expect. If it isn’t in the list, the index is -1.
Read it once, in Start() of QuestHeadsetManager, and store it in a field instead of calling it on every click.

* **Changes made:** Basically, I started to do trial and error. I tried to implement the stuff without help, then if I couldn't I asked Claude as you saw before. Then I tried to implement the changes and check if it worked, I gave it another go until it worked. (I did this mainly to understand more deeply what I was doing.) 
* **Verification:** I started the typical user flow and if the thing that I wanted happened on a scientific completion (aka repeating it a bunch of times) and if the result didn't change, I was satisfied with the results, also each try I prioritized changing something, and two big changes I asked my little brother to reserve a Quest and if nothing happened I consider it done. 
* **Related project files or work:** I created 2 scripts, RecomendPage and SceneChange, also I modified QuestHeadsetManager. I created the full scene of case B, also implemented with the Figma reference the recommendation panel, I tried my best to document my code. I gave the instructions that made my brother discover the bug. 

#### Additional AI Use

No additional AI use. All AI assistance for my part of the project is summarized in AI Use Record 1.


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
