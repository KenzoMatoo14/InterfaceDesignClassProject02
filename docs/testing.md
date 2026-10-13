# Testing Report

## 1. Tested Build

* Application: Quest Checkout
* Unity version: Unity 6.5 (6000.5.4f1)
* Build or Release: [Insert exact identifier and link]
* Commit or version tested: [Insert identifier]
* Test date: October 12 2026

## 2. Functional Testing

### T1 — Checkout

**Setup:** [Initial inventory and application state.]

**Steps:**

1. [First action.]
2. [Second action.]
3. [Confirm the loan.]

**Expected result:** A valid 90-minute loan is completed, the inventory is updated, and the correct pickup location and return deadline are displayed.

**Actual result:** [Record the observed result.]

**Status:** [Pass / Fail]

**Evidence:** [Screenshot or evidence link.]

### T2 — No Availability

**Setup:** [Prepare the application with no available kits.]

**Steps:**

1. [Attempt to start the borrowing process.]
2. [Observe the message.]
3. [Test the available next step.]

**Expected result:** The application explains that no kits are available, blocks an invalid loan, and provides a functional next step.

**Actual result:** [Record the observed result.]

**Status:** [Pass / Fail]

**Evidence:** [Screenshot or evidence link.]

### T3 — Invalid Kit

**Setup:** [Prepare an invalid or unavailable kit.]

**Steps:** [Record the actions used to attempt the invalid loan.]

**Expected result:** The invalid loan cannot be confirmed and the inventory remains correct.

**Actual result:** [Record the observed result.]

**Status:** [Pass / Fail]

**Evidence:** [Screenshot or evidence link.]

### T4 — Editing Choices

**Setup:** [Prepare a valid selection before confirmation.]

**Steps:** [Record the actions used to review and edit the selection.]

**Expected result:** The user can edit their choices without losing valid information.

**Actual result:** [Record the observed result.]

**Status:** [Pass / Fail]

**Evidence:** [Screenshot or evidence link.]

### T5 — Persistence

**Setup:** [Prepare a known inventory state and complete a valid loan.]

**Steps:**

1. Record the inventory and loan information.
2. Close the application.
3. Reopen it.
4. Compare the saved information with the original records.

**Expected result:** The loan and inventory remain correct after restarting the application.

**Actual result:** [Record the observed result.]

**Status:** [Pass / Fail]

**Evidence:** [Screenshots before and after restarting.]

### T6 — Scenario Reset

**Setup:** [Record the initial state.]

**Steps:** [Test the reset controls for Scenario A and Scenario B.]

**Expected result:** The controls restore the intended test states and allow both scenarios to be tested again.

**Actual result:** [Record the observed result.]

**Status:** [Pass / Fail]

**Evidence:** [Screenshot or evidence link.]

## 3. External Usability Testing

Three participants outside the team attempted Scenario A and Scenario B without step-by-step instructions. Participants are identified using anonymous IDs.

<img width="1164" height="153" alt="External usability test" src="https://github.com/user-attachments/assets/dd903722-07d7-43a6-b624-b29b22b53afa" />


## 4. Usability Problem 1

**Observed problem:** [Describe what happened during testing.]

**Evidence before the change:** [Screenshot and observation.]

**Implemented improvement:** [Describe the change.]

**Retest procedure:** [Describe how the improved version was tested.]

**Result after the change:** [Record the actual result.]

**Evidence after the change:** [Screenshot and observation.]

## 5. Usability Problem 2

**Observed problem:** [Describe what happened during testing.]

**Evidence before the change:** [Screenshot and observation.]

**Implemented improvement:** [Describe the change.]

**Retest procedure:** [Describe how the improved version was tested.]

**Result after the change:** [Record the actual result.]

**Evidence after the change:** [Screenshot and observation.]

## 6. Limitations and Unresolved Issues

[Document known problems, limitations, and any requirements that could not be verified.]
