# TechSpire
###### Tv scheduling project by Dominik W, Hlaing P H, Rudraa P, Azul D & Filip D

### Heres a quick setup guide:

## BACKEND Setup Guide

1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd tvscheduler
   ```

2. **Install Dependencies**
   ```bash
   # Install .NET dependencies:

      dotnet add package Microsoft.EntityFrameworkCore --version 8.0.4

      dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.4

      dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.0

      dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.4

      JwtBearer 804    

      dotnet restore
   ```

3. **Database Setup**

   a. Update the database connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=tvscheduler;User=your_username;Password=your_password;"
     }
   }
   ```

   b. Create and update the database:
   ```bash
   dotnet ef database update
   ```

4. **Environment Configuration**

   a. Update JWT settings in `appsettings.json`:
   ```json
   {
     "Jwt": {
       "Key": "your-secret-key",
       "Issuer": "your-issuer",
       "Audience": "your-audience"
     }
   }
   ```

   b. Update external API settings if you wish to use a different external API (not recommended):
   ```json
   {
     "ImageServer": {
       "BaseUrl": "your-image-server-url"
     }
   }
   ```


### System Initialization

1. **Start the Application**
   ```bash
   dotnet run
   ```

2. **Initial Database Population**
    - The system will automatically populate the database with initial channel data on first run
    - Verify the channels are loaded by checking the database

3. **Hangfire Jobs Setup**

   a. Access the Hangfire Dashboard:
    - Navigate to `/hangfire` in your browser
    - Login with administrator credentials

   b. Run the following jobs in sequence:

    1. **Fetch Show Data**
       ```bash
       # Run the channel schedule update job
       Job: "Update the channel schedule for two days"
       ```

    2. **Process Show Images**
       ```bash
       # Run the image processing job
       Job: "Process Show Images"
       ```

    3. **Generate Initial Recommendations**
       ```bash
       # Run the recommendation update job
       Job: "Update Global Recommendation"
       ```


### Regular Maintenance

1. **Daily Jobs**
    - Cache updates
    - Image processing
    - Recommendation updates


### Troubleshooting

1. **Database Connection Issues**
    - Verify connection string
    - Check MySQL service status
    - Ensure proper permissions

2. **Hangfire Job Failures**
    - Check Hangfire dashboard for error details
    - Verify external API connectivity
    - Check database permissions

3. **Image Processing Issues**
    - Verify image server URL
    - Check file system permissions
    - Use DBeaver to evaluate values in the database



    
## How to run the front-end: 
1. Make sure Node.js is installed (https://nodejs.org)
2. Clone the Repository and cd into the frontend folder (/tvscheduler-react-app)
3. Install Dependencies by running `npm install`
4. Start the Front-end by running `npm run dev`
