# **OK Bracket Manager Inception**
## OK Computer - 2020

___

## **Summary of Our Approach to Software Development**

*Inception phase of SCRUM Development.*

## **Vision Statement 1.0**

For tournament organizers who need to organize multiple brackets at a single event, the OK Bracket Manager is an application that will provide a single page to view and manage matches across all brackets at once. Unlike other bracket management applications, our product will unify multiple brackets into a single page, and have an understanding of what matches should happen when based on the needs of the event as a whole.

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
A: In our games, ties aren’t really allowed by the game state. I don’t think the algorithm should need to worry about ties, and if they did occur, challonge would likely handle them.
 
Q: Are there tournaments which involve more than two players per match?  
A: Yes, team formatted tournaments will have more than 2 players, but each player will be tracked as a single entity, we will handle this in our database by having players and teams be nullable entities in the match table.

Q: How many different types of brackets does this application need to be able to work with?  
A: This application will need to primarily need to run double elimination. It would be cool for it to run single elimination, swiss and round robin. But double elimination is the most common and will give the most utility. Challonge will likely accommodate a lot of bracket related functionality. 

Q: Could this application replace Challonge? What does Challonge offer that we cannot create?  
A: Challonge offers nifty features that are handy for the tournament organizer that I would like to use. Features like skill keeper. Challonge is missing the multiple bracket, and scheduling features. That’s why this app is needed. 

Q: Are stations dedicated to a single tournament?  
A: Stations will mostly be used for multiple games during the day, but should stay on an assigned game until that bracket is complete, or the station is no longer needed for that bracket.

Q: Will the location for a match within a tournament be the same for all matches, or will matches within a tournament vary from match to match?  
A: The event as a whole will be held in one location, stations can be stored abstractly (ie Station A, station B) and those will just correlate to the on-site stations.

Q:Will functionality for non-tournament organizers users to register accounts exist? If so, what functionality will it offer?  
A: It would be secondary, but this could offer viewing private events, and a competitor tracking their tag during an event to know specifically when their own matches should happen.

Q:Will we be able to stay up to date with challonge?  
A: If we design our program properly, we think we can use current tournament standings to produce a relevant, and efficient schedule throughout an event as a whole.

Q:What processes might be usable to a tournament organizer in generating a schedule?  
A: A TO would want to do things like make sure that certain events don’t conflict with one another, or make sure that an event’s matches have a higher priority than others, or to efficiently work through as many events as possible.

Q: Will we be able to update match scheduled times on challonge for people to be able to see their schedule on the platform they already use?  
A: Unfortunately the challonge API does not support updating this property, so we are just going to show them what they are missing.

___

## **Initial Modeling**

[Home Page UI DIagram](diagrams/UI_diagram_1.png)

[Search Page UI DIagram](diagrams/UI_diagram_2.png)

[Use Case Diagram](diagrams/use_case_diagram.png)

[ER Diagram](diagrams/ER_diagram.png)

[Architecture](diagrams/architecture_overview.png)
___

## **The product is centered around “three” core features:** 

1. Register existing tournaments from external services and communicate with them. Many features will be from [Challonge](https://challonge.com/,) but we may implement features to import other types of brackets later on.

2. Algorithmically, the application should schedule matches efficiently and handle all bracket movement based on match results.

3. View current, upcoming, and recent events in my area. Each event will have details associated that I can use to join or view results of.

## **List of Needs and Features**

1. An event organizer must create an account to add or manage any events or tournaments.

2. Create new events with relevant information such as organizer, date, and location with Google maps API integration. Organizers should also be able to decide the visibility of an event (public or private). If an event is private, only competitors with accounts can view the event.

3. Tournaments need to be added to events. Events need to have names, number of players, and competitors. There should be a list of templates that I can pull from.

4. Tournament info needs to be able to be imported from other sites such as challonge and work organically with other data imported or created through our application.

5. Standings in each tournament should be viewable by the public or accounts based on visibility. All results for past matches should also be viewable.

6. The next matches scheduled should be viewable by all users based on visibility. A competitor should know when they have been scheduled for their next match, and other competitors should see when they are scheduled.

7. The event organizer should be able to see what matches in the tournament are available and easily scheduled them. 

8. The event organizer should be able to start timers attached to both tournaments (to help keep the event on schedule), and competitor’s busy state to know how long they have been unavailable (primarily for disqualification purposes.)

9. Algorithmically, a short list of possible schedules should be generated so that the event organizer can schedule the next match based on standing and availability of competitors. The algorithm needs to understand the bracket structure, and when each competitor is in a match so that they are not double booked.

10. The application should look smooth and be easy to navigate. Multi-bracket events can be confusing, so our application should be intuitive and easy to view the data that each user is looking for. Responsive and few page redirects.

11. There needs to be search functionality for finding upcoming events near a users location, which events have the tournaments that a user is looking for, and recent event results.

12. Competitors and viewers need an easy way to find where events will take place. This will need Google Maps integration and specifics of the location (building, floor, etc.).

13. If an internet connection is lost, the Event page should still display the last available set of matches.

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

1. [E] As a user of this web application, I would like a modern sleek looking homepage that introduces me to the site and shows me all the features currently available.
    
    1. [U] **Title:**  
        As a visitor to this site, I would like to register as a Tournament Organizer and then be able to login as well.
 
        **Assumptions/Preconditions:**  
            There is a DB that handles logging in and registering.
 
        **Description:**  
            This is one of the most important parts of this project. Being able to login and register as a tournament organizer is one of key fundamental tools of this application. We want someone to be able to come to our website and then proceed to make an account to host events and tournaments. This capability will be located within the navbar on every page and upon clicking login or register it will take the user to the respective page.
        
        **Core needs:**
        1. Bootstrap 4 navbar
        2. Uniform page styles
        3. Error handling
        4. Bot protection
 
        **Tasks:**
        1. [T] Enable user sign-ons.
        2. [T] Make sure navbar links redirect to nice easy on the eyes login and register pages.
        3. [T] Change homepage layout if the user is login as a tournament organizer to display the tournaments they have created.
        4. [T] Setup reCAPTCHA on the register page.
 
    2. [U] **Title:**  
        As a visitor to this site, I would like to be able to search for events.
 
        **Assumptions/Preconditions:**  
            There is a DB that is storing events and is queryable. There is a search page that queries the events DB.
 
        **Description:**  
            When a user comes to our web application, we want them to be able to quickly search and find events that they are interested in. A search by city and state will be handled on the navbar. When a user searches for something it will redirect the user to another page of search results.
 
        **Core Needs:**
        1. Search functionality in navbar.
        2. View page of search results.
 
        **Tasks:**
        1. [T] Create search function in navbar that allows input for 2 words, a city and a state.
        2. [T] Make it so when they click search it will send them to a search results page.
 
    3. **[U] Title:**  
        As a visitor to this site, I would like to see upcoming events
 
        **Assumptions/Preconditions:**  
            There is a home page. There is a DB that holds events with starting times. There is a DB for tournament data.
        
        **Description:**  
            On the homepage we want to display a paged, 10 items per page, list of upcoming events, displaying date and time when they start and their locations. This list will be able to be filtered by typing in a zip code to search for events within that zip code. When a user clicks on an event on this list it will seamlessly display the tournaments going on in the event. 
    
        **Core Needs:**
        1. A nice looking block section on the homepage that displays events by 10 per list.
        2. When you click on an event it should tell you what tournaments are happening at that event below the list box, using AJAX.
        3. Able to filter events by typing in a zip code.
 
        **Tasks:**
        1. [T] Create a box on the homepage that displays 10 events per page of the box
        2. [T] Add filter option to filter by zip code.
        3. [T] Setup AJAX to handle tournament details when a Tournament is clicked.
        4. [T] If there is no filter the list should be listed in alphabetical order.
 
    4. [U] **Title:**  
        As an Event Organizer, I want to the home page to display a list of events that I have planned.
 
        **Assumptions/Preconditions:**  
            Login and register has been setup. Event DB and Tournament DB has been setup. Navbar handles searching by city and state.
        
        **Description:**  
            When a Tournament Organizer logs into our site we are gonna assume that they want to view events that they have created and are monitoring. The home page will be tailored to the person logged in. They will still have the option of searching by city and state in the navbar for other events. The events displayed are able to be clicked and this will display the tournaments happening in these events.
 
        **Core Needs:**
        1. Another view page that handles when an organizer logs in.
        2. A list of events the organizer is running.
        3. No page reloads when a tournament is clicked on, AJAX.
 
        **Tasks:**  
        1. [T] Create another view for the home page tailored to an organizer that is logged in.
        2. [T] Display a list of events that the logged in organizer is holding.
        3. [T] Make it so when an event is clicked on AJAX displays the tournaments happening at that event.



2. [E] As an event organizer, I want to create an account so that I can create events. 
	
    1. [U] **Title:**   
As an event organizer, I want to create an account so that I can manage events.
 
        **Assumptions/Preconditions:**  
This story will need the application to have already been made.

        **Description:**  
The stakeholder wants accounts initially for organizing events, and managing tournaments. Accounts will extend to competitors for additional features most notably, the ability to view private events. Registering/login should be easy, and a user should know that they are logged in at all times. This can be as simple as having the logged in username displayed in the navbar. Accounts should need email, username, and passwords only. Usernames don’t need to be unique and are more of identification of the event organizer, so something simple like an actual name or username. There should be some protection from bots creating accounts. This site will need to be primarily designed for desktop use as that is how the event organizer will likely manage, but mobile views may be nice for future features. 

        **Core needs described:**
        1. Site should look good on desktop
        2. Finding the login/register page should be easy to find
        3. Registering should be quick and responsive
        4. Usernames are open and do not have to be unique
        5. Passwords need to be 8+ characters
        6. Logins will be done with emails and passwords
        7. Users need to be able to change their username
        8. User will know when they are logged in without needed action
        9. Captcha security so that the site is not overrun with bot accounts

		**Tasks:**  
		1. [T] Create account database
		2. [T] Create login and register pages
		3. [T] Create user page for viewing and changing username
		4. [T] Add captcha security to register page

    2. [U] **Title:**   
As an event organizer, I want to create events so that I can add tournaments to manage.
 
        **Assumptions/Preconditions:**  
Tournaments can only be made by account users. All items in this feature will need to check if someone is logged in and make sure that they are looking at their data.

        **Description:**  
The stakeholder wants to create events and manage them as the core feature of this application. Events should have a unique name. The event creator’s username will be the event organizer name stored with the event. The date and time need to be stored as well. There may be an event that spans multiple days so we will have the datetime indicate the start of the event. We will have a short description field available to indicate multi-day events or cover any other unusual event details. Visibility of the event will need to be decided upon. This should be easy since it only needs to be a switch; public = true, private = false. All of these options should be decided on one page and ask for confirmation before creating an event. Only registered users can create accounts, so we will have the option to create an account available to everyone but indicate that a user must be logged in to create an event if they try to do so. The events will also need two attributes for location. An address stored in one space and a second for specific details such as which building or floor it will be at the address. (Though, note this will need to be included in the table but another epic will cover address entry).  

        **Core needs described:**
        1. Events need a name, location, and date to be created
        2. Description field can be null
        3. Private/public will be a checkbox defaulted to private
        4. The username of the creator will be automatically tied to the event
        5. Redirect login/register if non-account user tries to create an event
        6. Events are created without tournaments; they will be added later
        7. Events need to be able to be changed or deleted
        
        **Tasks:**  
        1. [T] Create database with event tables
        2. [T] Create page to enter event name, datetime(s), and other base info
        3. [T] Create indicator to tell user to login/register to create events
        4. [T] Create page to delete/change event values
	
    3. [U] **Title:**  
As a user, I want to be able to view events so that I know all relevant details of that event.  

        **Assumptions/Preconditions:**  
Tournaments can only be seen if public. Private ones need to give access to logged in accounts to view. This is found in the database from the previous user story.  

        **Description:**  
This page will include all entered data for the event in a coherent and relevant way. Name of the event, organizer username, description, date, and time prominently show with other data easily located. We would like to integrate Google Maps in future stories. All displayed data should take up only ½ to ⅔ of the screen as the bottom part will show all tournaments that will take place at the event. Tournament creation will be covered in a later epic but we do want to build this page with that feature in mind. This event will need to be findable on the site via searching or from a list of events. Best way to index event pages is by event Id that will be stored in the database.  

        **Core needs described:**
        1. Each event page will need to display all details on one page
        2. This page will also have links to included tournaments
        3. Page will have to verify that user has access to page
        4. User will get a page saying that event is private if they don’t have access
        5. Pages will be indexed and be able to be found via url (search and browse will come later.  
        
        **Tasks:**
        1. [T] Make event pages indexed and routed
        2. [T] Easily show all event details on one page
        3. [T] Verify that user can see page based on visibility
        4. [T] Create rejection page for inaccessible private events

3. [E] As an event organizer, I want to add a tournament to my event.
	1. [U]	**Title:**   
As an event organizer, I want to save my Challonge user information so that I can easily pull/push my data to my Challonge account.
 
        **Assumptions/Preconditions:**  
Challonge user data will need to be tied to account data. Identity database will need to be altered

        **Description:**  
The stakeholder wants to continue taking full advantage of Challonge’s features, so this app needs to work with their Challonge account. We need to store their credentials when pulling or pushing data to/from their api. This data will be entered on the same page used to change username. This field will be null upon account registrationThis data will need to be alterable so that a new account isn’t required if they lose access to the Challonge account. This code will create a template for when we expand upon other website api use such as smash.gg. 

        **Core needs described:**  
        1. No Challonge credentials are required
        2. Users can add/change credentials on same page they change the username

        **Tasks:**
        1. [T] Add Challong credentials attributes to account database
        2. [T] Adjust register page to add null values to new attributes
        3. [T] Alter user page to enter or change credentials

	2. [U]	**Title:**  
As an event organizer, I want to pull tournament data from Challonge so that I can add it to an event.
 
        **Assumptions/Preconditions:**  
This will use credentials tied to the account from the previous user story. This data pull will need to be altered and relate to the existing event table in our database.

        **Description:**  
Our application needs to use Challonge’s API to pull tournament data including custom details, matches, and competitors. The event organizer can give the url for the Challonge tournament. For each tournament we want the name, format, game, number of players and if it is a team based competition. This data will need to be entered into a tournament table that will be tied to an event. A tournament cannot be in our database without being associated with an event. There may be different stations at an event and there should be a nullable connection to a station entity to keep track of for each tournament. If the request or storage of the data does not work, then the user should have relevant data to know what went wrong without giving ugly auto generated error messages. 

        **Core needs described:**
        1. Use Challonge credentials to get tournament data
        2. Convert tournament data to fit in our database
        3. Confirm if the pull was successful or not
        4. Have a a tournament view page to see all details, that can be routed to from the event page

        **Tasks:**
        1. [T] Create tournament, stations, match, and event tables
        2. [T] Request data from Challonge using given url and event organizer’s credentials
        3. [T] Create tournament view page to see details of tournament
        4. [T] Create confirmation or otherwise notification for if the process succeeded or not

	3. [U]	**Title:**  
As an event organizer, I want to create Challonge tournaments so that I can send it to Challonge.
 
        **Assumptions/Preconditions:**  
The tables from the previous user story need to be created to save the data.

        **Description:**  
The stakeholder wants to be able to create tournaments and send that data back to Challonge. This will remove the need to move between the 2 sites to create events with tournaments. We will need a new page to enter this data with a button to save and an optional button to send it to Challonge. Both buttons will need a confirmation/error handling when pressed and the send button will need to return the URL for the new Challonge tournament. This will need to be saved in the tournament and immediately available after sending the data so that the user can verify that it worked as intended. 

        **Core needs described:**  
        1. Page to create tournament
        2. Confirmation that it was saved correctly
        3. Button to send it to Challonge with confirmation
        4. Needs to return a URL or link to matching Challonge Tournament

        **Tasks:**
        1. [T] Create tournament creation page
        2. [T] Make notifications for successful saves
        3. [T] Make a button that sends tournament to Challonge
        4. [T] Get URL of Challonge tournament and save it in the database

4. [E] As an event organizer, I want to easily pull competitor data from Challonge and associate duplicate competitors.
5. [E] As an event organizer, I want to send data such as completed matches to external sites. 
6. [E] As an event organizer, I want to keep track of all competitor statuses (availability and standing).
7. [E] As an event organizer, I want a view of all matches that are completed and need to be completed to progress to the next level.
8. [E] As an event organizer, I want to easily select who will be in the next match and when it will take place.
9. [E] As an event organizer, I want an algorithm to generate schedules based on all competitors statuses.
10. [E] As a competitor,  I want to be able to search for events based on locations or tournaments at that event.
11. [E] As a competitor, I want to see upcoming and recent events.
12. [E] As a competitor, I want to see event details such as location, tournaments, competitors, etc.
13. [E] As a competitor, I want to view up to date standings for each active tournament in an event.
14. [E] As a competitor, I want to see my standings and when my next match in the event will be.
15. [E] As an event organizer, I want a list of templates I can choose from when I set up tournaments or events.

___

## **Initial Architecture Envisioning**

[Architecture Overview](diagrams/architecture_overview.png)

___

## **Identification of Risks**

1. Not all data may be easily imported from Challonge. If so, our application should be robust enough to pick up the workload that Challonge or other bracket sites would take on. This is also true for sending data to an external site.

2. The algorithm for optimal scheduling may be unclear during development. We could attempt to implement a few different ones that each generate a schedule, but there may not be enough time and resources to do this.

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
