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

<h1>Frontend: MVC Architecture</h1>
<p>The frontend of this application is developed using the MVC architecture, providing a structured and modular approach to design. Key aspects include:</p>


<div class="mxgraph" style="max-width:100%;border:1px solid transparent;" data-mxgraph="{&quot;highlight&quot;:&quot;#0000ff&quot;,&quot;nav&quot;:true,&quot;resize&quot;:true,&quot;toolbar&quot;:&quot;zoom layers tags lightbox&quot;,&quot;edit&quot;:&quot;_blank&quot;,&quot;xml&quot;:&quot;&lt;mxfile host=\&quot;app.diagrams.net\&quot; modified=\&quot;2024-03-20T09:43:18.974Z\&quot; agent=\&quot;Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36\&quot; etag=\&quot;ZZaxC8ECVrLXaX1Knxfx\&quot; version=\&quot;24.0.7\&quot; type=\&quot;github\&quot;&gt;\n  &lt;diagram name=\&quot;Page-1\&quot; id=\&quot;GJ6CLmsnKcf3C3Ws2XPh\&quot;&gt;\n    &lt;mxGraphModel dx=\&quot;1050\&quot; dy=\&quot;522\&quot; grid=\&quot;1\&quot; gridSize=\&quot;10\&quot; guides=\&quot;1\&quot; tooltips=\&quot;1\&quot; connect=\&quot;1\&quot; arrows=\&quot;1\&quot; fold=\&quot;1\&quot; page=\&quot;1\&quot; pageScale=\&quot;1\&quot; pageWidth=\&quot;850\&quot; pageHeight=\&quot;1100\&quot; math=\&quot;0\&quot; shadow=\&quot;0\&quot;&gt;\n      &lt;root&gt;\n        &lt;mxCell id=\&quot;0\&quot; /&gt;\n        &lt;mxCell id=\&quot;1\&quot; parent=\&quot;0\&quot; /&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-3\&quot; style=\&quot;edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;entryX=0.5;entryY=0;entryDx=0;entryDy=0;\&quot; edge=\&quot;1\&quot; parent=\&quot;1\&quot; source=\&quot;o49BstXFcHLmxdN3CpSH-1\&quot; target=\&quot;o49BstXFcHLmxdN3CpSH-2\&quot;&gt;\n          &lt;mxGeometry relative=\&quot;1\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-4\&quot; value=\&quot;Http Request\&quot; style=\&quot;edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];\&quot; vertex=\&quot;1\&quot; connectable=\&quot;0\&quot; parent=\&quot;o49BstXFcHLmxdN3CpSH-3\&quot;&gt;\n          &lt;mxGeometry relative=\&quot;1\&quot; as=\&quot;geometry\&quot;&gt;\n            &lt;mxPoint as=\&quot;offset\&quot; /&gt;\n          &lt;/mxGeometry&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-1\&quot; value=\&quot;Front-end\&quot; style=\&quot;ellipse;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;\&quot; vertex=\&quot;1\&quot; parent=\&quot;1\&quot;&gt;\n          &lt;mxGeometry x=\&quot;330\&quot; y=\&quot;80\&quot; width=\&quot;120\&quot; height=\&quot;80\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-8\&quot; style=\&quot;edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;entryX=0.5;entryY=0;entryDx=0;entryDy=0;startArrow=classic;startFill=1;exitX=0.5;exitY=1;exitDx=0;exitDy=0;\&quot; edge=\&quot;1\&quot; parent=\&quot;1\&quot; source=\&quot;o49BstXFcHLmxdN3CpSH-2\&quot; target=\&quot;o49BstXFcHLmxdN3CpSH-5\&quot;&gt;\n          &lt;mxGeometry relative=\&quot;1\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-9\&quot; style=\&quot;edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;entryX=0.5;entryY=0;entryDx=0;entryDy=0;\&quot; edge=\&quot;1\&quot; parent=\&quot;1\&quot; source=\&quot;o49BstXFcHLmxdN3CpSH-2\&quot; target=\&quot;o49BstXFcHLmxdN3CpSH-6\&quot;&gt;\n          &lt;mxGeometry relative=\&quot;1\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-10\&quot; style=\&quot;edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;entryX=0.5;entryY=0;entryDx=0;entryDy=0;exitX=0.5;exitY=1;exitDx=0;exitDy=0;\&quot; edge=\&quot;1\&quot; parent=\&quot;1\&quot; source=\&quot;o49BstXFcHLmxdN3CpSH-2\&quot; target=\&quot;o49BstXFcHLmxdN3CpSH-7\&quot;&gt;\n          &lt;mxGeometry relative=\&quot;1\&quot; as=\&quot;geometry\&quot;&gt;\n            &lt;mxPoint x=\&quot;390\&quot; y=\&quot;320\&quot; as=\&quot;sourcePoint\&quot; /&gt;\n          &lt;/mxGeometry&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-11\&quot; value=\&quot;Route Request\&quot; style=\&quot;edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];\&quot; vertex=\&quot;1\&quot; connectable=\&quot;0\&quot; parent=\&quot;o49BstXFcHLmxdN3CpSH-10\&quot;&gt;\n          &lt;mxGeometry relative=\&quot;1\&quot; as=\&quot;geometry\&quot;&gt;\n            &lt;mxPoint x=\&quot;-125\&quot; y=\&quot;-15\&quot; as=\&quot;offset\&quot; /&gt;\n          &lt;/mxGeometry&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-2\&quot; value=\&quot;API Gateway\&quot; style=\&quot;rhombus;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;\&quot; vertex=\&quot;1\&quot; parent=\&quot;1\&quot;&gt;\n          &lt;mxGeometry x=\&quot;350\&quot; y=\&quot;230\&quot; width=\&quot;80\&quot; height=\&quot;80\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-5\&quot; value=\&quot;AuthService\&quot; style=\&quot;rounded=0;whiteSpace=wrap;html=1;fillColor=#d5e8d4;flipH=1;flipV=1;strokeColor=#82b366;\&quot; vertex=\&quot;1\&quot; parent=\&quot;1\&quot;&gt;\n          &lt;mxGeometry x=\&quot;80\&quot; y=\&quot;420\&quot; width=\&quot;120\&quot; height=\&quot;60\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-6\&quot; value=\&quot;PropertyService\&quot; style=\&quot;rounded=0;whiteSpace=wrap;html=1;fillColor=#d5e8d4;flipH=1;flipV=1;strokeColor=#82b366;\&quot; vertex=\&quot;1\&quot; parent=\&quot;1\&quot;&gt;\n          &lt;mxGeometry x=\&quot;330\&quot; y=\&quot;420\&quot; width=\&quot;120\&quot; height=\&quot;60\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n        &lt;mxCell id=\&quot;o49BstXFcHLmxdN3CpSH-7\&quot; value=\&quot;TransactionService\&quot; style=\&quot;rounded=0;whiteSpace=wrap;html=1;fillColor=#d5e8d4;flipH=1;flipV=1;strokeColor=#82b366;\&quot; vertex=\&quot;1\&quot; parent=\&quot;1\&quot;&gt;\n          &lt;mxGeometry x=\&quot;580\&quot; y=\&quot;420\&quot; width=\&quot;120\&quot; height=\&quot;60\&quot; as=\&quot;geometry\&quot; /&gt;\n        &lt;/mxCell&gt;\n      &lt;/root&gt;\n    &lt;/mxGraphModel&gt;\n  &lt;/diagram&gt;\n&lt;/mxfile&gt;\n&quot;}"></div>
<script type="text/javascript" src="https://viewer.diagrams.net/js/viewer-static.min.js"></script>

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

