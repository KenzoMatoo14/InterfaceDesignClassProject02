# Testing Report

## 1. Tested Build

* Application: Quest Checkout
* Unity version: Unity 6.5 (6000.5.4f1)
* Build or Release: GitHub Release `Release` (tag `Release`) – https://github.com/KenzoMatoo14/InterfaceDesignClassProject02/releases/tag/Release
* Commit or version tested: `3a85061` (full hash `3a85061f55390853a4675d8e1a73a5bbb163e226`, branch `main`, merge of pull request #4)
* Test date: October 12 2026

## 2. Functional Testing

**Initial inventory used in the tests (after pressing Reset):**

| Headset | Scenario A (Case A) | Scenario B (Case B) |
|---|---|---|
| Quest3 01 | Available | Reserved (90 min) |
| Quest3 02 | Reserved (90 min) | Reserved (90 min) |
| Quest3 03 | Reserved (35 min) | Reserved (35 min) |
| Quest3 04 | Reserved (30 min) | Reserved (30 min) |
| Quest3 05 | Loaned (55 min) | Loaned (55 min) |
| Quest3 06 | Charging | Charging |
| **Summary** | 1 Available / 3 Reserved / 1 Loaned / 1 Charging | 0 Available / 4 Reserved / 1 Loaned / 1 Charging |

### T1 — Checkout

**Setup:** Main Menu, **Reset** pressed, then **CASE A** opened. Inventory is 1 Available (Quest3 01), 3 Reserved, 1 Loaned, 1 Charging.

**Steps:**

1. Click **Start** on Quest3 01. The reservation panel opens with the device name, its status and the kit contents.
2. On the timeline, select three consecutive free 30-minute cells (90 minutes). The **Confirm** button becomes enabled.
3. Press **Confirm** (loan confirmed).

**Expected result:** A valid 90-minute loan is completed, the inventory is updated, and the correct pickup location and return deadline are displayed.

**Actual result:** The confirmation ticket appeared with the selected pick-up time, a return deadline 90 minutes later and the drop-off location. After closing the ticket, Quest3 01 appeared as Reserved and the summary changed to 0 Available / 4 Reserved / 1 Loaned / 1 Charging.

**Status:** Pass

**Evidence:** 

<img width="573" height="763" alt="T1" src="https://github.com/user-attachments/assets/781fa5f7-9ad6-44b3-beb8-ad5920dbf04b" />


### T2 — No Availability

**Setup:** Main Menu, **Reset** pressed, then **CASE B** opened. Inventory is 0 Available, 4 Reserved, 1 Loaned, 1 Charging.

**Steps:**

1. Click a headset card to attempt to start the borrowing process.
2. Read the message that appears.
3. Select one of the suggested headsets, choose a time on the timeline and press **Confirm**.

**Expected result:** The application explains that no kits are available, blocks an invalid loan, and provides a functional next step.

**Actual result:** The main screen showed "No headsets available right now." and no **Start** button was enabled. Clicking a card opened a message explaining that there are no more available Quest 3 headsets, together with the next available options, their status and the time left. Selecting one of them opened its reservation timeline, and confirming produced a valid ticket.

**Status:** Pass

**Evidence:** 

<img width="568" height="762" alt="T2" src="https://github.com/user-attachments/assets/cb344a88-ef15-4d1e-9a05-a68c06193d8c" />


### T3 — Invalid Kit

**Setup:** **Reset** pressed and **CASE A** opened. Quest3 02 (Reserved), Quest3 05 (Loaned) and Quest3 06 (Charging) are unavailable kits. A past or already occupied time on the timeline is also tested as an invalid selection.

**Steps:** Try to click **Start** on Quest3 02, Quest3 05 and Quest3 06. Then open the available kit (Quest3 01), try to click red (occupied/past) cells, and try to press **Confirm** before selecting any time.

**Expected result:** The invalid loan cannot be confirmed and the inventory remains correct.

**Actual result:** The **Start** button of every unavailable kit is disabled and shows its remaining time instead of "Start". Red cells on the timeline cannot be selected and the **Confirm** button stays disabled until a free time is selected. The inventory counters did not change.

**Status:** Pass

**Evidence:** 

<img width="570" height="762" alt="T3" src="https://github.com/user-attachments/assets/e8482123-9a51-4237-ae74-0dbfc3078d27" />


### T4 — Editing Choices

**Setup:** **Reset** pressed, **CASE A** opened, and the reservation panel of Quest3 01 is open with a valid time already selected (not yet confirmed).

**Steps:**

1. Select a 90-minute block (three cells).
2. Click an end cell to remove it, then click an adjacent free cell to extend it again.
3. Click a free cell on another day to start a new selection.
4. Confirm the final selection.

**Expected result:** The user can edit their choices without losing valid information.

**Actual result:** The selection changed as the cells were clicked (removing an end, extending the block, or moving to another day) while the headset information stayed on screen. The ticket showed the final selected time, not the earlier ones.

**Status:** Pass

**Evidence:** 

<img width="572" height="771" alt="T4" src="https://github.com/user-attachments/assets/44b67974-fdaf-4003-bd06-c13743d8b2c5" />


### T5 — Persistence

**Setup:** **Reset** pressed and **CASE A** opened with the known initial inventory. A valid 90-minute loan of Quest3 01 is completed.

**Steps:**

1. Record the inventory and loan information (counters, headset statuses, pick-up and return times).
2. Close the application.
3. Reopen it.
4. Compare the saved information with the original records.

**Expected result:** The loan and inventory remain correct after restarting the application.

**Actual result:** After reopening the application and entering Case A (without pressing Reset), Quest3 01 was still Reserved and the counters matched the values recorded before closing.

**Status:** Pass

**Evidence:** 

<img width="575" height="762" alt="T5-1" src="https://github.com/user-attachments/assets/138441f2-e2d4-4205-aa86-9caff05dfbcd" /> <img width="572" height="766" alt="T5-2" src="https://github.com/user-attachments/assets/5664a188-b8b2-440c-877c-b14be08c715e" />



### T6 — Scenario Reset

**Setup:** The application has a saved state with a confirmed loan (Case A shows 0 Available) and a saved loan made from Case B.

**Steps:** In the Main Menu, press **Reset**. Open **CASE A** and check the inventory, then return with **Go to main menu**, open **CASE B** and check the inventory again.

**Expected result:** The controls restore the intended test states and allow both scenarios to be tested again.

**Actual result:** After **Reset**, Case A returned to 1 Available / 3 Reserved / 1 Loaned / 1 Charging (Quest3 01 available again) and Case B returned to 0 Available / 4 Reserved / 1 Loaned / 1 Charging. Both scenarios could be repeated.

**Status:** Pass

**Evidence:** 

<img width="570" height="762" alt="WhatsApp Image 2026-10-12 at 21 29 56" src="https://github.com/user-attachments/assets/2b1965ec-aa81-49d1-81d5-92741dab9dfd" />
<img width="592" height="772" alt="WhatsApp Image 2026-10-12 at 21 29 56 (1)" src="https://github.com/user-attachments/assets/4b79f717-24e3-4c76-9de1-d20851a1f3ec" />
<img width="573" height="762" alt="WhatsApp Image 2026-10-12 at 21 30 51" src="https://github.com/user-attachments/assets/864f2d63-03e6-4c1e-ab2c-5b4a16cd8bb8" />

## 3. External Usability Testing

Three participants outside the team attempted Scenario A and Scenario B without step-by-step instructions. Participants are identified using anonymous IDs.

<img width="1164" height="153" alt="External usability test" src="https://github.com/user-attachments/assets/dd903722-07d7-43a6-b624-b29b22b53afa" />

**Summary of results**

* All six attempts (3 participants × 2 scenarios) were completed, and **no participant needed help**.
* Completion times: Scenario A – 29 s, 50 s and 43 s (average about 41 s); Scenario B – 34 s, 38 s and 41 s (average about 38 s).
* Even though everyone completed the tasks, the observations show four points of friction:
  * **#1 (Scenario A):** did not understand that the "Receipt sent to email" message meant the reservation had already been created.
  * **#2 (Scenario A):** struggled to select the desired time for the reservation on the timeline.
  * **#1 (Scenario B):** expected the system to suggest the next available headset automatically.
  * **#3 (Scenario B):** was unsure how to proceed after learning that no headsets were available.

## 4. Usability Problem 1

**Observed problem:** [PENDING – what happened during testing, which participant(s)]

**Evidence before the change:** [PENDING – screenshot and observation]

**Implemented improvement:** [PENDING – the change made in Unity]

**Retest procedure:** [PENDING – how the improved version was tested]

**Result after the change:** [PENDING]

**Evidence after the change:** [PENDING – screenshot and observation]

## 5. Usability Problem 2

**Observed problem:** [PENDING – what happened during testing, which participant(s)]

**Evidence before the change:** [PENDING – screenshot and observation]

**Implemented improvement:** [PENDING – the change made in Unity]

**Retest procedure:** [PENDING – how the improved version was tested]

**Result after the change:** [PENDING]

**Evidence after the change:** [PENDING – screenshot and observation]

## 6. Limitations and Unresolved Issues

* The remaining time of a kit is saved when the loan is made and does not count down in real time, so a reserved kit does not return to Available by itself; the **Reset** button is used to restore the inventory.
* The loan length is selected on the timeline in 30-minute slots (maximum 90 minutes). A full 90-minute loan requires selecting three slots.
* Kit contents are displayed for verification, but the user is not required to confirm each item.
* **Send to email** is a prototype screen and does not send a real email.
* The drop-off location on the ticket is a fixed text value (`booth x`).
* Charging kits are stored with 0 minutes remaining, so in Scenario B they can be listed among the "soonest available" options.
* Testing was done only on Windows with three external participants; other platforms and screen sizes were not verified.
