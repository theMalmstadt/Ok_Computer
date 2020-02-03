# **OK Bracket Manager Inception**
## OK Computer - 2020

___

## **Summary of Our Approach to Software Development**

*Inception phase of SCRUM Development.*

## **Vision Statement 1.0**

For tournament organizers who need to organize multiple brackets at a single event, the OK Bracket Manager is an application that will provide a single page to view and manage matches across all brackets at once. Unlike other bracket management applications, our product will unify multiple brackets into a single page, and have an understanding of what matches should happen based on the needs of the event as a whole.

___

## **Initial Requirements Elaboration and Elicitation:**


### Interview QA:

Q: Do we want private and public events?

A: We want both. We want to be able to set event visibility.

Q: Do we want brackets to be pulled automatically from Challonge?

A: This does sound nice, but the organizer would have to specify the tournament bracket that they want to pull.

Q: How many results per page per search?

A: 10 is fine.

Q: How are ties handled in matches? Will our algorithm need to anticipate extra playtime?

A: Ties are handled fairly quickly with sudden deaths or coin tosses. I don’t think the algorithm should need to worry about ties. 

___

**The product is centered around “three” core features:** 

1. Create new tournaments or register existing tournaments from external services and communicate with them. Many features will be from [Challonge](https://challonge.com/)

2. Algorithmically, the application should schedule matches efficiently and handle all bracket movement based on match results.

3. View current, upcoming, and recent events in my area. Each event will have details associated that I can use to join or view results of.

## **List of Needs and Features**

1. An event organizer must create an account to add or manage any events or tournaments.

2. Create new events with relevant information such as organizer, date, and location with Google maps API integration. Organizers should also be able to decide the visibility of an event (public or private). If an event is private, only competitors with accounts can view the event.

3. Tournaments need to be added to events. Events need to have names, number of players, and competitors. There should be a list of templates that I can pull from.

4. Tournament info needs to be able to be imported from other sites such as challonge and work organically with other data imported or created through our application.

5. Standings in each tournament should be viewable by the public or accounts based on visibility. All results for past matches should also be viewable.

6. The next matches scheduled should be viewable by all users based on visibility. A competitor should know when they have been scheduled for their next match, and other competitors should see when they are scheduled.

7. The event organizer should be able to see what matches in the tournament are next and easily scheduled them. 

8. Algorithmically, a short list of possible schedules should be generated so that the event organizer can schedule the next match based on standing and availability of competitors. The algorithm needs to understand the bracket structure, and when each competitor is in a match so that they are not double booked.

9. The application should look smooth and be easy to navigate. Multi-tournament brackets can be confusing, so our application should be intuitive and easy to view the data that each user is looking for. Responsive and few page redirects.

10. There needs to be search functionality for finding upcoming events near a users location, which events have the tournaments that a user is looking for, and recent event results.

11. Competitors and viewers need an easy way to find where events will take place. This will need Google Maps integration and specifics of the location (building, floor, etc.).

___

## **Initial Modeling**

[UI DIagram 1](/Diagrams/UI_diagram_1.png)

[UI DIagram 2](/Diagrams/UI_diagram_2.png)

[Use Case Diagram](/Diagrams/use_case_diagram.png)

[ER Diagram](/Diagrams/ER_diagram.png)

___

## **Identify Non-Functional Requirements**

1. Default language will be English.
2. Data does not need to be stored indefinitely, max 4 weeks.
3. Cute 404 or other error handling pages.
4. All server errors need to be logged for investigation reasons.
5. Tournament organizers usernames and passwords should not expire, passwords should be 8 characters minimum in length, one capital letter and one symbol.

___

## **Identify Functional Requirements (User Stories)**

E: Epic  
U: User Story  
T: Task  

1. [E] As an event organizer, I want to create an account and create a new event.

    1. [U] As an event organizer, I want to create an account so that I can manage events. Accounts will be used for adding data such as events, tournaments, and competitors. We will also allow events to be marked at public or private, where only users with accounts can view them if they are private. Public events should be viewable by every user. Accounts should be easy to create and a user should know when they are logged in. The account should be able to store basic information such as a username and the account user’s spoken name. Spoken name should be optional. We don’t want bots to make a million accounts flooding our database, so their should be a captcha at the point of account registration.

        1. [1] Create ASP.Net Webapp
        2. [2] Create account database
        3. [3] Create login and register pages
        4. [4] Add captcha security to register page

2. [U] As an event organizer, I want to create events so that I can add tournaments to manage. Events should have a unique name. The event creator’s username will be the event organizer name stored with the event. The date and time need to be stored as well. There may be an event that spans multiple days so we will have the datetime indicate the start of the event. We will have a short description field available to indicate multi-day events or cover any other unusual event details. Visibility of the event will need to be decided upon. This should be easy since it only needs to be a switch; public = true, private = false. All of these options should be decided on one page and ask for confirmation before creating event. Only registered users can create accounts, so we will have the option to create an account available to everyone but indicate that a user must be logged in to create an event if they try to do so. The events will also need two attributes for location. An address stored in one space and a second for specific details such as which building or floor it will be at the address. (Though, note this will need to be included in the table but another epic will cover address entry).

    1. [T] Create database with event tables
    2. [T] Create page to enter event name, datetime(s), and other base info
    3. [T] Create indicator to tell user to login/register to create events

3. [U] As an event organizer, I want to add the location for my event so that competitors can easily find the event. This feature will need to be added to the create event page. There will be two fields available for adding an address to an event. The first address will be a single line address that will give the right address when put into Google Maps. There should be a way for the event organizer to verify that the address in Google Maps is the correct one. This could be by finding the location in maps, and getting that address or searching for the location after an address is entered where then the event organizer can confirm it is the correct address. The second field will allow the event organizer to specify details of the location. This field will be optional so null values will be accepted.

    1. [T] Add address entry to create event page
    2. [T] Address confirmation through Google Maps

4. [U] As a user, I want to be able to view events so that I know all relevant details of that event. This page will include all entered data for the event in a coherent and relevant way. Name of the event, organizer username, description, date, and time prominently show with other data easily located. We would like to integrate Google Maps in this story as well. We want at minimum a link that will take us to the address within google maps. If time allows, a panel with the displayed destination on a smaller map located to the side would be nice. All displayed data should take up only ½ to ⅔ of the screen as the bottom part will show all tournaments that will take place at the event. Tournament creation will be covered in a later epic but we do want to build this page with that feature in mind. This event will need to be findable on the site via searching or from a list of events. Best way to index this each event page is by event Id that will be stored in the database.

    1. [T] Easily show all event details on one page
    2. [T] Connect to Google Maps API to give direct address info and options

2. [E] As an event organizer, I want to register a new tournament.

3. [E] As an event organizer, I want to add event data easily, such as competitors and match results.

4. [E] As an event organizer, I want to pull info from an existing tournament through external sites and associate duplicate competitors.

5. [E] As an event organizer, I want to send data such as scheduled brackets to external sites.

6. [E] As an event organizer, I want a list of templates I can choose from when I set up tournaments or events.

7. [E] As an event organizer, I want to keep track of all competitor statuses (availability and standing).

8. [E] As an event organizer, I want a view of all matches that are completed and need to be completed to progress to the next level.

9. [E] As an event organizer, I want to easily select who will be in the next match and when it will take place.

10. [E] As an event organizer, I want an algorithm to generate schedules based on all competitors statuses.

11. [E] As a competitor, I want to see upcoming and recent events.

12. [E] As a competitor, I want to see event details such as location, tournaments, competitors, etc.
13. [E] As a competitor,  I want to be able to search for events based on locations or tournaments at that event.

14. [E] As a competitor, I want to view up to date standings for each active tournament in an event.

15. [E] As a competitor, I want to see my standings and when my next match in the event will be.

___

## **Initial Architecture Envisioning**

[Architecture Overview](/Diagrams/architecture_overview.png)

___

## **Identification of Risks**

1. Not all data may be easily imported from Challonge. If so, our application should be robust enough to pick up the workload that Challonge or other bracket sites would take on. This is also true for sending data to an external site.

2. The algorithm for optimal scheduling maybe unclear during development. We could attempt to implement a few different ones that each generate a schedule, but there may not be enough time and resources to do this.

3. The complexity of viewing multiple brackets may cause our application to be hard to understand and not offer the ease of use that we want to deliver.

___

## **Timeline and Release Plan**

Sprint 1 (2/17/20 - 3/1/20) - Creation tools for accounts, events, and tournaments

Sprint 2 (3/2/20 - 3/15/20) - View of tournaments for all users

Sprint 3 (April) - Match scheduling tools & pulling data from external sites

Sprint 4 (April) - Algorithmic scheduling

Sprint 5 (May) - Testing and refinement

Sprint 6 (May) - Testing and refinement

All features delivered in May.
