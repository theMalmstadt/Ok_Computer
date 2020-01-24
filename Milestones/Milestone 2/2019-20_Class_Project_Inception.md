# **OK Computer 2019-20 Class Project Inception**


## **Summary of Our Approach to Software Development**

*Inception phase of SCRUM Development.*

## **Vision Statement 1.0**

For coaches who need to record and analyze their athlete's data, the Class Project is an analytical tool that will store athlete records and provide predictive race and meet results. Unlike Athletic.net, that only provides the data, our product will predict future race results and allow coaches to organize their teams in the most effective matter to win meets.
___

## **Initial Requirements Elaboration and Elicitation:**


### Interview QA:

Q: Whoever is entering the data, where are they getting the data from?

A: The data will be given by the event organizer or a participating coach to an admin, who will then enter the information.

Q: As this application gets bigger, the admins will have more work to do to enter and edit data.
Should coaches have some level of authorization to edit data?

A: I don't think coaches need to work with the data. I want swim meets results to be easy enough to enter and make corrections that it can all be done by the admins.

Q: Do you want to track team scores at events and in seasons?

A: Absolutely! I want the coaches tool to respond to results from the season in progress so that he/she may be able to allocate their swimmers effectively for the next meet.

Q: Will you want to track league competition ie. ranks?

A: Yes. Teams are not competing in a vacuum and will need to know how their competition is allocating their swimmers.

Q: Do you have a strategy for allocating swimmers or, do you want us to create that strategy?

A: I want the coaches tool to aid the coach in optimizing his strategy. Maybe an interactive tool to view his/her athletes in different events.

Q: What elements of the coaches strategy will be put into this tool?

A: Well the tool should use data from what the athletes usually compete in or don’t want to compete in or what events their strengths lie in.

Q: So would a multiple schedule generator be a good tool for finding the best way to allocate swimmers? That way the coach could choose from multiple options with different ranks of predicted success.

A: That sounds doable.

Q: Recording the data of athlete training could double the workload. Would athlete meet stats be good enough to track athlete performance for the coaches tool?

A: Performance at meets will be the main variable for predicting athlete performance in future meets.

___

## **List of Needs and Features**

1. They want a nice looking site, that clean looking and intuitive to use. They want imagery evoking swimming and competition. Account creation and website use should be easy to figure out and use. 

2. Anyone can view results by athlete name, team, coach or event date and location without creating an account.
3. Logins will be required for viewing statistics and all other advanced features.  We eventually plan to offer paid plans for accessing these advanced features.  They'll be free initially and we'll transition to paid plans once we get people hooked.
4. Admin logins are needed for entering new data.  Only employees and contractors will be allowed to enter, edit or delete data.
5. All accounts require email confirmation and an 8+ character password. Admin accounts will additionally require offline confirmation by employees and will then be added by "super" admins.
6. The core entity is the athlete.  They are essentially free agents in the system.  They can be a member of one or more teams at one time, then change at any time.  Later when we want to have teams and do predictive analysis we'll let the coaches assemble their own teams and add/remove athletes from their rosters.
7. The first stats we want are: 1) display PR's prominently in each race event, 2) show a historical picture/plot of performance, per race type and distance, 3) some measure of how they rank compared to other athletes, both current and historical, 4) something that shows how often they compete in each race event, i.e. which events are they competing in most frequently, and alternately, which events are they "avoiding".
8. Coaches should be able to generate schedules for swimmers at at a meet and choose the best one. The schedules will use performance and preference data to put athletes in events to give the team the best chance for winning the meet.
9. The ability to download a csv files with relevant data. CSV can include event results, or a single swimmer results over time. Also be able to print which event each athlete will compete in based off of a strategy that is made using this application. This will give the coach a physical paper to take to meets.
10. For administrators, there should be a tool to easilly upload a CSV file to add data to the database.

___

## **Initial Modeling**

[Use Case Diagram](https://raw.githubusercontent.com/theMalmstadt/Ok_Computer/master/Milestones/Milestone%202use_case_diagram.png)

[ER Diagram](https://raw.githubusercontent.com/theMalmstadt/Ok_Computer/master/Milestones/Milestone%202/er_diagram.png)

___
## **Identify Non-Functional Requirements**

1. User accounts and data must be stored indefinitely.  They don't want to delete; rather, mark items as "deleted" but don't actually delete them.  They also used the word "inactive" as a synonym for deleted.

2. Passwords should not expire
3. Site should never return debug error pages.  Web server should have a custom 404 page that is cute or funny and has a link to the main index page.
4. All server errors must be logged so we can investigate what is going on in a page accessible only to Admins.
5. English will be the default language.
6. We are going to use Bootstrap 4 instead of trying to use CSS.
7. Our scheme for the website is black and blue.
8. The software should be portable, OS to OS. E.g.: Android to Apple.

___
## **Identify Functional Requirements (User Stories)**

E: Epic  
U: User Story  
T: Task  

1. [U] As a visitor to the site I would like to see a fantastic and modern homepage that introduces me to the site and the features currently available.

   1. [T] Create starter ASP dot NET MVC 5 Web Application with Individual User Accounts and no unit test project
   2. [T] Choose CSS library (Bootstrap 3, 4, or ?) and use it for all pages
   3. [T] Create nice homepage: write initial content, customize navbar, hide links to login/register
   4. [T] Create SQL Server database on Azure and configure web app to use it. Hide credentials.
2. [U] As a visitor to the site I would like to be able to register an account so I will be able to access athlete statistics
   1. [T] Copy SQL schema from an existing ASP.NET Identity database and integrate it into our UP script
   2. [T] Configure web app to use our db with Identity tables in it
   3. [T] Create a user table and customize user pages to display additional data
   4. [T] Re-enable login/register links
   5. [T] Manually test register and login; user should easily be able to see that they are logged in
3. [E] As an administrator I want to be able to upload a spreadsheet of results so that new data can be added to our system
   1. [U] as an administrator, I would like the spreadsheet to be formatted as simply as possible so that I can easily format the data correctly without training.
   2. [U] as an administrator, I would like to use a graphical tool to upload a spreadsheet  containing results from a meet so that i can easily upload data without training.
4. [U] As an unregistered user, i would like to be able to find a specific athlete and view their performance in recent events, so that I have a taste of the functionality of the site before considering registering for an account.
5. [U] As a visitor I want to be able to view race results for an athlete so I can see how they have performed
6. [U] As a visitor I want to be able to view PR's (personal records) for an athlete so I can see their best performances
7. [E] As a coach, I would like two methods of procedurally assigning my team’s athletes to events at a meet so that my team will be more likely to win.
    1. [U] As a coach, I would like functionality to assign my team’s swimmers to events  at  meets given access to the opposing teams race results, so that my team is more likely to win the meet
    2. [U] As a coach, I would like functionality to assign my team’s swimmers to events at meets without information about the other teams race results so that my team is more likely to win the meet.
8. [U] As a robot I would like to be prevented from creating an account on your website so I don't ask millions of my friends to join your website and try to add comments about male enhancement drugs.
    1. [T] Use captcha on account registration.
9. [U] As a coach, I would like to be able to view the standings of athletes on my teams on a per event basis so that I can determine which of my athletes have to fastest times in each event
10. [U] As an unregistered visitor, I would like to be able to view standings of athletes in each event so that I can get an understanding of the fastest swimmers performance in each event.
11. [U] As an athlete, I would like any event records im currently holding to appear on my athlete page so that I can brag to my friends.
12. [U] As a visitor, if I cause an error I would like to be redirected to a cute or funny 404 error page so that i know i messed up, but don’t feel lost.

___
## **Initial Architecture Envisioning**

[Architecture Diagram](https://raw.githubusercontent.com/theMalmstadt/Ok_Computer/dev/Milestones/Milestone%202/Architecture%20Diagram.png)