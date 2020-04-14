# **Guidelines**
Follow these rules when contributing code to this project. Use this guide page if you are unsure about how to format your code.

# **Overall**
1. This project uses MVC 5 format with ASP.NET framework using CSharp, not Visual Baisc. If you do not know how to setup this type of project please follow this link; [Quickstart: Use Visual Studio to create your first ASP.NET Core web app](https://docs.microsoft.com/en-us/visualstudio/ide/quickstart-aspnet-core?view=vs-2019)

1. When adding to the project make sure to follow these steps; [Getting started with MVC 5](https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/).

1. Code Documentation should be handled within your variable and function names, if there is any ambiguous names please provide clear commenting to explain what is happening.

# **Git**
## **Forking:**
1.	Click fork button on github

2.	Clone Forked repo to your desktop

3.	Set upstream:

    1. git remote add upstream https://github.com/theMalmstadt/Ok_Computer

    1. git checkout –b dev

    1. git pull upstream dev

    1. git push origin dev


## **Developer Steps:**
___
**Before doing anything:**

1. git checkout dev

1. git pull upstream dev

1. git push origin dev

**Creating Features:**

1. git checkout -b (feature branch name)

1. Do some work

1. git push origin (feature branch name)

1. Repeat steps 2-3 as needed for feature

1. Make pull request (when feature is finished, tested and working on local project)

    1. Go to your Fork: https://github.com/(your_user)/Ok_Computer

    1. Click button create Pull Request

    1. Your feature branch name to dev

### **Pull Requests:**
Hopefully pull request will be asked to be merged to: theMalmstadt:dev from (user):feature-branch, if this has Master branch anywhere or if you do not pull request from your feature branch, the pull request will be denied.

**Merge conflicts will be handled by the user trying to merge**  
Like so:
1. git checkout dev
1. git pull upstream dev
1. git checkout (feature branch name)
1. git merge dev
    - This will create the merge conflict locally that you can then troubleshoot

Once merge conflict is fixed, repeat steps in Creating Features #5.
# **CSHTML Style**
1. \<body\> tags are should not be inside your custom .cshtml view files because all view files are already under a body tag within the _Layout.cshtml file.

1. For \<id\> naming conventions, we use camel case, for example:

```html
<div id="myIdTag"></div>
```

3. Use [Boostrap 4](https://getbootstrap.com/docs/4.4/getting-started/introduction/) whenever possible to add style to your view pages, follow id styling guidelines if you have to create ID tags for CSS styling. If CSS styling is necessary, inline CSS is not allowed and must be done within a .css file.

3. All html openings tags need to line up appropriately with their respective closing tag, for example both the beginning and closing tag need to be on seperate lines then your html code between them and they need to be at the same tab level:

```html 
<div class="container">
    <p>My first paragraph</p>
<\div>
```

5. When using Razor if the Razor includes brackets they should be on new lines and with the same tab indentation:
```csharp
@using (Html.BeginForm())
{
    (form code here)
}
```

# **SQL DataBase**
1. Table and attribute names are to be singular.
```sql
CREATE TABLE [dbo].[Team]
```
2. The ID of the table should be named in (tableName)ID format, if the table name is multiple words, choose the word that is the most descriptive for the that tabe for the ID.
```sql
CREATE TABLE [dbo].[Team]
(
    [TeamID] INT IDENTITY (1,1) PRIMARY KEY,
)

```

3. Foreign keys should be named the same way they are named within their respective table.
```sql
 CREATE TABLE [dbo].[Athlete]
 (
	[AthleteID]		INT IDENTITY (1,1)	NOT NULL,
	[Name]			NVARCHAR(128)		NOT NULL,
	[Gender]		NVARCHAR(64)		NOT NULL,
	[TeamID]		INT					NOT NULL
	CONSTRAINT [PK_dbo.Athlete] PRIMARY KEY CLUSTERED ([AthleteID] ASC),
	CONSTRAINT FK_Team FOREIGN KEY (TeamID)
	REFERENCES Team(TeamID) ON DELETE CASCADE
 );
```

4. Pascal case should be used when creating columns for tables.

5. Tables that are used to normalize a many to many relationship shall be handled like such:
```sql
CREATE TABLE [dbo].[TeamMeet]
(
	[TeamID]		INT	NOT NULL,
	[MeetID]		INT	NOT NULL
	CONSTRAINT [PK_dbo.TeamMeet] PRIMARY KEY CLUSTERED ([TeamID],[MeetID])
	CONSTRAINT [PK_dbo.TeamMeet_dbo.TeamID] FOREIGN KEY ([TeamID])
	REFERENCES [dbo].[Team] ([TeamID]) ON DELETE CASCADE,
	CONSTRAINT [PK_dbo.MeetID_dbo.MeetID] FOREIGN KEY ([MeetID])
	REFERENCES [dbo].[Meet] ([MeetID]) ON DELETE CASCADE
)
```


# **DAL**
All (tableName)Context.cs made be a developer should be inside the DAL folder in the solution explorer. Any auto-generated Context files can be left where they are.