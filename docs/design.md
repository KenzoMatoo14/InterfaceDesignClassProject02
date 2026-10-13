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

<img width="515" height="688" alt="Screenshot 2026-10-12 at 7 07 55 PM" src="https://github.com/user-attachments/assets/6ad02a01-24ac-4dbe-9587-1d118ac23289" />


## 4. Interaction Decision 3: No Kits Available

### User Problem

When no kits are available, users need to understand why they cannot continue and what they can do next.

### HCI Principle

[Insert the HCI principle that best describes the implemented design.]

### Design Solution

[Explain how the application communicates the lack of availability and provides a functional next step.]

### Evidence

[Insert a screenshot of the actual no-availability scenario.]

## 5. Comparison Between Figma and Unity

The original Figma design served as the visual and interaction reference for the Unity implementation.

### Main Differences

* [Describe the first actual difference.]
* [Describe the second actual difference.]
* [Describe any additional relevant differences.]

### Reasons for the Changes

[Explain why each meaningful change was necessary, referring to implementation requirements or usability testing evidence.]

## 6. Usability-Based Improvements

[Describe any relevant changes made after usability testing. Explain the observed problem, the implemented solution, and the evidence supporting the change.]

## 7. Conclusion

The design decisions aim to make the borrowing process understandable and to help users complete their tasks with fewer errors. The final evaluation of these decisions will be supported by the implemented interface and the usability testing results documented in `docs/testing.md`.
