# Design Justification

## 1. Introduction

Quest Checkout is a Unity application designed to help users borrow Meta Quest 3 kits through a kiosk interface. The application allows users to review kit availability, select a kit, verify its contents, review their choices, and confirm a loan.

The interface is based on the original Figma design. The Unity implementation adds the functionality needed to manage kit availability, process loans, and handle situations in which no kits are available.

## 2. Interaction Decision 1: Kit Availability

### User Problem

Users need to identify which kits are available before attempting to borrow one. If availability is unclear, they may try to select a kit that cannot be borrowed.

### HCI Principle

**Visibility of System Status**

The interface follows the principle of visibility of system status by showing the current status of each Quest headset. Users can quickly see which headsets are available, reserved, loaned, or charging before making a selection.

### Design Solution

The interface displays six Quest headsets in a two-column grid. Each card includes the headset name, its current status, an image, and information about its availability. The summary at the top also shows the total number of headsets in each status category.

Available headsets are visually distinguished from unavailable ones. In the current implementation, Quest3 01 is marked as "Available" and has a pink "Start" button, while reserved, loaned, and charging headsets have gray cards and display their respective statuses. Reserved headsets also show the remaining time information.

This design helps users understand the current inventory at a glance and reduces the chance of selecting a headset that cannot be borrowed. By presenting the status information directly on each card, users do not need to open every headset individually to check whether it is available.

### Evidence

<img width="1081" height="660" alt="Interface" src="https://github.com/user-attachments/assets/70c77e77-729a-473e-8405-d5c003b7db36" />

## 3. Interaction Decision 2: Reviewing and Editing Choices

### User Problem

Users may need to correct or change their selections before confirming a loan. If the interface does not allow them to review and edit their choices, they may have to repeat previous steps or lose valid information. Additionally, when no headsets are available, users need to understand the situation and know what alternatives they can choose.

### HCI Principle

**User Control and Freedom**

This principle allows users to review their options and make decisions before confirming an action. It also supports giving users a clear way to choose an alternative when their original choice is no longer available.

### Design Solution

The interface displays a message when there are no more available Quest headsets, explaining the situation and directing users to the next available options. A pop-up shows alternative headsets, their current loan status, and the estimated time until they become available. Users can review these options and select one before pressing the "Confirm selection" button.

This design helps users understand why they cannot continue with their original choice and gives them an alternative instead of leaving them at a dead end. Showing the status and remaining time of each headset also helps users make an informed decision before confirming their selection.

### Evidence

<img width="515" height="688" alt="Screenshot 2026-10-12 at 7 07 55 PM" src="https://github.com/user-attachments/assets/6ad02a01-24ac-4dbe-9587-1d118ac23289" />


## 4. Interaction Decision 3: Preventing Invalid Kit Selection

### User Problem

Users may accidentally try to borrow a headset that is already reserved, loaned, or charging. If the interface does not clearly distinguish these headsets from available ones, users may become confused or attempt to complete a loan that cannot be fulfilled.

### HCI Principle

**Error Prevention**

This principle focuses on designing the interface to prevent problems before they happen, rather than only informing users after an error occurs.

### Design Solution

The interface displays each of the six Quest headsets in an individual card with its name and current status. Available headsets are visually distinguished from reserved, loaned, and charging headsets. In the current Unity implementation, the available headset has a pink "Start" button, while the other headsets display their respective statuses on gray cards.

This design helps users identify which headsets they can select and reduces the likelihood of attempting to borrow an unavailable device. The status labels and visual differences make the available options easier to recognize before starting the checkout process.

### Evidence

<img width="413" height="555" alt="Evidence" src="https://github.com/user-attachments/assets/292fbcda-9107-4d31-9131-147ce7dc939b" />



## 5. Comparison Between Figma and Unity

The original Figma design served as the visual and interaction reference for the Unity implementation. The main layout and visual style were maintained, while the Unity version includes functional elements needed to manage the headset inventory and handle different checkout scenarios.

## Main Differences

* **Functional headset status:** The Unity implementation displays six headset cards with their current statuses: available, reserved, loaned, or charging. It also includes a summary at the top showing the number of headsets in each category.
* **Interactive availability options:** The Unity version connects the headset cards to the checkout flow. The available headset displays a "Start" button, while unavailable headsets display their current status and relevant time information.
* **No-availability feedback:** The implementation includes a message that appears when no headsets are available. It presents alternative headsets, their loan status, estimated availability times, and a "Confirm selection" button to give users a possible next step.

### Reasons for the Changes

These changes support the functional requirements of the project and make the interface more useful beyond its initial visual design. Displaying the inventory summary and individual headset statuses helps users understand which devices can be borrowed without checking each one separately.

Connecting the cards to the checkout flow helps prevent users from attempting to borrow unavailable headsets. The no-availability message also provides feedback when the user's original choice cannot be fulfilled and presents alternatives instead of leaving the user without a clear next step.

These decisions support the HCI principles of visibility of system status, error prevention, and user control and freedom. The changes were verified with the functional tests (T1–T6) and the external usability tests documented in [`testing.md`](testing.md), which check that unavailable headsets cannot be selected incorrectly and that confirming an alternative works as intended.


## 6. Usability-Based Improvements

Three participants outside the team tested both scenarios without step-by-step guidance. Two problems found in those sessions were fixed and retested; the full before-and-after evidence is in [`testing.md`](testing.md).

### Improvement 1: [PENDING – short title of the fix]

* **Observed problem:** [PENDING – what the participant(s) did or misunderstood, and which HCI principle it affected]
* **Implemented solution:** [PENDING – what changed in the Unity interface]
* **Evidence:** [PENDING – before/after screenshots and the result of the retest]

### Improvement 2: [PENDING – short title of the fix]

* **Observed problem:** [PENDING – what the participant(s) did or misunderstood, and which HCI principle it affected]
* **Implemented solution:** [PENDING – what changed in the Unity interface]
* **Evidence:** [PENDING – before/after screenshots and the result of the retest]

## 7. Conclusion

The design decisions aim to make the borrowing process understandable and to help users complete their tasks with fewer errors. The final evaluation of these decisions is supported by the implemented interface and the usability testing results documented in `docs/testing.md`.
