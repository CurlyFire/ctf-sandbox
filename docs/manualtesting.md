# Manual Test Procedure - CTF Sandbox Application

## Table of Contents

1. [Anonymous user Use Cases](#anonymous-user-use-cases)  
   1.1 [View emails](#11-view-emails)  
   1.2 [Sign up for a CTF Competition](#12-sign-up-for-a-ctf-competition)  
   1.3 [Confirm account](#13-confirm-account)  
   1.4 [Login account](#14-login-account)  
2. [Competitor Use Cases](#competitor-use-cases)  
   2.1 [Create team](#21-create-team)  
   2.2 [Invite member](#22-invite-member)  
   2.3 [Accept team invitation](#23-accept-team-invitation)  

---

## Anonymous user Use Cases

### 1.1 View emails

- **Role:** Anonymous 
- **Preconditions:**
- **Steps:**  
  1.  Open browser at ctf-sandbox url
  2.  Click the email enveloppe
- **Expected Result:**
  1.  Mailpit's UI should be displayed

### 1.2 Sign up for a CTF Competition

- **Role:** Anonymous 
- **Preconditions:**
  1.  [View emails
   ](#11-view-emails) 
- **Steps:**  
  1.  Open browser at ctf-sandbox url
  2.  Click the Register link
  3.  Enter a handle in a valid email format
  4.  Enter a access code with at least 6 characters
  5.  Enter same access code in Verify Access Code
  6.  Click create account
- **Expected Result:**
  1.  A register confirmation page should appear
  2.  A confirmation email should be received

### 1.3 Confirm account

- **Role:** Anonymous 
- **Preconditions:**
  1.  [Sign up for a CTF Competition](#12-sign-up-for-a-ctf-competition)   
- **Steps:**  
  1.  Open browser at ctf-sandbox url
  2.  Click the email enveloppe
  3.  Click on on the confirmation link in the confirmation email

- **Expected Result:**
  1.  A message from ctf-sandbox saying that your email is confirmed

### 1.4 Login account

- **Role:** Anonymous 
- **Preconditions:**
  1.  [Confirm account
   ](#13-confirm-account)   
- **Steps:**  
  1.  Open browser at ctf-sandbox url
  2.  Click the login link
  3.  Enter credentials
  4.  Click authenticate

- **Expected Result:**
  1.  Logged in
  2.  Teams and challenges links are available
  3.  Logout link is available
  4.  Account link is available (handle name)

## Competitor Use Cases

### 2.1 Create team

- **Role:** Competitor 
- **Preconditions:**
  1.  [Login account
  ](#14-login-account)  
- **Steps:**  
  1.  Click Teams link
  2.  Click Create New Team
  3.  Enter name that is not empty
  4.  Click Create
- **Expected Result:**
  1.  Team should be displayed with owner handle, creation date and invite members link

### 2.2 Invite member

- **Role:** Competitor 
- **Preconditions:**
  1.  Another account was created to be invited
  2.  [Login account
  ](#14-login-account)
- **Steps:**  
  1.  Click Teams link
  2.  Click invite members on team
  3.  Enter another account's email address
  4.  Click send invitation
- **Expected Result:**
  1.  Another account has received an invitation email

### 2.3 Accept team invitation

- **Role:** Competitor 
- **Preconditions:**
  1.  [Invite member
  ](#22-invite-member)
  2.  [Login account
  ](#14-login-account) as invitee
- **Steps:**  
  1.  Click the email enveloppe
  2.  Click invitation link
  3.  Click accept button
- **Expected Result:**
  1.  Team members has the invitee displayed
---
