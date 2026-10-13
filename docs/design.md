# Design Justification

## 1. Introduction

Quest Checkout is a Unity application designed to help users borrow Meta Quest 3 kits through a kiosk interface. The application allows users to review kit availability, select a kit, verify its contents, review their choices, and confirm a loan.

The interface is based on the original Figma design. The Unity implementation adds the functionality needed to manage kit availability, process loans, and handle situations in which no kits are available.

## 2. Interaction Decision 1: Kit Availability

### User Problem

Users need to identify which kits are available before attempting to borrow one. If availability is unclear, they may try to select a kit that cannot be borrowed.

### HCI Principle

Visibility of System Status

The interface follows the principle of visibility of system status by showing the current status of each Quest headset. Users can quickly see which headsets are available, reserved, loaned, or charging before making a selection.

### Design Solution

The interface displays six Quest headsets in a two-column grid. Each card includes the headset name, its current status, an image, and information about its availability. The summary at the top also shows the total number of headsets in each status category.

Available headsets are visually distinguished from unavailable ones. In the current implementation, Quest3 01 is marked as "Available" and has a pink "Start" button, while reserved, loaned, and charging headsets have gray cards and display their respective statuses. Reserved headsets also show the remaining time information.

This design helps users understand the current inventory at a glance and reduces the chance of selecting a headset that cannot be borrowed. By presenting the status information directly on each card, users do not need to open every headset individually to check whether it is available.

### Evidence

<img width="1081" height="660" alt="Interface" src="https://github.com/user-attachments/assets/70c77e77-729a-473e-8405-d5c003b7db36" />

## 3. Interaction Decision 2: Reviewing and Editing Choices

### User Problem

Users may need to correct or change their selections before confirming a loan. If the interface does not allow them to review and edit their choices, they may have to repeat previous steps or lose valid information.

### HCI Principle

[Insert the HCI principle that best describes the implemented design.]

### Design Solution

[Explain how users review their choices, edit them, and preserve valid information.]

### Evidence

[Insert a screenshot of the actual review or editing interface.]

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
