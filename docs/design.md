# Design Justification

## 1. Introduction

Quest Checkout is a Unity application designed to help users borrow Meta Quest 3 kits through a kiosk interface. The application allows users to review kit availability, select a kit, verify its contents, review their choices, and confirm a loan.

The interface is based on the original Figma design. The Unity implementation adds the functionality needed to manage kit availability, process loans, and handle situations in which no kits are available.

## 2. Interaction Decision 1: Kit Availability

### User Problem

Users need to identify which kits are available before attempting to borrow one. If availability is unclear, they may try to select a kit that cannot be borrowed.

### HCI Principle

[Insert the HCI principle that best describes the implemented design.]

### Design Solution

[Explain how the interface communicates each kit's availability and how users can distinguish available kits from unavailable ones.]

### Evidence

[Insert a screenshot of the actual Figma or Unity interface.]

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
