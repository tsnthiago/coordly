# Blazor WebAssembly App with .NET 8

It appears that the `--hosted` option has been removed or deprecated in recent versions of .NET, including .NET 8, as it no longer works with the `blazorwasm` template. This change reflects a broader update in the project templates provided by Microsoft. Previously, the `--hosted` option allowed you to create a Blazor WebAssembly app hosted by an ASP.NET Core server, but this template has been phased out in favor of the new "Blazor Web App" template【71†source】【72†source】.

### Solution for .NET 8:
To achieve similar functionality in .NET 8, where you have a Blazor WebAssembly frontend interacting with your existing API, you can manually set up a **Blazor WebAssembly** project alongside your API project:

#### 1. **Create a Blazor WebAssembly Project Manually**:
   Run the following command to create a standalone Blazor WebAssembly project:
   ```bash
   dotnet new blazorwasm -o BlazorClient
   ```

#### 2. **Add the Blazor WebAssembly Client to Your Existing Solution**:
   - Add this Blazor WebAssembly project to your existing API solution.
   - Update the **launch settings** to ensure both projects (API and Blazor client) can be run simultaneously (if necessary).

#### 3. **Configure API Calls in the Blazor Client**:
   Modify the `Program.cs` in the Blazor client to configure the **HttpClient** for API calls:
   ```csharp
   builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001") });
   ```

#### 4. **Host the Client App Using the API Project**:
   If you want to serve the Blazor app through your existing API backend, you can copy the Blazor WebAssembly build output to the `wwwroot` folder of your API. You can automate this during the build process by using the following steps:
   
   - **Publish the Blazor Client**:
     ```bash
     dotnet publish -c Release -o ../YourAPIProject/wwwroot
     ```
   
   - Now, your Blazor WebAssembly app will be served from the API project and can make API requests to your server-side logic.

#### 5. **Deploy**:
   Once integrated, you can deploy both the Blazor WebAssembly frontend and your API backend to services like **Azure App Service** or **IIS**【73†source】.

With these steps, you can achieve the same result that the `--hosted` option used to provide in earlier versions of .NET.
