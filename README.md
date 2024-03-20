<h1>About</h1>
<p>Welcome to the Real Estate Management System! This microservices-based application simplifies property management and transactions.</p>

<h1>Overview</h1>
<p>The Real Estate Management System comprises several microservices:</p>

<p><b>APIGateway:</b> Entry point for all requests, handles routing to the appropriate microservice.</p>
<p><b>AuthService:</b> Manages user authentication and authorization.</p>
<p><b>PropertyService:</b> CRUD operations for properties and property types.</p>
<p><b>TransactionService:</b> Manages property transactions.</p>

<h1><b>Microservices Architecture</b></h1>
<p>This application is built on a microservices architecture, emphasizing modularity and independence. Key characteristics include:</p>
<p><b>Individual Deployments: </b>Each microservice is independently deployed on Azure app services, allowing for scalability and flexibility.</p>
<p><b>Isolated Databases: </b>For databases I used Azure Sql Databases. Every microservice has its dedicated database, ensuring data autonomy and separation of concerns.</p>

<script src="https://viewer.diagrams.net/js/viewer-static.min.js"></script>

<h1>Frontend: MVC Architecture</h1>
<p>The frontend of this application is developed using the MVC architecture, providing a structured and modular approach to design. Key aspects include:</p>

<p><b>Model-View-Controller (MVC):</b></p>
<p>The frontend follows the MVC architectural pattern, dividing the application into three interconnected components:</p>
<ul>
  <li><b>Model:</b> Represents the application's data and business logic.</li>
  <li><b>View:</b> Handles the presentation and user interface.</li>
  <li><b>Controller: </b> Manages user input and updates the model and view accordingly.</li>
</ul>

<h1>User Roles</h1>
<p>This application is designed with three distinct user roles, each with specific functionalities:</p>

<p><b>Admin:</b></p>
<ul>
  <li>Manages users and property types.</li>
</ul>

<p><b>Company:</b></p>
<ul>
  <li>Can perform CRUD operations on properties.</li>
  <li>Manages users associated with the company.</li>
</ul>
<p><b>Individual:</b></p>
<ul>
  <li>Manages personal properties.</li>
  <li>Can initiate property transactions.</li>
</ul>


<h1>Tech Stack</h1>
<p><b>Backend: </b>.NET Core 8, .NET Core Web API, Entity Framework Core</p>
<p><b>API Gateway: </b>Ocelot</p>
<p><b>Database: </b> Azure SQL Database</p>
<p><b>Frontend:</b>HTML, CSS, Bootstrap, jQuery</p>
 

<h1>Demo</h1>
<p>Explore the app<a href="https://estatevillee.azurewebsites.net/" target=”_blank”> here</a></p>

<h1>Important Notes</h1>
<p><b>Simulated Transactions: </b>Please be aware that the transaction functionalities within this application are simulated for practice purposes. No real monetary transactions are processed, and the app does not integrate with any actual payment systems.</p>

<h1>Issues</h1>
<p>If you encounter any issues or have suggestions for improvements, please feel free to open an issue on the GitHub repository. Your feedback is highly valuable.</p>

<h3>How to Open an Issue</h3>
<p>Visit the Issues Section of the Repository <a href="https://github.com/FatbardhDurmishi/RealEstateWithMicorservices/issues">here</a></p>

<h4>Check Existing Issues:</h4>
<p>Before opening a new issue, please check if a similar issue already exists.</p>

<h4>New Issue:</h4>
<p>Click on the "New Issue" button.</p>

<h4>Details to Include:</h4>
<p>Title of the issue</p>
<p>Clearly describe the issue or enhancement you're proposing.</p>
<p>If applicable, include steps to reproduce the problem.</p>

<h4>Submit:</h4>
<p>Click "Submit new issue."</p>

